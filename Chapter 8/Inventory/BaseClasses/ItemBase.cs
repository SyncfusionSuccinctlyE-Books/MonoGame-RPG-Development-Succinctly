using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

//using MonoGameRPG.Interfaces;

namespace MonoGameRPG
{
    public class ItemBase : Sprite, IInventoryItem
    {
        /// <summary>
        /// Short description of the item
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Long detailed description of the item.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// The current monetary value of the item
        /// </summary>
        public decimal Value { get; set; }
        /// <summary>
        /// The weight of the item in kg (easer as it's base 10)
        /// </summary>
        public float Weight { get; set; }
        
        public float Condition { get; set; }
        /// <summary>
        /// Where if anywhere a player can wear/ equip the item.
        /// </summary>
        public EquipableLocation EquipableLocation { get; set; }

        /// <summary>
        /// String markers for the advantages, disadvantages of the item.
        /// </summary>
        public List<string> Mods { get; set; }

        public ItemBase(Texture2D asset, Point size) : base(asset, size, new Point(16, 16))
        {
            // Set some default values..
            Weight = .1f;
            Condition = 1;
        }

    }
}
