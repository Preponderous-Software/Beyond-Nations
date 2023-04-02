using System.Collections.Generic;

namespace osg {

    public class NationRepository {
        private Dictionary<NationId, Nation> nations;

        public NationRepository() {
            nations = new Dictionary<NationId, Nation>();
        }

        public Nation getNation(NationId id) {
            return nations[id];
        }

        public void addNation(Nation nation) {
            nations.Add(nation.getId(), nation);
        }

        public void removeNation(NationId id) {
            nations.Remove(id);
        }

        public Nation getNation(PlayerId playerId) {
            foreach (Nation nation in nations.Values) {
                if (nation.getLeaderId().Equals(playerId)) {
                    return nation;
                }
            }

            return null;
        }
    }
}