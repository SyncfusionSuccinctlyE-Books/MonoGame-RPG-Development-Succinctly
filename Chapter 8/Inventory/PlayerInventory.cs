using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using VoidEngineLight.Animation;

using MonoGameRPG.StateManagement;
//using MonoGameRPG.Interfaces;

namespace MonoGameRPG
{

    /// <summary>
    /// This is not what the player is wearing, it's what he is carrying.
    /// </summary>
    public class PlayerInventory
    {
        public Vector2 Position { get; set; }
        public InventoryBase Inventory { get; set; }

        protected IInventoryItem mouseOverItem = null;
        public Dictionary<EquipableLocation, IInventoryItem> Equipped { get; set; }

        public Sprite Background { get; set; }
        public Sprite WearingBG { get; set; }

        public SpriteFont Font { get; set; }
        protected SpriteFont smallFont { get; set; }


        public Sprite SlotBox { get; set; }

        public float Encumberance { get; set; }
        public int TotalArmorValue { get; set; }
        
        IInventoryContainer currentContainer { get; set; }
        public IInventoryItem DroppedItem { get; set; }

        public bool IsShowing { get; set; }


        public PlayerInventory(ContentManager _content)
        {
            Texture2D playerInventotyBG = _content.Load<Texture2D>("Sprites/Inventory/InventoryBox");
            Texture2D bodyBG = _content.Load<Texture2D>("Sprites/Inventory/bodyOutline");
            Texture2D slotBox = _content.Load<Texture2D>("Sprites/Inventory/SlotBox");
            SpriteFont font = _content.Load<SpriteFont>("Fonts/Inventory");
            SpriteFont fontSmall = _content.Load<SpriteFont>("Fonts/tinyFont");
            Texture2D testItem = _content.Load<Texture2D>("Sprites/Inventory/closeButton");
            Texture2D ubut = _content.Load<Texture2D>("Sprites/Inventory/UpButtton");
            Texture2D dbut = _content.Load<Texture2D>("Sprites/Inventory/DownButton");

            Font = font;
            smallFont = fontSmall;
            Background = new Sprite(playerInventotyBG, new Point(440, 440), new Point(512, 512));
            Background.Tint = Color.DarkGoldenrod;

            WearingBG = new Sprite(bodyBG, new Point(113, 228), new Point(151, 330));

            // Equipable slots
            Equipped = new Dictionary<EquipableLocation, IInventoryItem>()
            {
                {  EquipableLocation.Head, null },
                {  EquipableLocation.Neck, null },
                {  EquipableLocation.Body, null },
                {  EquipableLocation.Chest, null },
                {  EquipableLocation.Abdomen, null },
                {  EquipableLocation.Left_Arm, null },
                {  EquipableLocation.Right_Arm, null },
                {  EquipableLocation.Left_Leg, null },
                {  EquipableLocation.Right_Leg, null },
                {  EquipableLocation.Feet, null },
                {  EquipableLocation.TwoHanded, null },
                {  EquipableLocation.Left_Hand, null },
                {  EquipableLocation.Right_Hand,null },
            };

            SlotBox = new Sprite(slotBox, new Point(22, 22), new Point(64, 64));
            SlotBox.Tint = Color.CornflowerBlue;

            Inventory = new InventoryBase(playerInventotyBG, slotBox, font, fontSmall, ubut, dbut);            
        }

