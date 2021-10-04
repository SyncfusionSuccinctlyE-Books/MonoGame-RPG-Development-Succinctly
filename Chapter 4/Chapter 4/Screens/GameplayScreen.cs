using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameRPG.StateManagement;
using RPGEngine;

namespace MonoGameRPG.Screens
{
  
    public class GameplayScreen : GameScreen
    {
        private ContentManager content;
        private SpriteFont gameFont;

        private float pauseAlpha;
        private readonly InputAction pauseAction;

        EntityGameObject character;
        List<EntityGameObject> npcs;

        Rectangle backgroundRect;

        ConversationRenderer conversationRenderer;


        private Texture2D hasQuestIcon;
        private Texture2D questIncompleteIcon;

        ConversationManager conversationManager;

        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            pauseAction = new InputAction(
                new[] { Buttons.Start, Buttons.Back },
                new[] { Keys.Back }, true);
        }

        // Load graphics content for the game
        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                content = Game1.GetContentManager();

                gameFont = content.Load<SpriteFont>("gamefont");

                character = new EntityGameObject(ScreenManager.Game, "Sprites/Test/TestSheet1");
                character.Type = ObjectType.Character;
                character.Initialize();
                character.Entity = new Character();

                npcs = new List<EntityGameObject>();
                npcs.AddRange(GameObject.LoadNPCs());

                foreach (EntityGameObject obj in npcs)
                { 
                    obj.Initialize(ScreenManager.Game, obj.GameSpriteFileName);
                    obj.NPCClicked += NPCClicked;
                }

                ((Entity)npcs[0].Entity).AddConversation(ConversationManager.LoadConversation("1"));
                ((Entity)npcs[0].Entity).AddQuest(1);

                backgroundRect = new Rectangle(0, 0, ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height);

                hasQuestIcon = content.Load<Texture2D>("Sprites/UI/hasquest");
                questIncompleteIcon = content.Load<Texture2D>("Sprites/UI/questincomplete");

                conversationRenderer = new ConversationRenderer();

                Globals.FunctionClasses = new Dictionary<ConversationFunctions, object>();

                Globals.FunctionClasses.Add(ConversationFunctions.CheckKnownNPC, character.Entity);
                Globals.FunctionClasses.Add(ConversationFunctions.AssignQuest, character.Entity);
                Globals.FunctionClasses.Add(ConversationFunctions.QuestAssigned, character.Entity);
                Globals.FunctionClasses.Add(ConversationFunctions.CheckForQuestCompletion, character.Entity);
                Globals.FunctionClasses.Add(ConversationFunctions.CompleteQuest, character.Entity);


                // once the load has finished, we use ResetElapsedTime to tell the game's
                // timing mechanism that we have just finished a very long frame, and that
                // it should not try to catch up.
                ScreenManager.Game.ResetElapsedTime();
            }
        }

        private void NPCClicked(NPCClickedEventArgs e)
        {
            Entity entity = (Entity)npcs.Find(npc => ((Entity)npc.Entity).ID == e.ID).Entity;

            if (conversationManager == null)
            {
                conversationManager = new ConversationManager(((Entity)npcs.Find(npc => ((Entity)npc.Entity).ID == e.ID).Entity).GetConversation(e.ConversationID), ((Entity)character.Entity), entity);
            }

            conversationManager.Start();
            conversationManager.IsActive = true;
        }

        protected override void Deactivate()
        {
            base.Deactivate();
        }

        public override void Unload()
        {
            content.Unload();
        }

        // This method checks the GameScreen.IsActive property, so the game will
        // stop updating when the pause menu is active, or if you tab away to a different application.
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);

            if (IsActive)
            {
                character.Update(gameTime);

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
            if (pauseAction.Occurred(input, ControllingPlayer, out player) || gamePadDisconnected)
            { 
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
            }
            else
            {
                if (conversationManager != null && !conversationManager.IsActive)
                {
                    //no need to call this for any game object other than entities
                    character.HandleInput(input, ControllingPlayer, ControllingPlayer.Value);
                }
                else if(conversationManager != null)
                {
                    conversationManager.HandleInput(input, ControllingPlayer, ControllingPlayer.Value);
                }
                    
                foreach (EntityGameObject obj in npcs)
                {
                    obj.HandleInput(input, ControllingPlayer, ControllingPlayer.Value);
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 0, 0);

            var spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            // Draw Background..
            spriteBatch.Draw(content.Load<Texture2D>("Backgrounds/TestBG"), backgroundRect, null, Color.White);

            character.Draw(gameTime, spriteBatch);

            foreach (GameObject obj in npcs)
            {
                obj.Draw(gameTime, spriteBatch);

                if(obj.Type == ObjectType.Entity && Global.CharacterInRangeofNPC(character, obj))
                {
                    //check range and show icon if applicable
                    spriteBatch.Draw(hasQuestIcon, new Rectangle((int)obj.GameSprite.Position.X, (int)obj.GameSprite.Position.Y - hasQuestIcon.Height - 5, hasQuestIcon.Width, hasQuestIcon.Height), Color.White);
                }
            }

            // Draw Foreground..
            spriteBatch.Draw(content.Load<Texture2D>("Backgrounds/TestFG"), backgroundRect, null, Color.White);

            if (conversationManager != null && conversationManager.IsActive)
                conversationRenderer.Render(spriteBatch, conversationManager.GetCurrentNode());

            spriteBatch.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }
        }
    }
}