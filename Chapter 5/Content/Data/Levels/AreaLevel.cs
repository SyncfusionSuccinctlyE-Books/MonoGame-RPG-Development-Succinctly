using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework.Content;


using MonoGameRPG.Animation;

namespace MonoGameRPG
{
    public class AreaLevel : TownLevel
    {
        public AreaLevel(ContentManager contentMgr, string tileSheetAsset, string mapAsset, string overlayMapAsset, string objectMapAsset, string mobMapAsset, Point tileSize, Point cellSize)
            : base(contentMgr, tileSheetAsset, mapAsset, overlayMapAsset, objectMapAsset, mobMapAsset, tileSize, cellSize)
        { }

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

                    if (buildingData[w + (h * width)] == Color.White)
                    {
                        if (!overlay.ContainsKey(p))
                            overlay.Add(p, new List<TileData>());

                        data = new TileData();
                        data.TileType = "Inn";
                        data.ExitTo = "Town";
                        data.EnterIn = new Vector2(13, 5);

                        overlay[p].Add(data);
                    }
                    else if (buildingData[w + (h * width)] == Color.Red || buildingData[w + (h * width)] == Color.Green || buildingData[w + (h * width)] == Color.Blue || buildingData[w + (h * width)] == Color.Yellow)
                    {
                        if (!overlay.ContainsKey(p))
                            overlay.Add(p, new List<TileData>());

                        data = new TileData();
                        if(buildingData[w + (h * width)] == Color.Red)
                            data.TileType = "TLHole";
                        if(buildingData[w + (h * width)] == Color.Green)
                            data.TileType = "TRHole";
                        if (buildingData[w + (h * width)] == Color.Blue)
                            data.TileType = "BLHole";
                        if (buildingData[w + (h * width)] == Color.Yellow)
                            data.TileType = "BRHole";

                        data.ExitTo = "Dungeon";
                        data.EnterIn = new Vector2(3, 1);

                        overlay[p].Add(data);
                        
                    }
                }
            }

            GenerateMap(spriteSheet, tileSize, cellSize, map, overlay, animation);
        }
    }

}
