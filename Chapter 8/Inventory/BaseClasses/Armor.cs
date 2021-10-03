using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameRPG
{
    public class Armor : ItemBase, IArmor
    {
        public int ArmorValue { get; set; }

        public Armor(Texture2D asset, Point size) : base(asset, size)
        {
            ArmorValue = 1;
        }
    }
}
