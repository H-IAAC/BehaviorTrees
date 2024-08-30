using UnityEngine;

namespace HIAAC.BehaviorTrees
{
    public class StartTimer : ActionNode
    {
        public StartTimer()
        {
            CreateProperty(typeof(FloatBlackboardProperty), "timerTime");
            CreateProperty(typeof(BoolBlackboardProperty), "resetIfAlreadyStarted");
        }

        public override void OnStart()
        {
        }

        public override void OnStop()
        {
        }

        public override NodeState OnUpdate()
        {
            float time = GetPropertyValue<float>("timerTime");

            if (time > 0)
            {
                bool reset = GetPropertyValue<bool>("resetIfAlreadyStarted");
                if (reset)
                {
                    SetPropertyValue("timerTime", Time.time);
                }
            }
            else
            {
                SetPropertyValue("timerTime", Time.time);
            }

            return NodeState.Success;
        }
    }
}