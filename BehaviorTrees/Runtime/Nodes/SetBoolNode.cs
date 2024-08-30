namespace HIAAC.BehaviorTrees
{
    public class SetBoolNode : ActionNode
    {
        public SetBoolNode()
        {
            CreateProperty(typeof(BoolBlackboardProperty), "variable");
            CreateProperty(typeof(BoolBlackboardProperty), "valueToSet");
        }

        public override void OnStart()
        {
        }

        public override void OnStop()
        {
        }

        public override NodeState OnUpdate()
        {
            bool valueToSet = GetPropertyValue<bool>("valueToSet");

            SetPropertyValue("variable", valueToSet);

            return NodeState.Success;
        }
    }
}