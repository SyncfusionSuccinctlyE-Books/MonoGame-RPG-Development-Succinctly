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
    public class ConversationManager
    {
        Conversation conversation;
        Entity player;
        Entity npc;

        ConversationNode curNode;

        public bool IsActive;

        public ConversationManager(Conversation conversation, Entity player, Entity npc)
        {
            this.conversation = conversation;
            this.player = player;
            this.npc = npc;

            Globals.FunctionClasses.Add(ConversationFunctions.HasQuest, npc);
        }

        public static Conversation LoadConversation(string id)
        {
            Conversation conversation = new Conversation();

            string data = File.ReadAllText(@"Content\Data\Conversations\" + id.ToString() + ".json");

            conversation = JsonConvert.DeserializeObject<Conversation>(data);

            return conversation;
        }

        public string Start()
        {
            string text = "";

            IsActive = true;

            conversation.Initialize();

            curNode = conversation.GetCurNode();

            return text;
        }

        public int GetResponseCount()
        {
            List<ConversationNode> nodes = conversation.GetResponses(curNode.ID);

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
            conversation.SelectResponse(index);
            curNode = conversation.GetCurNode();
            if(curNode == null)
            {
                IsActive = false;
            }
        }

        public ConversationNode GetCurrentNode()
        {
            if (conversation.Status == ConversationStatus.Completed)
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
