using System;
using System.Collections.Generic;
using UnityEngine;

namespace HIAAC.BehaviorTrees.SmartAreas
{
    public class AddBehaviorToArea : MonoBehaviour
    {
        [SerializeField] BehaviorTag bTag;

        void Start()
        {
            if(bTag == null)
            {
                return;
            }

            BehaviorTag tagClone = Instantiate(bTag);

            SmartArea area = AreaManager.instance.GetArea(transform.position);
            area.AddBehavior(tagClone);    
        }
    }
}