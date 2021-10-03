using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameRPG
{
    public class ConsumableItem : ItemBase, IConsumable
    {
        public int Quantity { get; set; }

        public ConsumableItem(Texture2D asset, Point size) : base(asset, size) { }
    }
}
