using UnityEngine;

namespace HIAAC.BehaviorTree
{
    /// <summary>
    /// Prints a debug message.
    /// 
    /// Define the message to print on the "message" property.
    /// </summary>
    public class DebugLogNode : ActionNode
    {
        [Tooltip("If should print the message on the node start.")][SerializeField] bool onStart = false;
        [Tooltip("If should print the message on the node stop.")][SerializeField] bool onStop = false;
        [Tooltip("If should print the message on the node update.")][SerializeField] bool onUpdate = true;

        public DebugLogNode()
        {
            CreateProperty(typeof(StringBlackboardProperty), "message");
        }

        public override void OnStart()
        {
            if (!onStart)
            {
                return;
            }

            //Get and print message
            string message = GetPropertyValue<string>("message");
            Debug.Log($"OnStart {message}");
        }

        public override void OnStop()
        {
            if (!onStop)
            {
                return;
            }

            //Get and print message
            string message = GetPropertyValue<string>("message");
            Debug.Log($"OnStop {message}");
        }

        public override NodeState OnUpdate()
        {
            if (!onUpdate)
            {
                return NodeState.Success;
            }

            //Get and print message
            string message = GetPropertyValue<string>("message");
            Debug.Log($"OnUpdate {message}");

            return NodeState.Success;
        }
    }
}