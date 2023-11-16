using System.Collections.Generic;
using UnityEngine;

namespace HIAAC.BehaviorTrees.Needs
{
    [System.Serializable]
    public class NeedsContainer
    {
        [SerializeField] public List<NeedValue> needs;
        Dictionary<Need, NeedValue> needMap;

        public float getNeedValue(Need need)
        {
            if(Application.isPlaying)
            {
                if(needMap == null)
                {
                    needMap = new();
                    for(int i = 0; i<needs.Count; i++)
                    {
                        needMap.Add(needs[i].need, needs[i]);
                    }
                }

                return needMap[need].value;
            }

            for(int i = 0; i<needs.Count; i++)
            {
                if(needs[i].need == need)
                {
                    return needs[i].value;
                }
            }

            throw new System.ArgumentException("Need not exist in container");
        }

        public void addNeed(Need need, float value=0f)
        {
            NeedValue needValue = new()
            {
                need = need,
                value = value
            };


            if(needMap != null)
            {
                needMap.Add(need, needValue);
            }
        }

    }
}