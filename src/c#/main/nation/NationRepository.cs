using UnityEngine;
using System.Collections.Generic;

namespace osg
{
    public class NationRepository
    {
        private Dictionary<NationId, Nation> nations;
        private List<NationId> nationIds;

        public NationRepository()
        {
            nations = new Dictionary<NationId, Nation>();
            nationIds = new List<NationId>();
        }

        public Nation getNation(NationId id)
        {
            try
            {
                return nations[id];
            }
            catch (KeyNotFoundException)
            {
                return null;
            }
        }

        public void addNation(Nation nation)
        {
            nations.Add(nation.getId(), nation);
            nationIds.Add(nation.getId());
        }

        public void removeNation(NationId id)
        {
            nations.Remove(id);
            nationIds.Remove(id);
        }

        public Nation getNation(EntityId entityId)
        {
            foreach (Nation nation in nations.Values)
            {
                if (nation.getLeaderId().Equals(entityId))
                {
                    return nation;
                }
            }

            return null;
        }

        public int getNumberOfNations()
        {
            return nations.Count;
        }

        public Nation getRandomNation()
        {
            int randomIndex = Random.Range(0, nationIds.Count);
            NationId randomNationId = nationIds[randomIndex];
            return nations[randomNationId];
        }
    }
}
