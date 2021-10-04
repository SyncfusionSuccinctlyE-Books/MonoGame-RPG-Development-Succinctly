using MonoGameRPG;
using MonoGameRPG.StateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using RPGEngine;

namespace MonoGameRPG.Screens
{
    public class ConversationRenderer
    {
        private Texture2D background;
        private Rectangle rect;
        private Vector2 conversationLine;

        private SpriteFont font;

        public ConversationRenderer()
        {
            ContentManager content = Game1.GetContentManager();

            background = content.Load<Texture2D>("Sprites/UI/conversationbackground");
            rect = new Rectangle(0, Game1.GetScreenManager().GraphicsDevice.Viewport.Height - 100, Game1.GetScreenManager().GraphicsDevice.Viewport.Width, 100);
            font = content.Load<SpriteFont>("conversationfont");

            conversationLine = new Vector2(rect.X + 15, rect.Y + 5);
        }

        public void Render(SpriteBatch spriteBatch, ConversationNode curNode)
        {
            if (curNode != null)
            {
                spriteBatch.Draw(background, rect, Color.White);

                spriteBatch.DrawString(font, curNode.Text, conversationLine, Color.Black);

                int y = (int)conversationLine.Y + 15;
                int x = (int)conversationLine.X + 20;

                int i = 0;

                if (curNode.Responses != null)
                {
                    foreach (ConversationNode node in curNode.Responses)
                    {
                        spriteBatch.DrawString(font, (i + 1).ToString() + ")  " + node.Text, new Vector2(x, y + (20 * i)), Color.Black);
                        i++;
                    }
                }
                else
                {
                    spriteBatch.DrawString(font, (i + 1).ToString() + ")  Leave conversation.", new Vector2(x, y + (20 * i)), Color.Black);
                }
            }
        }
    }
}
