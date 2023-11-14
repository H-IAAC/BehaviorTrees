using System.Collections.Generic;
using UnityEngine;

namespace HIAAC.BehaviorTrees.Needs
{
    [System.Serializable]
    public class NeedsContainer
    {
        [SerializeField] List<Need> needs;
        [SerializeField] List<float> values;

        Dictionary<Need, int> needMap;

        public float getNeedValue(Need need)
        {
            if(Application.isPlaying)
            {
                if(needMap == null)
                {
                    needMap = new();
                    for(int i = 0; i<needs.Count; i++)
                    {
                        needMap.Add(needs[i], i);
                    }
                }

                int index = needMap[need];
                return values[index];
            }

            for(int i = 0; i<needs.Count; i++)
            {
                if(needs[i] == need)
                {
                    return values[i];
                }
            }

            throw new System.ArgumentException("Need not exist in container");
        }

        public void addNeed(Need need, float value=0f)
        {
            needs.Add(need);
            values.Add(value);

            if(needMap != null)
            {
                needMap.Add(need, needs.Count-1);
            }
        }

        public Need[] GetNeeds()
        {
            return needs.ToArray();
        }

    }
}