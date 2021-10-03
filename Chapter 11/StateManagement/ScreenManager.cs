using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

using VoidEngineLight.Interfaces;
using Newtonsoft.Json;

namespace MonoGameRPG.StateManagement
{
    // The screen manager is a component which manages one or more GameScreen
    // instances. It maintains a stack of screens, calls their Update and Draw
    // methods at the appropriate times, and automatically routes input to the
    // topmost active screen.

    public enum Screens
    {
        MainMenu,
        Options,
        Game
    }

    public class ScreenManager : DrawableGameComponent
    {
#if WINDOWS_PHONE
        private const string StateFilename = "ScreenManagerState.xml";
#endif
        public IAudioManager audioManager { get { return Game.Services.GetService<IAudioManager>(); } }

        public string BackgroundSongAsset { get; set; }

        private readonly List<GameScreen> _screens = new List<GameScreen>();
        private readonly List<GameScreen> _tempScreensList = new List<GameScreen>();

        private readonly InputState _input = new InputState();

        private SpriteBatch _spriteBatch;
        private SpriteFont _font;
        private Texture2D _blankTexture;

        private bool _isInitialized;
        private bool _traceEnabled;

        // A default SpriteBatch shared by all the screens. This saves
        // each screen having to bother creating their own local instance.
        public SpriteBatch SpriteBatch => _spriteBatch;

        // A default font shared by all the screens. This saves
        // each screen having to bother loading their own local copy.
        public SpriteFont Font => _font;

        // If true, the manager prints out a list of all the screens
        // each time it is updated. This can be useful for making sure
        // everything is being added and removed at the right times.
        public bool TraceEnabled
        {
            get => _traceEnabled;
            set => _traceEnabled = value;
        }

        // Gets a blank texture that can be used by the screens.
        public Texture2D BlankTexture => _blankTexture;

        public Screens CurScreen;

        public ScreenManager(Game game) : base(game)
        {
            // we must set EnabledGestures before we can query for them, but
            // we don't assume the game wants to read them.
            TouchPanel.EnabledGestures = GestureType.None;
        }

        public override void Initialize()
        {
            base.Initialize();
            _isInitialized = true;
        }

        protected override void LoadContent()
        {
            // Load content belonging to the screen manager.
            var content = Game.Content;

            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _font = content.Load<SpriteFont>("menufont");
            _blankTexture = content.Load<Texture2D>("blank");

            // Tell each of the screens to load their content.
            foreach (var screen in _screens)
            {
                screen.Activate(false);
            }

            LoadAudioSettings();
        }

        protected override void UnloadContent()
        {
            foreach (var screen in _screens)
            {
                screen.Unload();
            }
        }

        // Allows each screen to run logic.
        public override void Update(GameTime gameTime)
        {
            _input.Update();    // Read the keyboard and gamepad

            // Make a copy of the master screen list, to avoid confusion if
            // the process of updating one screen adds or removes others.
            _tempScreensList.Clear();

            foreach (var screen in _screens)
                _tempScreensList.Add(screen);

            bool otherScreenHasFocus = !Game.IsActive;
            bool coveredByOtherScreen = false;

            // Loop as long as there are screens waiting to be updated.
            while (_tempScreensList.Count > 0)
            {
                // Pop the topmost screen off the waiting list.
                var screen = _tempScreensList[_tempScreensList.Count - 1];

                _tempScreensList.RemoveAt(_tempScreensList.Count - 1);

                string musicAsset = BackgroundSongAsset;

                if (!string.IsNullOrEmpty(screen.BackgroundSongAsset))
                    musicAsset = screen.BackgroundSongAsset;

                if (!string.IsNullOrEmpty(musicAsset))
                {
                    if (!audioManager.IsMusicPlaying)
                        audioManager.PlaySong(musicAsset, 1, true);
                    else
                    {
                        if (musicAsset != audioManager.CurrentSongAsset)
                        {
                            // Should really fade out and back in...
                            audioManager.StopMusic();
                        }
                    }
                }

                screen.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

                if (screen.ScreenState == ScreenState.TransitionOn || screen.ScreenState == ScreenState.Active)
                {
                    // If this is the first active screen we came across,
                    // give it a chance to handle input.
                    if (!otherScreenHasFocus)
                    {
                        screen.HandleInput(gameTime, _input);
                        otherScreenHasFocus = true;
                    }

                    // If this is an active non-popup, inform any subsequent
                    // screens that they are covered by it.
                    if (!screen.IsPopup)
                        coveredByOtherScreen = true;
                }
            }

            if (_traceEnabled)
                TraceScreens();
        }

