using UnityEngine;

namespace HIAAC.BehaviorTrees.Needs
{
    [System.Serializable]
    public class NeedValue
    {
        public Need need;
        public float value;

        public AnimationCurve weight;

        public NeedValue()
        {
            weight = AnimationCurve.Linear(0f, 1f, 1f, 0f);
        }
        
    }
}