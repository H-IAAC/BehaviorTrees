using System.Collections.Generic;
using UnityEngine;

namespace HIAAC.BehaviorTrees
{
    public class StringConcatNode : ActionNode
    {
        public StringConcatNode()
        {
            CreateProperty(typeof(StringBlackboardProperty), "firstString");
            CreateProperty(typeof(StringBlackboardProperty), "secondString");
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
            string firstString = GetPropertyValue<string>("firstString");
            string secondString = GetPropertyValue<string>("secondString");

            string result = firstString + secondString;

            SetPropertyValue("destination", result);

            return NodeState.Success;
        }
    }
}