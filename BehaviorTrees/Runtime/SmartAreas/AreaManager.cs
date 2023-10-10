using System;
using System.Collections.Generic;
using UnityEngine;

namespace HIAAC.BehaviorTrees.SmartAreas
{
    public class AreaManager
    {
        private static Lazy<AreaManager> _instance = new (() => new AreaManager());
        public static AreaManager instance => _instance.Value;
        
        List<SmartArea> areas;

        private AreaManager()
        {
            areas = new();
        }

        public void Register(SmartArea area)
        {
            if(areas.Contains(area))
            {
                return;
            }

            int index = areas.BinarySearch(area, new SmartAreaComparer());

            if(index<0)
            {
                index = ~index;
            }

            areas.Insert(index, area);
        }

        public void Unregister(SmartArea area)
        {
            areas.Remove(area);
        }

        public List<BehaviorTag> GetTags(List<BTagParameter> agentParameters, Vector3 position)
        {
            List<BehaviorTag> tags = new();

            foreach(SmartArea area in areas)
            {
                if(area.IsInside(position))
                {
                    area.ProvideTags(agentParameters, tags);
                }
            }

            return tags;
        }

    }

    public class SmartAreaComparer : IComparer<SmartArea>
    {
        public int Compare(SmartArea x, SmartArea y)
        {
            if(x.Priority > y.Priority)
            {
                return 1;
            }
            else if (x.Priority < y.Priority)
            {
                return -1;
            }

            return 0;
        }
    }
}
