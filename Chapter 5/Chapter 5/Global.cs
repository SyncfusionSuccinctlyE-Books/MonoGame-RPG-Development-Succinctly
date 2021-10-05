using Microsoft.Xna.Framework;
using System.Collections.Generic;
using RPGEngine;

namespace MonoGameRPG
{
    public class Global
    {
        public static string CharacterName;
        public static Point TileSize = new Point();

        public static string CurrentLevelName = "Town";
        public static bool FirstTimeLoad = true;

        public static bool CharacterInRangeofNPC(GameObject character, GameObject npc)
        {
            int charX = (int)character.GameSprite.Position.X / (int)TileSize.X;
            int charY = (int)character.GameSprite.Position.Y / (int)TileSize.Y;
            int npcX = (int)npc.GameSprite.Position.X / (int)TileSize.X;
            int npcY = (int)npc.GameSprite.Position.Y / (int)TileSize.Y;

            //assumes a range of 3 tiles
            return (MathHelper.Distance(charX, npcX) <= 3 && MathHelper.Distance(charY, npcY) <= 3);
        }

        public static bool RectClicked(Vector2 position, Point size, float x, float y)
        {
            return new Rectangle(new Point((int)position.X, (int)position.Y), size).Contains(x, y);
        }
    }
}
