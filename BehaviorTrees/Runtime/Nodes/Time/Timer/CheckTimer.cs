using UnityEngine;

namespace HIAAC.BehaviorTrees
{
    public class CheckTimer : ActionNode
    {
        public CheckTimer()
        {
            CreateProperty(typeof(FloatBlackboardProperty), "timerTime");
            CreateProperty(typeof(FloatBlackboardProperty), "maxTime");
        }

        public override void OnStart()
        {
        }

        public override void OnStop()
        {
        }

        public override NodeState OnUpdate()
        {
            float timerTime = GetPropertyValue<float>("timerTime");
            float maxTime = GetPropertyValue<float>("maxTime");

            if (timerTime <= 0)
            {
                return NodeState.Failure;
            }

            if (Time.time - timerTime >= maxTime)
            {
                return NodeState.Success;
            }

            return NodeState.Failure;
        }
    }
}