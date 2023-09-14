using UnityEngine;

namespace HIAAC.BehaviorTree
{
    /// <summary>
    /// Prints the binded game object in update.
    /// </summary>
    public class DebugGameObjectNode : ActionNode
    {

        public override void OnStart()
        {

        }

        public override void OnStop()
        {
        }

        public override NodeState OnUpdate()
        {
            Debug.Log($"GameObject: {gameObject}");

            return NodeState.Success;
        }
    }
}