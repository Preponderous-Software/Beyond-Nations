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

        public void buyItem(Pawn buyer, Entity seller, ItemType itemType, int numItems) {
            Inventory buyerInventory = buyer.getInventory();
            Inventory sellerInventory = seller.getInventory();

            // check if seller has item
            if (sellerInventory.getNumItems(itemType) < numItems) {
                Debug.LogWarning("Seller " + seller + " does not have " + numItems + " of item type " + itemType + ".");
                return;
            }
            
            // decide price
            int price = 0;
            switch (itemType) {
                case ItemType.WOOD:
                    price = 1;
                    break;
                case ItemType.STONE:
                    price = 2;
                    break;
                case ItemType.APPLE:
                    price = 5;
                    break;
                default:
                    Debug.LogWarning("Seller " + seller + " tried to sell item type " + itemType + " but it is not a valid item type.");
                    return;
            }

            // check if buyer has enough coins
            if (buyerInventory.getNumItems(ItemType.COIN) < price) {
                Debug.LogWarning("Buyer " + buyer + " does not have enough coins to buy item type " + itemType + ". Price: " + price + ", Buyer coins: " + buyerInventory.getNumItems(ItemType.COIN));
                return;
            }

            // transfer items
            sellerInventory.removeItem(itemType, numItems);
            buyerInventory.addItem(itemType, numItems);
            buyerInventory.removeItem(ItemType.COIN, price);
            sellerInventory.addItem(ItemType.COIN, price);

            // // increase relationship
            // int increase = UnityEngine.Random.Range(1, 5);
            // buyer.increaseRelationship(seller, increase);
            // eventProducer.producePawnRelationshipIncreaseEvent(buyer, seller, increase);

            // update status
            if (seller.getType() == EntityType.PLAYER) {
                Player player = (Player)seller;
                player.getStatus().update(buyer.getName() + " bought " + numItems + " " + itemType + " from you. Relationship: " + buyer.getRelationships()[player.getId()]);
            }
        }

        public void sellItem(Pawn seller, Entity buyer, ItemType itemType, int numItems) {
            Inventory sellerInventory = seller.getInventory();
            Inventory buyerInventory = buyer.getInventory();

            // check if seller has item
            if (sellerInventory.getNumItems(itemType) < numItems) {
                Debug.LogWarning("Seller " + seller + " does not have " + numItems + " of item type " + itemType + ".");
                return;
            }
            
            // decide price
            int price = 0;
            switch (itemType) {
                case ItemType.WOOD:
                    price = 1;
                    break;
                case ItemType.STONE:
                    price = 2;
                    break;
                case ItemType.APPLE:
                    price = 5;
                    break;
                default:
                    Debug.LogWarning("Seller " + seller + " tried to sell item type " + itemType + " but it is not a valid item type.");
                    return;
            }

            // check if buyer has enough coins
            if (buyerInventory.getNumItems(ItemType.COIN) < price * numItems) {
                Debug.LogWarning("Buyer " + buyer + " does not have enough coins to purchase " + numItems + " of item type " + itemType + ".");
                return;
            }

            // transfer items
            sellerInventory.removeItem(itemType, numItems);
            buyerInventory.addItem(itemType, numItems);
            buyerInventory.removeItem(ItemType.COIN, price * numItems);
            sellerInventory.addItem(ItemType.COIN, price * numItems);

            // // increase relationship
            // int increase = UnityEngine.Random.Range(1, 5);
            // seller.increaseRelationship(buyer, increase);
            // eventProducer.producePawnRelationshipIncreaseEvent(seller, buyer, increase);

            if (buyer.getType() == EntityType.PLAYER) {
                Player player = (Player)buyer;
                player.getStatus().update(seller.getName() + " sold " + numItems + " " + itemType + " to you. Relationship: " + seller.getRelationships()[player.getId()]);
            }
        }
    }
}