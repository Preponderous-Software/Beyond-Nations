using UnityEngine;

using beyondnations;

namespace beyondnationstests {

    public static class TestItem {

        public static void runTests() {
            testInitialization();
            testGetType();
            testIsSameTypeAs();
            testIsSameTypeAsWithNull();
        }

        public static void testInitialization() {
            // run
            Item item = new Item(ItemType.WOOD);

            // check
            UnityEngine.Debug.Assert(item != null);
            UnityEngine.Debug.Assert(item.getType() == ItemType.WOOD);
        }

        public static void testGetType() {
            // setup
            Item coinItem = new Item(ItemType.COIN);
            Item stoneItem = new Item(ItemType.STONE);

            // check
            UnityEngine.Debug.Assert(coinItem.getType() == ItemType.COIN);
            UnityEngine.Debug.Assert(stoneItem.getType() == ItemType.STONE);
        }

        public static void testIsSameTypeAs() {
            // setup
            Item wood1 = new Item(ItemType.WOOD);
            Item wood2 = new Item(ItemType.WOOD);
            Item stone = new Item(ItemType.STONE);

            // check
            UnityEngine.Debug.Assert(wood1.isSameTypeAs(wood2));
            UnityEngine.Debug.Assert(!wood1.isSameTypeAs(stone));
        }

        public static void testIsSameTypeAsWithNull() {
            // setup
            Item wood = new Item(ItemType.WOOD);

            // check
            UnityEngine.Debug.Assert(!wood.isSameTypeAs(null));
        }
    }
}
