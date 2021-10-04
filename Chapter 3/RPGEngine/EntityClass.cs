using System;
using System.Collections.Generic;
using System.Text;

namespace RPGEngine
{
    public class EntityClass
    {
        public string Name = "";
        public string Description = "";
        public DieType HPDice;

        private Dictionary<string, int> statModifiers;

        public int StatModifiersCount
        {
            get { return (statModifiers != null) ? statModifiers.Count : 0; }
        }

        public EntityClass()
        {

        }

        public EntityClass(string name)
        {
            Name = name;
        }

        public int GetStatModifier(string name)
        {
            int value = 0;

            if (statModifiers != null)
                statModifiers.TryGetValue(name, out value);

            return value;
        }

        public void AddStatModifier(string name, int value)
        {
            if (statModifiers == null)
                statModifiers = new Dictionary<string, int>();

            statModifiers.Add(name, value);
        }

        public void RemoveStatModifier(string name)
        {
            if (statModifiers == null)
                return;

            statModifiers.Remove(name);
        }

        public void ClearStatModifiers()
        {
			if (statModifiers != null)
			    statModifiers.Clear();
        }

        public Dictionary<string, int> GetStatMods()
        {
            return statModifiers;
        }
    }
}
