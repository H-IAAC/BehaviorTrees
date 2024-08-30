using UnityEngine;

namespace HIAAC.BehaviorTrees
{
    public class WaitUntil : ActionNode
    {
        public WaitUntil() : base(MemoryMode.Memoried)
        {
            CreateProperty(typeof(FloatBlackboardProperty), "targetTime");
            CreateProperty(typeof(FloatBlackboardProperty), "scale");
            CreateProperty(typeof(FloatBlackboardProperty), "startTime");

            SetPropertyValue("scale", 1f);
            SetPropertyValue("startTime", 0f);
        }

        public override void OnStart()
        {
            
        }

        public override void OnStop()
        {
        }

        public override NodeState OnUpdate()
        {
            float scale = GetPropertyValue<float>("scale");

            float targetTime = scale*GetPropertyValue<float>("targetTime");
            float startTime = scale*GetPropertyValue<float>("startTime");
            float currentTime = Time.time + startTime;

            if(currentTime > targetTime)
            {
                return NodeState.Success;
            }

            return NodeState.Runnning;
        }
    }
}