using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace HIAAC.BehaviorTree
{
    public class RequestBehaviorNode : SubtreeNode
    {
        public RequestBehaviorNode() : base()
        {
            CreateProperty(typeof(TagProviderProperty), "tagProvider");
            passValue.Add(false);

            propertiesDontDeleteOnValidate.Add("tagProvider");
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
            object providerObj = GetPropertyValue("tagProvider");

            IBTagProvider provider = providerObj as IBTagProvider;

            if (provider == null)
            {
                return null;
            }

            List<BehaviorTag> tags = provider.ProvideTags(tree.bTagParameters);
            foreach (BehaviorTag tag in tags)
            {
                if (BTagParameter.IsCompatible(tag.parameters, minimumValueParameters, maximumValueParameters))
                {
                    return tag;
                }
            }

            return null;
        }

        public override void OnStart()
        {
            if (currentTag == null || overrideMode)
            {
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
                    base.Subtree = currentTag.tree;
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
