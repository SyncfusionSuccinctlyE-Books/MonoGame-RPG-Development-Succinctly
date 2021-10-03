using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameRPG.StateManagement;
using VoidEngineLight.Animation;
using MonoGameRPG;

namespace MonoGameRPG.Screens
{

    // refs: https://itch.io/game-assets/free/tag-top-down
    public class GameplayScreen : GameScreen
    {
        private ContentManager _content;
        private SpriteFont _gameFont;

        private float _pauseAlpha;
        private readonly InputAction _pauseAction;

        protected Sprite playerAvatar;
        protected LevelBase currentLevel;

        public string currentLevelName = "Town";
        public Vector2 playerPos { get; set; }

        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            _pauseAction = new InputAction(
                new[] { Buttons.Start, Buttons.Back },
                new[] { Keys.Back }, true);
        }

        // Load graphics content for the game
        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                if (_content == null)
                    _content = new ContentManager(ScreenManager.Game.Services, "Content");

                _gameFont = _content.Load<SpriteFont>("gamefont");


                Point size = new Point(32, 32);
                
                if (playerPos == Vector2.Zero) // Initial position.
                    playerPos = new Vector2(14, 7);

                // For testing...
                //if (currentLevelName == "Town")
                //{
                //    currentLevelName = "Dungeon";
                //    playerPos = new Vector2(3, 1);
                //}

                switch (currentLevelName)
                {
                    case "Town":
                        size = new Point(32, 32);
                        currentLevel = new TownLevel(_content, "Sprites/SP-Tileset", "Levels/Town/TerrainMap32", "Levels/Town/TerrainOverlayMap32", "Levels/Town/BuildingMap32", null, size, new Point(16, 16));
                        GeneratePlayerAvatar(size);
                        playerAvatar.Position = new Vector2(playerPos.X * size.X, playerPos.Y * size.Y);
                        break;
                    case "Inn":
                        size = new Point(32, 32);
                        currentLevel = new InnLevel(_content, "Sprites/basictiles", "Levels/Inn/InnFloorPlan32", "Levels/Inn/InnWallPlan32", "Levels/Inn/InnObjectsPlan32", null, size, new Point(16, 16));
                        GeneratePlayerAvatar(size);
                        playerAvatar.Position = new Vector2(playerPos.X * size.X, playerPos.Y * size.Y);
                        break;
                    case "AreaMap":
                        size = new Point(16, 16);
                        currentLevel = new AreaLevel(_content, "Sprites/SP-Tileset", "Levels/Area/map16", "Levels/Area/overlay16", "Levels/Area/structure16", null, size, new Point(16, 16));
                        GeneratePlayerAvatar(size);
                        playerAvatar.Position = new Vector2(playerPos.X * size.X, playerPos.Y * size.Y);
                        break;
                    case "Dungeon":
                        size = new Point(32, 32);
                        currentLevel = new Dungeon(_content, "Sprites/Tileset", "Levels/Dungeon/map32", "Levels/Dungeon/overlay32", "Levels/Dungeon/structure32", null, size, new Point(32, 32));                        
                        GeneratePlayerAvatar(size);
                        playerAvatar.Position = new Vector2(playerPos.X * size.X, playerPos.Y * size.Y);
                        break;
                }
                
                currentLevel.PlayerReference = playerAvatar;

