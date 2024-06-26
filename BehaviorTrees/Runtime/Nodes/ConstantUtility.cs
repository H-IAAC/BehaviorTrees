using UnityEngine;

namespace HIAAC.BehaviorTrees
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
            if(child)
            {
                return child.Update();
            }
            return NodeState.Failure;
            
        }


        protected override float OnComputeUtility()
        {
            return GetPropertyValue<float>("value");
        }
    }
}