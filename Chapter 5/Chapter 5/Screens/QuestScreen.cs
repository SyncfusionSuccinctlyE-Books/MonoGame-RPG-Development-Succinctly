using System;
using System.Collections.Generic;
using MonoGameRPG.StateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RPGEngine;

namespace MonoGameRPG.Screens
{
    // A popup message box screen, used to display "are you sure?" confirmation messages.
    public class QuestScreen : GameScreen
    {
        private Texture2D gradientTexture;
        private Texture2D background;
        private Rectangle backgroundRect;
        private readonly InputAction closeAction;
        private List<Quest> quests;
        private List<AssignedQuest> assignedQuests;
        private SpriteFont font;
        private SpriteFont titleFont;
        private Vector2 headerPosition;
        private Vector2 questNamePosition;
        private Vector2 rewardPosition;
        private Vector2 stepNamePosition;
        private Vector2 stepDescriptionPosition;

        public event EventHandler<PlayerIndexEventArgs> Cancelled;

        public QuestScreen(List<Quest> quests, List<AssignedQuest> assignedQuests)
        {
            IsPopup = true;

            TransitionOnTime = TimeSpan.FromSeconds(0.2);
            TransitionOffTime = TimeSpan.FromSeconds(0.2);

            closeAction = new InputAction(
                new[] { Buttons.B },
                new[] { Keys.Escape }, true);

            this.quests = quests;
            this.assignedQuests = assignedQuests;
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
                background = content.Load<Texture2D>("Backgrounds/scroll");
                backgroundRect = new Rectangle(25, 25, ScreenManager.GraphicsDevice.Viewport.Width - 25, ScreenManager.GraphicsDevice.Viewport.Height - 25);

                font = content.Load<SpriteFont>("conversationfont");
                titleFont = content.Load<SpriteFont>("gamefont");

                headerPosition = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2 - 50, 40);
                questNamePosition = new Vector2(75, 160);
                rewardPosition = new Vector2(75, 180);
                stepNamePosition = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2 - 175, 160);
                stepDescriptionPosition = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2 - 175, 180);
            }
        }

        public override void HandleInput(GameTime gameTime, InputState input)
        {
            PlayerIndex playerIndex;

            if (closeAction.Occurred(input, ControllingPlayer, out playerIndex))
            {
                Cancelled?.Invoke(this, new PlayerIndexEventArgs(playerIndex));
                ExitScreen();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            var spriteBatch = ScreenManager.SpriteBatch;

            // Darken down any other screens that were drawn beneath the popup.
            ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 2 / 3);

            // Center the message text in the viewport.
            var viewport = ScreenManager.GraphicsDevice.Viewport;
            var viewportSize = new Vector2(viewport.Width, viewport.Height);

            var color = Color.White * TransitionAlpha;    // Fade the popup alpha during transitions

            spriteBatch.Begin();

            spriteBatch.Draw(gradientTexture, viewportSize, color);
            spriteBatch.Draw(background, backgroundRect, Color.White);

            spriteBatch.DrawString(titleFont, "Quests", headerPosition, Color.Black);

            if (quests != null)
            {
                for(int i = 0; i < quests.Count; i++)
                {
                    spriteBatch.DrawString(font, quests[i].Name, new Vector2(questNamePosition.X, questNamePosition.Y + (i * 25)), Color.Black);
                    if(quests[i].IsRewardShown)
                    {
                        //once items are added we'll look up the item name if reward is an item
                        spriteBatch.DrawString(font, "Reward: " + (quests[i].RewardType == QuestRewardType.Money ? "$" + quests[i].RewardItemID.ToString() : "Item - " + quests[i].RewardItemID.ToString()),
                            new Vector2(rewardPosition.X, rewardPosition.Y + (i * quests[i].Steps.Count * 25)), Color.Black);
                    }

                    for(int i2 = 0; i2 < quests[i].Steps.Count; i2++)
                    {
                        if (assignedQuests[i].CurStep >= i2)
                        {
                            if (assignedQuests[i].CurStep > i2)
                            {
                                spriteBatch.DrawString(font, "X", new Vector2(stepNamePosition.X - 25, stepNamePosition.Y + (i2 * 25)), Color.Black);
                            }

                            spriteBatch.DrawString(font, quests[i].Steps[i2].JournalEntry, new Vector2(stepNamePosition.X, stepNamePosition.Y + (i2 * 25)), Color.Black);
                        }
                    }
                }
            }

            spriteBatch.End();
        }
    }
}