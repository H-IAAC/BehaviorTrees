using System.Collections.Generic;
using UnityEngine;

namespace HIAAC.BehaviorTrees
{

    /// <summary>
    /// Base class for composite nodes.
    /// 
    /// Inherit classes should use the "NextChild" method to select next child to visit, 
    /// not direct use the list of children.
    /// </summary>
    public abstract class CompositeNode : Node
    {
        [HideInInspector] public List<Node> children = new(); // List of node children

        [SerializeField] public bool useUtility = false; //If should use utility to select next child
        [SerializeField] public UtilityPropagationMethod utilityPropagationMethod = UtilityPropagationMethod.MAXIMUM; //Method for propagating the utility
        [SerializeField] public UtilitySelectionMethod utilitySelectionMethod = UtilitySelectionMethod.MAXIMUM; //Method for selecting the next child using utility
        [SerializeField] public float utilityThreshould = 0f; //Threshould for RANDOM_THRESHOULD selection method.
        List<Node> nextChildren = new(); //List with the sequence of next children.
        int currentIndex = -1; //Index of the current child.

        /// <summary>
        /// CompositeNode node constructor.
        /// </summary>
        /// <param name="memoryMode">MemoryMode specifing if node can use memory.</param>
        public CompositeNode(MemoryMode memoryMode = MemoryMode.Memoryless) : base(memoryMode)
        {

        }

        /// <summary>
        /// Create a copy of the node.
        /// </summary>
        /// <returns>Copy of the node.</returns>
        public override Node Clone()
        {
            //Clone node itself
            CompositeNode node = Instantiate(this);
            node.guid = guid;

            //Clone children
            node.children = new List<Node>();
            foreach (Node child in children)
            {
                node.children.Add(child.Clone());
            }

            return node;
        }

        /// <summary>
        /// Add child to the node.
        /// </summary>
        /// <param name="child">Child to add to the node.</param>
        public override void AddChild(Node child)
        {
            children.Add(child);
            child.parent = this;
        }

        /// <summary>
        /// Remove child from the node.
        /// </summary>
        /// <param name="child">Child to remove from node</param>
        public override void RemoveChild(Node child)
        {
            children.Remove(child);
            child.parent = null;
        }

        /// <summary>
        /// Get list of all children of the node.
        /// </summary>
        /// <returns>List with node children.</returns>
        public override List<Node> GetChildren()
        {
            return children;
        }

        /// <summary>
        /// Reset next node to be visited
        /// </summary>
        protected void ResetNext()
        {
            currentIndex = -1;
        }

        /// <summary>
        /// Retrieves next child that should be visited
        /// </summary>
        /// <returns>Nexto child to visit.</returns>
        protected Node NextChild()
        {
            currentIndex += 1;
            if (currentIndex >= nextChildren.Count)
            {
                return null;
            }

            return nextChildren[currentIndex];
        }

        /// <summary>
        /// Updates sequence of next children.
        /// </summary>
        void UpdateNextChildren()
        {
            currentIndex = -1;
            if (!useUtility) //No Utility -> use default sequence
            {
                nextChildren = children;
                return;
            }

            nextChildren = SortByUtility.Sort(children, utilitySelectionMethod, utilityThreshould);
        }


        /// <summary>
        /// Update node utility, using utilityPropagationMethod.
        /// </summary>
        /// <returns>Computed node utility.</returns>
        protected override float OnComputeUtility()
        {
            //Compute utility of children
            foreach (Node child in children)
            {
                child.ComputeUtility();
            }

            //Update next children, as utility changed
            UpdateNextChildren();

            //Compute itself utility
            switch (utilityPropagationMethod)
            {
                case UtilityPropagationMethod.MAXIMUM: //Biggest utility of all children
                    {
                        float biggest = 0;
                        foreach (Node child in children)
                        {
                            float u = child.GetUtility();
                            if (u > biggest)
                            {
                                biggest = u;
                            }
                        }

                        return biggest;
                    }
                case UtilityPropagationMethod.MINIMUM: //Smallest utility of all children
                    {
                        float smallest = 1;
                        foreach (Node child in children)
                        {
                            float u = child.GetUtility();
                            if (u < smallest)
                            {
                                smallest = u;
                            }
                        }

                        return smallest;
                    }
                case UtilityPropagationMethod.ALL_SUCESS_PROBABILITY:               
                    {
                        //Probability of all nodes successing, child utility is probability of success
                        //Π child_utility
                        float p = 1;
                        foreach (Node child in children)
                        {
                            p *= child.GetUtility();
                        }

                        return p;
                    }
                case UtilityPropagationMethod.AT_LEAST_ONE_SUCESS_PROBABILITY:
                    {
                        //Probability at least one child successing, child utility is probability of success
                        //1 - Π (1-child_utility) 
                        if (children.Count == 0)
                        {
                            return 0;
                        }

                        float p = 1;
                        foreach (Node child in children)
                        {
                            p *= 1 - child.GetUtility();
                        }

                        return 1 - p;
                    }
                case UtilityPropagationMethod.SUM:
                    {
                        //Sum of children utility

                        float utility = 0f;
                        foreach (Node child in children)
                        {
                            utility += child.GetUtility();
                        }

                        return utility;
                    }
                case UtilityPropagationMethod.AVERAGE:
                    {
                        //Average of children utility

                        float utility = 0f;
                        foreach (Node child in children)
                        {
                            utility += child.GetUtility();
                        }

                        return utility / children.Count;
                    }
                default:
                    return 0;
            }

        }

    }

}