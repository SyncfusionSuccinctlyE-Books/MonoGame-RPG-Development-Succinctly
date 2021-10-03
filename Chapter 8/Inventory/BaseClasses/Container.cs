using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using MonoGameRPG.StateManagement;

namespace MonoGameRPG
{
    public class Container : ItemBase, IContainerItem
    {
        protected bool IsOpen { get; set; }
        public IInventoryContainer Content { get; set; }
        public int? MaxVolume { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Container(Texture2D asset, Point size) : base(asset, size)
        { }

        public void Open()
        {
            IsOpen = true;
        }

        public void AddItem(IInventoryItem item)
        {
            Content.AddItem(item);
        }

        public void RemoveItem(IInventoryItem item)
        {
            Content.RemoveItem(item);
        }

        public void HandleInput(GameTime gameTime, PlayerIndex? playerIndex, InputState input)
        {
            Content.HandleInput(gameTime, playerIndex, input);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);

            if (IsOpen)
                Content.Draw(gameTime, spriteBatch);
        }
    }
}
