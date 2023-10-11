using System.Collections.Generic;

namespace HIAAC.BehaviorTrees
{
    public class AddTagNode : ActionNode
    {
        public AddTagNode()
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
                if(!container.tags.Contains(tag))
                {
                    container.tags.Add(tag);
                }
            }

            return NodeState.Success;
        }
    }
}