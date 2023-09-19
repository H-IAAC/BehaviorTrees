using UnityEngine;

namespace HIAAC.BehaviorTrees
{
    /// <summary>
    /// Wait some time before Success
    /// 
    /// The time can be defined in the "duration" property.
    /// </summary>
    public class WaitNode : ActionNode
    {
        float startTime; //Time the node started waiting

        public WaitNode() : base(MemoryMode.Memoried)
        {
            CreateProperty(typeof(FloatBlackboardProperty), "duration");
        }

        public override void OnStart()
        {
            startTime = Time.time;
        }

        public override void OnStop()
        {

        }

        public override NodeState OnUpdate()
        {
            float duration = GetPropertyValue<float>("duration");
            if (Time.time - startTime >= duration)
            {
                return NodeState.Success;
            }

            return NodeState.Runnning;
        }
    }
}