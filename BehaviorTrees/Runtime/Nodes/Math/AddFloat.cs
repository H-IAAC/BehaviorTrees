namespace HIAAC.BehaviorTrees
{
    public class AddFloat : ActionNode
    {
        public AddFloat()
        {
            CreateProperty(typeof(FloatBlackboardProperty), "input1");
            CreateProperty(typeof(FloatBlackboardProperty), "input2");
            CreateProperty(typeof(FloatBlackboardProperty), "result");
        }

        public override void OnStart()
        {
        }

        public override void OnStop()
        {
        }

        public override NodeState OnUpdate()
        {
            float input1 = GetPropertyValue<float>("input1");
            float input2 = GetPropertyValue<float>("input2");

            float result = input1 + input2;

            SetPropertyValue("result", result);

            return NodeState.Success;
        }
    }
}