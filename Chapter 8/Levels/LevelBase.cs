using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using VoidEngineLight.Animation;

//using MonoGameRPG.Interfaces;

namespace MonoGameRPG
{
    public abstract class LevelBase
    {
        /// <summary>
        /// This is a reference to our player in the level
        /// </summary>
        public virtual Sprite PlayerReference { get; set; }

        /// <summary>
        /// This is a list of all our base tiles in the level.
        /// </summary>
        public virtual List<MapTile> Tiles { get; protected set; }
        /// <summary>
        /// This is a list of all our overlay tiles in the level
        /// </summary>
        public virtual List<MapTile> OverlayTiles { get; protected set; }

        /// <summary>
        /// This is the size of each sprite tile in the level.
        /// </summary>
        protected virtual Point tileSize { get; set; }

        /// <summary>
        /// A reference to the content manager so we can load assets
        /// </summary>
        protected virtual ContentManager contentManager { get; set; }

        /// <summary>
        /// The sprite sheet all tiles are rendered from
        /// </summary>
        protected virtual Texture2D spriteSheet { get; set; }
        /// <summary>
        /// The tile map used for floor tiles
        /// </summary>
        protected virtual Texture2D map { get; set; }
        /// <summary>
        /// The tile map used for overlay objects
        /// </summary>
        protected virtual Texture2D overlay { get; set; }
        /// <summary>
        /// The tile map used for overlay objects
        /// </summary>
        protected virtual Texture2D objects { get; set; }
        /// <summary>
        /// The tile map used for NPC avatars.
        /// </summary>
        protected virtual Texture2D mobs { get; set; }

        static Dictionary<string, List<IInventoryItem>> Items { get; set; }

        protected string Name { get; set; }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="contentMgr">A reference to the content manager</param>
        /// <param name="tileSheetAsset">The asset for the sprite sheet</param>
        /// <param name="mapAsset">Tile map for floor tiles</param>
        /// <param name="overlayMapAsset">Tile map for overlay</param>
        /// <param name="objectMapAsset">Tile map of objects</param>
        /// <param name="mobMapAsset">Tile map for MOBs</param>
        /// <param name="tileSize">Size of the sprites</param>
        /// <param name="cellSize">Cell size of the sprites (see chapter 2)</param>
        public LevelBase(ContentManager contentMgr, string tileSheetAsset, string mapAsset, string overlayMapAsset, string objectMapAsset, string mobMapAsset,  Point tileSize, Point cellSize)
        {
            this.tileSize = tileSize;

            contentManager = contentMgr;

            spriteSheet = contentManager.Load<Texture2D>(tileSheetAsset);

            map = contentManager.Load<Texture2D>(mapAsset);
            overlay = contentManager.Load<Texture2D>(overlayMapAsset);
            objects = contentManager.Load<Texture2D>(objectMapAsset);

            int width = map.Width;
            int height = map.Height;

            GenerateTileData(spriteSheet, tileSize, cellSize, map, overlay, objects, width, height, GetAnimationClips(spriteSheet));
        }

        public void AddItem(IInventoryItem item)
        {
            if (Items == null)
                Items = new Dictionary<string, List<IInventoryItem>>();

            if (!Items.ContainsKey(Name))
                Items.Add(Name, new List<IInventoryItem>());

            item.Position = new Vector2((int)PlayerReference.Position.X,(int)PlayerReference.Position.Y);
            item.Size = PlayerReference.Size;
            Items[Name].Add(item);
        }

