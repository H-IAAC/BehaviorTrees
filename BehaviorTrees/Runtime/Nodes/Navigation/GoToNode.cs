using UnityEngine;
using UnityEngine.AI;

namespace HIAAC.BehaviorTrees
{
    public class GoToNode : ActionNode
    {
        [SerializeField] GoToStrategies strategy = GoToStrategies.Linear;

        NavMeshAgent agent;
        Vector3 origin;
        Vector3 destination;
        float velocity;

        float startTime;
        float endTime;
        float duration;

        bool parameterChange = false;

        public GoToStrategies Strategy
        {
            set
            {
                strategy = value;
                parameterChange = true;
            }

            get
            {
                return strategy;
            }
        }

        public GoToNode() : base(MemoryMode.Memoried)
        {
            CreateProperty(typeof(Vector3BlackboardProperty), "destination");
            CreateProperty(typeof(FloatBlackboardProperty), "velocity");

            SetPropertyValue("velocity", 3.5f);
        }

        void OnValidate()
        {
            Strategy = strategy;
        }

        public override void OnStart()
        {
        }

        public override void OnStop()
        {
        }

        public override NodeState OnUpdate()
        {
            Vector3 newDestination = GetPropertyValue<Vector3>("destination");
            float newVelocity = GetPropertyValue<float>("velocity");

            if (newDestination != destination)
            {
                destination = newDestination;
                parameterChange = true;
            }
            if (newVelocity != velocity)
            {
                velocity = newVelocity;
                parameterChange = true;
            }
            if (parameterChange)
            {
                Vector3 position = gameObject.transform.position;
                origin = new Vector3(position.x, position.y, position.z);
            }

            NodeState state = NodeState.Failure;
            switch (strategy)
            {
                case GoToStrategies.Linear:
                    state = LinearGoTo(parameterChange);
                    break;

                case GoToStrategies.NavMesh:
                    state = NavMeshGoTo(parameterChange);
                    break;
                case GoToStrategies.Immediate:
                    state = ImmediateGoTo(parameterChange);
                    break;
            }

            parameterChange = false;

            return state;
        }

        NodeState LinearGoTo(bool parameterChange)
        {
            disableNavMeshAgent();

            if (parameterChange)
            {
                recomputeTime(destination, velocity);
            }

            if (Time.time > endTime)
            {
                return NodeState.Success;
            }

            float t = (Time.time - startTime) / duration;

            if (t > 1 || t < 0)
            {
                Debug.LogError("WRONG t");
            }

            //Updates the current position
            Vector3 currentPosition = Vector3.Lerp(origin, destination, t);

            gameObject.transform.position = currentPosition;

            return NodeState.Runnning;
        }

        NodeState NavMeshGoTo(bool parameterChange)
        {
            enableNavMeshAgent();

            if (agent == null)
            {
                return NodeState.Failure;
            }

            if (parameterChange)
            {
                agent.SetDestination(destination);

                return NodeState.Runnning;
            }

            if (agent.pathPending == false && agent.remainingDistance <= agent.stoppingDistance)
            {
                return NodeState.Success;
            }

            return NodeState.Runnning;
        }

        NodeState ImmediateGoTo(bool parameterChange)
        {
            gameObject.transform.position = destination;

            return NodeState.Success;
        }

        void disableNavMeshAgent()
        {
            if (agent == null)
            {
                agent = gameObject.GetComponent<NavMeshAgent>();
            }

            if (agent != null)
            {
                if (agent.enabled == true)
                {
                    agent.enabled = false;
                }
            }
        }

        void enableNavMeshAgent()
        {
            if (agent == null)
            {
                agent = gameObject.GetComponent<NavMeshAgent>();
            }

            if (agent != null)
            {
                if (agent.enabled == false)
                {
                    agent.enabled = true;
                }
            }
        }

        void recomputeTime(Vector3 destination, float velocity)
        {
            if (origin == null || destination == null)
            {
                startTime = 0;
                duration = 0;
                endTime = 0;
                return;
            }

            startTime = Time.time;
            duration = Vector3.Distance(origin, destination) / velocity;
            endTime = startTime + duration;
        }
    }
}