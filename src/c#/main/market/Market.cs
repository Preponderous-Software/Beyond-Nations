using System.Collections.Generic;
using UnityEngine;

namespace osg {

    public class Market {
        private List<Stall> stalls = new List<Stall>();
        private int maxNumStalls;

        public Market(int maxNumStalls) {
            this.maxNumStalls = maxNumStalls;
        }

        public List<Stall> getStalls() {
            return stalls;
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

        public int getNumStallsForSale() {
            int numStallsForSale = 0;
            foreach (Stall stall in stalls) {
                if (stall.getOwnerId() == null) {
                    numStallsForSale++;
                }
            }
            return numStallsForSale;
        }

        public Stall getStallForSale() {
            foreach (Stall stall in stalls) {
                if (stall.getOwnerId() == null) {
                    return stall;
                }
            }
            return null;
        }

        public int getNumStalls() {
            return stalls.Count;
        }

        public int getMaxNumStalls() {
            return maxNumStalls;
        }


        public bool purchaseFood(Pawn pawn) {
            return buyItem(pawn, ItemType.APPLE, 1);
        }

        public bool buyItem(Pawn pawn, ItemType itemType, int quantity) {
            int cost_for_anything = 1;
            foreach(Stall stall in stalls) {
                if (stall.getOwnerId() == null) {
                    continue;
                }
                if (stall.getOwnerId() == pawn.getId()) {
                    continue;
                }
                if (!stall.getInventory().hasItem(itemType)) {
                    continue;
                }
                if (stall.getInventory().getNumItems(itemType) < quantity) {
                    continue;
                }
                if (pawn.getInventory().getNumItems(ItemType.COIN) < cost_for_anything * quantity) {
                    continue;
                }

                // transfer coins
                pawn.getInventory().removeItem(ItemType.COIN, cost_for_anything * quantity);
                stall.getInventory().addItem(ItemType.COIN, cost_for_anything * quantity);

                // transfer items
                pawn.getInventory().addItem(itemType, quantity);
                stall.getInventory().removeItem(itemType, quantity);

                UnityEngine.Debug.Log("Pawn " + pawn.getName() + " bought " + quantity + " " + itemType + " from " + stall.getOwnerId());

                return true;
            }
            return false;
        }
    }
}