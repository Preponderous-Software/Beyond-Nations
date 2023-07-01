using System.Collections.Generic;

namespace osg {

    public class Market {
        private List<Stall> stalls = new List<Stall>();
        private int maxNumStalls;

        public Market(int maxNumStalls) {
            this.maxNumStalls = maxNumStalls;
        }

        public bool hasStall(EntityId ownerId) {
            foreach (Stall stall in stalls) {
                if (stall.getOwnerId() == ownerId) {
                    return true;
                }
            }
            return false;
        }

        public Stall getStall(EntityId ownerId) {
            foreach (Stall stall in stalls) {
                if (stall.getOwnerId() == ownerId) {
                    return stall;
                }
            }
            return null;
        }

        public bool createStall() {
            if (stalls.Count < maxNumStalls) {
                stalls.Add(new Stall());
                return true;
            }
            return false;
        }

        public void destroyStall(EntityId ownerId) {
            foreach (Stall stall in stalls) {
                if (stall.getOwnerId() == ownerId) {
                    stalls.Remove(stall);
                    return;
                }
            }
        }

        public void transferOwnership(EntityId oldOwnerId, EntityId newOwnerId) {
            foreach (Stall stall in stalls) {
                if (stall.getOwnerId() == oldOwnerId) {
                    stall.setOwnerId(newOwnerId);
                    return;
                }
            }
        }
    }
}