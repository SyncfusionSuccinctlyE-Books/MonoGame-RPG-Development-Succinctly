using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework.Content;


using VoidEngineLight.Animation;

namespace MonoGameRPG
{
    public class InnLevel : LevelBase
    {
        public InnLevel(ContentManager contentMgr, string tileSheetAsset, string mapAsset, string overlayMapAsset, string objectMapAsset, string mobMapAsset, Point tileSize, Point cellSize)
            : base(contentMgr, tileSheetAsset, mapAsset, overlayMapAsset, objectMapAsset, mobMapAsset, tileSize, cellSize)
        { Name = "InnLevel"; ; }

        protected override Dictionary<string, SpriteSheetAnimationClip> GetAnimationClips(Texture2D spriteSheet)
        {
            SpriteAnimationClipGenerator sacg = new SpriteAnimationClipGenerator(new Vector2(spriteSheet.Width, spriteSheet.Height), new Vector2(8, 15));

            return new Dictionary<string, SpriteSheetAnimationClip>()
            {
                {"Blank", sacg.Generate("Blank", new Vector2(2, 1), new Vector2(2, 1), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"Floor", sacg.Generate("Floor", new Vector2(0, 1), new Vector2(0, 1), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"TLCornerWall", sacg.Generate("TLCornerWall", new Vector2(0, 3), new Vector2(0, 3), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"TWall", sacg.Generate("TWall", new Vector2(1, 3), new Vector2(1, 3), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"TRCornerWall", sacg.Generate("TRCornerWall", new Vector2(2, 3), new Vector2(2, 3), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"LWall", sacg.Generate("LWall", new Vector2(0, 4), new Vector2(0, 4), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"RWall", sacg.Generate("LWall", new Vector2(2, 4), new Vector2(2, 4), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"BLCornerWall", sacg.Generate("BLCornerWall", new Vector2(0, 5), new Vector2(0, 5), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"BWall", sacg.Generate("BWall", new Vector2(1, 5), new Vector2(1, 5), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"BRCornerWall", sacg.Generate("BRCornerWall", new Vector2(2, 5), new Vector2(2, 5), new TimeSpan(0, 0, 0, 0, 500), true) },

                {"Door", sacg.Generate("Door", new Vector2(0, 6), new Vector2(0, 6), new TimeSpan(0, 0, 0, 0, 500), true) },

                {"Fire", sacg.Generate("Fire", new Vector2(4, 7), new Vector2(5, 7), new TimeSpan(0, 0, 0, 0, 500), true) },
            };
        }

        protected override void GenerateTileData(Texture2D spriteSheet, Point tileSize, Point cellSize, Texture2D floorMap, Texture2D overlays, Texture2D objects, int width, int height, Dictionary<string, SpriteSheetAnimationClip> animation)
        {
            int seed = 1971;
            Random rnd = new Random(seed);

            Dictionary<Point, TileData> innFloorPlan = new Dictionary<Point, TileData>();
            Dictionary<Point, List<TileData>> overlay = new Dictionary<Point, List<TileData>>();

            Color[] floorMapData = new Color[floorMap.Width * floorMap.Height];
            floorMap.GetData(floorMapData);

            Color[] overlayData = new Color[overlays.Width * overlays.Height];
            overlays.GetData(overlayData);

            Color[] objectData = new Color[objects.Width * objects.Height];
            objects.GetData(objectData);


            for (int w = 0; w < width; w++)
            {
                for (int h = 0; h < height; h++)
                {
                    TileData data = new TileData();
                    Point p = new Point(w * tileSize.X, h * tileSize.Y);

                    if (floorMapData[w + (h * width)] == Color.Black)
                    {
                        data.TileType = "Floor";
                    }
                    else if (floorMapData[w + (h * width)] == Color.Transparent)
                    {
                        data.TileType = "Blank";
                        data.IsSolid = true;
                    }

                    if (!string.IsNullOrEmpty(data.TileType) && !innFloorPlan.ContainsKey(p))
                        innFloorPlan.Add(p, data);

                    if (overlayData[w + (h * width)] == Color.White)
                    {
                        if (!overlay.ContainsKey(p))
                            overlay.Add(p, new List<TileData>());

                        data = new TileData();
                        data.TileType = "Door";

                        innFloorPlan[p].IsSolid = false;

                        data.ExitTo = "Town";
                        data.EnterIn = data.EnterIn = new Vector2(8, 6);

                        overlay[p].Add(data);
                    }
                    

                    if (overlayData[w + (h * width)] == Color.Black)
                    {
                        if (!overlay.ContainsKey(p))
                            overlay.Add(p, new List<TileData>());

                        data = new TileData();
                        data.TileType = "TWall";

                        overlay[p].Add(data);
                    }
                    else if (overlayData[w + (h * width)] == Color.Red)
                    {
                        if (!overlay.ContainsKey(p))
                            overlay.Add(p, new List<TileData>());

                        data = new TileData();
                        data.TileType = "TLCornerWall";

                        overlay[p].Add(data);
                    }
                    else if (overlayData[w + (h * width)] == Color.Gold)
                    {
                        if (!overlay.ContainsKey(p))
                            overlay.Add(p, new List<TileData>());

                        data = new TileData();
                        data.TileType = "LWall";

                        overlay[p].Add(data);
                    }
                    else if (overlayData[w + (h * width)] == Color.Blue)
                    {
                        if (!overlay.ContainsKey(p))
                            overlay.Add(p, new List<TileData>());

                        data = new TileData();
                        data.TileType = "BLCornerWall";

                        overlay[p].Add(data);
                    }
                    else if (overlayData[w + (h * width)] == Color.DarkBlue)
                    {
                        if (!overlay.ContainsKey(p))
                            overlay.Add(p, new List<TileData>());

                        data = new TileData();
                        data.TileType = "BWall";

                        overlay[p].Add(data);
                    }
                    else if (overlayData[w + (h * width)] == Color.Yellow)
                    {
                        if (!overlay.ContainsKey(p))
                            overlay.Add(p, new List<TileData>());

                        data = new TileData();
                        data.TileType = "BRCornerWall";

                        overlay[p].Add(data);
                    }
                    else if (overlayData[w + (h * width)] == Color.Orange)
                    {
                        if (!overlay.ContainsKey(p))
                            overlay.Add(p, new List<TileData>());

                        data = new TileData();
                        data.TileType = "RWall";

                        overlay[p].Add(data);
                    }
                    else if (overlayData[w + (h * width)] == Color.Green)
                    {
                        if (!overlay.ContainsKey(p))
                            overlay.Add(p, new List<TileData>());

                        data = new TileData();
                        data.TileType = "TRCornerWall";

                        overlay[p].Add(data);
                    }
                    else if (overlayData[w + (h * width)] == Color.Beige)
                    {
                        if (!overlay.ContainsKey(p))
                            overlay.Add(p, new List<TileData>());

                        data = new TileData();
                        data.TileType = "Fire";
                        data.IsSolid = true;

                        overlay[p].Add(data);
                    }
                }
            }

            GenerateMap(spriteSheet, tileSize, cellSize, innFloorPlan, overlay, animation);
        }

       
    }
}
