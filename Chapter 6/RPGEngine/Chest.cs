using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace RPGEngine
{
    public class Chest : Item
    {
        public bool Locked{ get; set; }

        //ID of the key item required to open the lock, 
        //set to 0 if a generic key item will open the lock
        public int KeyRequired{ get; set; }

        public bool Open { get; set; }

        public bool RandomlyGenerate{ get; set; }
        public ItemType RandomType{ get; set; }
        public int MaxValue{ get; set; }

        public bool Trapped{ get; set; }
        public TrapType TrapType{ get; set; }
        public Spell TrapSpell{ get; set; }
        public Difficulty TrapLevel{ get; set; }
        public int TrapDamage{ get; set; }

        public List<int> Items{ get; set; }


        public bool IsOpen
        {
            get { return Open; }
            set 
            { 
                Open = value; 
                if (Open)
                    Locked = false; 
            }
        }

        public override bool Use(ref object target, ref Entity wielder)
        {
            //if the chest is already open return
            if (Open)
                return true;

            //target in this case is null, since the chest is the target
            //wielder is the entity trying to open the chest
            //call PickLock if the chest is locked
            if (Locked)
            {
                if (!wielder.HasItem(KeyRequired))
                    return false;

                Open = true;
            }

            return true;
        }

        public bool PickLock(ref Entity entity)
        {
            bool ret = false;
            int ID;

            //check for trapped first and resolve avoiding or getting damaged by trap
            if (Trapped)
            {
                //see if entity has disarm skill
                ID = GlobalFunctions.GetIDForSkillName("Disarm Trap");
                if (ID > 0)
                    Trapped = false;
                else
                {
                    //convert trap type to damage type
                    DamageType type = GlobalFunctions.GetDamageTypeForTrap(TrapType);

                    entity.Damage(TrapDamage, type);

                }
            }

            //function should never be called for an unlocked chest but just in case...
            if (Locked)
            {
                ID = GlobalFunctions.GetIDForSkillName("Pick Lock");
                if (ID > 0)
                {
                    if (entity.HasSkillByID(ID))
                    {

                    }
                }
            }
            else
                ret = true;

            return ret;
        }

        public List<int> GetItems()
        {
            if (RandomlyGenerate)
            {
                int value = 0;
                List<int> ret = new List<int>();
                int itemType;
                Item item;
                do
                {
                    //get a random item type
                    itemType = GlobalFunctions.GetRandomNumber(0, Enum.GetValues(typeof(ItemType)).Length);

                    //get a random item of that type
                    item = GlobalData.Items[itemType][GlobalFunctions.GetRandomNumber(0, GlobalData.Items[itemType].Count)];

                    //add the value of that item to the value of the items in the list
                    if (value + item.Cost <= MaxValue)
                    {
                        value += item.Cost;
                    }
                }
                while (value < MaxValue);

                return ret;
            }
            else
            {
                if (Items != null)
                {
                    List<int> items = new List<int>();

                    items.AddRange(Items);

                    Items.Clear();

                    return items;
                }
                else
                    throw (new Exception("Invalid items list"));
            }
        }
    }
}
