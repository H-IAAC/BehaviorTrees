using System;
using UnityEngine;

namespace HIAAC.BehaviorTrees
{
    [Serializable]
    public class TagProviderProperty : BlackboardProperty
    {
        [SerializeField]
        UnityEngine.Object value;

        public override object Value
        {
            get
            {
                return value;
            }
            set
            {
                if (value is IBTagProvider)
                {
                    this.value = value as UnityEngine.Object;
                }
                else
                {
                    this.value = null;
                }
            }
        }

        public IBTagProvider TagProvider
        {
            get
            {
                return value as IBTagProvider;
            }
        }

        public override string PropertyTypeName
        {
            get
            {
                return "TagProvider";
            }
        }

        public override void Validate()
        {
            if (value is not IBTagProvider)
            {
                value = null;
            }
        }
    }
}