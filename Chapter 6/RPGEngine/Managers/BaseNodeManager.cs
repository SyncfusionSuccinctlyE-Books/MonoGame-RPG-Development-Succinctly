using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using RPGEngine.Interfaces;

namespace RPGEngine.Managers
{
    public class BaseNodeManager<T> : INodeManager
    {
        public INode RootNode { get; set; }

        public bool AddNodeTo(INode node, INode target = null)
        {
            if (target == null)
                target = RootNode;
            
            return target.AddNode(node);            
        }

        public virtual void ClearNodes(INode node = null)
        {
            if (node == null)
                node = RootNode;

            node.ClearNodes();
        }

        public virtual bool RemoveNodeFrom(INode node, INode target = null)
        {
            if (target == null)
                target = RootNode;

            return target.RemoveNode(node);
        }

        public virtual bool MoveNode(INode node, INode destinationNode, INode fromNode)
        {
            bool retVal = true;

            if (destinationNode == fromNode)
                retVal = false;

            retVal = destinationNode.AddNode(node);
            if (retVal)
                retVal = fromNode.RemoveNode(node);
            
            return retVal;
        }

        public virtual INode GetInstanceNode(object instance)
        {
            INode node = RootNode.ChildNodes.FirstOrDefault(n => n.Instance == instance);

            if (node == null)
                node = SearchForInstanceNode(RootNode, instance);

            return node;

        }

        protected virtual INode SearchForInstanceNode(INode node, object instance)
        {
            INode instanceNode = node.ChildNodes.FirstOrDefault(n => n.Instance == instance);

            if (instanceNode == null)
            {
                foreach (INode childNode in node.ChildNodes)
                {
                    instanceNode = SearchForInstanceNode(childNode, instance);

                    if (instanceNode != null)
                        break;
                }

            }

            return node;
        }
    }
}
