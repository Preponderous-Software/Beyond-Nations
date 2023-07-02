using UnityEngine;

using osg;

namespace osgtests {

    public static class TestInventory {

        public static void runTests() {
            testInitialization();
            testAddWood();
            testRemoveWood();
            testAddStone();
            testRemoveStone();
            testSetNumWood();
            testSetNumStone();
        }

        public static void testInitialization() {
            // run
            Inventory inventory = new Inventory(0);

            // check
            UnityEngine.Debug.Assert(inventory.getNumItems(ItemType.GOLD_COIN) == 0);
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
    }
}