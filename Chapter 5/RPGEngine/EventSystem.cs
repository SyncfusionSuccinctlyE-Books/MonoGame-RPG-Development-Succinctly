using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGEngine
{
    //Event system could be used for more than quests so events aren't in quest manager
    //For example, achievements based on obtaining a certain amount of loot
    //This allow multiple system to use the same events

    public class EventSystemEventArgs
    {
        public int ObjectID;    //specific to event, Entity events = entity ID, Item event = item ID, etc.
        public string Tag;      //could be anything
    }

    public class EventSystem
    {
        public delegate void EventSystemEventHandler(EventSystemEventArgs e);

        public static event EventSystemEventHandler EntityKilled;
        public static event EventSystemEventHandler LootObtained;
        public static event EventSystemEventHandler LocationReached;
        public static event EventSystemEventHandler EntityTalkedTo;
        public static event EventSystemEventHandler ItemObtained;
        public static event EventSystemEventHandler LevelEntered;
        public static event EventSystemEventHandler QuestAssigned;

        public static void OnEntityKilled(EventSystemEventArgs e)
        {
            EntityKilled?.Invoke(e);
        }

        public static void OnLootObtained(EventSystemEventArgs e)
        {
            LootObtained?.Invoke(e);
        }

        public static void OnLocationReached(EventSystemEventArgs e)
        {
            LocationReached?.Invoke(e);
        }

        public static void OnEntityTalkedTo(EventSystemEventArgs e)
        {
            EntityTalkedTo?.Invoke(e);
        }

        public static void OnItemObtained(EventSystemEventArgs e)
        {
            ItemObtained?.Invoke(e);
        }

        public static void OnLevelEntered(EventSystemEventArgs e)
        {
            LevelEntered?.Invoke(e);
        }

        public static void OnQuestAssigned(EventSystemEventArgs e)
        {
            QuestAssigned?.Invoke(e);
        }
    }
}
