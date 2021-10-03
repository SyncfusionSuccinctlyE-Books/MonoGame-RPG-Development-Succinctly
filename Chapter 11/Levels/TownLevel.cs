using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework.Content;


using VoidEngineLight.Animation;

namespace MonoGameRPG
{
    
    public class TownLevel : LevelBase
    {
        public TownLevel(ContentManager contentMgr, string tileSheetAsset, string mapAsset, string overlayMapAsset, string objectMapAsset, string mobMapAsset, Point tileSize, Point cellSize)
            : base(contentMgr, tileSheetAsset, mapAsset, overlayMapAsset, objectMapAsset, mobMapAsset, tileSize, cellSize)
        { }

        protected override Dictionary<string, SpriteSheetAnimationClip> GetAnimationClips(Texture2D spriteSheet)
        {
            SpriteAnimationClipGenerator sacg = new SpriteAnimationClipGenerator(new Vector2(spriteSheet.Width, spriteSheet.Height), new Vector2(16, 39));

            return new Dictionary<string, SpriteSheetAnimationClip>()
            {
                {"Blank", sacg.Generate("Blank", new Vector2(0, 0), new Vector2(0, 0), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"Green", sacg.Generate("Green", new Vector2(1, 0), new Vector2(1, 0), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"Flower1", sacg.Generate("Flower1", new Vector2(2, 0), new Vector2(2, 0), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"Flower2", sacg.Generate("Flower2", new Vector2(3, 0), new Vector2(3, 0), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"Pebble1", sacg.Generate("Pebble1", new Vector2(4, 0), new Vector2(4, 0), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"Pebble2", sacg.Generate("Pebble2", new Vector2(5, 0), new Vector2(5, 0), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"Gnol1", sacg.Generate("Gnol1", new Vector2(6, 0), new Vector2(6, 0), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"Gnol2", sacg.Generate("Gnol2", new Vector2(7, 0), new Vector2(7, 0), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"Log1", sacg.Generate("Log1", new Vector2(8, 0), new Vector2(8, 0), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"Log2", sacg.Generate("Log2", new Vector2(9, 0), new Vector2(9, 0), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"Rock1", sacg.Generate("Rock1", new Vector2(1, 0), new Vector2(10, 0), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"Rock2", sacg.Generate("Rock2", new Vector2(11, 0), new Vector2(11, 0), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"Hay1", sacg.Generate("Hay1", new Vector2(12, 0), new Vector2(12, 0), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"Stick", sacg.Generate("Stick", new Vector2(13, 0), new Vector2(13, 0), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"TLHole", sacg.Generate("TLHole", new Vector2(14, 0), new Vector2(14, 0), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"TRHole", sacg.Generate("TRHole", new Vector2(15, 0), new Vector2(15, 0), new TimeSpan(0, 0, 0, 0, 500), true) },

                {"Water", sacg.Generate("Water", new Vector2(0, 1), new Vector2(0, 1), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"Grass", sacg.Generate("Grass", new Vector2(1, 1), new Vector2(1, 1), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"Flower3", sacg.Generate("Flower3", new Vector2(2, 1), new Vector2(2, 1), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"Flower4", sacg.Generate("Flower4", new Vector2(3, 1), new Vector2(3, 1), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"Pebble3", sacg.Generate("Pebble3", new Vector2(4, 1), new Vector2(4, 1), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"Pebble4", sacg.Generate("Pebble4", new Vector2(5, 1), new Vector2(5, 1), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"Gnol3", sacg.Generate("Gnol3", new Vector2(6, 1), new Vector2(6, 1), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"Gnol4", sacg.Generate("Gnol4", new Vector2(7, 1), new Vector2(7, 1), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"Log3", sacg.Generate("Log3", new Vector2(8, 1), new Vector2(8, 1), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"Log4", sacg.Generate("Log4", new Vector2(9, 1), new Vector2(9, 1), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"Rock3", sacg.Generate("Rock3", new Vector2(1, 1), new Vector2(10, 1), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"Rock4", sacg.Generate("Rock4", new Vector2(11, 1), new Vector2(11, 1), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"Stumps", sacg.Generate("Stumps", new Vector2(12, 1), new Vector2(12, 1), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"MudGnol", sacg.Generate("MudGnol", new Vector2(13, 1), new Vector2(13, 1), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"BLHole", sacg.Generate("BLHole", new Vector2(14, 1), new Vector2(14, 1), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"BRHole", sacg.Generate("BRHole", new Vector2(15, 1), new Vector2(15, 1), new TimeSpan(0, 0, 0, 0, 500), true) },

                {"TLCliffGreen", sacg.Generate("TLCliffGreen", new Vector2(5, 8), new Vector2(5, 8), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"BRCliffGreen", sacg.Generate("BRCliffGreen", new Vector2(5, 9), new Vector2(5, 9), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"RCliffGreen", sacg.Generate("RCliffGreen", new Vector2(8, 9), new Vector2(8, 9), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"TCliffGreen", sacg.Generate("TCliffGreen", new Vector2(7, 10), new Vector2(7, 10), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"BCliffGreen", sacg.Generate("BCliffGreen", new Vector2(7, 8), new Vector2(7, 8), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"Tree", sacg.Generate("Tree", new Vector2(1, 23), new Vector2(1, 23), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"Tree2", sacg.Generate("Tree2", new Vector2(1, 26), new Vector2(1, 26), new TimeSpan(0, 0, 0, 0, 500), true) },

                {"Mountain", sacg.Generate("Mountain", new Vector2(1, 29), new Vector2(1, 29), new TimeSpan(0, 0, 0, 0, 500), true) },

                {"VPath1", sacg.Generate("VPath1", new Vector2(1, 36), new Vector2(1, 36), new TimeSpan(0, 0, 0, 0, 500), true) },

                {"PathToOpenN", sacg.Generate("PathToOpenN", new Vector2(4, 38), new Vector2(4, 38), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"WallTop", sacg.Generate("WallTop", new Vector2(14, 19), new Vector2(14, 19), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"WallBottom", sacg.Generate("WallBottom", new Vector2(14, 19), new Vector2(14, 19), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"Open", sacg.Generate("Open", new Vector2(6, 35), new Vector2(6, 35), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"WallTopLeft", sacg.Generate("WallTopLeft", new Vector2(10, 19), new Vector2(10, 19), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"WallBottomLeft", sacg.Generate("WallBottomLeft", new Vector2(10, 21), new Vector2(10, 21), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"WallLeft", sacg.Generate("WallLeft", new Vector2(9, 20), new Vector2(9, 20), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"WallRight", sacg.Generate("WallRight", new Vector2(9, 20), new Vector2(9, 20), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"WallTopRight", sacg.Generate("WallTopRight", new Vector2(12, 19), new Vector2(12, 19), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"WallBottomRight", sacg.Generate("WallBottomRight", new Vector2(12, 21), new Vector2(12, 21), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"WallHEndL", sacg.Generate("WallHEndL", new Vector2(13, 19), new Vector2(13, 19), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"WallHEndR", sacg.Generate("WallHEndR", new Vector2(15, 19), new Vector2(15, 19), new TimeSpan(0, 0, 0, 0, 500), true) },

                {"Inn", sacg.Generate("Inn", new Vector2(5, 21), new Vector2(5, 21), new TimeSpan(0, 0, 0, 0, 500), true) },

                {"House1", sacg.Generate("House1", new Vector2(0, 21), new Vector2(0, 21), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"House2", sacg.Generate("House2", new Vector2(1, 21), new Vector2(1, 21), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"House3", sacg.Generate("House3", new Vector2(2, 21), new Vector2(2, 21), new TimeSpan(0, 0, 0, 0, 500), true) },
                {"House4", sacg.Generate("House4", new Vector2(3, 21), new Vector2(3, 21), new TimeSpan(0, 0, 0, 0, 500), true) },

            };
        }

        protected override void GenerateTileData(Texture2D spriteSheet, Point tileSize, Point cellSize, Texture2D terrainMap, Texture2D terrainOverlayMap, Texture2D terrainBuildingMap, int width, int height, Dictionary<string, SpriteSheetAnimationClip> animation)
        {
            int seed = 1971;
            Random rnd = new Random(seed);

            Dictionary<Point, TileData> map = new Dictionary<Point, TileData>();
            Dictionary<Point, List<TileData>> overlay = new Dictionary<Point, List<TileData>>();

            Color[] terrainData = new Color[terrainMap.Width * terrainMap.Height];
            terrainMap.GetData(terrainData);

            Color[] terrainOverlayData = new Color[terrainOverlayMap.Width * terrainOverlayMap.Height];
            terrainOverlayMap.GetData(terrainOverlayData);

            Color[] buildingData = new Color[terrainBuildingMap.Width * terrainBuildingMap.Height];
            terrainBuildingMap.GetData(buildingData);


            for (int w = 0; w < width; w++)
            {
                for (int h = 0; h < height; h++)
                {

                    TileData data = new TileData();
                    Point p = new Point(w * tileSize.X, h * tileSize.Y);

                    data.TileType = "Blank";

                    // Solid Terrain
                    if (terrainData[w + (h * width)] == Color.Black)
                        data.TileType = "Green";
                    else if (terrainData[w + (h * width)] == Color.Blue)
                    {
                        data.TileType = "Water";
                        data.IsSolid = true;
                    }
                    else if (terrainData[w + (h * width)] == Color.Green)
                        data.TileType = "Grass";


                    if (!map.ContainsKey(p))
                        map.Add(p, data);

                    // Overlay
                    if (terrainOverlayData[w + (h * width)] == Color.White)
                    {
                        if (!overlay.ContainsKey(p))
                            overlay.Add(p, new List<TileData>());

                        data = new TileData();
                        data.TileType = "TLCliffGreen";
                        data.IsSolid = true;

                        overlay[p].Add(data);
                    }
                    else if (terrainOverlayData[w + (h * width)] == Color.Gray)
                    {
                        if (!overlay.ContainsKey(p))
                            overlay.Add(p, new List<TileData>());

                        data = new TileData();
                        data.TileType = "BRCliffGreen";
                        data.IsSolid = true;

                        overlay[p].Add(data);
                    }
                    else if (terrainOverlayData[w + (h * width)] == Color.DarkGray)
                    {
                        if (!overlay.ContainsKey(p))
                            overlay.Add(p, new List<TileData>());

                        data = new TileData();
                        data.TileType = "RCliffGreen";
                        data.IsSolid = true;

                        overlay[p].Add(data);
                    }
                    else if (terrainOverlayData[w + (h * width)] == Color.DimGray)
                    {
                        if (!overlay.ContainsKey(p))
                            overlay.Add(p, new List<TileData>());

                        data = new TileData();
                        data.TileType = "TCliffGreen";
                        data.IsSolid = true;

                        overlay[p].Add(data);
                    }
                    else if (terrainOverlayData[w + (h * width)] == Color.LightGray)
                    {
                        if (!overlay.ContainsKey(p))
                            overlay.Add(p, new List<TileData>());

                        data = new TileData();
                        data.TileType = "BCliffGreen";
                        data.IsSolid = true;

                        overlay[p].Add(data);
                    }
                    else if (terrainOverlayData[w + (h * width)] == Color.Orange)
                    {
                        int f = rnd.Next(0, 101);

                        data = new TileData();

                        if (f <= 25)
                            data.TileType = "Flower1";
                        else if (f <= 50)
                            data.TileType = "Flower2";
                        if (f <= 75)
                            data.TileType = "Flower3";
                        else
                            data.TileType = "Flower4";

                        if (!overlay.ContainsKey(p))
                            overlay.Add(p, new List<TileData>());



                        overlay[p].Add(data);
                    }
                    else if (terrainOverlayData[w + (h * width)] == Color.Green)
                    {
                        data = new TileData();

                        data.TileType = "Tree";

                        int f = rnd.Next(0, 101);

                        if (f <= 50)
                            data.TileType = "Tree2";

                        data.IsSolid = true;

                        if (!overlay.ContainsKey(p))
                            overlay.Add(p, new List<TileData>());


                        overlay[p].Add(data);
                    }
                    else if (terrainOverlayData[w + (h * width)] == Color.Yellow)
                    {
                        data = new TileData();

                        data.TileType = "Stumps";

                        if (!overlay.ContainsKey(p))
                            overlay.Add(p, new List<TileData>());


                        overlay[p].Add(data);
                    }
                    else if (terrainOverlayData[w + (h * width)] == Color.Blue)
                    {
                        data = new TileData();

                        data.TileType = "Log1";


                        int f = rnd.Next(0, 101);

                        if (f <= 25)
                            data.TileType = "Log1";
                        else if (f <= 50)
                            data.TileType = "Log2";
                        else if (f <= 75)
                            data.TileType = "Log3";
                        else
                            data.TileType = "Log4";

                        if (!overlay.ContainsKey(p))
                            overlay.Add(p, new List<TileData>());


                        overlay[p].Add(data);
                    }
                    else if (terrainOverlayData[w + (h * width)] == Color.Red)
                    {
                        data = new TileData();

                        data.TileType = "Mountain";
                        data.IsSolid = true;

                        if (!overlay.ContainsKey(p))
                            overlay.Add(p, new List<TileData>());


                        overlay[p].Add(data);
                    }

                    // Buildings and Roads
                    if (buildingData[w + (h * width)] == Color.White)
                    {
                        if (!overlay.ContainsKey(p))
                            overlay.Add(p, new List<TileData>());

                        data = new TileData();
                        data.TileType = "VPath1";

                        overlay[p].Add(data);
                    }
                    else if (buildingData[w + (h * width)] == Color.Red)
                    {
                        if (!overlay.ContainsKey(p))
                            overlay.Add(p, new List<TileData>());

                        data = new TileData();
                        data.TileType = "PathToOpenN";
                        data.ExitTo = "AreaMap";
                        data.EnterIn = new Vector2(12, 25);

                        overlay[p].Add(data);
                    }
                    else if (buildingData[w + (h * width)] == Color.Blue)
                    {
                        if (!overlay.ContainsKey(p))
                            overlay.Add(p, new List<TileData>());


                        data = new TileData();
                        data.TileType = "WallTop";
                        data.IsSolid = true;

                        overlay[p].Add(data);
                    }
                    else if (buildingData[w + (h * width)] == Color.DarkBlue)
                    {
                        if (!overlay.ContainsKey(p))
                            overlay.Add(p, new List<TileData>());

                        data = new TileData();
                        data.TileType = "WallBottom";
                        data.IsSolid = true;

                        overlay[p].Add(data);
                    }
                    else if (buildingData[w + (h * width)] == Color.LightBlue)
                    {
                        if (!overlay.ContainsKey(p))
                            overlay.Add(p, new List<TileData>());

                        data = new TileData();
                        data.TileType = "WallHEndL";
                        data.IsSolid = true;

                        overlay[p].Add(data);
                    }
                    else if (buildingData[w + (h * width)] == Color.SkyBlue)
                    {
                        if (!overlay.ContainsKey(p))
                            overlay.Add(p, new List<TileData>());

                        data = new TileData();
                        data.TileType = "WallHEndR";
                        data.IsSolid = true;

                        overlay[p].Add(data);
                    }
                    else if (buildingData[w + (h * width)] == Color.BlueViolet)
                    {
                        if (!overlay.ContainsKey(p))
                            overlay.Add(p, new List<TileData>());

                        data = new TileData();
                        data.TileType = "Open";

                        overlay[p].Add(data);

                        data = new TileData();
                        data.TileType = "House1";

                        int f = rnd.Next(0, 101);

                        if (f <= 25)
                            data.TileType = "House1";
                        else if (f <= 50)
                            data.TileType = "House2";
                        else if (f <= 75)
                            data.TileType = "House3";
                        else
                            data.TileType = "House4";

                        data.IsSolid = true;

                        overlay[p].Add(data);
                    }
                    else if (buildingData[w + (h * width)] == Color.Yellow)
                    {
                        if (!overlay.ContainsKey(p))
                            overlay.Add(p, new List<TileData>());

                        data = new TileData();
                        data.TileType = "Open";

                        overlay[p].Add(data);
                    }
                    else if (buildingData[w + (h * width)] == Color.Green)
                    {
                        if (!overlay.ContainsKey(p))
                            overlay.Add(p, new List<TileData>());

                        data = new TileData();
                        data.TileType = "WallTopLeft";
                        data.IsSolid = true;

                        overlay[p].Add(data);
                    }
                    else if (buildingData[w + (h * width)] == Color.Lime)
                    {
                        if (!overlay.ContainsKey(p))
                            overlay.Add(p, new List<TileData>());

                        data = new TileData();
                        data.TileType = "WallBottomLeft";
                        data.IsSolid = true;

                        overlay[p].Add(data);
                    }
                    else if (buildingData[w + (h * width)] == Color.Gold)
                    {
                        if (!overlay.ContainsKey(p))
                            overlay.Add(p, new List<TileData>());

                        data = new TileData();
                        data.TileType = "WallLeft";
                        data.IsSolid = true;

                        overlay[p].Add(data);
                    }
                    else if (buildingData[w + (h * width)] == Color.Orange)
                    {
                        if (!overlay.ContainsKey(p))
                            overlay.Add(p, new List<TileData>());

                        data = new TileData();
                        data.TileType = "WallRight";
                        data.IsSolid = true;

                        overlay[p].Add(data);
                    }
                    else if (buildingData[w + (h * width)] == Color.Black)
                    {
                        if (!overlay.ContainsKey(p))
                            overlay.Add(p, new List<TileData>());

                        data = new TileData();
                        data.TileType = "WallTopRight";
                        data.IsSolid = true;

                        overlay[p].Add(data);
                    }
                    else if (buildingData[w + (h * width)] == Color.DarkGray)
                    {
                        if (!overlay.ContainsKey(p))
                            overlay.Add(p, new List<TileData>());

                        data = new TileData();
                        data.TileType = "WallBottomRight";
                        data.IsSolid = true;

                        overlay[p].Add(data);
                    }
                    else if (buildingData[w + (h * width)] == Color.Beige)
                    {
                        if (!overlay.ContainsKey(p))
                            overlay.Add(p, new List<TileData>());


                        data = new TileData();
                        data.TileType = "Open";

                        overlay[p].Add(data);

                        data = new TileData();
                        data.TileType = "Inn";
                        data.ExitTo = "Inn";
                        data.EnterIn = new Vector2(11, 1);

                        overlay[p].Add(data);
                    }

                }
            }

            GenerateMap(spriteSheet, tileSize, cellSize, map, overlay, animation);
        }

        

    }
}
