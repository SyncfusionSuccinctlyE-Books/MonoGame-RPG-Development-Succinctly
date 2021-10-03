using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

//using MonoGameRPG.Interfaces;
using MonoGameRPG.StateManagement;

namespace MonoGameRPG
{
    public class InventoryBase : IInventoryContainer
    {
        
        public string Name { get; set; }

        public string Description { get; set; }
        public Sprite InvSlotBox { get; set; }

        public Vector2 Position { get; set; }

        public Sprite InventoryContainer { get; set; }

        public int invSize = 8;
        public int invBox = 32;
        public float invVPos = 0;
        
        int maxLines = 0;

        protected IInventoryItem mouseOver = null;
        public IInventoryItem SelectedItem = null;

        Sprite up, down;

        Texture2D scrollBarRect;

        public SpriteFont Font { get; set; }
        public SpriteFont tinyFont { get; set; }

        /// <summary>
        /// Maximum number of slots. NOTE this is a nullable int, a null means that there is no limit
        /// </summary>
        public int? MaxVolume { get; set; }

        public List<IInventoryItem> Items { get; set; }

        public float TotalWeight { get; set; }

        protected Sprite closeButton { get; set; }

        public bool IsShowing { get; set; }

        public InventoryBase(Texture2D background, Texture2D slotBox, SpriteFont font, SpriteFont fontSmall, Texture2D upButton, Texture2D downButton, Texture2D closeBtn = null)
        {
            Font = font;
            tinyFont = fontSmall;
            Items = new List<IInventoryItem>();

            InventoryContainer = new Sprite(background, new Point(32 * invSize + 32, 255), new Point(512, 512));
            InventoryContainer.Tint = Color.Cyan;

            InvSlotBox = new Sprite(slotBox, new Point(invBox, invBox), new Point(64, 64));
            InvSlotBox.Tint = Color.DarkCyan;

            up = new Sprite(upButton, new Point(16, 16), new Point(64, 64));
            down = new Sprite(downButton, new Point(16, 16), new Point(64, 64));

            if (closeBtn != null)
            {
                closeButton = new Sprite(closeBtn, new Point(16, 16), new Point(closeBtn.Width, closeBtn.Height));
                closeButton.Tint = Color.White;
            }

            IsShowing = true;
        }

        public void AddItem(IInventoryItem item)
        {
            if(MaxVolume == null || Items.Count + 1 <= MaxVolume.Value)
                Items.Add(item);
        }

        public void RemoveItem(IInventoryItem item)
        {
            Items.Remove(item);
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (scrollBarRect == null)
            {
                scrollBarRect = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                scrollBarRect.SetData(new Color[] { Color.White });
            }

            // Inventory container
            InventoryContainer.Position = Position;
            InventoryContainer.Draw(gameTime, spriteBatch);

            Rectangle orgRect = spriteBatch.GraphicsDevice.ScissorRectangle;

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.DepthRead, new RasterizerState() { ScissorTestEnable = true, });
            spriteBatch.GraphicsDevice.ScissorRectangle = InventoryContainer.BoundsRectangle;
            int c = 0;

            if (SelectedItem == null && Items.Count > 0)
                SelectedItem = Items[0];

            foreach (IInventoryItem item in Items)
            {
                int l = c / invSize;
                int nl = l % invSize;
                Vector2 itemPos = Position + new Vector2(4 + ((c * invBox) - (l * invSize * invBox)), 4 + (l * invBox));

                if (l == 0)
                    maxLines++;

                itemPos.Y += invVPos;

                if (item == SelectedItem)
                    InvSlotBox.Tint = Color.White;
                else if (item == mouseOver)
                    InvSlotBox.Tint = Color.Cyan;
                else
                    InvSlotBox.Tint = Color.DarkCyan;

                InvSlotBox.Position = itemPos;
                InvSlotBox.Draw(gameTime, spriteBatch);

                // Render Item in it...
                if (item is ItemBase)
                {
                    item.Position = itemPos;
                    ((ItemBase)item).Draw(gameTime, spriteBatch);
                }

                c++;

                spriteBatch.DrawString(tinyFont, $"{c}", itemPos + new Vector2(2, 2), Color.White);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.DepthRead);            