        protected void DrawEquipedSlot(EquipableLocation location, GameTime gameTime, SpriteBatch spriteBatch)
        {
            Vector2 position = WearingBG.Position;

            switch (location)
            {
                case EquipableLocation.Head:
                    position += new Vector2((WearingBG.Size.X / 2) - 10, 4);
                    break;
                case EquipableLocation.Chest:
                    position += new Vector2((WearingBG.Size.X / 2) - 10, 40);
                    break;
                case EquipableLocation.Left_Arm:
                    position += new Vector2((WearingBG.Size.X / 2) - 40, 40);
                    break;
                case EquipableLocation.Left_Hand:
                    position += new Vector2((WearingBG.Size.X / 2) - 50, 120);
                    break;
                case EquipableLocation.Right_Hand:
                    position += new Vector2((WearingBG.Size.X / 2) + 32, 120);
                    break;
                case EquipableLocation.Right_Arm:
                    position += new Vector2((WearingBG.Size.X / 2) + 22, 40);
                    break;
                case EquipableLocation.Abdomen:
                    position += new Vector2((WearingBG.Size.X / 2) - 10, 80); ;
                    break;
                case EquipableLocation.Left_Leg:
                    position += new Vector2((WearingBG.Size.X / 2) - 26, 145);
                    break;
                case EquipableLocation.Right_Leg:
                    position += new Vector2((WearingBG.Size.X / 2) + 3, 145);
                    break;
                case EquipableLocation.Feet:
                    position += new Vector2((WearingBG.Size.X / 2) - 10, 220);
                    break;
            }

            SlotBox.Position = position;
            SlotBox.Tint = Color.CornflowerBlue;

            ItemBase item = null;
            if (Equipped[EquipableLocation.TwoHanded] != null && (location == EquipableLocation.Left_Hand || location == EquipableLocation.Right_Hand))
            {
                location = EquipableLocation.TwoHanded;
            }

            if (Equipped[location] != null)
            {
                item = (ItemBase)Equipped[location];
                item.Position = SlotBox.Position - new Vector2(5, 8);

                if (item == mouseOverItem)
                    SlotBox.Tint = Color.White;
            }

            SlotBox.Draw(gameTime, spriteBatch);

            if (item != null)
                item.Draw(gameTime, spriteBatch);
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Vector2 titleSize = Font.MeasureString("Inventory");
            
            Background.Position = Position;
            WearingBG.Position = Position + new Vector2(8, titleSize.Y + 32);

            Background.Draw(gameTime, spriteBatch);
            WearingBG.Draw(gameTime, spriteBatch);

            // Draw equable slots..
            // Head.
            DrawEquipedSlot(EquipableLocation.Head, gameTime, spriteBatch);

            // Chest
            DrawEquipedSlot(EquipableLocation.Chest, gameTime, spriteBatch);

            // Left Arm
            DrawEquipedSlot(EquipableLocation.Left_Arm, gameTime, spriteBatch);

            // Left Hand
            DrawEquipedSlot(EquipableLocation.Left_Hand, gameTime, spriteBatch);

            // Right Hand
            DrawEquipedSlot(EquipableLocation.Right_Hand, gameTime, spriteBatch);

            // Right Arm
            DrawEquipedSlot(EquipableLocation.Right_Arm, gameTime, spriteBatch);

            // Abdomen
            DrawEquipedSlot(EquipableLocation.Abdomen, gameTime, spriteBatch);

            // Left Leg
            DrawEquipedSlot(EquipableLocation.Left_Leg, gameTime, spriteBatch);

            // Right Leg
            DrawEquipedSlot(EquipableLocation.Right_Leg, gameTime, spriteBatch);

            // Feet.
            DrawEquipedSlot(EquipableLocation.Feet, gameTime, spriteBatch);

            Vector2 containerPos = Position + new Vector2(WearingBG.Size.X + 16, titleSize.Y + 8 + Font.LineSpacing);
            Inventory.Position = containerPos;
            if(Inventory.IsShowing)
                Inventory.Draw(gameTime, spriteBatch);

            

            float cl = (Background.Size.X / 2) - titleSize.X / 2;
            spriteBatch.DrawString(Font, "Inventory", Position + new Vector2(cl-2, 6), Color.Black);
            spriteBatch.DrawString(Font, "Inventory", Position + new Vector2(cl, 4), Color.Gold);

            Encumberance = Inventory.TotalWeight + Equipped.Sum(i => i.Value == null ? 0 : i.Value.Weight);
            

            TotalArmorValue = Equipped.Sum(i => i.Value == null ? 0 : i.Value is IArmor ? ((IArmor)i.Value).ArmorValue : 0);

            spriteBatch.DrawString(Font, $"Total Encumbrance: {Encumberance}", Position + new Vector2(16, Font.LineSpacing + 4), Color.White);
            spriteBatch.DrawString(Font, $"Total AV: {TotalArmorValue}", Position + new Vector2(256, Font.LineSpacing + 4), Color.White);

            foreach (EquipableLocation loc in Equipped.Keys)
            {
                IInventoryItem item = Equipped[loc];
                if (item != null && item == mouseOverItem)
                {
                    string iloc = $"{loc.ToString().Replace("_"," ")}: {item.Name}";
                    
                    Point s = smallFont.MeasureString(iloc).ToPoint();
                    Point p = (item.Position + new Vector2(0, smallFont.LineSpacing * 2)).ToPoint();
                    
                    spriteBatch.Draw(SlotBox.spriteTexture, new Rectangle(p.X-4, p.Y-4, s.X+8, s.Y+8), Color.Gold);
                    spriteBatch.DrawString(smallFont, iloc, p.ToVector2(), Color.White);
                }
            }
        }
       
        
        public void HandleInput(GameTime gameTime,PlayerIndex? playerIndex, InputState input)
        {
            PlayerIndex pidx;
            Inventory.HandleInput(gameTime, playerIndex, input);

            // Drop/remove item
            if (Inventory.SelectedItem != null && input.IsNewKeyPress(Keys.D, playerIndex, out pidx))
            {
                // Drop the selected item
                Drop(Inventory.SelectedItem);
            }

            // Equip item.
            if (input.IsNewKeyPress(Keys.Enter, playerIndex, out pidx) && Inventory.SelectedItem != null)
            {
                EquipItem(Inventory.SelectedItem);
            }

            mouseOverItem = null;

            // Mouse over any equiped items?
            foreach (EquipableLocation location in Equipped.Keys)
            {
                if (Equipped[location] != null)
                {
                    ItemBase item = (ItemBase)Equipped[location];
                    
                    if (input.MousePointerRect.Intersects(item.BoundsRectangle))
                    {
                        mouseOverItem = item;

                        if (input.IsNewMouseButtonPressed())
                        {
                            UnEquip(location);
                            break;
                        }
                    }
                }
            }
        }


