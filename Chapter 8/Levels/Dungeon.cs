using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework.Content;


using VoidEngineLight.Animation;

namespace MonoGameRPG
{
    public class Dungeon : LevelBase
    {
        public Dungeon(ContentManager contentMgr, string tileSheetAsset, string mapAsset, string overlayMapAsset, string objectMapAsset, string mobMapAsset, Point tileSize, Point cellSize)
            : base(contentMgr, tileSheetAsset, mapAsset, overlayMapAsset, objectMapAsset, mobMapAsset, tileSize, cellSize)
        { Name = "DungeonLevel"; }

        protected override Dictionary<string, SpriteSheetAnimationClip> GetAnimationClips(Texture2D spriteSheet)
        {
            SpriteAnimationClipGenerator sacg = new SpriteAnimationClipGenerator(new Vector2(spriteSheet.Width, spriteSheet.Height), new Vector2(4, 9));

            return new Dictionary<string, SpriteSheetAnimationClip>()
            {
                {"Blank", sacg.Generate("Blank", new Vector2(3, 0), new Vector2(3, 0), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"Floor", sacg.Generate("Floor", new Vector2(1, 1), new Vector2(1, 1), new TimeSpan(0, 0, 0, 0, 500), true) },


                {"TLWall", sacg.Generate("TLWall", new Vector2(0, 0), new Vector2(0, 0), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"LWall", sacg.Generate("LWall", new Vector2(0, 1), new Vector2(0, 1), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"BackWall", sacg.Generate("BackWall", new Vector2(1, 0), new Vector2(1, 0), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"TRWall", sacg.Generate("TRWall", new Vector2(2, 0), new Vector2(2, 0), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"RWall", sacg.Generate("RWall", new Vector2(2, 1), new Vector2(2, 1), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"BLWall", sacg.Generate("BLWall", new Vector2(0, 2), new Vector2(0, 2), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"BWall", sacg.Generate("BWall", new Vector2(1, 2), new Vector2(1, 2), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"BRWall", sacg.Generate("BRWall", new Vector2(2, 2), new Vector2(2, 2), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"LBackWall", sacg.Generate("LBackWall", new Vector2(0, 5), new Vector2(0, 5), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"RBackWall", sacg.Generate("RBackWall", new Vector2(1, 5), new Vector2(1, 5), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"LFrontWall", sacg.Generate("LFrontWall", new Vector2(1, 6), new Vector2(1, 6), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"RFrontWall", sacg.Generate("RFrontWall", new Vector2(0, 6), new Vector2(0, 6), new TimeSpan(0, 0, 0, 0, 500), true) },

                {"BrickOverlay", sacg.Generate("BrickOverlay", new Vector2(2, 5), new Vector2(2, 5), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"BrickOverlay2", sacg.Generate("BrickOverlay2", new Vector2(3, 5), new Vector2(3, 5), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"BrickOverlay3", sacg.Generate("BrickOverlay3", new Vector2(2, 6), new Vector2(2, 6), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"BrickOverlay4", sacg.Generate("BrickOverlay4", new Vector2(3, 6), new Vector2(3, 6), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"BrickOverlay5", sacg.Generate("BrickOverlay5", new Vector2(3, 7), new Vector2(3, 7), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"BrickOverlay6", sacg.Generate("BrickOverlay6", new Vector2(3, 8), new Vector2(3, 8), new TimeSpan(0, 0, 0, 0, 500), true) },

                {"Hole", sacg.Generate("Hole", new Vector2(1, 7), new Vector2(1, 7), new TimeSpan(0, 0, 0, 0, 500), true) },
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

                    if (floorMapData[w + (h * width)] == Color.Transparent)
                    {
                        data.TileType = "Blank";
                        data.IsSolid = true;
                    }
                    else if (floorMapData[w + (h * width)] == Color.White)
                    {
                        int r = rnd.Next(0, 100);

                        if (r <= 25)
                        {
                            if (!overlay.ContainsKey(p))
                                overlay.Add(p, new List<TileData>());

                            data = new TileData();

                            r = rnd.Next(0, 100);

                            if(r <= 16)
                                data.TileType = "BrickOverlay";
                            else if(r <= 32)
                                data.TileType = "BrickOverlay2";
                            else if (r <= 48)
                                data.TileType = "BrickOverlay3";
                            else if (r <= 64)
                                data.TileType = "BrickOverlay4";
                            else if (r <= 80)
                                data.TileType = "BrickOverlay5";
                            else 
                                data.TileType = "BrickOverlay6";

                            overlay[p].Add(data);
                        }                        

                        data = new TileData();
                        data.TileType = "Floor";
                    }
                    else if (floorMapData[w + (h * width)] == Color.Black)
                    {
                        data.TileType = "Hole";
                        data.ExitTo = "AreaMap";
                        data.EnterIn = new Vector2(31, 15);
                    }
                    else if (floorMapData[w + (h * width)] == Color.Gray)
                    {
                        data.TileType = "BackWall";
                        data.IsSolid = true;
                    }
                    else if (floorMapData[w + (h * width)] == Color.DarkGray)
                    {
                        data.TileType = "BWall";
                        data.IsSolid = true;
                    }
                    else if (floorMapData[w + (h * width)] == Color.DimGray)
                    {
                        data.TileType = "LBackWall";
                        data.IsSolid = true;
                    }
                    else if (floorMapData[w + (h * width)] == Color.DarkSlateGray)
                    {
                        data.TileType = "RBackWall";
                        data.IsSolid = true;
                    }
                    else if (floorMapData[w + (h * width)] == Color.Red)
                    {
                        data.TileType = "TLWall";
                        data.IsSolid = true;
                    }
                    else if (floorMapData[w + (h * width)] == Color.RosyBrown)
                    {
                        data.TileType = "LWall";
                        data.IsSolid = true;
                    }
                    else if (floorMapData[w + (h * width)] == Color.Brown)
                    {
                        data.TileType = "BLWall";
                        data.IsSolid = true;
                    }
                    else if (floorMapData[w + (h * width)] == Color.Green)
                    {
                        data.TileType = "TRWall";
                        data.IsSolid = true;
                    }
                    else if (floorMapData[w + (h * width)] == Color.Lime)
                    {
                        data.TileType = "RWall";
                        data.IsSolid = true;
                    }
                    else if (floorMapData[w + (h * width)] == Color.LightGreen)
                    {
                        data.TileType = "BRWall";
                        data.IsSolid = true;
                    }
                    else if (floorMapData[w + (h * width)] == Color.GreenYellow)
                    {
                        data.TileType = "LFrontWall";
                        data.IsSolid = true;
                    }
                    else if (floorMapData[w + (h * width)] == Color.DarkGreen)
                    {
                        data.TileType = "RFrontWall";
                        data.IsSolid = true;
                    }
                    else { }
                    if (!string.IsNullOrEmpty(data.TileType) && !innFloorPlan.ContainsKey(p))
                        innFloorPlan.Add(p, data);

                    
                }
            }

            GenerateMap(spriteSheet, tileSize, cellSize, innFloorPlan, overlay, animation);
        }

    }
}
