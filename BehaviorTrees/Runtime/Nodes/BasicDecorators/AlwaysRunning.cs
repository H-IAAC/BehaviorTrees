using UnityEngine;

namespace HIAAC.BehaviorTrees
{
    /// <summary>
    /// Always return running.
    /// </summary>
    public class AlwaysRunning : DecoratorNode
    {

        public override void OnStart()
        {
        }

        public override void OnStop()
        {

        }

        /// <summary>
        /// Update child, ignoring state and returning Running.
        /// </summary>
        /// <returns>NodeState.Runnning</returns>
        public override NodeState OnUpdate()
        {
            child.Update();

            return NodeState.Runnning;
        }
    }
}