using System;
using System.Collections.Generic;
using System.Text;

namespace RPGEngine
{
    
    public class Character : Entity
    {
        public int Experience { get; set; }

        private List<string> knownNPCs;

        public List<AssignedQuest> AssignedQuests;

        public void AddKnownNPC(string name)
        {
            if (knownNPCs == null)
                knownNPCs = new List<string>();

            knownNPCs.Add(name);
        }

        // param needs to be list as the code calling the method doesn't know anything about the method 
        // and the conversation file holds a list of parameters for various methods
        public bool CheckKnownNPC(string[] id)
        {
            if (knownNPCs == null)
                knownNPCs = new List<string>();

            if (!knownNPCs.Contains(id[0]))
            { 
                knownNPCs.Add(id[0]);
                return false;
            }
            else
            {
                return true;
            }
        }

        public void AddQuestItem(string objectName)
        {
            //loop through quests to find object and increment
            foreach(AssignedQuest aq in AssignedQuests)
            {
                for(int i = 0; i < aq.NumItemsDone.Count; i++)
                {
                    Dictionary<string, int>.Enumerator enumerator = aq.NumItemsDone.GetEnumerator();

                    while (enumerator.MoveNext())
                    {
                        KeyValuePair<string, int> kvp = enumerator.Current;

                        if (kvp.Key == objectName)
                        {
                            kvp = new KeyValuePair<string, int>(kvp.Key, kvp.Value + 1);
                        }
                    }
                }
            }
        }

        public bool IsQuestCompleted(string id)
        {
            AssignedQuest aq = AssignedQuests.Find(q => q.QuestID == Convert.ToInt32(id));
            Quest quest = QuestManager.LoadQuest(Convert.ToInt32(id));

            if (quest != null && aq != null)
            {
                if (aq.CurStep == (quest.Steps.Count - 1))
                {

                    //if last step is talk to entity, quest is finished
                    if (quest.Steps[aq.CurStep].StepInteractionType == InteractionType.Talk &&
                        quest.Steps[0].StepEntity == quest.Steps[aq.CurStep].StepEntity)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool AssignQuest(string id)
        {
            if (AssignedQuests == null)
                AssignedQuests = new List<AssignedQuest>();

            AssignedQuest aq = new AssignedQuest();
            Quest q = QuestManager.LoadQuest(Convert.ToInt32(id));

            aq.QuestID = Convert.ToInt32(id);
            aq.TimeQuestStarted = aq.TimeStepStarted = DateTime.Now.ToBinary();
            aq.CurStep = 1;

            foreach(QuestStep step in q.Steps)
            {
                if (aq.NumItemsDone == null)
                    aq.NumItemsDone = new Dictionary<string, int>();

                if (step.Quantity > 0)
                {
                    aq.NumItemsDone.Add(step.QuantityName, 0);
                }
            }
            
            AssignedQuests.Add(aq);
            
            EventSystem.OnQuestAssigned(new EventSystemEventArgs() { ObjectID = aq.QuestID });

            return true;
        }

        public bool QuestAssigned(string[] id)
        {
            if (AssignedQuests != null)
            {
                //method will only be called with one item in the array
                if (AssignedQuests.Find(q => q.QuestID == Convert.ToInt32(id[0])) != null)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        public List<Quest> GetQuests()
        {
            List<Quest> quests = new List<Quest>();

            for(int i = 0; i < AssignedQuests.Count; i++)
            {
                Quest quest = QuestManager.LoadQuest(AssignedQuests[i].QuestID);
                quests.Add(quest);
            }

            return quests;
        }

        public void CompleteQuest(string id)
        {
            if(AssignedQuests != null)
            {
                AssignedQuest aq = AssignedQuests.Find(q => q.QuestID == Convert.ToInt32(id));

                if (aq != null)
                {
                    aq.QuestFinished = true;

                    object reward;

                    QuestRewardType type = QuestManager.GetQuestReward(Convert.ToInt32(id), out reward);

                    switch(type)
                    {
                        case QuestRewardType.Money:

                            Money += (int)reward;
                            break;

                        case QuestRewardType.Item:

                            //items handled later
                            
                            break;
                    }
                }
            }
        }
    }
}
