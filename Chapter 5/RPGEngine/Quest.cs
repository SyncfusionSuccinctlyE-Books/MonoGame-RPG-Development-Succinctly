using Newtonsoft.Json;
using System.Collections.Generic;

namespace RPGEngine
{
    //Other quest types could be added to fit event system
    public enum QuestStepType 
    {
        FedEx,
        Item,
        Interact
    }

    public enum QuestRewardType 
    {
        Item,
        Money
    }

    public enum InteractionType 
    {
        Talk,
        Rescue,
        Kill,
        Get,
        Give
    }

    public class QuestStep 
    {
        public string StepName;
        public int StepEntity;
        public InteractionType StepInteractionType;
        public QuestStepType Type;
        public string QuantityName;
        public int Quantity;

        // array of IDs of subquests
        public List<int> SubQuests;

        // required level to start the step, each step may have a different level
        public byte MinimumLevel;

        // number of minutes the step can take
        public long TimeLimit;

        public string JournalEntry;
        
        public bool Started;

        public QuestStep() 
        {
            
        }
    }

    // An array of this goes in the Entity class
    public class AssignedQuest
    {
		public int QuestID;
		public int CurStep;
		public long TimeStepStarted;
		public long TimeStepFinished;
		public long TimeQuestStarted;
		public long TimeQuestFinished;
		public int QuestGiverID;
        //number of items done for each step, if necessary. Key is object name
        public Dictionary<string, int> NumItemsDone;
		public bool QuestFinished;
	 }

    public class Quest 
    {
        public int ID;
        public string Name;
        public string Description;
        public QuestRewardType RewardType;
        public int RewardItemID;
        public List<int> AllowedClasses;
        public int RequiredLevel;
        public bool IsMultipleAllowed;

        // 0 means no limit
        public long TimeBetweenQuests;

        // total time in which quest must be completed, 0 for none
        public long TimeLimit;

        // is the reward known by the player when accepting the quest
        public bool IsRewardShown;

        // array of QuestStep
        public List<QuestStep> Steps = new List<QuestStep>();
        
        public Quest() 
        {

        }

        public Quest(int id)
        {
            ID = id;
        }
    }
}
