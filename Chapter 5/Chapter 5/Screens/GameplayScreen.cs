using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameRPG.StateManagement;
using MonoGameRPG.Animation;
using RPGEngine;

namespace MonoGameRPG.Screens
{
  
    public class GameplayScreen : GameScreen
    {
        private ContentManager content;
        private SpriteFont gameFont;

        private float pauseAlpha;
        private readonly InputAction pauseAction;
        private readonly InputAction questWindowAction;

        static EntityGameObject character;

        List<EntityGameObject> npcs;
        protected LevelBase currentLevel;
        Rectangle backgroundRect;

        ConversationRenderer conversationRenderer;

        private Texture2D hasQuestIcon;
        private Texture2D questIncompleteIcon;
        private Texture2D notification;

        ConversationManager conversationManager;

        private Texture2D dungeonBackground;

        private bool levelIsTransitioning;

        private double loadingTimer;
        private double notificationTimer;

        private SpriteFont notificationFont;
        private string notificationText;
        private Vector2 notificationPosition;
        private Vector2 notificationBackgroundPosition;

        private QuestManager questManager;


        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            pauseAction = new InputAction(
                new[] { Buttons.Start, Buttons.Back },
                new[] { Keys.Escape }, true);

            questWindowAction = new InputAction(new[] { Buttons.Y }, new[] { Keys.Q }, true);

            levelIsTransitioning = false;
        }

        // Load graphics content for the game
        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                content = Game1.GetContentManager();

                gameFont = content.Load<SpriteFont>("gamefont");

                if (Global.FirstTimeLoad)
                {
                    character = new EntityGameObject(ScreenManager.Game, "Sprites/Test/TestSheet1");
                    character.Type = ObjectType.Character;
                    character.Initialize();
                    character.Entity = new Character();

                    Global.TileSize = new Point(32, 32);

                    character.GameSprite.Position = new Vector2(14 * Global.TileSize.X, 7 * Global.TileSize.Y);
                    Global.FirstTimeLoad = false;
                }

                LoadLevel();


                backgroundRect = new Rectangle(0, 0, ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height);

                hasQuestIcon = content.Load<Texture2D>("Sprites/UI/hasquest");
                questIncompleteIcon = content.Load<Texture2D>("Sprites/UI/questincomplete");
                notification = content.Load<Texture2D>("Sprites/UI/notificationbackground");

