using UnityEngine;

namespace HIAAC.BehaviorTree
{
    /// <summary>
    /// Modifies the child utility using a function.
    /// 
    /// The function can be defined in the "modifierFunction" property.
    /// </summary>
    public class UtilityModifier : DecoratorNode
    {
        public UtilityModifier()
        {
            CreateProperty(typeof(CurveBlackboardProperty), "modifierFunction");
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
            //Get child utility
            child.ComputeUtility();
            float childUtility = child.GetUtility();

            //Modify utility using the curve
            AnimationCurve curve = GetPropertyValue<AnimationCurve>("modifierFunction");
            return curve.Evaluate(childUtility);
        }
    }
}