        private void TraceScreens()
        {
            var screenNames = new List<string>();

            foreach (var screen in _screens)
                screenNames.Add(screen.GetType().Name);

            Debug.WriteLine(string.Join(", ", screenNames.ToArray()));
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (var screen in _screens)
            {
                if (screen.ScreenState == ScreenState.Hidden)
                    continue;

                screen.Draw(gameTime);
            }
        }

        public void AddScreen(GameScreen screen, PlayerIndex? controllingPlayer)
        {
            screen.ControllingPlayer = controllingPlayer;
            screen.ScreenManager = this;
            screen.IsExiting = false;

            // If we have a graphics device, tell the screen to load content.
            if (_isInitialized)
                screen.Activate(false);

            _screens.Add(screen);

            // update the TouchPanel to respond to gestures this screen is interested in
            TouchPanel.EnabledGestures = screen.EnabledGestures;
        }

        // Removes a screen from the screen manager. You should normally
        // use GameScreen.ExitScreen instead of calling this directly, so
        // the screen can gradually transition off rather than just being
        // instantly removed.
        public void RemoveScreen(GameScreen screen)
        {
            // If we have a graphics device, tell the screen to unload content.
            if (_isInitialized)
                screen.Unload();

            _screens.Remove(screen);
            _tempScreensList.Remove(screen);

            // if there is a screen still in the manager, update TouchPanel
            // to respond to gestures that screen is interested in.
            if (_screens.Count > 0)
                TouchPanel.EnabledGestures = _screens[_screens.Count - 1].EnabledGestures;
        }

        // Expose an array holding all the screens. We return a copy rather
        // than the real master list, because screens should only ever be added
        // or removed using the AddScreen and RemoveScreen methods.
        public GameScreen[] GetScreens()
        {
            return _screens.ToArray();
        }

        // Helper draws a translucent black fullscreen sprite, used for fading
        // screens in and out, and for darkening the background behind popups.
        public void FadeBackBufferToBlack(float alpha)
        {
            _spriteBatch.Begin();
            _spriteBatch.Draw(_blankTexture, GraphicsDevice.Viewport.Bounds, Color.Black * alpha);
            _spriteBatch.End();
        }

        internal class AudioSettings
        {
            public float MasterVolume { get; set; }
            public float MusicVolume { get; set; }
            public float SFXVolume { get; set; }

            public AudioSettings(IAudioManager audioManager = null)
            {
                if (audioManager != null)
                {
                    MasterVolume = audioManager.MasterVolume;
                    MusicVolume = audioManager.MusicVolume;
                    SFXVolume = audioManager.SFXVolume;
                }
            }

            public void SetAudioManager(IAudioManager audioManager)
            {
                audioManager.MasterVolume = MasterVolume;
                audioManager.MusicVolume = MusicVolume;
                audioManager.SFXVolume = SFXVolume;
            }
        }

        public void SaveAudioSettings()
        {
            AudioSettings settings = new AudioSettings(audioManager);

            string json = JsonConvert.SerializeObject(settings);

            File.WriteAllText("AudioSettings.json", json);
        }

        public void LoadAudioSettings()
        {
            if (File.Exists("AudioSettings.json"))
            {
                string json = File.ReadAllText("AudioSettings.json");
                AudioSettings settings = JsonConvert.DeserializeObject<AudioSettings>(json);

                settings.SetAudioManager(audioManager);
            }
            else
                SaveAudioSettings();
        }