            if (closeButton != null)
            {
                closeButton.Position = Position + new Vector2(InventoryContainer.Size.X - 24, 4);
                up.Position = Position + new Vector2(InventoryContainer.Size.X - 24, 4 + 16);
            }
            else
                up.Position = Position + new Vector2(InventoryContainer.Size.X - 24, 4);

            down.Position = Position + new Vector2(InventoryContainer.Size.X - 24, InventoryContainer.Size.Y - 18);

            // Draw scroll bar BG
            spriteBatch.Draw(scrollBarRect, new Rectangle((int)up.Position.X, (int)up.Position.Y, 16, (int)((down.Position.Y) - (up.Position.Y)) + 16), Color.DarkGray);

            float v = (Items.IndexOf(SelectedItem) + 1f) / (float)Items.Count;
            Vector2 t = Vector2.Lerp(up.Position + new Vector2(0, 16), down.Position - new Vector2(0, 64), v);

            spriteBatch.Draw(scrollBarRect, new Rectangle((int)t.X, (int)t.Y, 16, 64), Color.White);

            if (closeButton != null)
                closeButton.Draw(gameTime, spriteBatch);

            up.Draw(gameTime, spriteBatch);
            down.Draw(gameTime, spriteBatch);

            Color baseColor = Color.Silver;

            float baseY = InventoryContainer.Size.Y + 8;

            if (SelectedItem != null)
            {
                spriteBatch.DrawString(tinyFont, $"Name: {SelectedItem.Name}", Position + new Vector2(0, baseY), baseColor);
                spriteBatch.DrawString(tinyFont, $"Description: {SelectedItem.Description}", Position + new Vector2(0, baseY + tinyFont.LineSpacing), baseColor);
                spriteBatch.DrawString(tinyFont, $"Value: {SelectedItem.Value}", Position + new Vector2(0, baseY + tinyFont.LineSpacing * 2), baseColor);
                spriteBatch.DrawString(tinyFont, $"Weight: {SelectedItem.Weight}", Position + new Vector2(0, baseY + tinyFont.LineSpacing * 3), baseColor);

                Color conditionColor = Color.Lerp(Color.Red, Color.LimeGreen, SelectedItem.Condition);
                spriteBatch.DrawString(tinyFont, $"Condition: {(int)(SelectedItem.Condition * 100f)}%", Position + new Vector2(0, baseY + tinyFont.LineSpacing * 4), conditionColor);
            }
            else
            {
                spriteBatch.DrawString(tinyFont, $"Name: ", Position + new Vector2(0, baseY), baseColor);
                spriteBatch.DrawString(tinyFont, $"Description: ", Position + new Vector2(0, baseY + tinyFont.LineSpacing), baseColor);
                spriteBatch.DrawString(tinyFont, $"Value: ", Position + new Vector2(0, baseY + tinyFont.LineSpacing * 2), baseColor);
                spriteBatch.DrawString(tinyFont, $"Weight: ", Position + new Vector2(0, baseY + tinyFont.LineSpacing * 3), baseColor);
                spriteBatch.DrawString(tinyFont, $"Condition: %", Position + new Vector2(0, baseY + tinyFont.LineSpacing * 4), baseColor);
            }

            if (SelectedItem is IWeapon)
            {
                spriteBatch.DrawString(tinyFont, $"Damage: {((IWeapon)SelectedItem).Damage}", Position + new Vector2(0, baseY + tinyFont.LineSpacing * 5), Color.Gold);
                spriteBatch.DrawString(tinyFont, $"Range: {((IWeapon)SelectedItem).Range}", Position + new Vector2(0, baseY + tinyFont.LineSpacing * 6), Color.Gold);
            }

            if (SelectedItem is IArmor)
            {
                spriteBatch.DrawString(tinyFont, $"Armour Value: {((IArmor)SelectedItem).ArmorValue}", Position + new Vector2(0, baseY + tinyFont.LineSpacing * 5), Color.Lime);
                spriteBatch.DrawString(tinyFont, $"Location: {SelectedItem.EquipableLocation}", Position + new Vector2(0, baseY + tinyFont.LineSpacing * 6), Color.Lime);
            }

            if (SelectedItem is IAmmunition)
            {
                spriteBatch.DrawString(tinyFont, $"Quantity: {((IAmmunition)SelectedItem).Quantity}", Position + new Vector2(0, baseY + tinyFont.LineSpacing * 5), Color.Lime);
                spriteBatch.DrawString(tinyFont, $"Weapon: {string.Join('-',((IAmmunition)SelectedItem).Weapons.ToArray())}", Position + new Vector2(0, baseY + tinyFont.LineSpacing * 6), Color.Lime);
            }

