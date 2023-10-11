using System;
using System.Collections.Generic;
using UnityEngine;

namespace HIAAC.BehaviorTrees.SmartAreas
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    [DisallowMultipleComponent]
    public class SmartArea : MonoBehaviour, IBTagProvider
    {
        List<Collider> colliders = new();

        [SerializeReference]
        public BTagContainer tagContainer;

        [SerializeField] int priority;

        [SerializeField] BehaviorTree onEnterTree;
        [SerializeField] BehaviorTree onExitTree;

        public BehaviorTree OnEnterTree
        {
            get
            {
                if(onEnterTree.Runtime)
                {
                    return onEnterTree;
                }
                return null;
            }
        }

        public BehaviorTree OnExitTree
        {
            get
            {
                if(onExitTree.Runtime)
                {
                    return onExitTree;
                }
                return null;
            }
        }

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
            colliders.ForEach(collider => collider.isTrigger = true);

            AreaManager.instance.Register(this);

            if(onEnterTree != null)
            {
                onEnterTree = onEnterTree.Clone();
                onEnterTree.Bind(gameObject);
            }

            if(onExitTree != null)
            {
                onExitTree = onExitTree.Clone();
                onExitTree.Bind(gameObject);
            }
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
            if(onEnterTree == null)
            {
                return;
            }

            GameObject otherGO = getColliderGO(other);

            if(otherGO != null)
            {
                onEnterTree.SetPropertyValue("lastEnteredObject", otherGO);
                onEnterTree.ResetStates();
                onEnterTree.Update();
            }
        }

        void OnTriggerExit(Collider other)
        {
            if(onExitTree == null)
            {
                return;
            }

            GameObject otherGO = getColliderGO(other);

            if(otherGO != null)
            {
                onExitTree.SetPropertyValue("lastExitedObject", otherGO);
                onExitTree.ResetStates();
                onExitTree.Update();
            }
        }


        void OnValidate()
        {
            colliders.ForEach(collider => collider.isTrigger = true);

            Rigidbody rb = GetComponent<Rigidbody>();

            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezeAll;

            if(onEnterTree != null)
            {
                if(!onEnterTree.HasProperty("lastEnteredObject"))
                {
                    onEnterTree.CreateProperty(typeof(GameObjectBlackboardProperty), "lastEnteredObject");
                }
            }

            if(onExitTree != null)
            {
                if(!onExitTree.HasProperty("lastExitedObject"))
                {
                    onExitTree.CreateProperty(typeof(GameObjectBlackboardProperty), "lastExitedObject");
                }
            }
        }
        
    }
}
