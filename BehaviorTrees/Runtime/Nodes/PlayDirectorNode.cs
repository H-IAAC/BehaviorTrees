using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace HIAAC.BehaviorTrees
{
    public class PlayDirectorNode : ActionNode
    {
        bool failure;
        PlayableDirector playableDirector;

        public PlayDirectorNode() : base(MemoryMode.Memoried)
        {
            CreateProperty(typeof(GameObjectBlackboardProperty), "gameObject");
        }

        public override void OnStart()
        {
            failure = false;

            GameObject go = GetPropertyValue<GameObject>("gameObject");

            if(go == null)
            {
                failure = true;
            }

            playableDirector = go.GetComponent<PlayableDirector>();

            if(playableDirector == null)
            {
                failure = true;
            }
            else
            {
                playableDirector.Play();
            }
        }

        public override void OnStop()
        {
        }

        public override NodeState OnUpdate()
        {
            if(failure)
            {
                return NodeState.Failure;
            }

            if(playableDirector.state == PlayState.Paused)
            {
                return NodeState.Success;
            }

            return NodeState.Runnning;
        }
    }
}