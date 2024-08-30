using UnityEngine;

namespace HIAAC.BehaviorTrees
{
    public class CheckExistNode : ActionNode
    {
        [SerializeField] NodeState trueState = NodeState.Success;
        [SerializeField] NodeState falseState = NodeState.Failure;

        public CheckExistNode()
        {
            CreateProperty(typeof(GameObjectBlackboardProperty), "object");
        }

        public override void OnStart()
        {
        }

        public override void OnStop()
        {
        }

        public override NodeState OnUpdate()
        {
            GameObject value = GetPropertyValue<GameObject>("object");

            if(value)
            {
                return trueState;
            }

            return falseState;
        }
    }
}