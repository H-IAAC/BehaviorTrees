using System.Collections.Generic;
using UnityEngine;

namespace HIAAC.BehaviorTrees
{
    public class GetTagUserNode : ActionNode
    {
        enum Mode
        {
            NEW_USERS,
            DROPPED_USERS
        }

        [SerializeField] Mode mode; 

        public GetTagUserNode()
        {
            CreateProperty(typeof(BehaviorTagProperty), "tag");
            CreateProperty(typeof(GameObjectBlackboardProperty), "output");
        }

        public override void OnStart()
        {
        }

        public override void OnStop()
        {
        }

        public override NodeState OnUpdate()
        {
            BehaviorTag tag = GetPropertyValue<BehaviorTag>("tag");

            List<GameObject> users;
            switch (mode)
            {
                case Mode.NEW_USERS:
                    users = tag.newUsers;
                    break;

                case Mode.DROPPED_USERS:
                    users = tag.droppedUsers;
                    break;

                default:
                    return NodeState.Failure;
            }

            if(users.Count == 0)
            {
                return NodeState.Failure;
            }

            GameObject user = users[0];
            users.RemoveAt(0);

            SetPropertyValue("output", user);
            
            return NodeState.Success;
        }
    }
}