using UnityEngine;

namespace HIAAC.BehaviorTrees
{
    public class GetObjectDistance : ActionNode
    {
        public GetObjectDistance()
        {
            CreateProperty(typeof(GameObjectBlackboardProperty), "gameObject");
            CreateProperty(typeof(FloatBlackboardProperty), "distance");
        }

        public override void OnStart()
        {
        }

        public override void OnStop()
        {
        }

        public override NodeState OnUpdate()
        {
            GameObject go = GetPropertyValue<GameObject>("gameObject");

            if (go == null)
            {
                return NodeState.Failure;
            }

            Vector3 this_position = gameObject.gameObject.transform.position;
            Vector3 other_position = go.transform.position;

            float distance = Vector3.Distance(this_position, other_position);

            SetPropertyValue("distance", distance);

            return NodeState.Success;
        }
    }
}