using UnityEngine;

namespace HIAAC.BehaviorTrees
{
    public class CheckBoolNode : ActionNode
    {
        [SerializeField] NodeState trueState = NodeState.Success;
        [SerializeField] NodeState falseState = NodeState.Failure;

        public CheckBoolNode()
        {
            CreateProperty(typeof(BoolBlackboardProperty), "bool");
        }

        public override void OnStart()
        {
        }

        public override void OnStop()
        {
        }

        public override NodeState OnUpdate()
        {
            bool value = GetPropertyValue<bool>("bool");

            if(value)
            {
                return trueState;
            }

            return falseState;
        }
    }
}