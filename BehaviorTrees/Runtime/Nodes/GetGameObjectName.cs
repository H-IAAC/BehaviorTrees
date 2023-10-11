using System.Collections.Generic;
using UnityEngine;

namespace HIAAC.BehaviorTrees
{
    public class GetGameObjectName : ActionNode
    {
        public GetGameObjectName()
        {
            CreateProperty(typeof(GameObjectBlackboardProperty), "gameObject");
            CreateProperty(typeof(StringBlackboardProperty), "destination");
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

            SetPropertyValue("destination", go.name);

            return NodeState.Success;
        }
    }
}