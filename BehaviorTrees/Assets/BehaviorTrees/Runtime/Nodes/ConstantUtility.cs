using UnityEngine;

namespace HIAAC.BehaviorTree
{
    /// <summary>
    /// Have a constant utility value.
    /// 
    /// Set utility value in property "value".
    /// </summary>
    public class ConstantUtility : DecoratorNode
    {
        /// <summary>
        /// ConstantUtility constructor.
        /// </summary>
        public ConstantUtility()
        {
            CreateProperty(typeof(FloatBlackboardProperty), "value");
        }


        public override void OnStart()
        {
        }

        public override void OnStop()
        {
        }

        public override NodeState OnUpdate()
        {
            return child.Update();
        }


        protected override float OnComputeUtility()
        {
            return GetPropertyValue<float>("value");
        }
    }
}