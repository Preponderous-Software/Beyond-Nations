using UnityEngine;

using beyondnations;

namespace beyondnationstests {

    public static class TestItemStack {

        public static void runTests() {
            testInitializationWithItem();
            testInitializationWithItemType();
            testGetQuantity();
            testSetQuantity();
            testAddQuantity();
            testRemoveQuantity();
            testAddNegativeQuantity();
            testRemoveExcessQuantity();
            testIsEmpty();
            testCanStackWith();
        }

        public static void testInitializationWithItem() {
            // setup
            Item item = new Item(ItemType.WOOD);
            
            // run
            ItemStack stack = new ItemStack(item, 5);

            // check
            UnityEngine.Debug.Assert(stack != null);
            UnityEngine.Debug.Assert(stack.getItem() == item);
            UnityEngine.Debug.Assert(stack.getQuantity() == 5);
        }

        public static void testInitializationWithItemType() {
            // run
            ItemStack stack = new ItemStack(ItemType.STONE, 10);

            // check
            UnityEngine.Debug.Assert(stack != null);
            UnityEngine.Debug.Assert(stack.getItemType() == ItemType.STONE);
            UnityEngine.Debug.Assert(stack.getQuantity() == 10);
        }

        public static void testGetQuantity() {
            // setup
            ItemStack stack = new ItemStack(ItemType.COIN, 100);

            // check
            UnityEngine.Debug.Assert(stack.getQuantity() == 100);
        }

        public static void testSetQuantity() {
            // setup
            ItemStack stack = new ItemStack(ItemType.APPLE, 5);

            // run
            stack.setQuantity(15);

            // check
            UnityEngine.Debug.Assert(stack.getQuantity() == 15);
        }

        public static void testAddQuantity() {
            // setup
            ItemStack stack = new ItemStack(ItemType.WOOD, 10);

            // run
            stack.addQuantity(5);

            // check
            UnityEngine.Debug.Assert(stack.getQuantity() == 15);
        }

        public static void testRemoveQuantity() {
            // setup
            ItemStack stack = new ItemStack(ItemType.STONE, 20);

            // run
            stack.removeQuantity(8);

            // check
            UnityEngine.Debug.Assert(stack.getQuantity() == 12);
        }

        public static void testAddNegativeQuantity() {
            // setup
            ItemStack stack = new ItemStack(ItemType.WOOD, 10);

            // run - addQuantity can accept negative values
            stack.addQuantity(-3);

            // check
            UnityEngine.Debug.Assert(stack.getQuantity() == 7);
        }

        public static void testRemoveExcessQuantity() {
            // setup
            ItemStack stack = new ItemStack(ItemType.STONE, 5);

            // run - removing more than available results in negative quantity
            stack.removeQuantity(10);

            // check
            UnityEngine.Debug.Assert(stack.getQuantity() == -5);
            UnityEngine.Debug.Assert(stack.isEmpty()); // Should be empty when negative
        }

        public static void testIsEmpty() {
            // setup
            ItemStack stack1 = new ItemStack(ItemType.COIN, 0);
            ItemStack stack2 = new ItemStack(ItemType.WOOD, 5);
            ItemStack stack3 = new ItemStack(ItemType.STONE, -1);

            // check
            UnityEngine.Debug.Assert(stack1.isEmpty());
            UnityEngine.Debug.Assert(!stack2.isEmpty());
            UnityEngine.Debug.Assert(stack3.isEmpty());
        }

        public static void testCanStackWith() {
            // setup
            ItemStack woodStack1 = new ItemStack(ItemType.WOOD, 5);
            ItemStack woodStack2 = new ItemStack(ItemType.WOOD, 10);
            ItemStack stoneStack = new ItemStack(ItemType.STONE, 7);

            // check
            UnityEngine.Debug.Assert(woodStack1.canStackWith(woodStack2));
            UnityEngine.Debug.Assert(!woodStack1.canStackWith(stoneStack));
            UnityEngine.Debug.Assert(!woodStack1.canStackWith(null));
        }
    }
}
