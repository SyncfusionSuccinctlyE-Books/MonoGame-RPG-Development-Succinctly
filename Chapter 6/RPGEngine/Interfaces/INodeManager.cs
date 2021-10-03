using System;
using System.Collections.Generic;
using System.Text;

namespace RPGEngine.Interfaces
{
    public interface INodeManager
    {
        INode RootNode { get; set; }

        bool AddNodeTo(INode node, INode target = null);
        bool RemoveNodeFrom(INode node, INode target = null);
        bool MoveNode(INode node, INode destinationNode, INode fromNode);
        void ClearNodes(INode node = null);

        INode GetInstanceNode(object instance);
    }
}
