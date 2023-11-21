using System.Linq;
using UnityEngine;

namespace HIAAC.BehaviorTrees.Needs
{

    public class ApplyAdvertisedNeedsNode : ActionNode
    {

        public ApplyAdvertisedNeedsNode()
        {
            CreateProperty(typeof(NeedValueArrayBlackboardProperty), "advertisedNeeds");
        }

        public override void OnStart()
        {

        }

        public override void OnStop()
        {

        }

        public override NodeState OnUpdate()
        {
            NeedValue[] needs = GetPropertyValue<NeedValue[]>("advertisedNeeds");

            Debug.Log(needs.Count());

            foreach(NeedValue needValue in needs)
            {
                float currentValue;

                try
                {
                    currentValue = tree.needsContainer.getNeedValue(needValue.need);
                }
                catch (System.ArgumentException)
                {
                    continue;
                }

                currentValue += needValue.value;

                tree.needsContainer.setNeedValue(needValue.need, currentValue);
            }

            return NodeState.Success;
        }
    }
}