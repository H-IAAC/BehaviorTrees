using System.Collections.Generic;
using UnityEngine;

namespace HIAAC.BehaviorTrees
{
    public class GetObjectPosition : ActionNode
    {
        public GetObjectPosition()
        {
            CreateProperty(typeof(GameObjectBlackboardProperty), "gameObject");
            CreateProperty(typeof(Vector3BlackboardProperty), "destination");
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

            if(go == null)
            {
                return NodeState.Failure;
            }

            SetPropertyValue("destination", go.transform.position);

            return NodeState.Success;
        }
    }
}