using System.Collections.Generic;
using UnityEngine;

namespace HIAAC.BehaviorTree
{
    /// <summary>
    /// Base class for action nodes.
    /// 
    /// A Action Node don't have child (is leaf).
    /// </summary>
    public abstract class ActionNode : Node
    {
        /// <summary>
        /// Action node constructor.
        /// </summary>
        /// <param name="memoryMode">MemoryMode specifing if node can use memory.</param>
        public ActionNode(MemoryMode memoryMode = MemoryMode.Memoryless) : base(memoryMode)
        {

        }

        /// <summary>
        /// Add child to the node.
        /// 
        /// As the node don't have child, do nothing.
        /// </summary>
        /// <param name="child">Not used.</param>
        public override void AddChild(Node child)
        {

        }

        /// <summary>
        /// Remove child from the node.
        /// 
        /// As the node don't have child, do nothing.
        /// </summary>
        /// <param name="child">Not used.</param>
        public override void RemoveChild(Node child)
        {

        }

        /// <summary>
        /// Get list of children.
        /// </summary>
        /// <returns>Empty list.</returns>
        public override List<Node> GetChildren()
        {
            return new List<Node>();
        }
    }
}
