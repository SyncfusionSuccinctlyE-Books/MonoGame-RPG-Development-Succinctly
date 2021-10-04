using System;
using System.Collections;
using System.Collections.Generic;

namespace RPGEngine
{
    public enum ConversationFunctions
    {
        CheckKnownNPC,
        HasQuest,
		AssignQuest,
        QuestAssigned,
        CheckForQuestCompletion,
        CompleteQuest
    }

    public enum FunctionType
    {
        FunctionNone,
        PreFunction,
        PostFunction
    }

    public enum CaseType
    {
        CaseNone,
        CaseTrue,
        CaseFalse
    }

    public enum ConversationStatus
    {
        NotStarted,
        Started,
        Completed
    }

    public class ConversationNode
    {
        public int ID;
        public string Text;
        public FunctionType NodeFunctionType;
        public string FunctionName;
        public string[] FunctionParams;
        public CaseType NodeCaseType;

        private List<ConversationNode> responses;

        public ConversationNode()
        {

        }

        public List<ConversationNode> Responses
        {
            get { return responses; }
            set { responses = value; }
        }

        public void AddResponse(ConversationNode node)
        {
            responses.Add(node);
        }

        public void RemoveResponse(int index)
        {
            responses.RemoveAt(index);
        }

        public void UpdateResponse(int index, ConversationNode node)
        {
            responses[index] = node;
        }

    }

    public class Conversation
    {
        private int id;
        public List<ConversationNode> nodes;
        private ConversationNode curNode;
        public ConversationStatus Status;

        public Conversation()
        {
        }

        public int ID
        {
            get { return id; }
            set { id = value; }
        }

		public ConversationNode GetCurNode()
		{
		    return curNode; 
		}

        public void Initialize()
        {
            //need to check if there is a pre-function on the top set of nodes to determine which to use
            //assume the first node is the start of the conversation
            curNode = nodes[0];
            CheckPreFunction();
        }

        public void CheckPreFunction()
        {
            if (curNode.NodeFunctionType == FunctionType.PreFunction)
            {
                int index = 0;

                object obj = Globals.FunctionClasses[(ConversationFunctions)Enum.Parse(typeof(ConversationFunctions), curNode.FunctionName)];

                if (obj.GetType().IsGenericType && obj is IList)
                {
                    index = Convert.ToInt32(curNode.FunctionParams[0]);

                    //first function param would be the id of the object in the list
                    object ret = Globals.FunctionClasses[(ConversationFunctions)Enum.Parse(typeof(ConversationFunctions), curNode.FunctionName)].GetType().GetMethod(curNode.FunctionName)
                        .Invoke(((IList)obj)[index], new[] { curNode.FunctionParams });

                    if ((bool)ret)
                    {
                        curNode.Responses = curNode.Responses.Find(c => c.Text == "true").Responses;
                    }
                    else
                    {
                        curNode.Responses = curNode.Responses.Find(c => c.Text == "false").Responses;
                    }
                }
                else
                {
                    object ret = Globals.FunctionClasses[(ConversationFunctions)Enum.Parse(typeof(ConversationFunctions), curNode.FunctionName)].GetType().GetMethod(curNode.FunctionName)
                        .Invoke(obj, new[] { curNode.FunctionParams });

                    if ((bool)ret)
                    {
                        curNode.Responses = curNode.Responses.Find(c => c.NodeCaseType == CaseType.CaseTrue).Responses;
                    }
                    else
                    {
                        curNode.Responses = curNode.Responses.Find(c => c.NodeCaseType == CaseType.CaseFalse).Responses;
                    }
                }
            }
        }

        public void SelectResponse(int index)
        {
            //check post function
            if(curNode.Responses[index].NodeFunctionType == FunctionType.PostFunction && curNode.Responses != null)
            {
                int param = 0;

                object obj = Globals.FunctionClasses[(ConversationFunctions)Enum.Parse(typeof(ConversationFunctions), curNode.Responses[index].FunctionName)];
                object ret;

                if (obj.GetType().IsGenericType && obj is IList)
                {
                    //first function param would be the id of the object in the list
                    param = Convert.ToInt32(curNode.Responses[index].FunctionParams[0]);

                    ret = Globals.FunctionClasses[(ConversationFunctions)Enum.Parse(typeof(ConversationFunctions), curNode.Responses[index].FunctionName)].GetType().GetMethod(curNode.Responses[index].FunctionName)
                        .Invoke(((IList)obj)[param], new[] { curNode.Responses[index].FunctionParams });
                }
                else
                {
                    string[] functionParams = curNode.Responses[index].FunctionParams;

                    ret = Globals.FunctionClasses[(ConversationFunctions)Enum.Parse(typeof(ConversationFunctions), curNode.Responses[index].FunctionName)].GetType().GetMethod(curNode.Responses[index].FunctionName)
                        .Invoke(obj, functionParams);
                }

                if (curNode.Responses != null)
                {
                    if ((bool)ret)
                    {
                        curNode = curNode.Responses[index].Responses.Find(c => c.NodeCaseType == CaseType.CaseTrue);
                    }
                    else
                    {
                        curNode = curNode.Responses[index].Responses.Find(c => c.NodeCaseType == CaseType.CaseFalse);
                    }
                }
                else
                {
                    curNode = null;
                }
            }
            else
            {
                if (curNode.Responses[index].Responses == null)
                {
                    //end conversation
                    curNode = null;
                }
                else
                {
                    curNode = curNode.Responses[index];
                }
            }

            if(curNode == null)
            {
                Status = ConversationStatus.Completed;
            }
        }

        public List<ConversationNode> GetResponses(int index)
        {
            ConversationNode node = null;

            foreach (ConversationNode child in nodes)
            {
                if (child.ID == id)
                {
                    node = child;
                    break;
                }
                if (child.Responses.Count > 0)
                {
                    node = FindNodeByID(child, id);
                    if (node != null)
                        break;
                }
            }

            return node.Responses; 
        }

        ConversationNode FindNodeByID(ConversationNode node, int id)
        {
            ConversationNode parent = null;

            foreach (ConversationNode child in node.Responses)
            {
                if (child.ID == id)
                {
                    parent = child;
                    break;
                }

                if (child.Responses != null && child.Responses.Count > 0)
                {
                    parent = FindNodeByID(child, id);
                    if (parent != null)
                        break;
                }
            }

            return parent;
        }
    }
}