                notificationFont = content.Load<SpriteFont>("datafont");
                notificationBackgroundPosition = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2 - notification.Width / 2, 0);
                

                conversationRenderer = new ConversationRenderer();

                Globals.FunctionClasses = new Dictionary<ConversationFunctions, object>();

                Globals.FunctionClasses.Add(ConversationFunctions.CheckKnownNPC, character.Entity);
                Globals.FunctionClasses.Add(ConversationFunctions.AssignQuest, character.Entity);
                Globals.FunctionClasses.Add(ConversationFunctions.QuestAssigned, character.Entity);
                Globals.FunctionClasses.Add(ConversationFunctions.IsQuestCompleted, character.Entity);
                Globals.FunctionClasses.Add(ConversationFunctions.CompleteQuest, character.Entity);

                questManager = new QuestManager((Character)character.Entity);
                questManager.QuestUpdated += QuestUpdated;

                EventSystem.QuestAssigned += QuestAssigned;

                ScreenManager.Game.ResetElapsedTime();
            }
        }

        private void ShowNotification(string text)
        {
            notificationTimer = 3000;
            notificationText = text;
            notificationPosition = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2 - notificationFont.MeasureString(notificationText).X / 2, 5);
        }

        private void QuestAssigned(EventSystemEventArgs e)
        {
            ShowNotification("Quest Assigned - " + QuestManager.GetQuestName(Convert.ToInt32(e.ObjectID)));
        }

        private void QuestUpdated(QuestEventArgs e)
        {
            notificationTimer = 3000;
            notificationText = "Quest Updated - " + e.Text;
            notificationPosition = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2 - notificationFont.MeasureString(notificationText).X / 2, 5);
        }

        private void LoadLevel()
        {
            levelIsTransitioning = true;
            loadingTimer = 0.0;

            switch (Global.CurrentLevelName)
            {
                case "Town":
                    Global.TileSize = new Point(32, 32);
                    currentLevel = new TownLevel(content, "Sprites/SP-Tileset", "Levels/Town/TerrainMap32", "Levels/Town/TerrainOverlayMap32", "Levels/Town/BuildingMap32", null, Global.TileSize, new Point(16, 16));
                    break;
                case "Inn":
                    Global.TileSize = new Point(32, 32);
                    currentLevel = new InnLevel(content, "Sprites/basictiles", "Levels/Inn/InnFloorPlan32", "Levels/Inn/InnWallPlan32", "Levels/Inn/InnObjectsPlan32", null, Global.TileSize, new Point(16, 16));
                    break;
                case "AreaMap":
                    Global.TileSize = new Point(16, 16);
                    currentLevel = new AreaLevel(content, "Sprites/SP-Tileset", "Levels/Area/map16", "Levels/Area/overlay16", "Levels/Area/structure16", null, Global.TileSize, new Point(16, 16));
                    break;
                case "Dungeon":
                    Global.TileSize = new Point(32, 32);
                    currentLevel = new Dungeon(content, "Sprites/Tileset", "Levels/Dungeon/map32", "Levels/Dungeon/overlay32", "Levels/Dungeon/structure32", null, Global.TileSize, new Point(32, 32));

                    if(dungeonBackground == null)
                    {
                        dungeonBackground = content.Load<Texture2D>("Sprites/Dirt");
                    }

                    break;
            }

            character.GameSprite.Size = Global.TileSize;
            currentLevel.PlayerReference = character.GameSprite;

            npcs = new List<EntityGameObject>();
            npcs.AddRange(GameObject.LoadNPCs());

            foreach (EntityGameObject obj in npcs)
            {
                obj.Initialize(obj.GameSpriteFileName);
                obj.NPCClicked += NPCClicked;
                obj.EntityKilled += EntityKilled;
            }

            if (Global.CurrentLevelName == "Town")
            {
                ((Entity)npcs[0].Entity).AddConversation(ConversationManager.GetConversation("1"));
                ((Entity)npcs[0].Entity).AddQuest(1);
            }
        }

        private void EntityKilled(EntityKilledEventArgs e)
        {
            EntityGameObject entity = npcs.Find(n => ((Entity)n.Entity).ID == e.ID);

            //add to character kill count
            ((Character)character.Entity).AddQuestItem(((Entity)entity.Entity).Name);

            //remove entity from list so it's not rendered anymore
            npcs.Remove(entity);

            EventSystem.OnEntityKilled(new EventSystemEventArgs { ObjectID = e.ID, Tag = ((Entity)entity.Entity).Name });
        }

        private void NPCClicked(NPCClickedEventArgs e)
        {
            Entity entity = (Entity)npcs.Find(npc => ((Entity)npc.Entity).ID == e.ID).Entity;

            if (conversationManager == null)
                conversationManager = new ConversationManager();

            conversationManager.LoadConversation(e.ConversationID, ((Entity)character.Entity), entity);
            conversationManager.StartConversation(e.ConversationID);
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
                if (!levelIsTransitioning)
                {
                    currentLevel.Update(gameTime);

                    float translateSpeed = .5f;

                    switch (character.GameSprite.animationPlayer.CurrentClip.Name)
                    {
                        case "WalkDown":
                            if (!currentLevel.IsSolid(character.GameSprite.Position + new Vector2(0, translateSpeed)))
                                character.GameSprite.Position += new Vector2(0, translateSpeed);
                            break;
                        case "WalkLeft":
                            if (!currentLevel.IsSolid(character.GameSprite.Position + new Vector2(-translateSpeed, 0)))
                                character.GameSprite.Position += new Vector2(-translateSpeed, 0);
                            break;
                        case "WalkRight":
                            if (!currentLevel.IsSolid(character.GameSprite.Position + new Vector2(translateSpeed, 0)))
                                character.GameSprite.Position += new Vector2(translateSpeed, 0);
                            break;
                        case "WalkUp":
                            if (!currentLevel.IsSolid(character.GameSprite.Position + new Vector2(0, -translateSpeed)))
                                character.GameSprite.Position += new Vector2(0, -translateSpeed);
                            break;
                        case "Idle":
                            break;
                    }

                    TileData exitTo = currentLevel.IsExitTo(character.GameSprite.Position);
                    if (exitTo != null)
                    {
                        Global.CurrentLevelName = exitTo.ExitTo;
                        LoadLevel();
                        character.GameSprite.Position = new Vector2(exitTo.EnterIn.X * Global.TileSize.X, exitTo.EnterIn.Y * Global.TileSize.Y);
                    }

                    character.GameSprite.Position = Vector2.Min(new Vector2(ScreenManager.GraphicsDevice.Viewport.Width - character.GameSprite.Size.X, ScreenManager.GraphicsDevice.Viewport.Height - character.GameSprite.Size.Y), Vector2.Max(Vector2.Zero, character.GameSprite.Position));
                }
                else
                {
                    loadingTimer += gameTime.ElapsedGameTime.TotalMilliseconds;

                    if(loadingTimer >= 500)
                    {
                        levelIsTransitioning = false;
                    }
                }

                if(!string.IsNullOrEmpty(notificationText))
                {
                    notificationTimer -= gameTime.ElapsedGameTime.TotalMilliseconds;

                    if(notificationTimer <= 0)
                    {
                        notificationText = "";
                    }
                }
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
            else if (questWindowAction.Occurred(input, ControllingPlayer, out player))
            {
                ScreenManager.AddScreen(new QuestScreen(((Character)character.Entity).GetQuests(), ((Character)character.Entity).AssignedQuests), ControllingPlayer);
            }
            else
            {
                if (!levelIsTransitioning)
                {
                    if (conversationManager == null || !conversationManager.IsActive)
                    {
                        //no need to call this for any game object other than entities
                        character.HandleInput(input, ControllingPlayer, ControllingPlayer.Value);
                    }
                    else if (conversationManager != null)
                    {
                        conversationManager.HandleInput(input, ControllingPlayer, ControllingPlayer.Value);
                    }

                    if (input.IsKeyPressed(Keys.Down, ControllingPlayer, out player))
                        character.GameSprite.animationPlayer.StartClip("WalkDown");
                    else if (input.IsKeyPressed(Keys.Up, ControllingPlayer, out player))
                        character.GameSprite.animationPlayer.StartClip("WalkUp");
                    else if (input.IsKeyPressed(Keys.Left, ControllingPlayer, out player))
                        character.GameSprite.animationPlayer.StartClip("WalkLeft");
                    else if (input.IsKeyPressed(Keys.Right, ControllingPlayer, out player))
                        character.GameSprite.animationPlayer.StartClip("WalkRight");
                    else
                        character.GameSprite.animationPlayer.StartClip("Idle");

                    for (int i = 0; i < npcs.Count; i++)
                    {
                        npcs[i].HandleInput(input, ControllingPlayer, ControllingPlayer.Value);
                    }
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 0, 0);

            var spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.DepthRead);
            if (!levelIsTransitioning)
            {
                if (Global.CurrentLevelName == "Dungeon")
                    spriteBatch.Draw(dungeonBackground, new Rectangle(0, 0, ScreenManager.Game.GraphicsDevice.Viewport.Width, ScreenManager.Game.GraphicsDevice.Viewport.Height), new Color(.25f, .25f, .25f, .25f));

                currentLevel.Draw(gameTime, spriteBatch);

                foreach (GameObject obj in npcs)
                {
                    obj.Draw(gameTime, spriteBatch);

                    int id;

                    if (obj.Type == ObjectType.Entity && ((EntityGameObject)obj).EntityType == EntityType.NPC && Global.CharacterInRangeofNPC(character, obj) && ((Entity)((EntityGameObject)obj).Entity).HasConversation(out id))
                    {
                        //check range and show icon if applicable
                        spriteBatch.Draw(hasQuestIcon, new Rectangle((int)obj.GameSprite.Position.X, (int)obj.GameSprite.Position.Y - hasQuestIcon.Height - 5, hasQuestIcon.Width, hasQuestIcon.Height), Color.White);
                    }
                }

                if (conversationManager != null && conversationManager.IsActive)
                    conversationRenderer.Render(spriteBatch, conversationManager.GetCurrentNode());
            }
            else
            {
                const string message = "Loading...";

                // Center the text in the viewport.
                var viewport = ScreenManager.GraphicsDevice.Viewport;
                var viewportSize = new Vector2(viewport.Width, viewport.Height);
                var textSize = ScreenManager.Font.MeasureString(message);
                var textPosition = (viewportSize - textSize) / 2;

                spriteBatch.DrawString(ScreenManager.Font, message, textPosition, Color.LightGray);
            }

            if(notificationTimer > 0)
            {
                spriteBatch.Draw(notification, notificationBackgroundPosition, Color.White);
                spriteBatch.DrawString(notificationFont, notificationText, notificationPosition, Color.Black);
            }

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