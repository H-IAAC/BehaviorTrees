using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using HIAAC.BehaviorTrees.SmartAreas;


namespace HIAAC.BehaviorTrees
{
    public class RequestBehaviorNode : SubtreeNode
    {
        [SerializeField] bool fromArea;

        public RequestBehaviorNode() : base()
        {
            CreateProperty(typeof(TagProviderProperty), "tagProvider");
            passValue.Add(false);

            propertiesDontDeleteOnValidate.Add("tagProvider");
        }

        void OnValidate()
        {
            if(fromArea)
            {
                if(HasProperty("tagProvider"))
                {
                    DeleteProperty("tagProvider");
                }
            }
            else
            {
                if(!HasProperty("tagProvider"))
                {
                    CreateProperty(typeof(TagProviderProperty), "tagProvider");
                }
            }
        }

        public override BehaviorTree Subtree
        {
            get
            {
                return subtree;
            }

            set
            {
                Debug.LogError("Can't direct assign RequestBehaviorNode subtree. Use SubtreeNode instead.");
            }
        }

        public List<BTagParameter> minimumValueParameters = new();
        public List<BTagParameter> maximumValueParameters = new();


        bool overrideMode = false;

        BehaviorTag currentTag;


        BehaviorTag requestTag()
        {
            List<BehaviorTag> tags;
            if (fromArea)
            {
                tags = AreaManager.instance.GetTags(tree.bTagParameters, gameObject.transform.position);
            }
            else
            {
                object providerObj = GetPropertyValue("tagProvider");

                IBTagProvider provider = providerObj as IBTagProvider;

                if (provider == null)
                {
                    return null;
                }
                tags = provider.ProvideTags(tree.bTagParameters);
            }

            
            return IBTagProvider.GetFirstCompatible(tags, minimumValueParameters, maximumValueParameters);
        }

        public override void OnStart()
        {
            if (currentTag == null || overrideMode)
            {
                BehaviorTag oldTag = currentTag;
                if(oldTag != null)
                {
                    oldTag.UnregisterUser(gameObject);
                }

                BehaviorTag newTag = requestTag();

                if (newTag == null)
                {
                    if (!overrideMode)
                    {
                        currentTag = newTag;
                    }
                }
                else
                {
                    currentTag = newTag;
                    overrideMode = false;
                }

                if (currentTag != null)
                {
                    base.Subtree = currentTag.RegisterUser(gameObject);
                }
                else
                {
                    base.Subtree = null;
                }

            
            }

            base.OnStart();
        }

        void DoLifecycleBehavior(TagLifecycleType lifecyleType, NodeState state)
        {
            switch (lifecyleType)
            {
                case TagLifecycleType.DROP:
                    currentTag.UnregisterUser(gameObject);
                    currentTag = null;
                    break;
                case TagLifecycleType.HOLD:
                    break;
                case TagLifecycleType.OVERRIDABLE:
                    if (state != NodeState.Runnning)
                    {
                        overrideMode = true;
                    }

                    break;
            }
        }

        public override NodeState OnUpdate()
        {
            if (currentTag == null)
            {
                return NodeState.Failure;
            }

            NodeState state = base.OnUpdate();

            switch (state)
            {
                case NodeState.Runnning:
                    DoLifecycleBehavior(currentTag.onRunning, state);
                    break;
                case NodeState.Failure:
                    DoLifecycleBehavior(currentTag.onFailure, state);
                    break;
                case NodeState.Success:
                    DoLifecycleBehavior(currentTag.onSuccess, state);
                    break;
            }

            return state;
        }
    }
}
