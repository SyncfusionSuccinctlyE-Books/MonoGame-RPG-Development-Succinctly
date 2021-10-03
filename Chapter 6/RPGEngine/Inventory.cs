using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RPGEngine
{
    public class Inventory
    {
		//can't use a Dictionary here because the inventory
		//could contain multiple of the same item, therefore the same id
		private List<EntityItem> items;
		private Dictionary<int, int> equippedItems; //Key is an ArmorArea

        public List<EntityItem> Items
        {
            get { return items; }
            set { items = value; }
        }
        
        public Item EquippedItem(ArmorArea area)
        {
            return GlobalFunctions.GetItem(equippedItems[(int)area]);
        }

        public float TotalWeight()
        {
            float weight = 0.0f;

            foreach (EntityItem item in items)
                weight += GlobalFunctions.GetItem(item.ID).Weight;

            return weight;
        }

		public void Add(EntityItem item)
        {
            //check to see if entity is overweight
            //needs to be done in the Entity class
            items.Add(item);
        }

		public EntityItem Get(int id)
        {
			  foreach (EntityItem item in items)
                if (item.ID == id)
                    return item;

            return null;
        }

        public void Remove(int id)
        {
          //if the item is equipped remove it from the equipped list
          if (equippedItems.ContainsValue(id))
          {
	          foreach (KeyValuePair<int, int> item in equippedItems)
	          {
		          if (item.Value == id)
		          {
			          equippedItems.Remove(item.Key);
			          break;
		          }
	          }
          }

          items.Remove(items[id]);
        }

        public bool HasItem(int id)
        {
            return items.Find(i => i.ID == id) != null;
        }

        public Texture2D GetArmorAreaPicture(ArmorArea area)
        {
          foreach (EntityItem item in items)
	          if (item.ID == equippedItems[(int)area])
		          return GlobalFunctions.GetItem(item.ID).Picture;

          return null;
        }
    }
}
