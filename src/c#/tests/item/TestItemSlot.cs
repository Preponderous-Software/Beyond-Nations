using UnityEngine;

using beyondnations;

namespace beyondnationstests {

    public static class TestItemSlot {

        public static void runTests() {
            testEmptyInitialization();
            testInitializationWithItemStack();
            testIsEmpty();
            testSetItemStack();
            testClear();
            testHasItemOfType();
            testGetQuantity();
        }

        public static void testEmptyInitialization() {
            // run
            ItemSlot slot = new ItemSlot();

            // check
            UnityEngine.Debug.Assert(slot != null);
            UnityEngine.Debug.Assert(slot.isEmpty());
            UnityEngine.Debug.Assert(slot.getItemStack() == null);
        }

        public static void testInitializationWithItemStack() {
            // setup
            ItemStack stack = new ItemStack(ItemType.WOOD, 5);
            
            // run
            ItemSlot slot = new ItemSlot(stack);

            // check
            UnityEngine.Debug.Assert(slot != null);
            UnityEngine.Debug.Assert(!slot.isEmpty());
            UnityEngine.Debug.Assert(slot.getItemStack() == stack);
        }

        public static void testIsEmpty() {
            // setup
            ItemSlot emptySlot = new ItemSlot();
            ItemSlot fullSlot = new ItemSlot(new ItemStack(ItemType.STONE, 10));
            ItemSlot zeroQuantitySlot = new ItemSlot(new ItemStack(ItemType.COIN, 0));

            // check
            UnityEngine.Debug.Assert(emptySlot.isEmpty());
            UnityEngine.Debug.Assert(!fullSlot.isEmpty());
            UnityEngine.Debug.Assert(zeroQuantitySlot.isEmpty());
        }

        public static void testSetItemStack() {
            // setup
            ItemSlot slot = new ItemSlot();
            ItemStack stack = new ItemStack(ItemType.APPLE, 3);

            // run
            slot.setItemStack(stack);

            // check
            UnityEngine.Debug.Assert(slot.getItemStack() == stack);
            UnityEngine.Debug.Assert(!slot.isEmpty());
        }

        public static void testClear() {
            // setup
            ItemSlot slot = new ItemSlot(new ItemStack(ItemType.WOOD, 15));

            // run
            slot.clear();

            // check
            UnityEngine.Debug.Assert(slot.isEmpty());
            UnityEngine.Debug.Assert(slot.getItemStack() == null);
        }

        public static void testHasItemOfType() {
            // setup
            ItemSlot woodSlot = new ItemSlot(new ItemStack(ItemType.WOOD, 5));
            ItemSlot emptySlot = new ItemSlot();

            // check
            UnityEngine.Debug.Assert(woodSlot.hasItemOfType(ItemType.WOOD));
            UnityEngine.Debug.Assert(!woodSlot.hasItemOfType(ItemType.STONE));
            UnityEngine.Debug.Assert(!emptySlot.hasItemOfType(ItemType.WOOD));
        }

        public static void testGetQuantity() {
            // setup
            ItemSlot fullSlot = new ItemSlot(new ItemStack(ItemType.COIN, 100));
            ItemSlot emptySlot = new ItemSlot();

            // check
            UnityEngine.Debug.Assert(fullSlot.getQuantity() == 100);
            UnityEngine.Debug.Assert(emptySlot.getQuantity() == 0);
        }
    }
}
