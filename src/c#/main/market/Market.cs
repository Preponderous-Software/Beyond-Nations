using System.Collections.Generic;
using UnityEngine;
using Enum = System.Enum;

namespace osg {

    public class Market {
        private List<Stall> stalls = new List<Stall>();
        private int maxNumStalls;
        
        private int totalNumItemsBought = 0;
        private int totalNumItemsSold = 0;

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

        public int getTotalNumItemsBought() {
            return totalNumItemsBought;
        }

        public int getTotalNumItemsSold() {
            return totalNumItemsSold;
        }

        public EntityId purchaseFood(Pawn pawn) {
            return buyItem(pawn, ItemType.APPLE, 1);
        }

        /**
        * @return the id of the stall owner, or null if purchase failed
        */
        public EntityId buyItem(Pawn pawn, ItemType itemType, int quantity) {
            int cost = ItemCostCalculator.calculateCostBasedOnSupply(itemType, this);
            List<Stall> stallsToBuyFrom = new List<Stall>();
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
                if (pawn.getInventory().getNumItems(ItemType.COIN) < cost * quantity) {
                    continue;
                }
                stallsToBuyFrom.Add(stall);
            }
            if (stallsToBuyFrom.Count == 0) {
                return null;
            }

            int randomStallIndex = Random.Range(0, stallsToBuyFrom.Count);
            Stall stallToBuyFrom = stallsToBuyFrom[randomStallIndex];

            // transfer coins
            pawn.getInventory().removeItem(ItemType.COIN, cost * quantity);
            stallToBuyFrom.getInventory().addItem(ItemType.COIN, cost * quantity);

            // transfer items
            pawn.getInventory().addItem(itemType, quantity);
            stallToBuyFrom.getInventory().removeItem(itemType, quantity);

            totalNumItemsBought += quantity;

            UnityEngine.Debug.Log("Pawn " + pawn.getName() + " bought " + quantity + " " + itemType + " from " + stallToBuyFrom.getOwnerId() + " for " + cost * quantity + " coins");
            return stallToBuyFrom.getOwnerId();
        }

        /**
        * @return the id of the stall owner, or null if purchase failed
        */
        public EntityId sellResources(Pawn pawn) {
            List<Stall> stallsToSellTo = new List<Stall>();
            foreach(Stall stall in stalls) {
                if (stall.getOwnerId() == null) {
                    continue;
                }
                if (stall.getOwnerId() == pawn.getId()) {
                    continue;
                }
                foreach(ItemType itemType in Enum.GetValues(typeof(ItemType))) {
                    if (!pawn.getInventory().hasItem(itemType)) {
                        continue;
                    }

                    if (itemType == ItemType.COIN) {
                        continue;
                    }

                    int cost = ItemCostCalculator.calculateCostBasedOnSupply(itemType, this);

                    if (stall.getInventory().getNumItems(ItemType.COIN) < cost) {
                        continue;
                    }

                    stallsToSellTo.Add(stall);
                }
            }
            if (stallsToSellTo.Count == 0) {
                return null;
            }

            int randomStallIndex = Random.Range(0, stallsToSellTo.Count);
            Stall stallToSellTo = stallsToSellTo[randomStallIndex];

            foreach(ItemType itemType in Enum.GetValues(typeof(ItemType))) {
                if (!pawn.getInventory().hasItem(itemType)) {
                    continue;
                }

                if (itemType == ItemType.COIN) {
                    continue;
                }

                int cost = ItemCostCalculator.calculateCostBasedOnSupply(itemType, this);

                if (stallToSellTo.getInventory().getNumItems(ItemType.COIN) < cost) {
                    continue;
                }
                
                // transfer items
                pawn.getInventory().removeItem(itemType, 1);
                stallToSellTo.getInventory().addItem(itemType, 1);

                // transfer coins
                pawn.getInventory().addItem(ItemType.COIN, cost);
                stallToSellTo.getInventory().removeItem(ItemType.COIN, cost);

                totalNumItemsSold++;
                UnityEngine.Debug.Log("Pawn " + pawn.getName() + " sold 1 " + itemType + " to " + stallToSellTo.getOwnerId() + " for " + cost + " coins");
            }
            return stallToSellTo.getOwnerId();
        }

        public int getQuantityAvailable(ItemType itemType) {
            int quantityAvailable = 0;
            foreach(Stall stall in stalls) {
                if (stall.getOwnerId() == null) {
                    continue;
                }
                if (!stall.getInventory().hasItem(itemType)) {
                    continue;
                }
                quantityAvailable += stall.getInventory().getNumItems(itemType);
            }
            return quantityAvailable;
        }

        public int getTotalCoins() {
            return getQuantityAvailable(ItemType.COIN);
        }
    }
}