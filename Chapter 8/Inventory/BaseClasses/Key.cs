using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameRPG
{
    public class Key : ItemBase, IKey
    {
        public long LockID { get; set; }

        public Key(Texture2D asset, Point size) : base(asset, size) { }
    }
}
