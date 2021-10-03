using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameRPG.StateManagement;
using VoidEngineLight.Animation;

namespace MonoGameRPG.Screens
{
    public class GameplayScreen : GameScreen
    {
        private ContentManager _content;
        private SpriteFont _gameFont;

        private float _pauseAlpha;
        private readonly InputAction _pauseAction;

        protected Sprite playerAvatar;

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

                playerAvatar = new Sprite(spriteSheet, new Point(32, 40), new Point(16, 20));
                playerAvatar.animationPlayer = new SpriteSheetAnimationPlayer(spriteAnimationClips);
                playerAvatar.StartAnimation("Idle");
                
                // once the load has finished, we use ResetElapsedTime to tell the game's
                // timing mechanism that we have just finished a very long frame, and that
                // it should not try to catch up.
                ScreenManager.Game.ResetElapsedTime();
            }
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
                playerAvatar.Update(gameTime);

                float translateSpeed = .5f;

                switch (playerAvatar.animationPlayer.CurrentClip.Name)
                {
                    case "WalkDown":
                        playerAvatar.Position += new Vector2(0, translateSpeed);
                        break;
                    case "WalkLeft":
                        playerAvatar.Position += new Vector2(-translateSpeed, 0);
                        break;
                    case "WalkRight":
                        playerAvatar.Position += new Vector2(translateSpeed, 0);
                        break;
                    case "WalkUp":
                        playerAvatar.Position += new Vector2(0, -translateSpeed);
                        break;
                    case "Idle":
                        break;
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

            spriteBatch.Begin();

            // Draw Background..
            spriteBatch.Draw(_content.Load<Texture2D>("Backgrounds/TestBG"), new Rectangle(0, 0, ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height), null, Color.White);

            playerAvatar.Draw(gameTime, spriteBatch);

            // Draw Foreground..
            spriteBatch.Draw(_content.Load<Texture2D>("Backgrounds/TestFG"), new Rectangle(0, 0, ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height), null, Color.White);

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