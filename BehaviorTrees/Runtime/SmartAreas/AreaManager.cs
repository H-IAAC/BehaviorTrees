using System;
using System.Collections.Generic;
using UnityEngine;

namespace HIAAC.BehaviorTrees.SmartAreas
{
    public class AreaManager
    {
        private static Lazy<AreaManager> _instance = new (() => new AreaManager());
        public static AreaManager instance => _instance.Value;
        
        List<SmartArea> areas; //Change for priority tree?

        private AreaManager()
        {
            areas = new();
        }

        public void Register(SmartArea area)
        {
            int index = areas.BinarySearch(area, new SmartAreaComparer());
			if(index<0)
			{
				index = ~index;
			}
			
			int insertIndex = index;

            //Search for the area
            //BinarySearch area may be other area with same priority
            bool found = false;
			if(areas.Count > index)
			{
				if(areas[index] == area)
				{
					found = true;
				}
				else
				{
					if(areas[index].Priority > area.Priority && index != 0 && areas[index-1].Priority == area.Priority )
					{
						index --;
					}
					
					int baseIndex = index;
					
                    //Areas before baseIndex
					while(index >=0 && areas[index].Priority == area.Priority)
					{
						if(areas[index] == area)
						{
							found = true;
							break;
						}
						index --;
					}
					
					if(!found)
					{
						index = baseIndex;

                        //Areas after baseIndex
						while(areas.Count > index && areas[index].Priority == area.Priority)
						{
							if(areas[index] == area)
							{
								found = true;
								break;
							}
							index ++;
						}
					}
				}
			}
			
			if(!found)
			{
				areas.Insert(insertIndex, area);
			}
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

        public SmartArea GetArea(Vector3 position)
        {
            SmartArea resultArea = null;

            foreach (SmartArea area in areas)
            {
                if (area.IsInside(position))
                {
                    if(resultArea == null)
                    {
                        resultArea = area;
                    }
                    else if (resultArea.Priority > area.Priority)
                    {
                        resultArea = area;
                    }
                }
            }

            return resultArea;
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
