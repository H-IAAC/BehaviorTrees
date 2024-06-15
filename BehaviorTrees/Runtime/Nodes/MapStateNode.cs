using UnityEngine;

namespace HIAAC.BehaviorTrees
{
    public class MapStateNode : DecoratorNode
    {
        [SerializeField] NodeState runningStateTarget = NodeState.Runnning;
        [SerializeField] NodeState failureStateTarget = NodeState.Failure;
        [SerializeField] NodeState successStateTarget = NodeState.Success;

        public override void OnStart()
        {
        }

        public override void OnStop()
        {
        }

        public override NodeState OnUpdate()
        {
            NodeState childState = child.Update();

            switch (childState)
            {
                case NodeState.Runnning:
                    return runningStateTarget;
                case NodeState.Failure:
                    return failureStateTarget;
                case NodeState.Success:
                    return successStateTarget;
            }

            return NodeState.Failure;
        }
    }
}