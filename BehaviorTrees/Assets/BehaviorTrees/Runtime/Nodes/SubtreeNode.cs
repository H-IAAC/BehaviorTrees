using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace HIAAC.BehaviorTree
{
    public class SubtreeNode : ActionNode
    {
        [SerializeField][SerializeProperty("Subtree")] protected BehaviorTree subtree;
        [HideInInspector][SerializeField] public List<bool> passValue = new();
        [SerializeField] bool autoRemapOnAssign = false;

        [HideInInspector][SerializeField] protected List<string> propertiesDontDeleteOnValidate = new();

        BehaviorTree runtimeTree;

        public virtual BehaviorTree Subtree
        {
            get
            {
                return subtree;
            }
            set
            {
                if (runtimeTree != null)
                {
                    runtimeTree = null;
                }


                if (value != subtree)
                {
                    subtree = value;
                    ValidateSubtree();
                }
            }
        }

        protected void ValidateSubtree()
        {
            checkProperties();

            if (autoRemapOnAssign)
            {
                autoRemap();
            }
        }

        public BehaviorTree RuntimeTree
        {
            get
            {
                return runtimeTree;
            }
        }

        public SubtreeNode() : base(MemoryMode.Memoried)
        {
            
        }

        public override void OnStart()
        {
            if (!subtree)
            {
                return;
            }

            if (runtimeTree == null)
            {
                runtimeTree = subtree.Clone();
                runtimeTree.Bind(gameObject);
                runtimeTree.Start();
            }
        }

        public override void OnStop()
        {
        }

        public override NodeState OnUpdate()
        {
            if (!runtimeTree)
            {
                return NodeState.Failure;
            }

            for (int i = 0; i < runtimeTree.blackboard.properties.Count; i++)
            {
                if (passValue[i])
                {
                    BlackboardProperty property = runtimeTree.blackboard.properties[i].property;
                    property.Value = GetPropertyValue<object>(property.PropertyName);
                }

            }

            return runtimeTree.Update();
        }

        void checkProperties()
        {
            if (!subtree)
            {
                ClearPropertyDefinitions(propertiesDontDeleteOnValidate);
                return;
            }

            foreach (BlackboardOverridableProperty property in subtree.blackboard.properties)
            {
                if (!HasProperty(property.property.PropertyName))
                {
                    CreateProperty(property.property.GetType(), property.property.PropertyName);
                    passValue.Add(false);
                }
            }

            for (int i = blackboard.properties.Count - 1; i >= 0; i--)
            {
                BlackboardOverridableProperty op = blackboard.properties[i];

                if (propertiesDontDeleteOnValidate.Contains(op.property.PropertyName))
                {
                    continue;
                }

                if (!subtree.blackboard.properties.Any(x => x.property.PropertyName == op.property.PropertyName))
                {
                    blackboard.properties.RemoveAt(i);
                }
            }
        }

        void OnValidate()
        {
            checkProperties();
        }

        public void autoRemap()
        {
            for (int i = 0; i < blackboard.properties.Count; i++)
            {
                BlackboardOverridableProperty myProperty = blackboard.properties[i];

                foreach (BlackboardOverridableProperty bbProperty in tree.blackboard.properties)
                {
                    if (myProperty.property.PropertyName == bbProperty.property.PropertyName)
                    {
                        myProperty.parentName = bbProperty.property.PropertyName;
                        passValue[i] = true;
                    }
                }
            }
        }

        protected override float OnComputeUtility()
        {
            if (!subtree)
            {
                return 0f;
            }

            if (runtimeTree == null)
            {
                runtimeTree = subtree.Clone();
                runtimeTree.Bind(gameObject);

                runtimeTree.Start();
            }

            return runtimeTree.GetUtility();
        }
    }
}