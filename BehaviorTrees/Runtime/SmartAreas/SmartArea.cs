using System;
using System.Collections.Generic;
using UnityEngine;

namespace HIAAC.BehaviorTrees.SmartAreas
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    [DisallowMultipleComponent]
    public class SmartArea : BehaviorTreeRunner, IBTagProvider
    {
        List<Collider> colliders = new();

        [SerializeReference]
        BTagContainer tagContainer;

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

        public BTagContainer TagContainer
        {
            get
            {
                return tagContainer;
            }

            set
            {
                tagContainer = value;

                if(tree != null)
                {
                    tree.SetPropertyValue("tagContainer", tagContainer);
                }
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
            colliders.ForEach(collider => collider.isTrigger = true);

            AreaManager.instance.Register(this);
        }

        void OnDisable()
        {
            AreaManager.instance.Unregister(this);
        }

        GameObject getColliderGO(Collider collider)
        {
            GameObject go = collider.gameObject;

            if(go.GetComponent<BehaviorTreeRunner>() == null)
            {
                go = collider.attachedRigidbody.gameObject;

                if(go.GetComponent<BehaviorTreeRunner>() == null)
                {
                    go = null;
                }
            }

            return go;

        }

        void OnTriggerEnter(Collider other)
        {
            if(tree == null)
            {
                return;
            }

            GameObject otherGO = getColliderGO(other);

            if(otherGO != null)
            {
                tree.SetPropertyValue("lastEnteredObject", otherGO);
                tree.SetPropertyValue("objectEntered", true);
                tree.Update();
                tree.SetPropertyValue("objectEntered", false);
            }
        }

        void OnTriggerExit(Collider other)
        {
            if(tree == null)
            {
                return;
            }

            GameObject otherGO = getColliderGO(other);

            if(otherGO != null)
            {
                tree.SetPropertyValue("lastExitedObject", otherGO);
                tree.SetPropertyValue("objectExited", true);
                tree.Update();
                tree.SetPropertyValue("objectExited", false);
            }
        }


        void OnValidate()
        {
            colliders.ForEach(collider => collider.isTrigger = true);

            Rigidbody rb = GetComponent<Rigidbody>();

            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezeAll;

            if(tree != null)
            {
                string[] goNames = { "lastEnteredObject", "lastExitedObject" };
                foreach (string propertyName in goNames)
                {
                    if (!tree.HasProperty(propertyName))
                    {
                        tree.CreateProperty(typeof(GameObjectBlackboardProperty), propertyName);
                    }
                }

                string[] boolNames = {"objectEntered", "objectExited"};
                foreach (string propertyName in boolNames)
                {
                    if (!tree.HasProperty(propertyName))
                    {
                        tree.CreateProperty(typeof(BoolBlackboardProperty), propertyName);
                    }
                }

                if(!tree.HasProperty("tagContainer"))
                {
                    tree.CreateProperty(typeof(BTagContainerProperty), "tagContainer");
                }

                tree.SetPropertyValue("tagContainer", tagContainer);
            }
        }
        
    }
}
