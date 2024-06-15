using UnityEngine;

namespace HIAAC.BehaviorTrees.Needs
{
    public class ScoredBTag : IUseful
    {
        public BehaviorTag tag;
        public float utility;

        public ScoredBTag(BehaviorTag tag, NeedsContainer agentNeeds)
        {
            float score = 0f;
            
            for(int i = 0; i<agentNeeds.needs.Count; i++)
            {
                Need need = agentNeeds.needs[i].need;
                float currentValue = agentNeeds.needs[i].value;
                AnimationCurve weight = agentNeeds.needs[i].weight;

                if(tag.advertisedNeeds.HasNeed(need))
                {
                    float advertisedValue = tag.advertisedNeeds.getNeedValue(need);

                    score += weight.Evaluate(currentValue);
                    score += -weight.Evaluate(currentValue + advertisedValue);

                }
            }

            this.tag = tag;
            utility = score;
        }

        public float GetUtility()
        {
            return utility;
        }

        
    }
}