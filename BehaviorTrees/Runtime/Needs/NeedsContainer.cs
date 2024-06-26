using System;
using System.Collections.Generic;
using UnityEngine;

namespace HIAAC.BehaviorTrees.Needs
{
    [System.Serializable]
    public class NeedsContainer
    {
        [SerializeField] public List<NeedValue> needs;
        Dictionary<Need, NeedValue> needMap;

        NeedValue getNeed(Need need)
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

                return needMap[need];
            }

            for(int i = 0; i<needs.Count; i++)
            {
                if(needs[i].need == need)
                {
                    return needs[i];
                }
            }

            throw new System.ArgumentException("Need not exist in container");
        }

        public float getNeedValue(Need need)
        {
            return getNeed(need).value;         
        }

        public void setNeedValue(Need need, float value)
        {
            value = Mathf.Clamp01(value);
            getNeed(need).value = value;         
        }

        public float getWeightedNeedAt(Need need, float value)
        {
            return getNeed(need).weight.Evaluate(value);
        }

        public float getWeightedNeed(Need need)
        {
            NeedValue needValue = getNeed(need);
            return needValue.weight.Evaluate(needValue.value);
        }

        public bool HasNeed(Need need)
        {
            try
            {
                getNeed(need);
            }
            catch(Exception)
            {
                return false;
            }
            return true;
        }


        public void addNeed(Need need, float value=0f)
        {
            if(HasNeed(need))
            {
                NeedValue needValue1 = getNeed(need);
                needValue1.value = value;

                return;
            }

            NeedValue needValue = new()
            {
                need = need,
                value = value
            };


            if(needMap != null)
            {
                needMap.Add(need, needValue);
            }

            needs.Add(needValue);
        }

    }
}