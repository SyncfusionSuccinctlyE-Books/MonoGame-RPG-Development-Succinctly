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
       
        public Dictionary<string,int> StatModifiers
        {
            get {return statModifiers;}
            set { statModifiers = value; }
        }

        public List<Modifier> Weaknesses
        {
            get { return weaknesses; }
            set { weaknesses = value; }
        }

        public List<Modifier> Resistances
        {
            get { return resistances; }
            set { resistances = value; }
        }
        
        public int StatModifiersCount
        {
            get { return (statModifiers != null) ? statModifiers.Count : 0; }
        }

        public int WRsCount
        {
            get { return ((resistances != null) ? resistances.Count : 0) + ((weaknesses != null) ? weaknesses.Count : 0); }
        }

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

        public Dictionary<string, int> GetStatMods()
        {
            return statModifiers;
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

        public int CheckResistance(ModifierType type)
        {
            int amount = 0;

            if (resistances != null)
            {
                foreach (Modifier resistance in resistances)
                {
                    if (resistance.Type == type)
                        amount = resistance.Amount;
                }
            }

            return amount;
        }

        public int CheckWeakness(ModifierType type)
        {
            int amount = 0;

            if (weaknesses != null)
            {
                foreach (Modifier weakness in weaknesses)
                {
                    if (weakness.Type == type)
                        amount = weakness.Amount;
                }
            }

            return amount;
        }

        public void ClearStatModifiers()
        {
            statModifiers.Clear();
        }

        public void ClearWRs()
        {
            if (resistances != null)
                resistances.Clear();
            if (weaknesses != null)
                weaknesses.Clear();
        }

#endregion

    }
}
