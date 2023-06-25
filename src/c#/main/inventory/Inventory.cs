using System.Collections.Generic;

namespace osg {

    public class Inventory {
        private Dictionary<ItemType, int> items = new Dictionary<ItemType, int>();

        public Inventory(int numGoldCoins) {
            items.Add(ItemType.GOLD_COIN, numGoldCoins);
            items.Add(ItemType.WOOD, 0);
            items.Add(ItemType.STONE, 0);
            items.Add(ItemType.APPLE, 0);
            items.Add(ItemType.SAPLING, 0);
        }
        
        public int getNumItems(ItemType itemType) {
            return items[itemType];
        }

        public void addItem(ItemType itemType, int numItems) {
            items[itemType] = items[itemType] + numItems;
        }

        public void removeItem(ItemType itemType, int numItems) {
            items[itemType] = items[itemType] - numItems;
        }

        public bool hasItem(ItemType itemType) {
            return items[itemType] > 0;
        }

        public void setNumItems(ItemType itemType, int numItems) {
            items[itemType] = numItems;
        }

        public void clear() {
            items[ItemType.GOLD_COIN] = 0;
            items[ItemType.WOOD] = 0;
            items[ItemType.STONE] = 0;
            items[ItemType.APPLE] = 0;
            items[ItemType.SAPLING] = 0;
        }

        public void transferContentsOfInventory(Inventory otherInventory) {
            items[ItemType.GOLD_COIN] += otherInventory.getNumItems(ItemType.GOLD_COIN);
            items[ItemType.WOOD] += otherInventory.getNumItems(ItemType.WOOD);
            items[ItemType.STONE] += otherInventory.getNumItems(ItemType.STONE);
            items[ItemType.APPLE] += otherInventory.getNumItems(ItemType.APPLE);
            items[ItemType.SAPLING] += otherInventory.getNumItems(ItemType.SAPLING);
            otherInventory.clear();
        }
    }
}