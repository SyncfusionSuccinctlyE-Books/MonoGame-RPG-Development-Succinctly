using System.Collections.Generic;

namespace MonoGameRPG
{
    public interface IInventoryItem : ISprite
    {
        /// <summary>
        /// Short description of the item
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// Long detailed description of the item.
        /// </summary>
        string Description { get; set; }
        /// <summary>
        /// The current monetary value of the item
        /// </summary>
        decimal Value { get; set; }
        /// <summary>
        /// The weight of the item in kg (easer as it's base 10)
        /// </summary>
        float Weight { get; set; }
        
        float Condition { get; set; }


        List<string> Mods { get; set; }
        /// <summary>
        /// Where if anywhere a player can wear/ equip the item.
        /// </summary>
        EquipableLocation EquipableLocation { get; set; }

    }
}
