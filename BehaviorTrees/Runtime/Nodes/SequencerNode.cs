using UnityEngine;

namespace HIAAC.BehaviorTrees
{
    /// <summary>
    /// Sequence node.
    /// </summary>
    /// <remarks>
    /// Keeps executing it's children until one fail or all success..<br/>
    /// <br/>
    /// If memoryless, executes all child in the same pass, always from the first, until failure or running all.<br/>
    /// If memoried, return execution from last "Running" child if any. Continue until failure or all.<br/>
    /// Both modes return Running if the child is Running.
    /// </remarks>
    public class SequencerNode : CompositeNode
    {
        Node currentChild; //Current child
        bool first;

        public SequencerNode() : base(MemoryMode.Both)
        {

        }

        public override void OnStart()
        {
            first = true;
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
        /// Memoried behavior
        /// </summary>
        /// <returns>Current node state</returns>
        NodeState memoriedUpdate()
        {
            if(first)
            {
                currentChild = NextChild();
                first = false;
            }
            
            //All child runned -> Success
            if (currentChild == null)
            {
                return NodeState.Success;
            }

            //Run child
            NodeState childState = currentChild.Update();

            switch (childState)
            {
                case NodeState.Runnning: // Runnning -> Runnning
                    return NodeState.Runnning;
                case NodeState.Failure: //Failure -> Failure
                    return NodeState.Failure;
                case NodeState.Success: //Sucess -> Next child
                    currentChild = NextChild();
                    return memoriedUpdate();
            }

            return NodeState.Success;
        }

        /// <summary>
        /// Memoryless behavior
        /// </summary>
        /// <returns>Current node state</returns>
        NodeState memorylessUpdate()
        {
            ResetNext();
            currentChild = NextChild();

            //Run all children
            while (currentChild != null)
            {
                //Run child
                NodeState state = currentChild.Update();

                //If no success -> return
                if (state != NodeState.Success)
                {
                    return state;
                }

                //Sucess -> continue
                currentChild = NextChild();
            }

            //All success -> Success
            return NodeState.Success;
        }

    }
}