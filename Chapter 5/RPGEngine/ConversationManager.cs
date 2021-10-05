using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using MonoGameRPG.StateManagement;

namespace RPGEngine
{
    public class ConversationManager
    {
        List<Conversation> conversations;
        Entity player;

        ConversationNode curNode;

        public bool IsActive;
        private int curConversationIndex;

        public ConversationManager()
        {
        }

        public void LoadConversation(int id, Entity player, Entity npc)
        {
            conversations = new List<Conversation>();
            conversations.Add(ConversationManager.GetConversation(id.ToString()));
            this.player = player;

            if (!Globals.FunctionClasses.ContainsKey(ConversationFunctions.HasQuest))
                Globals.FunctionClasses.Add(ConversationFunctions.HasQuest, npc);
            else
                Globals.FunctionClasses[ConversationFunctions.HasQuest] = npc;
        }

        public static Conversation GetConversation(string id)
        {
            Conversation conversation = new Conversation();

            string data = File.ReadAllText(@"Content\Data\Conversations\" + id.ToString() + ".json");

            conversation = JsonConvert.DeserializeObject<Conversation>(data);

            return conversation;
        }

        public string StartConversation(int id)
        {
            string text = "";

            IsActive = true;

            curConversationIndex = conversations.FindIndex(c => c.ID == id);
            conversations[curConversationIndex].Initialize();

            curNode = conversations[curConversationIndex].GetCurNode();

            return text;
        }

        public int GetResponseCount()
        {
            List<ConversationNode> nodes = conversations[curConversationIndex].GetResponses(curNode.ID);

            return nodes == null ? 0 : nodes.Count;
        }

        public void HandleInput(InputState input, PlayerIndex? controllingPlayer, PlayerIndex player)
        {
            int count = GetResponseCount();

            for (int i = 0; i < count; i++)
            {
                if (input.IsNewKeyPress((Keys)((int)(Keys.D1) + i), controllingPlayer, out player))
                {
                    SelectResponse(i);
                }
            }
        }

        public void SelectResponse(int index)
        {
            conversations[curConversationIndex].SelectResponse(index);
            curNode = conversations[curConversationIndex].GetCurNode();
            if(curNode == null)
            {
                IsActive = false;
            }
        }

        public ConversationNode GetCurrentNode()
        {
            if (conversations[curConversationIndex].Status == ConversationStatus.Completed)
            {
                IsActive = false;
                return null;
            }
            else
            {
                return curNode;
            }
        }
    }
}