            if (SelectedItem is IConsumable)
            {
                spriteBatch.DrawString(tinyFont, $"Quantity: {((IConsumable)SelectedItem).Quantity}", Position + new Vector2(0, baseY + tinyFont.LineSpacing * 5), Color.Lime);
            }

            TotalWeight = Items.Sum(i => i.Weight);
            //spriteBatch.DrawString(tinyFont, $"Encumbrance: {TotalWeight}", Position + new Vector2(0, baseY + tinyFont.LineSpacing * 8), Color.Cyan);
        }

        public void HandleInput(GameTime gameTime, PlayerIndex? playerIndex, InputState input)
        {
            PlayerIndex pidx;

            // Navigate the inventory items.
            if (Items.Count > 0)
            {
                
                if (input.IsNewKeyPress(Keys.Right, playerIndex, out pidx))
                {
                    int idx = Items.IndexOf(SelectedItem);
                    idx = MathHelper.Min(++idx, Items.Count - 1);

                    SelectedItem = Items[idx];
                }

                if (input.IsNewKeyPress(Keys.Left, playerIndex, out pidx))
                {
                    int idx = Items.IndexOf(SelectedItem);
                    idx = MathHelper.Max(--idx, 0);

                    SelectedItem = Items[idx];
                }

                if (input.IsNewKeyPress(Keys.Down, playerIndex, out pidx))
                {
                    int idx = Items.IndexOf(SelectedItem);
                    idx = MathHelper.Min(idx + invSize, Items.Count - 1);
                    SelectedItem = Items[idx];
                }

                if (input.IsNewKeyPress(Keys.Up, playerIndex, out pidx))
                {
                    int idx = Items.IndexOf(SelectedItem);
                    idx = MathHelper.Max(idx - invSize, 0);
                    SelectedItem = Items[idx];
                }

                if (SelectedItem != null)
                {
                    if (SelectedItem.Position.Y < InventoryContainer.Position.Y)
                        invVPos += invBox;

                    if (SelectedItem.Position.Y + SelectedItem.Size.Y > InventoryContainer.Position.Y + InventoryContainer.Size.Y)
                        invVPos -= invBox;
                }

                if (invVPos > 0)
                    invVPos = 0;

                float max = (-(Items.Count / invSize) * invBox);// + invBox;
                if (invVPos < max)
                    invVPos = max;
            }

            if (input.MousePointerRect.Intersects(InventoryContainer.BoundsRectangle))
            {
                InventoryContainer.Tint = Color.White;

                // Are we over any of the items?
                mouseOver = null;
                foreach (ItemBase item in Items)
                {
                    if (input.MousePointerRect.Intersects(item.BoundsRectangle))
                    {
                        mouseOver = item;

                        if (input.IsNewMouseButtonPressed())
                            SelectedItem = item;

                        break;
                    }

                }

                if (input.MousePointerRect.Intersects(up.BoundsRectangle))
                {
                    up.Tint = Color.DarkCyan;
                    if (input.IsNewMouseButtonPressed())
                    {
                        up.Tint = Color.Cyan;
                        int idx = Items.IndexOf(SelectedItem);
                        idx = MathHelper.Max(idx - invSize, 0);
                        SelectedItem = Items[idx];
                    }
                }
                else
                    up.Tint = Color.White;

                if (input.MousePointerRect.Intersects(down.BoundsRectangle))
                {
                    down.Tint = Color.DarkCyan;
                    if (input.IsNewMouseButtonPressed())
                    {
                        down.Tint = Color.Cyan;
                        int idx = Items.IndexOf(SelectedItem);
                        idx = MathHelper.Min(idx + invSize, Items.Count - 1);
                        SelectedItem = Items[idx];
                    }
                }
                else
                    down.Tint = Color.White;

                if (closeButton != null)
                {
                    if (input.MousePointerRect.Intersects(closeButton.BoundsRectangle))
                    {
                        closeButton.Tint = Color.Red;
                        if (input.IsNewMouseButtonPressed())
                            IsShowing = false;
                    }
                    else
                        closeButton.Tint = Color.White;
                }
            }
            else
                InventoryContainer.Tint = Color.Cyan;
            
        }
    }
}
