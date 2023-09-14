using UnityEngine;

namespace HIAAC.BehaviorTree
{
    /// <summary>
    /// Repeat the child execution.
    ///
    /// The number of times to repeat can be defined on de "repeatCount" property.
    /// </summary>
    /// <remarks>
    /// If using memory, execute one time and return running until "repeatCount". <br/>
    /// If not using memory, execute the child "repeatCount" times in the same update.<br/>
    /// <br/>
    /// If child returns "Running", both modes return "Running", and memoryless don't execute "repeatCount" times.
    /// </remarks>
    public class RepeatNode : DecoratorNode
    {
        uint repeatCount; // Number of times to repeat
        int currentRepeatCount = 0; // Current repeat count

        public RepeatNode() : base(MemoryMode.Both)
        {
            CreateProperty(typeof(UIntBlackboardProperty), "repeatCount");
            SetPropertyValue<uint>("repeatCount", 1);
        }


        /// <summary>
        /// Get repeat count property value.
        /// </summary>
        public override void OnStart()
        {
            currentRepeatCount = 0;
            repeatCount = GetPropertyValue<uint>("repeatCount");
        }

        public override void OnStop()
        {

        }

        public override NodeState OnUpdate()
        {
            if (UseMemory)
            {
                return memoriedUpdate();
            }
            else
            {
                return memorylessUpdate();
            }
        }

        /// <summary>
        /// Update the node in the memoryless mode. 
        /// See class documentation for more information.
        /// </summary>
        /// <returns>Current state.</returns>
        NodeState memorylessUpdate()
        {
            for (int i = 0; i < repeatCount; i++)
            {
                NodeState state = child.Update();

                switch (state)
                {
                    case NodeState.Runnning:
                        return NodeState.Runnning;
                    case NodeState.Failure:
                        currentRepeatCount += 1;
                        break;
                    case NodeState.Success:
                        currentRepeatCount += 1;
                        break;
                }
            }

            return NodeState.Success;
        }

        /// <summary>
        /// Update the node in the memoried mode. 
        /// See class documentation for more information.
        /// </summary>
        /// <returns>Current state.</returns>
        NodeState memoriedUpdate()
        {
            NodeState state = child.Update();

            switch (state)
            {
                case NodeState.Runnning:
                    return NodeState.Runnning;
                case NodeState.Failure:
                    currentRepeatCount += 1;
                    break;
                case NodeState.Success:
                    currentRepeatCount += 1;
                    break;
            }

            if (currentRepeatCount >= repeatCount)
            {
                currentRepeatCount = 0;
                return NodeState.Success;
            }

            return NodeState.Runnning;
        }
    }
}