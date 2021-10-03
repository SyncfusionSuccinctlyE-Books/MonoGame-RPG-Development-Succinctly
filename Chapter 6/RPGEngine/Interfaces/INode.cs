using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGEngine.Interfaces
{
    public interface INode
    {
        int ID { get; set; }

        object Instance { get; set; }

        List<INode> ParentNodes { get; set; }
        List<INode> ChildNodes { get; set; }

        bool AddNode(INode node);
        bool RemoveNode(INode node);
        bool MoveNode(INode node, INode destinationNode, INode fromNode = null);
        void ClearNodes();
        bool IsKnownNode(INode node);
    }
}
