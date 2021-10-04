using System;
using System.Collections.Generic;
using System.Text;

namespace RPGEngine
{
    public enum StatType
    {
        Regular,
        Calculated
    }
    
    public class Stat
    {
        public StatType Type { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public string Description { get; set; }

        public string StatCalculation { get; set; }

        private static readonly char[] operators = { '+', '-', '*', '/' };

        public const short MaxValue = (short)DieType.d100;
        public const int PoundsPerStatPoint = 3;
    }
    
    [Serializable]
    public class EntityStat
    {
        private short Value;
        public string StatName { get; set; }
        public short BaseValue { get; set; }
        public List<Bonus> Bonuses { get; set; }

        public EntityStat()
        {
        }

        public EntityStat(string stat, short value)
        {
            StatName = stat;
            Value = value;
        }

        public short CurrentValue
        {
            get 
            {
                short val = Value;

                if (Bonuses != null)
                {
                    foreach (Bonus bonus in Bonuses)
                        val += bonus.Amount;
                }
                
                return val; 
            }

            set { }
        }

        public void IncreaseValue(short val)
        {
            Value += val;
            if (Value > Stat.MaxValue)
                Value = Stat.MaxValue;
        }

        public void ReduceValue(short val)
        {
            Value -= val;
            if (Value < 1)
                Value = 1;
        }

        public void AddBonus(Bonus bonus)
        {
            if (Bonuses == null)
                Bonuses = new List<Bonus>();

            Bonuses.Add(bonus);
        }

        public void Update(float time)
        {
			  if (Bonuses == null)
				  return;

            Bonus bonus;

            for (int i = Bonuses.Count - 1; i >= 0; i--)
            {
                bonus = Bonuses[i];

                bonus.ElapsedTime += time;

                if ((int)bonus.ElapsedTime >= bonus.Duration)
                    Bonuses.Remove(bonus);
                else
                    Bonuses[i] = bonus;
            }
        }
    }
}
