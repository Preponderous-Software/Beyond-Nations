using UnityEngine;

using beyondnations;

namespace beyondnationstests {

    public static class TestInventory {

        public static void runTests() {
            testInitialization();
            testAddWood();
            testRemoveWood();
            testAddStone();
            testRemoveStone();
            testSetNumWood();
            testSetNumStone();
            testTransferContentsOfInventory();
            testContainsAbundanceOfResources();
            testGetTotalNumItems();
            testHasItem();
            testClear();
            testGetSlots();
        }

        public static void testInitialization() {
            // run
            Inventory inventory = new Inventory(0);

            // check
            UnityEngine.Debug.Assert(inventory.getNumItems(ItemType.COIN) == 0);
            UnityEngine.Debug.Assert(inventory.getNumItems(ItemType.WOOD) == 0);
            UnityEngine.Debug.Assert(inventory.getNumItems(ItemType.STONE) == 0);
        }

        public static void testAddWood() {
            // setup
            Inventory inventory = new Inventory(0);

            // run
            inventory.addItem(ItemType.WOOD, 5);

            // check
            UnityEngine.Debug.Assert(inventory.getNumItems(ItemType.WOOD) == 5);
        }

        public static void testRemoveWood() {
            // setup
            Inventory inventory = new Inventory(0);
            inventory.addItem(ItemType.WOOD, 5);

            // run
            inventory.removeItem(ItemType.WOOD, 3);

            // check
            UnityEngine.Debug.Assert(inventory.getNumItems(ItemType.WOOD) == 2);
        }

        public static void testAddStone() {
            // setup
            Inventory inventory = new Inventory(0);

            // run
            inventory.addItem(ItemType.STONE, 5);

            // check
            UnityEngine.Debug.Assert(inventory.getNumItems(ItemType.STONE) == 5);
        }

        public static void testRemoveStone() {
            // setup
            Inventory inventory = new Inventory(0);
            inventory.addItem(ItemType.STONE, 5);

            // run
            inventory.removeItem(ItemType.STONE, 3);

            // check
            UnityEngine.Debug.Assert(inventory.getNumItems(ItemType.STONE) == 2);
        }
        
        public static void testSetNumWood() {
            // setup
            Inventory inventory = new Inventory(0);

            // run
            inventory.setNumItems(ItemType.WOOD, 5);

            // check
            UnityEngine.Debug.Assert(inventory.getNumItems(ItemType.WOOD) == 5);
        }

        public static void testSetNumStone() {
            // setup
            Inventory inventory = new Inventory(0);

            // run
            inventory.setNumItems(ItemType.STONE, 5);

            // check
            UnityEngine.Debug.Assert(inventory.getNumItems(ItemType.STONE) == 5);
        }

        public static void testTransferContentsOfInventory() {
            // setup
            Inventory inventory1 = new Inventory(0);
            Inventory inventory2 = new Inventory(10);
            inventory1.addItem(ItemType.WOOD, 5);
            inventory2.addItem(ItemType.STONE, 3);

            // run
            inventory1.transferContentsOfInventory(inventory2);

            // check
            UnityEngine.Debug.Assert(inventory1.getNumItems(ItemType.COIN) == 10);
            UnityEngine.Debug.Assert(inventory1.getNumItems(ItemType.WOOD) == 5);
            UnityEngine.Debug.Assert(inventory1.getNumItems(ItemType.STONE) == 3);
            UnityEngine.Debug.Assert(inventory2.getNumItems(ItemType.COIN) == 0);
            UnityEngine.Debug.Assert(inventory2.getNumItems(ItemType.STONE) == 0);
        }

        public static void testContainsAbundanceOfResources() {
            // setup
            Inventory inventory1 = new Inventory(0);
            Inventory inventory2 = new Inventory(0);
            inventory2.addItem(ItemType.WOOD, 15);
            inventory2.addItem(ItemType.STONE, 15);
            inventory2.addItem(ItemType.APPLE, 15);

            // check
            UnityEngine.Debug.Assert(!inventory1.containsAbundanceOfResources());
            UnityEngine.Debug.Assert(inventory2.containsAbundanceOfResources());
        }

        public static void testGetTotalNumItems() {
            // setup
            Inventory inventory = new Inventory(10);
            inventory.addItem(ItemType.WOOD, 5);
            inventory.addItem(ItemType.STONE, 3);

            // check
            UnityEngine.Debug.Assert(inventory.getTotalNumItems() == 18);
        }

        public static void testHasItem() {
            // setup
            Inventory inventory = new Inventory(0);
            inventory.addItem(ItemType.WOOD, 5);

            // check
            UnityEngine.Debug.Assert(inventory.hasItem(ItemType.WOOD));
            UnityEngine.Debug.Assert(!inventory.hasItem(ItemType.STONE));
        }

        public static void testClear() {
            // setup
            Inventory inventory = new Inventory(10);
            inventory.addItem(ItemType.WOOD, 5);
            inventory.addItem(ItemType.STONE, 3);

            // run
            inventory.clear();

            // check
            UnityEngine.Debug.Assert(inventory.getNumItems(ItemType.COIN) == 0);
            UnityEngine.Debug.Assert(inventory.getNumItems(ItemType.WOOD) == 0);
            UnityEngine.Debug.Assert(inventory.getNumItems(ItemType.STONE) == 0);
        }

        public static void testGetSlots() {
            // setup
            Inventory inventory = new Inventory(0);
            inventory.addItem(ItemType.WOOD, 5);

            // check - Inventory initializes with 5 slots (one for each ItemType: COIN, WOOD, STONE, APPLE, SAPLING)
            UnityEngine.Debug.Assert(inventory.getSlots() != null);
            UnityEngine.Debug.Assert(inventory.getSlots().Count == 5);
        }
    }
}