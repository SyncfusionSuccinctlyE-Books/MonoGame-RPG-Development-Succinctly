using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameRPG
{
    public class Weapon : ItemBase, IWeapon
    {
        public string Damage { get; set; }
        public int Range { get; set; }

        public Weapon(Texture2D asset, Point size) : base(asset, size)
        {
            Damage = "D6";
            Mods = new List<string>();
            Range = 1;            
        }
    }
}