        // Informs the screen manager to serialize its state to disk.
        public void Deactivate()
        {
#if WINDOWS_PHONE
            // Open up isolated storage
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                // Create an XML document to hold the list of screen types currently in the stack
                XDocument doc = new XDocument();
                XElement root = new XElement("ScreenManager");
                doc.Add(root);

                // Make a copy of the master screen list, to avoid confusion if
                // the process of deactivating one screen adds or removes others.
                tempScreensList.Clear();
                foreach (GameScreen screen in screens)
                    tempScreensList.Add(screen);

                // Iterate the screens to store in our XML file and deactivate them
                foreach (GameScreen screen in tempScreensList)
                {
                    // Only add the screen to our XML if it is serializable
                    if (screen.IsSerializable)
                    {
                        // We store the screen's controlling player so we can rehydrate that value
                        string playerValue = screen.ControllingPlayer.HasValue
                            ? screen.ControllingPlayer.Value.ToString()
                            : "";

                        root.Add(new XElement(
                            "GameScreen",
                            new XAttribute("Type", screen.GetType().AssemblyQualifiedName),
                            new XAttribute("ControllingPlayer", playerValue)));
                    }

                    // Deactivate the screen regardless of whether we serialized it
                    screen.Deactivate();
                }

                // Save the document
                using (IsolatedStorageFileStream stream = storage.CreateFile(StateFilename))
                {
                    doc.Save(stream);
                }
            }
#endif
        }

        public bool Activate(bool instancePreserved)
        {
#if !WINDOWS_PHONE
            return false;
#else
            // If the game instance was preserved, the game wasn't dehydrated so our screens still exist.
            // We just need to activate them and we're ready to go.
            if (instancePreserved)
            {
                // Make a copy of the master screen list, to avoid confusion if
                // the process of activating one screen adds or removes others.
                tempScreensList.Clear();

                foreach (GameScreen screen in screens)
                    tempScreensList.Add(screen);

                foreach (GameScreen screen in tempScreensList)
                    screen.Activate(true);
            }

            // Otherwise we need to refer to our saved file and reconstruct the screens that were present
            // when the game was deactivated.
            else
            {
                // Try to get the screen factory from the services, which is required to recreate the screens
                IScreenFactory screenFactory = Game.Services.GetService(typeof(IScreenFactory)) as IScreenFactory;
                if (screenFactory == null)
                {
                    throw new InvalidOperationException(
                        "Game.Services must contain an IScreenFactory in order to activate the ScreenManager.");
                }

                // Open up isolated storage
                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    // Check for the file; if it doesn't exist we can't restore state
                    if (!storage.FileExists(StateFilename))
                        return false;

                    // Read the state file so we can build up our screens
                    using (IsolatedStorageFileStream stream = storage.OpenFile(StateFilename, FileMode.Open))
                    {
                        XDocument doc = XDocument.Load(stream);

                        // Iterate the document to recreate the screen stack
                        foreach (XElement screenElem in doc.Root.Elements("GameScreen"))
                        {
                            // Use the factory to create the screen
                            Type screenType = Type.GetType(screenElem.Attribute("Type").Value);
                            GameScreen screen = screenFactory.CreateScreen(screenType);

                            // Rehydrate the controlling player for the screen
                            PlayerIndex? controllingPlayer = screenElem.Attribute("ControllingPlayer").Value != ""
                                ? (PlayerIndex)Enum.Parse(typeof(PlayerIndex), screenElem.Attribute("ControllingPlayer").Value, true)
                                : (PlayerIndex?)null;
                            screen.ControllingPlayer = controllingPlayer;

                            // Add the screen to the screens list and activate the screen
                            screen.ScreenManager = this;
                            screens.Add(screen);
                            screen.Activate(false);

                            // update the TouchPanel to respond to gestures this screen is interested in
                            TouchPanel.EnabledGestures = screen.EnabledGestures;
                        }
                    }
                }
            }

            return true;
#endif
        }
    }
}