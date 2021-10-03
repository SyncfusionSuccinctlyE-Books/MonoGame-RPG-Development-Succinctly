using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameRPG
{
    public class Ammunition : ItemBase, IAmmunition
    {
        public int Quantity { get; set; }
        public List<string>  Weapons { get; protected set; }

        public Ammunition(Texture2D asset, Point size, params string[] weapons) : base(asset, size)
        {
            Weapons = new List<string>();
            Weapons.AddRange(weapons);
        }
    }
}
