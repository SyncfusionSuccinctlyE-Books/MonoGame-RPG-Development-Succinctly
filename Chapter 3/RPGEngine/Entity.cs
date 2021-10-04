using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace RPGEngine
{
    public delegate void EntityLevelUpDelegate(Entity entity);

    [Serializable]
    public class Entity
    {
        public EntityType Type { get; set; }

        public string Name { get; set; }
        public string ClassID { get; set; }
        public byte Level { get; set; }

        public string RaceID { get; set; }

        public short BaseHP { get; set; }
        private short curHP;

        public EntityAlignment Alignment { get; set; }

        public EntitySex Sex { get; set; }
        public short Age { get; set; }

        private List<EntityStat> stats;

        public string PortraitFileName { get; set; }

        public string SpriteFilename { get; set; }

        //requires a strength stat
        private float maxWeight;

        public int MaxWeight()
        {
            int str = 0;

            //find the strength stat
            foreach (EntityStat stat in stats)
            {
                if (stat.StatName.ToLower() == "strength")
                {
                    str = stat.CurrentValue;
                    break;
                }
            }

            return str * Stat.PoundsPerStatPoint;
        }

        public void AddStat(EntityStat stat)
        {
            if(stats == null)
            {
                stats = new List<EntityStat>();
            }

            stats.Add(stat);
        }
    }
}