        public string Pickup(IInventoryItem item)
        {
            if (item != null)
            {
                item.Size = Inventory.InvSlotBox.Size;
                Inventory.AddItem(item);

                return $"You have picked up {item.Name}";
            }
            else
                return "Nothing to pick up...";
        }

        public string Drop(IInventoryItem item)
        {
            Inventory.RemoveItem(item);
            DroppedItem = item;

            return $"You have dropped {item.Name}";
        }

        public string UnEquip(EquipableLocation targetLocation)
        {
            if (Equipped[targetLocation] == null)
                return $"There is nothing to remove here.";

            IInventoryItem item = Equipped[targetLocation];
            Equipped[targetLocation] = null;

            // Add it back to inventory.
            Pickup(item);

            return $"You are no longer using {item.Name}";
        }


        public string EquipItem(IInventoryItem item)
        {
            EquipableLocation location = Inventory.SelectedItem.EquipableLocation;

            if (location != EquipableLocation.None)
            {

                if (location == EquipableLocation.Hand)
                {
                    // Is there anything in either hand?
                    if (Equipped[EquipableLocation.Left_Hand] == null || Equipped[EquipableLocation.Right_Hand] == null)
                    {
                        if (Equipped[EquipableLocation.Left_Hand] == null)
                            location = EquipableLocation.Left_Hand;
                        else
                            location = EquipableLocation.Right_Hand;
                    }
                    else
                        location = EquipableLocation.Right_Hand;
                }

                if (Equipped.ContainsKey(location))
                {
                    UnEquip(location);

                    if (location == EquipableLocation.TwoHanded)
                    {
                        UnEquip(EquipableLocation.Left_Hand);
                        UnEquip(EquipableLocation.Right_Hand);
                    }
                    else if (location == EquipableLocation.Left_Hand || location == EquipableLocation.Right_Hand)
                    {
                        if (Equipped[EquipableLocation.TwoHanded] != null)
                            UnEquip(EquipableLocation.TwoHanded);
                    }

                    Equipped[location] = Inventory.SelectedItem;
                    Inventory.RemoveItem(Inventory.SelectedItem);

                    Inventory.SelectedItem = null;

                    return $"{Equipped[location].Name} has been equiped...";
                }
                else
                    return "You can't equip this item.";
            }
            else
            {
                if (Inventory.SelectedItem is IInventoryContainer)
                {
                    // Render the container...
                    currentContainer = (IInventoryContainer)Inventory.SelectedItem;
                }
            }

            return "You can't equip this item.";
        }
    }
}

