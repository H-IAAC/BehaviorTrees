using UnityEngine;

namespace HIAAC.BehaviorTrees.Needs
{

    public class DecreaseNeedNode : ActionNode
    {

        public DecreaseNeedNode()
        {
            CreateProperty(typeof(NeedBlackboardProperty), "need");
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
            Need need = GetPropertyValue<Need>("need");
            float value = GetPropertyValue<float>("value");
            float needValue;

            try
            {
                needValue = tree.needsContainer.getNeedValue(need);
            }
            catch (System.ArgumentException)
            {
                return NodeState.Failure;
            }

            needValue -= value;

            tree.needsContainer.setNeedValue(need, needValue);
            
            return NodeState.Success;
        }
    }
}