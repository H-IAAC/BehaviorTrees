using UnityEngine;

namespace HIAAC.BehaviorTrees
{

    public class RepeatUntilFailureNode : DecoratorNode
    {

        public override void OnStart()
        {
        }

        public override void OnStop()
        {
        }

        public override NodeState OnUpdate()
        {
            NodeState state = NodeState.Success;

            while(state != NodeState.Failure)
            {
                state = child.Update();
            }

            return state;
        }
    }
}