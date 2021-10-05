using MonoGameRPG.Screens;
using MonoGameRPG.StateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace MonoGameRPG
{
    // Sample showing how to manage different game states, with transitions
    // between menu screens, a loading screen, the game itself, and a pause
    // menu. This main game class is extremely simple: all the interesting
    // stuff happens in the ScreenManager component.
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private readonly ScreenManager screenManager;

        private static Game1 instance;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            //graphics.PreferredBackBufferWidth = 1280;
            //graphics.PreferredBackBufferHeight = 720;

            var screenFactory = new ScreenFactory();
            Services.AddService(typeof(IScreenFactory), screenFactory);

            screenManager = new ScreenManager(this);
            Components.Add(screenManager);
            
            AddInitialScreens();

            instance = this;
        }
        
        public static ScreenManager GetScreenManager()
        {
            return instance.screenManager;
        }

        public static ContentManager GetContentManager()
        {
            return instance.Content;
        }

        public static int GameWidth()
        {
            return instance.screenManager.GraphicsDevice.Viewport.Width;
        }

        public static int GameHeight()
        {
            return instance.screenManager.GraphicsDevice.Viewport.Height;
        }

        private void AddInitialScreens()
        {
            screenManager.AddScreen(new MainMenuScreen(), null);
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent() {}

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            base.Draw(gameTime);    // The real drawing happens inside the ScreenManager component
        }
    }
}
