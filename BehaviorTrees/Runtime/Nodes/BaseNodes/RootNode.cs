using System.Collections.Generic;
using UnityEngine;

namespace HIAAC.BehaviorTrees
{
    /// <summary>
    /// Root node of the tree.
    /// 
    /// The tree have one and only one root node.
    /// </summary>
    public class RootNode : DecoratorNode
    {

        public override void OnStart()
        {

        }

        public override void OnStop()
        {

        }

        /// <summary>
        /// Root update.
        /// 
        /// Call if child update.
        /// </summary>
        /// <returns>New state (same as child)</returns>
        public override NodeState OnUpdate()
        {
            if(child == null)
            {
                return NodeState.Failure;
            }
            
            return child.Update();
        }
    }
}