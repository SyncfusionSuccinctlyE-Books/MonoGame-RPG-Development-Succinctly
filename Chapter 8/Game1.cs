using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using MonoGameRPG.Screens;
using MonoGameRPG.StateManagement;

using VoidEngineLight.Managers;

namespace MonoGameRPG
{
    // Sample showing how to manage different game states, with transitions
    // between menu screens, a loading screen, the game itself, and a pause
    // menu. This main game class is extremely simple: all the interesting
    // stuff happens in the ScreenManager component.
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private readonly ScreenManager _screenManager;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            AudioManager audioManager = new AudioManager(this);

            var screenFactory = new ScreenFactory();
            Services.AddService(typeof(IScreenFactory), screenFactory);

            _screenManager = new ScreenManager(this);
            _screenManager.BackgroundSongAsset = "Audio/Music/Misty-Bog_remixed";
            Components.Add(_screenManager);
            
            AddInitialScreens();
        }
        
        private void AddInitialScreens()
        {
            _screenManager.AddScreen(new BackgroundScreen(), null);
            _screenManager.AddScreen(new MainMenuScreen(), null);
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent() {}

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            base.Draw(gameTime);    // The real drawing happens inside the ScreenManager component
        }
    }
}
