using System;
using System.Collections.Generic;
using System.Text;

namespace RPGEngine
{
    
    public class Character : Entity
    {
        public int Experience { get; set; }

        private List<string> knownNPCs;

        public void AddKnownNPC(string name)
        {
            if (knownNPCs == null)
                knownNPCs = new List<string>();

            knownNPCs.Add(name);
        }

        public bool CheckKnownNPC(string name)
        {
            if (knownNPCs == null)
                return false;

            return knownNPCs.Contains(name);
        }

        public bool AssignQuest(string id)
        {
            //do nothing for now
            return true;
        }
    }
}
