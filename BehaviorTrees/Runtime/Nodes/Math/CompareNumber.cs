using UnityEngine;

namespace HIAAC.BehaviorTrees
{
    public class CompareNumber : ActionNode
    {
        [SerializeField] public CompareOperation operation = CompareOperation.EQUAL;

        public CompareNumber()
        {
            CreateProperty(typeof(FloatBlackboardProperty), "a");
            CreateProperty(typeof(FloatBlackboardProperty), "b");
        }

        public override void OnStart()
        {
        }

        public override void OnStop()
        {
        }

        public override NodeState OnUpdate()
        {
            float a = GetPropertyValue<float>("a");
            float b = GetPropertyValue<float>("b");

            switch (operation)
            {
                case CompareOperation.GREATER:
                    if (a > b)
                    {
                        return NodeState.Success;
                    }
                    break;
                case CompareOperation.LESS:
                    if (a < b)
                    {
                        return NodeState.Success;
                    }
                    break;
                case CompareOperation.EQUAL:
                    if (a == b)
                    {
                        return NodeState.Success;
                    }
                    break;
                case CompareOperation.GREATER_EQUAL:
                    if (a >= b)
                    {
                        return NodeState.Success;
                    }
                    break;
                case CompareOperation.LESS_EQUAL:
                    if (a <= b)
                    {
                        return NodeState.Success;
                    }
                    break;
            }
            return NodeState.Failure;
        }
    }
}