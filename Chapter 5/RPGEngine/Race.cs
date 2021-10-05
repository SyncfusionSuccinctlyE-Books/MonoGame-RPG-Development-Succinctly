using System;
using System.Collections.Generic;
using System.Text;

namespace RPGEngine
{
    public class Race
    {
        public string Name { get; set; }
        public string Description { get; set; }

        private Dictionary<string, int> statModifiers;

        private List<Modifier> weaknesses;
        private List<Modifier> resistances;

        public Race()
        {

        }

        public Race(string name)
        {
            Name = name;
        }
        
    #region Stat Modifier functions

        public int GetStatModifier(string abbr)
        {
            int value = 0;

            if (statModifiers != null)
                statModifiers.TryGetValue(abbr, out value);

            return value;
        }

        public void AddStatModifier(string abbr, int value)
        {
            if (statModifiers == null)
                statModifiers = new Dictionary<string, int>();

            statModifiers.Add(abbr, value);
        }

        public void RemoveStatModifier(string abbr)
        {
            if (statModifiers == null)
                return;

            statModifiers.Remove(abbr);
        }

    #endregion

    #region Weakness/Resistance functions

        public void AddWeakness(Modifier weakness)
        {
            if (weaknesses == null)
                weaknesses = new List<Modifier>();

            weaknesses.Add(weakness);
        }

        public void AddResistance(Modifier resistance)
        {
            if (resistances == null)
                resistances = new List<Modifier>();

            resistances.Add(resistance);
        }

        public void RemoveWeakness(int index)
        {
            if (weaknesses == null)
                return;

            weaknesses.RemoveAt(index);
        }

        public void RemoveResistance(int index)
        {
            if (resistances == null)
                return;

            resistances.RemoveAt(index);
        }

        public int GetResistance(ModifierType type)
        {
            int amount = 0;

            if (resistances != null)
            {
                var list = resistances.FindAll(r => r.Type == type);

                list.ForEach(delegate (Modifier item)
                {
                    amount += item.Amount;
                });
            }

            return amount;
        }

        public int GetWeakness(ModifierType type)
        {
            int amount = 0;

            if (weaknesses != null)
            {
                var list = weaknesses.FindAll(w => w.Type == type);

                list.ForEach(delegate (Modifier item)
                {
                    amount += item.Amount;
                });
            }

            return amount;
        }

        public void ClearStatModifiers()
        {
            statModifiers.Clear();
        }

        public void ClearWeaknessesResistances()
        {
            if (resistances != null)
                resistances.Clear();
            if (weaknesses != null)
                weaknesses.Clear();
        }

    #endregion

    }
}
