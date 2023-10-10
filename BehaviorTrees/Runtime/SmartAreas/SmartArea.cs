using System;
using System.Collections.Generic;
using UnityEngine;

namespace HIAAC.BehaviorTrees.SmartAreas
{
    [RequireComponent(typeof(Collider))]
    [DisallowMultipleComponent]
    public class SmartArea : MonoBehaviour, IBTagProvider
    {
        List<Collider> colliders = new();

        [SerializeReference]
        public BTagContainer tagContainer;

        [SerializeField] int priority;

        public int Priority
        {
            get { return priority; }
        }

        public string Name
        {
            get
            {
                return gameObject.name;
            }
        }

        public bool IsInside(Vector3 position)
        {
            bool inside = false;

            foreach(Collider c in colliders)
            {
                if(c.ClosestPoint(position) == position)
                {
                    inside = true;
                    break;
                }
            }

            return inside;
        }

        public List<BehaviorTag> ProvideTags(List<BTagParameter> agentParameters)
        {
            return tagContainer.ProvideTags(agentParameters);
        }

        public void ProvideTags(List<BTagParameter> agentParameters, List<BehaviorTag> tags)
        {
            tagContainer.ProvideTags(agentParameters, tags);
        }

        void OnEnable()
        {
            GetComponents(colliders);

            AreaManager.instance.Register(this);
        }

        void OnDisable()
        {
            AreaManager.instance.Unregister(this);
        }
    }
}
