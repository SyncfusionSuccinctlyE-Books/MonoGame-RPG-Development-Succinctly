using System;
using MonoGameRPG.StateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGameRPG.Screens
{
    // A popup message box screen, used to display "are you sure?" confirmation messages.
    public class MessageBoxScreen : GameScreen
    {
        private readonly string message;
        private Texture2D gradientTexture;
        private readonly InputAction menuSelect;
        private readonly InputAction menuCancel;

        public event EventHandler<PlayerIndexEventArgs> Accepted;
        public event EventHandler<PlayerIndexEventArgs> Cancelled;

        // Constructor lets the caller specify whether to include the standard
        // "A=ok, B=cancel" usage text prompt.
        public MessageBoxScreen(string message, bool includeUsageText = true)
        {
            const string usageText = "\nA button, Space, Enter = ok" +
                                     "\nB button, Backspace = cancel"; 
            
            if (includeUsageText)
                this.message = message + usageText;
            else
                this.message = message;

            IsPopup = true;

            TransitionOnTime = TimeSpan.FromSeconds(0.2);
            TransitionOffTime = TimeSpan.FromSeconds(0.2);

            menuSelect = new InputAction(
                new[] { Buttons.A, Buttons.Start },
                new[] { Keys.Enter, Keys.Space }, true);
            menuCancel = new InputAction(
                new[] { Buttons.B, Buttons.Back },
                new[] { Keys.Back }, true);
        }

        // Loads graphics content for this screen. This uses the shared ContentManager
        // provided by the Game class, so the content will remain loaded forever.
        // Whenever a subsequent MessageBoxScreen tries to load this same content,
        // it will just get back another reference to the already loaded data.
        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                var content = ScreenManager.Game.Content;
                gradientTexture = content.Load<Texture2D>("gradient");
            }
        }

        public override void HandleInput(GameTime gameTime, InputState input)
        {
            PlayerIndex playerIndex;

            // We pass in our ControllingPlayer, which may either be null (to
            // accept input from any player) or a specific index. If we pass a null
            // controlling player, the InputState helper returns to us which player
            // actually provided the input. We pass that through to our Accepted and
            // Cancelled events, so they can tell which player triggered them.
            if (menuSelect.Occurred(input, ControllingPlayer, out playerIndex))
            {
                Accepted?.Invoke(this, new PlayerIndexEventArgs(playerIndex));
                ExitScreen();
            }
            else if (menuCancel.Occurred(input, ControllingPlayer, out playerIndex))
            {
                Cancelled?.Invoke(this, new PlayerIndexEventArgs(playerIndex));
                ExitScreen();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            var spriteBatch = ScreenManager.SpriteBatch;
            var font = ScreenManager.Font;

            // Darken down any other screens that were drawn beneath the popup.
            ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 2 / 3);

            // Center the message text in the viewport.
            var viewport = ScreenManager.GraphicsDevice.Viewport;
            var viewportSize = new Vector2(viewport.Width, viewport.Height);
            var textSize = font.MeasureString(message);
            var textPosition = (viewportSize - textSize) / 2;

            // The background includes a border somewhat larger than the text itself.
            const int hPad = 32;
            const int vPad = 16;

            var backgroundRectangle = new Rectangle((int)textPosition.X - hPad, 
                (int)textPosition.Y - vPad, (int)textSize.X + hPad * 2, (int)textSize.Y + vPad * 2);

            var color = Color.White * TransitionAlpha;    // Fade the popup alpha during transitions

            spriteBatch.Begin();

            spriteBatch.Draw(gradientTexture, backgroundRectangle, color);
            spriteBatch.DrawString(font, message, textPosition, color);

            spriteBatch.End();
        }
    }
}