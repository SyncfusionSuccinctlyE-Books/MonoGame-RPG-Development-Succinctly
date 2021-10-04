using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace MonoGameRPG.StateManagement
{
    // The screen manager is a component which manages one or more GameScreen
    // instances. It maintains a stack of screens, calls their Update and Draw
    // methods at the appropriate times, and automatically routes input to the
    // topmost active screen.
    public class ScreenManager : DrawableGameComponent
    {
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

    }
}