                // once the load has finished, we use ResetElapsedTime to tell the game's
                // timing mechanism that we have just finished a very long frame, and that
                // it should not try to catch up.
                ScreenManager.Game.ResetElapsedTime();
            }
        }

        protected void GeneratePlayerAvatar(Point size)
        {
            Texture2D spriteSheet = _content.Load<Texture2D>("Sprites/Test/TestSheet1");
            SpriteAnimationClipGenerator sacg = new SpriteAnimationClipGenerator(new Vector2(spriteSheet.Width, spriteSheet.Height), new Vector2(2, 4));

            Dictionary<string, SpriteSheetAnimationClip> spriteAnimationClips = new Dictionary<string, SpriteSheetAnimationClip>()
                {
                    { "Idle", sacg.Generate("Idle", new Vector2(1, 0), new Vector2(1, 0), new TimeSpan(0, 0, 0, 0, 500), true) },
                    { "WalkDown", sacg.Generate("WalkDown", new Vector2(0, 0), new Vector2(1, 0), new TimeSpan(0, 0, 0, 0, 500), true) },
                    { "WalkLeft", sacg.Generate("WalkLeft", new Vector2(0, 1), new Vector2(1, 1), new TimeSpan(0, 0, 0, 0, 500), true) },
                    { "WalkRight", sacg.Generate("WalkRight", new Vector2(0, 2), new Vector2(1, 2), new TimeSpan(0, 0, 0, 0, 500), true) },
                    { "WalkUp", sacg.Generate("WalkUp", new Vector2(0, 3), new Vector2(1, 3), new TimeSpan(0, 0, 0, 0, 500), true) },
                };

            playerAvatar = new Sprite(spriteSheet, size, new Point(16, 20));
            playerAvatar.animationPlayer = new SpriteSheetAnimationPlayer(spriteAnimationClips);
            playerAvatar.StartAnimation("Idle");
            playerAvatar.Position = new Vector2(ScreenManager.Game.GraphicsDevice.Viewport.Width / 2, ScreenManager.Game.GraphicsDevice.Viewport.Height / 2);
        }


        protected override void Deactivate()
        {
            base.Deactivate();
        }

        public override void Unload()
        {
            _content.Unload();
        }

        // This method checks the GameScreen.IsActive property, so the game will
        // stop updating when the pause menu is active, or if you tab away to a different application.
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                _pauseAlpha = Math.Min(_pauseAlpha + 1f / 32, 1);
            else
                _pauseAlpha = Math.Max(_pauseAlpha - 1f / 32, 0);

            if (IsActive)
            {
                currentLevel.Update(gameTime);

                float translateSpeed = .5f;

                switch (playerAvatar.animationPlayer.CurrentClip.Name)
                {
                    case "WalkDown":
                        if (!currentLevel.IsSolid(playerAvatar.Position + new Vector2(0, translateSpeed)))
                            playerAvatar.Position += new Vector2(0, translateSpeed);
                        break;
                    case "WalkLeft":
                        if (!currentLevel.IsSolid(playerAvatar.Position + new Vector2(-translateSpeed, 0)))
                            playerAvatar.Position += new Vector2(-translateSpeed, 0);
                        break;
                    case "WalkRight":
                        if (!currentLevel.IsSolid(playerAvatar.Position + new Vector2(translateSpeed, 0)))
                            playerAvatar.Position += new Vector2(translateSpeed, 0);
                        break;
                    case "WalkUp":
                        if (!currentLevel.IsSolid(playerAvatar.Position + new Vector2(0, -translateSpeed)))
                            playerAvatar.Position += new Vector2(0, -translateSpeed);
                        break;
                    case "Idle":
                        break;
                }

                TileData exitTo = currentLevel.IsExitTo(playerAvatar.Position);
                if (exitTo != null)
                {
                    // Transition to exit.
                    LoadingScreen.Load(ScreenManager, true,PlayerIndex.One, new GameplayScreen() { ScreenManager = ScreenManager, currentLevelName = exitTo.ExitTo, playerPos = exitTo.EnterIn });
                }

                playerAvatar.Position = Vector2.Min(new Vector2(ScreenManager.GraphicsDevice.Viewport.Width - playerAvatar.Size.X, ScreenManager.GraphicsDevice.Viewport.Height - playerAvatar.Size.Y), Vector2.Max(Vector2.Zero, playerAvatar.Position));
            }
        }

        // Unlike the Update method, this will only be called when the gameplay screen is active.
        public override void HandleInput(GameTime gameTime, InputState input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;

            var keyboardState = input.CurrentKeyboardStates[playerIndex];
            var gamePadState = input.CurrentGamePadStates[playerIndex];

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !gamePadState.IsConnected && input.GamePadWasConnected[playerIndex];

            PlayerIndex player;
            if (_pauseAction.Occurred(input, ControllingPlayer, out player) || gamePadDisconnected)
            { 
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
            }
            else
            {
                if (input.IsKeyPressed(Keys.Down, ControllingPlayer, out player))
                    playerAvatar.animationPlayer.StartClip("WalkDown");
                else if (input.IsKeyPressed(Keys.Up, ControllingPlayer, out player))
                    playerAvatar.animationPlayer.StartClip("WalkUp");
                else if (input.IsKeyPressed(Keys.Left, ControllingPlayer, out player))
                    playerAvatar.animationPlayer.StartClip("WalkLeft");
                else if (input.IsKeyPressed(Keys.Right, ControllingPlayer, out player))
                    playerAvatar.animationPlayer.StartClip("WalkRight");
                else
                    playerAvatar.animationPlayer.StartClip("Idle");
            }
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 0, 0);

            var spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.DepthRead);

            if (currentLevelName == "Dungeon")
                spriteBatch.Draw(_content.Load<Texture2D>("Sprites/Dirt"), new Rectangle(0, 0, ScreenManager.Game.GraphicsDevice.Viewport.Width, ScreenManager.Game.GraphicsDevice.Viewport.Height), new Color(.25f,.25f,.25f,.25f));

            // Draw Background..
            //spriteBatch.Draw(_content.Load<Texture2D>("Backgrounds/TestBG"), new Rectangle(0, 0, ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height), null, Color.White);

            //playerAvatar.Draw(gameTime, spriteBatch);
            currentLevel.Draw(gameTime, spriteBatch);

            // Draw Foreground..
            //spriteBatch.Draw(_content.Load<Texture2D>("Backgrounds/TestFG"), new Rectangle(0, 0, ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height), null, Color.White);

            spriteBatch.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || _pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, _pauseAlpha / 2);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }
        }
    }
}
