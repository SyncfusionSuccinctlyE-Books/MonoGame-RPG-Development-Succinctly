using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework.Input;

namespace RPGEngine
{
    public class EntitySpell
    {
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public int ID { get; set; }
        public bool Learned { get; set; }
        public bool Memorized { get; set; }
        public int SkillPointsAllocated { get; set; }
        public long LastCast { get; set; }
        public Keys HotKey { get; set; }

        public void AllocatePoints(int amount)
        {
            SkillPointsAllocated += amount;
        }

        public void Memorize()
        {
            if (Learned)
                Memorized = true;
        }

        public bool Learn(int bonus)
        {
            short roll = GlobalFunctions.GetRandomNumber(DieType.d100);

            if (roll + bonus + (SkillPointsAllocated * 5) >= 100)
            {
                Learned = true;
                SkillPointsAllocated = 0;
            }

            return Learned;
        }
    }
}
