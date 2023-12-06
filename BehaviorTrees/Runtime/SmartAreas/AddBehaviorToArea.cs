using System;
using System.Collections.Generic;
using HIAAC.BehaviorTrees.Needs;
using UnityEngine;

namespace HIAAC.BehaviorTrees.SmartAreas
{
    public class AddBehaviorToArea : MonoBehaviour
    {
        [SerializeField] BehaviorTag bTag;

        [SerializeField] public Blackboard blackboard;
        [SerializeField][HideInInspector] public List<bool> passValue = new();

        [SerializeField] public NeedsContainer overrideNeeds;

        public AddBehaviorToArea()
        {
            blackboard = new(this);
        }

        void Start()
        {
            if(bTag == null)
            {
                return;
            }

            BehaviorTag tagClone = Instantiate(bTag);
            tagClone.tree = bTag.tree.Clone();

            //Override blackboard properties
            for(int i = 0; i<blackboard.properties.Count; i++)
            {
                BlackboardOverridableProperty thisP = blackboard.properties[i];

                int index = tagClone.blackboard.properties.FindIndex(x => x.Name == thisP.Name);

                tagClone.blackboard.properties[i].property.Value = thisP.property.Value;
                tagClone.passValue[i] = passValue[i];
            }

            //Override needs
            for(int i = 0; i<overrideNeeds.needs.Count; i++)
            {
                tagClone.advertisedNeeds.addNeed(overrideNeeds.needs[i].need, overrideNeeds.needs[i].value);
            }
            tagClone.UpdateAdvertisedNeeds();

            SmartArea area = AreaManager.instance.GetArea(transform.position);

            if(area != null)
            {
                area.AddBehavior(tagClone);
            }    
        }

        public void OnValidate()
        {

            if(bTag == null)
            {
                return;
            }

            foreach(BlackboardOverridableProperty tagP in bTag.blackboard.properties)
            {
                if(!blackboard.HasProperty(tagP.Name))
                {
                    blackboard.CreateProperty(tagP.property.GetType(), tagP.Name);
                    passValue.Add(false);
                }
            }

            for(int i = blackboard.properties.Count-1; i>= 0; i--)
            {
                BlackboardOverridableProperty tagP = blackboard.properties[i];

                if (!bTag.blackboard.HasProperty(tagP.Name))
                {
                    blackboard.properties.RemoveAt(i);
                    passValue.RemoveAt(i);
                }
            }
        }
    }
}