namespace HIAAC.BehaviorTrees
{
    /// <summary>
    /// Fallback/Selector node.
    /// </summary>
    /// <remarks>
    /// Keeps executing it's children until Success or all fail.<br/>
    /// <br/>
    /// If memoryless, executes all child in the same pass, always from the first, until success.<br/>
    /// If memoried, return execution from last "Running" child if any. Continue until Running or Success.<br/>
    /// Both modes return Running if the child is Running.
    /// </remarks>
    public class FallbackNode : CompositeNode
    {
        Node currentChild; //Current executing child.

        public FallbackNode() : base(MemoryMode.Both)
        {

        }

        public override void OnStart()
        {
        }

        public override void OnStop()
        {

        }

        public override NodeState OnUpdate()
        {
            if (!UseMemory) //No memory -> start from the beginning
            {
                ResetNext();
                currentChild = NextChild();
            }
            else if (currentChild == null) // Memory -> start from the next child
            {
                currentChild = NextChild();
            }


            while (currentChild != null)
            {
                //Execute child
                NodeState state = currentChild.Update();

                switch (state)
                {
                    case NodeState.Runnning: //Running -> Returns running.
                        return NodeState.Runnning;
                    case NodeState.Failure: //Failured -> next one
                        currentChild = NextChild();
                        break;
                    case NodeState.Success: //Sucess -> Sucess
                        return NodeState.Success;
                }

            }

            //No more child -> Failure.
            return NodeState.Failure;
        }
    }
}