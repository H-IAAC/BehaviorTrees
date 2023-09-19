using System.Collections.Generic;
using UnityEngine;

namespace HIAAC.BehaviorTrees
{
    /// <summary>
    /// Base class for decorator nodes.
    /// 
    /// A decorator node is a node that have only one child.
    /// </summary>
    public abstract class DecoratorNode : Node
    {
        [HideInInspector] public Node child; //Child of the node

        /// <summary>
        /// Decorator node constructor.
        /// </summary>
        /// <param name="memoryMode">MemoryMode specifing if node can use memory.</param>
        public DecoratorNode(MemoryMode memoryMode = MemoryMode.Memoryless) : base(memoryMode)
        {

        }

        /// <summary>
        /// Create a copy of the node.
        /// </summary>
        /// <returns>Copy of the node.</returns>
        public override Node Clone()
        {
            DecoratorNode node = Instantiate(this);
            node.child = child.Clone();
            node.guid = guid;
            return node;
        }

        /// <summary>
        /// Change the node child.
        /// 
        /// The other child is removed.
        /// </summary>
        /// <param name="child">Child for the node.</param>
        public override void AddChild(Node child)
        {
            this.child = child;
            child.parent = this;
        }

        /// <summary>
        /// Remove the current child of the node.
        /// </summary>
        /// <param name="child">Not used.</param>
        public override void RemoveChild(Node child)
        {
            this.child.parent = null;
            this.child = null;
        }

        /// <summary>
        /// Get list of all children of the node.
        /// 
        /// List have only the single node child.
        /// </summary>
        /// <returns>List with the node child.</returns>
        public override List<Node> GetChildren()
        {
            List<Node> children = new();

            if (child != null)
            {
                children.Add(child);
            }

            return children;
        }

        /// <summary>
        /// Compute the node utility.
        /// 
        /// The node utility is defined as the child utility.
        /// </summary>
        /// <returns>Node utility.</returns>
        protected override float OnComputeUtility()
        {
            child.ComputeUtility();
            return child.GetUtility();
        }
    }
}