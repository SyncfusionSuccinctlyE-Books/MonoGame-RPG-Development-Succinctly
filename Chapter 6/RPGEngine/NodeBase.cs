using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using RPGEngine.Interfaces;

namespace RPGEngine
{
    public class NodeBase : INode
    {
        // ID must be unique, drawback here is this will limit us to int.MaxValue Nodes in any one game..

        static int LastID = -1;
        public int ID { get; set; }

        public object Instance { get; set; }

        public List<INode> ParentNodes { get; set; }
        public List<INode> ChildNodes { get; set; }

        public NodeBase(object instance)
        {
            ID = ++LastID;

            Instance = instance;

            ParentNodes = new List<INode>();
            ChildNodes = new List<INode>();
        }

        public virtual bool AddNode(INode node)
        {
            if (!IsKnownNode(node))
            {
                node.ParentNodes.Add(this);
                ChildNodes.Add(node);
                return true;
            }

            return false;
        }

        public virtual void ClearNodes()
        {
            int nodeCount = ChildNodes.Count;

            for (int n = nodeCount - 1; n >= 0; n--)
                RemoveNode(ChildNodes[n]);
        }

        public virtual bool MoveNode(INode node, INode destinationNode, INode fromNode = null)
        {
            throw new NotImplementedException();
        }

        public virtual bool RemoveNode(INode node)
        {
            if (IsKnownNode(node))
            {
                node.ParentNodes.Remove(this);
                ChildNodes.Remove(node);
                return false;
            }
            else
                return true;
        }

        public virtual bool IsKnownNode(INode node)
        {
            return node.ParentNodes.Any(n => n.ID == ID) && ChildNodes.Any(n => n.ID == node.ID);
        }

    }
}
