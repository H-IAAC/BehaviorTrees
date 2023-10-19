using System.Collections.Generic;

namespace HIAAC.BehaviorTrees
{
    public class RemoveTagNode : ActionNode
    {
        public RemoveTagNode()
        {
            CreateProperty(typeof(BTagContainerProperty), "container");
            CreateProperty(typeof(BehaviorTagListProperty), "tags");
        }

        public override void OnStart()
        {
        }

        public override void OnStop()
        {
        }

        public override NodeState OnUpdate()
        {
            List<BehaviorTag> tags = GetPropertyValue<List<BehaviorTag>>("tags");
            BTagContainer container = GetPropertyValue<BTagContainer>("container");

            foreach(BehaviorTag tag in tags)
            {
                container.RemoveTag(tag);
            }

            return NodeState.Success;
        }
    }
}