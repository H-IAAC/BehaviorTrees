namespace HIAAC.BehaviorTrees
{
    public class StopTimer : ActionNode
    {
        public StopTimer()
        {
            CreateProperty(typeof(FloatBlackboardProperty), "timerTime");
        }

        public override void OnStart()
        {
        }

        public override void OnStop()
        {
        }

        public override NodeState OnUpdate()
        {
            SetPropertyValue("timerTime", -1f);

            return NodeState.Success;
        }
    }
}