        /// <summary>
        /// THis function is used to map the sprite sheet to the tile maps.
        /// </summary>
        /// <param name="spriteSheet"></param>
        /// <returns></returns>
        protected abstract Dictionary<string, SpriteSheetAnimationClip> GetAnimationClips(Texture2D spriteSheet);
        /// <summary>
        /// This method generates the level tile data based on the tile maps and the animation clips
        /// </summary>
        /// <param name="spriteSheet"></param>
        /// <param name="tileSize"></param>
        /// <param name="cellSize"></param>
        /// <param name="terrainMap"></param>
        /// <param name="terrainOverlayMap"></param>
        /// <param name="terrainBuildingMap"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="animation"></param>
        protected abstract void GenerateTileData(Texture2D spriteSheet, Point tileSize, Point cellSize, Texture2D terrainMap, Texture2D terrainOverlayMap, Texture2D terrainBuildingMap, int width, int height, Dictionary<string, SpriteSheetAnimationClip> animation);
        /// <summary>
        /// THis method generates the level from the Tile data generated
        /// </summary>
        /// <param name="spriteSheet"></param>
        /// <param name="tileSize"></param>
        /// <param name="cellSize"></param>
        /// <param name="map"></param>
        /// <param name="overlayMap"></param>
        /// <param name="animation"></param>
        protected virtual void GenerateMap(Texture2D spriteSheet, Point tileSize, Point cellSize, Dictionary<Point, TileData> map, Dictionary<Point, List<TileData>> overlayMap, Dictionary<string, SpriteSheetAnimationClip> animation)
        {
            if (map != null)
            {
                Tiles = new List<MapTile>();
                OverlayTiles = new List<MapTile>();

                foreach (Point p in map.Keys)
                {
                    MapTile tile = new MapTile(spriteSheet, tileSize, cellSize, animation, map[p].TileType);
                    tile.Data = map[p];
                    tile.Location = new Vector2(p.X, p.Y);

                    Tiles.Add(tile);

                    if (overlayMap != null && overlayMap.ContainsKey(p))
                    {
                        foreach (TileData td in overlayMap[p])
                        {
                            tile = new MapTile(spriteSheet, tileSize, cellSize, animation, td.TileType);
                            tile.Data = td;
                            tile.Location = new Vector2(p.X, p.Y);
                            OverlayTiles.Add(tile);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns true if a given level location is solid
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public virtual bool IsSolid(Vector2 point)
        {

            if (Tiles == null)
                return false;
            // Lock coords to grid.
            //point = new Vector2((int)(point.X / TileSize.X), (int)(point.Y / TileSize.Y));
            Rectangle target = new Rectangle((int)point.X, (int)point.Y, tileSize.X, tileSize.Y);

            List<MapTile> tiles = Tiles.Where(t => t.Data.IsSolid && target.Intersects(t.TileBase.BoundsRectangle)).ToList();

            if (tiles != null && tiles.Count > 0)
                return true;

            tiles = OverlayTiles.Where(t => t.Data.IsSolid && target.Intersects(t.TileBase.BoundsRectangle)).ToList();

            if (tiles != null && tiles.Count > 0)
                return true;

            return false;
        }

        /// <summary>
        /// Returns the TileData instance of the exit if found at this level location
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public virtual TileData IsExitTo(Vector2 point)
        {
            if (Tiles == null)
                return null;

            Rectangle target = new Rectangle((int)point.X, (int)point.Y, tileSize.X, tileSize.Y);

            MapTile tile = Tiles.FirstOrDefault(t => !string.IsNullOrEmpty(t.Data.ExitTo) && target.Intersects(t.TileBase.BoundsRectangle));

            if (tile != null)
                return tile.Data;

            tile = OverlayTiles.FirstOrDefault(t => !string.IsNullOrEmpty(t.Data.ExitTo) && target.Intersects(t.TileBase.BoundsRectangle));

            if (tile != null)
                return tile.Data;

            return null;
        }

        public virtual void Update(GameTime gameTime)
        {
            if (Tiles != null)
            {
                foreach (MapTile tile in Tiles)
                    tile.Update(gameTime);
            }

            if (OverlayTiles != null)
            {
                foreach (MapTile tile in OverlayTiles)
                    tile.Update(gameTime);
            }

            PlayerReference.Update(gameTime);
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (Tiles != null)
            {
                foreach (MapTile tile in Tiles)
                    tile.Draw(gameTime, spriteBatch);
            }

            if (OverlayTiles != null)
            {
                foreach (MapTile tile in OverlayTiles)
                    tile.Draw(gameTime, spriteBatch);
            }

            if (Items != null && Items.ContainsKey(Name))
            {
                foreach (IInventoryItem item in Items[Name])
                    ((ItemBase)item).Draw(gameTime, spriteBatch);
            }

            PlayerReference.Draw(gameTime, spriteBatch);
        }
        public IInventoryItem PickupItem()
        {
            if (Items != null)
            {
                IInventoryItem item = Items[Name].FirstOrDefault(i => new Rectangle((int)i.Position.X, (int)i.Position.Y, i.Size.X, i.Size.Y).Intersects(PlayerReference.BoundsRectangle));

                if (item != null)
                {
                    Items[Name].Remove(item);
                    return item;
                }
            }

            return null;
        }
    }

    public class TileData
    {
        public bool IsSolid { get; set; }
        public string TileType { get; set; }
        public string ExitTo { get; set; }
        public Vector2 EnterIn { get; set; }
    }

    public class MapTile
    {
        public float Layer { get; protected set; }
        public Vector2 Location { get; set; }

        public List<Sprite> Items { get; protected set; }

        public Sprite TileBase { get; set; }

        public TileData Data { get; set; }

       
        public MapTile(Texture2D spriteSheet, Point tileSize, Point cellSize, Dictionary<string, SpriteSheetAnimationClip> animation = null, string initialAnimation = null)
        {
            
            TileBase = new Sprite(spriteSheet, tileSize, cellSize);

            if (animation != null)
                TileBase.animationPlayer = new SpriteSheetAnimationPlayer(animation);

            if (initialAnimation != null)
                TileBase.StartAnimation(initialAnimation);
        }

        public virtual void Update(GameTime gameTime)
        {
            if (TileBase != null)
                TileBase.Update(gameTime);
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (TileBase != null)
            {
                TileBase.Position = Location;
                TileBase.Draw(gameTime, spriteBatch);
            }
        }
    }
}
