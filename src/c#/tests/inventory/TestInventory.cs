using UnityEngine;

using osg;

namespace osgtests {

    public static class TestInventory {

        public static void runTests() {
            testInitialization();
        }

        public static void testInitialization() {
            // run
            Inventory inventory = new Inventory();

            // check
            Debug.Assert(inventory.getNumWood() == 0);
            Debug.Assert(inventory.getNumStone() == 0);
        }

        public static void testAddWood() {
            // setup
            Inventory inventory = new Inventory();

            // run
            inventory.addWood(5);

            // check
            Debug.Assert(inventory.getNumWood() == 5);
        }

        public static void testRemoveWood() {
            // setup
            Inventory inventory = new Inventory();
            inventory.addWood(5);

            // run
            inventory.removeWood(3);

            // check
            Debug.Assert(inventory.getNumWood() == 2);
        }

        public static void testAddStone() {
            // setup
            Inventory inventory = new Inventory();

            // run
            inventory.addStone(5);

            // check
            Debug.Assert(inventory.getNumStone() == 5);
        }

        public static void testRemoveStone() {
            // setup
            Inventory inventory = new Inventory();
            inventory.addStone(5);

            // run
            inventory.removeStone(3);

            // check
            Debug.Assert(inventory.getNumStone() == 2);
        }
        
        public static void testSetNumWood() {
            // setup
            Inventory inventory = new Inventory();

            // run
            inventory.setNumWood(5);

            // check
            Debug.Assert(inventory.getNumWood() == 5);
        }

        public static void testSetNumStone() {
            // setup
            Inventory inventory = new Inventory();

            // run
            inventory.setNumStone(5);

            // check
            Debug.Assert(inventory.getNumStone() == 5);
        }
    }
}