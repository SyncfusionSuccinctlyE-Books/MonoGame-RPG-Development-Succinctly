using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Newtonsoft.Json;
using System.IO;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using MonoGameRPG.StateManagement;

namespace RPGEngine
{
    public class QuestEventArgs
    {
        public string Text;

        public QuestEventArgs(string text)
        {
            Text = text;
        }
    }

    public class QuestManager
    {
        private Character character;

        public delegate void QuestEventHandler(QuestEventArgs e);

        public event QuestEventHandler QuestUpdated;

        public QuestManager(Character character)
        {
            EventSystem.EntityKilled += EventSystem_EntityKilled;
            EventSystem.EntityTalkedTo += EventSystem_EntityTalkedTo;
            EventSystem.ItemObtained += EventSystem_ItemObtained;
            EventSystem.LocationReached += EventSystem_LocationReached;
            EventSystem.LootObtained += EventSystem_LootObtained;
            EventSystem.LevelEntered += EventSystem_LevelEntered;

            this.character = character;
        }

        private void EventSystem_LevelEntered(EventSystemEventArgs e)
        {
        }

        private void EventSystem_LootObtained(EventSystemEventArgs e)
        {
            
        }

        private void EventSystem_LocationReached(EventSystemEventArgs e)
        {
            

        }

        private void EventSystem_ItemObtained(EventSystemEventArgs e)
        {
            //Quest type - Item
            //GetItemID = e.ObjectID
        }

        private void EventSystem_EntityTalkedTo(EventSystemEventArgs e)
        {
            //Quest type - Interact
            //InteractionType = Talk
            //StepEntity = e.ObjectID

            //Quest type - Fedex
            //GiveItemID = e.ObjectID

            foreach(Quest q in character.GetQuests())
            {
                AssignedQuest aq = character.AssignedQuests.Find(a => a.QuestID == q.ID);
                if (aq != null)
                {
                    QuestStep step = q.Steps[aq.CurStep];
                    if (step.Type == QuestStepType.Interact
                    && step.StepInteractionType == InteractionType.Kill
                    && step.StepEntity == e.ObjectID)
                    {
                        aq.CurStep++;
                        QuestUpdated(new QuestEventArgs(q.Name));
                    }
                }
            }
        }

        private void EventSystem_EntityKilled(EventSystemEventArgs e)
        {
            //Quest type - Interact
            //InteractionType = Kill
            //StepEntity = e.ObjectID

            foreach (Quest q in character.GetQuests())
            {
                AssignedQuest aq = character.AssignedQuests.Find(a => a.QuestID == q.ID);
                if (aq != null)
                {
                    aq.NumItemsDone[e.Tag]++;

                    QuestStep step = q.Steps[aq.CurStep];

                    if (step.Type == QuestStepType.Interact
                        && step.StepInteractionType == InteractionType.Kill
                        && step.StepEntity == e.ObjectID
                        && step.Quantity == aq.NumItemsDone[e.Tag])
                    {
                        aq.CurStep++;
                        QuestUpdated(new QuestEventArgs(q.Name));

                    }
                }
            }
        }

        public static string GetQuestName(int id)
        {
            return LoadQuest(id).Name;
        }

        public static Quest LoadQuest(int id)
        {
            Quest quest = new Quest();

            string data = File.ReadAllText(@"Content\Data\Quests\" + id.ToString() + ".json");

            quest = JsonConvert.DeserializeObject<Quest>(data);

            return quest;
        }

        public static QuestRewardType GetQuestReward(int id, out object reward)
        {
            Quest quest = QuestManager.LoadQuest(id);

            switch(quest.RewardType)
            {
                case QuestRewardType.Money:

                    reward = quest.RewardItemID;
                    return QuestRewardType.Money;

                case QuestRewardType.Item:

                    //items handled later
                    reward = quest.RewardItemID;
                    return QuestRewardType.Item;
            }

            reward = null;
            return QuestRewardType.Item;
        }
    }
}
