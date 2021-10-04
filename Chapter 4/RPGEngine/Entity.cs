using System;
using System.Collections.Generic;
using Newtonsoft.Json;


namespace RPGEngine
{
    public delegate void EntityLevelUpDelegate(Entity entity);

    
    public class Entity
    {
        public int ID { get; set; }

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

        //requires a strength stat
        private float maxWeight;

        private List<Conversation> conversations;

        private List<int> questIDs;

        public int MaxWeight()
        {
            //find the strength stat
            return stats.Find(s => s.StatName == "strength").CurrentValue & Stat.PoundsPerStatPoint;
        }

        public void AddStat(EntityStat stat)
        {
            if(stats == null)
            {
                stats = new List<EntityStat>();
            }

            stats.Add(stat);
        }

        public void AddConversation(Conversation conversation)
        {
            if (conversations == null)
                conversations = new List<Conversation>();

            conversations.Add(conversation);
        }

        public bool HasConversationToStart(out int id)
        {
            if (conversations != null)
            {
                foreach (Conversation conversation in conversations)
                {
                    if (conversation.Status == ConversationStatus.NotStarted)
                    {
                        id = conversation.ID;
                        return true;
                    }
                }
            }

            id = 0;
            return false;
        }

        public Conversation GetConversation(int id)
        {
            return conversations.Find(c => c.ID == id);
        }

        public void AddQuest(int id)
        {
            if (questIDs == null)
                questIDs = new List<int>();

            questIDs.Add(id);
        }


        public bool HasQuest()
        {
            //query npc to see if he has a quest instead of using functions
            //eventually this could check data from the character and match to a quest for that data - character's class, race, etc.
            return questIDs != null;
        }
    }
}
