using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using RPGEngine.Interfaces;

namespace RPGEngine.Managers
{
    /// <summary>
    /// OK, sot his node manager is dedicated to managing an inventory, this could be the players inventory, it could be the contents
    /// of a treasure chest, it could be the inventory of a store..
    /// </summary>
    public class InventoryManager : BaseNodeManager<Item>
    {
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="root">This is the root node, so a chest, a store, a player's back pack</param>
        public InventoryManager(INode root) : base()
        {
            RootNode = root;
        }

        /// <summary>
        /// Add an item to the inventory
        /// </summary>
        /// <param name="item"></param>
        public void AddItem(Item item)
        {
            NodeBase newNode = new NodeBase(item);

            // now the item is in a node object, we can add it to the root node.
            AddNodeTo(newNode);
        }

        /// <summary>
        /// Get the contents of an item in the inventory.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public List<Item> GetContainedItems(Item item)
        {
            INode containerNode = GetInstanceNode(item);

            if (containerNode != null)
                return containerNode.ChildNodes.Select(i => i.Instance).OfType<Item>().ToList();
            else
                return null;
        }
    }
}
