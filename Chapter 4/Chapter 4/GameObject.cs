using System;
using System.IO;
using System.Collections.Generic;
using RPGEngine;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoidEngineLight.Animation;
using MonoGameRPG.StateManagement;
using Newtonsoft.Json;

namespace MonoGameRPG
{
    public enum ObjectType
    {
        Building,           //man-made structure differs from WorldObject
        Entity,
        WorldObject,        //natural, non-item
        Item,
        Character
    }
    public class NPCClickedEventArgs
    {
        public int ID;
        public int ConversationID;
    }

    public delegate void NPCClickedEventHandler(NPCClickedEventArgs e);

    public class GameObject
    {
        public string GameSpriteFileName;
        public Sprite GameSprite;

        public ObjectType Type;

        public Vector2 StartLocation;


        public event NPCClickedEventHandler NPCClicked;

        public GameObject()
        {

        }

        public GameObject(Game game, string fileName)
        {
        }

        public void Initialize(Game game, string fileName)
        {
            Texture2D spriteSheet = Game1.GetContentManager().Load<Texture2D>(fileName);
            SpriteAnimationClipGenerator sacg = new SpriteAnimationClipGenerator(new Vector2(spriteSheet.Width, spriteSheet.Height), new Vector2(2, 4));

            Dictionary<string, SpriteSheetAnimationClip> spriteAnimationClips = new Dictionary<string, SpriteSheetAnimationClip>()
            {
                { "Idle", sacg.Generate("Idle", new Vector2(1, 0), new Vector2(1, 0), new TimeSpan(0, 0, 0, 0, 500), true) },
                { "WalkDown", sacg.Generate("WalkDown", new Vector2(0, 0), new Vector2(1, 0), new TimeSpan(0, 0, 0, 0, 500), true) },
                { "WalkLeft", sacg.Generate("WalkLeft", new Vector2(0, 1), new Vector2(1, 1), new TimeSpan(0, 0, 0, 0, 500), true) },
                { "WalkRight", sacg.Generate("WalkRight", new Vector2(0, 2), new Vector2(1, 2), new TimeSpan(0, 0, 0, 0, 500), true) },
                { "WalkUp", sacg.Generate("WalkUp", new Vector2(0, 3), new Vector2(1, 3), new TimeSpan(0, 0, 0, 0, 500), true) },
            };

            GameSprite = new Sprite(game, spriteSheet, new Point(32, 40), new Point(16, 20));
            GameSprite.animationPlayer = new SpriteSheetAnimationPlayer(spriteAnimationClips);
            GameSprite.Position = new Vector2() { X = StartLocation.X * Global.TileSize.X, Y = StartLocation.Y * Global.TileSize.Y};
            GameSprite.StartAnimation("Idle");
        }

        public void HandleInput(InputState input, PlayerIndex? ControllingPlayer, PlayerIndex player)
        {
            if (this.Type == ObjectType.Character)
            {
                if (input.IsKeyPressed(Keys.Down, ControllingPlayer, out player))
                    GameSprite.animationPlayer.StartClip("WalkDown");
                else if (input.IsKeyPressed(Keys.Up, ControllingPlayer, out player))
                    GameSprite.animationPlayer.StartClip("WalkUp");
                else if (input.IsKeyPressed(Keys.Left, ControllingPlayer, out player))
                    GameSprite.animationPlayer.StartClip("WalkLeft");
                else if (input.IsKeyPressed(Keys.Right, ControllingPlayer, out player))
                    GameSprite.animationPlayer.StartClip("WalkRight");
                else
                    GameSprite.animationPlayer.StartClip("Idle");
            }
            else
            {
              
            }
        }

        public void Update(GameTime time)
        {
            float translateSpeed = .5f;

            switch (GameSprite.animationPlayer.CurrentClip.Name)
            {
                case "WalkDown":
                    GameSprite.Position += new Vector2(0, translateSpeed);
                    break;
                case "WalkLeft":
                    GameSprite.Position += new Vector2(-translateSpeed, 0);
                    break;
                case "WalkRight":
                    GameSprite.Position += new Vector2(translateSpeed, 0);
                    break;
                case "WalkUp":
                    GameSprite.Position += new Vector2(0, -translateSpeed);
                    break;
                case "Idle":
                    break;
            }

            GameSprite.Position = Vector2.Min(new Vector2(Game1.GameWidth() - GameSprite.Size.X, Game1.GameHeight() - GameSprite.Size.Y), Vector2.Max(Vector2.Zero, GameSprite.Position));

            GameSprite.Update(time);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            GameSprite.Draw(gameTime, spriteBatch);
        }

        public static List<EntityGameObject> LoadNPCs()
        {
            List<EntityGameObject> objects = new List<EntityGameObject>();

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };

            string data = File.ReadAllText(@"Content\Data\npcs.json");

            objects = JsonConvert.DeserializeObject<List<EntityGameObject>>(data, settings);

            return objects;
        }

        public void Click(int id, int conversationID)
        {
            NPCClicked(new NPCClickedEventArgs() { ID = id, ConversationID = conversationID });
        }
    }

    public class EntityGameObject : GameObject
    {
        public EntityType EntityType;
        public object Entity;           //this could be either a Character object or just an Entity object

        public GameObject Target;       //logic is customized for type, targeting an Item handled differently than an Entity or Building 

        public EntityGameObject(Game game, string fileName)
        {
            if(!string.IsNullOrEmpty(fileName))
            {
                base.Initialize(game, fileName);
            }
        }

        public void Initialize()
        {
            this.GameSprite.Position = new Vector2(this.StartLocation.X * 32, this.StartLocation.Y * 32);
        }

        //called every frame before update
        public void Think()
        {
            //detect character in range and show conversation icon



        }

        public new void HandleInput(InputState input, PlayerIndex? ControllingPlayer, PlayerIndex player)
        {
            if (Type == ObjectType.Character)
            {
                base.HandleInput(input, ControllingPlayer, player);
            }
            else
            {
                //check for click on NPC
                if (input.IsNewMouseClick(MouseButtons.Left))
                {
                    if (Global.RectClicked(GameSprite.Position, GameSprite.Size, input.CurrentMouseState[0].X, input.CurrentMouseState[0].Y))
                    {
                        int id;

                        if (((Entity)Entity).HasConversationToStart(out id))
                        {
                            Click(((Entity)Entity).ID, id);
                        }
                    }
                }
            }
        }

        public bool CheckKnownNPC(string name)
        {
            if(EntityType == EntityType.Character)
            {
                return ((Character)Entity).CheckKnownNPC(name);
            }
            else
            {
                return false;
            }
        }

    }

    public class BuildingGameObject : GameObject
    {
        public BuildingGameObject(Game game, string fileName) : base(game, fileName)
        {


        }
    }

    public class WorldGameObject : GameObject
    {
        public WorldGameObject(Game game, string fileName) : base(game, fileName)
        {


        }
    }
}
