using System;
using System.Collections.Generic;
using System.Text;

namespace RPGEngine
{
    [Serializable()]
    public class Armor : Item
    {
        public ArmorArea Location { get; set; }
        public short DefValue { get; set; }
        public bool Equipped { get; set; }

        public override bool Use(ref Object target, ref Entity wielder)
        {
            //Use for this class means to equip, so set member if it's being equipped
            short loc = (short)target;

            if (loc > -1)
            {
                //the value tells us where it's trying to be equipped so check it
                //for instance, a piece is dragged onto the wrong location in the character screen
                if (loc != (short)Location)
                    Equipped = true;
            }
            else
                Equipped = false;
            
            return true;
        }
    }
}
