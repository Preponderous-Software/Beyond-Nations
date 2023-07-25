using UnityEngine;
using UnityEngine.UI;

namespace osg {
    public class InventoryInfoBox : InfoBox {
        private Inventory inventory;
        private int numDataPoints = 5;

        public InventoryInfoBox(int padding, int width, int height, int x, int y, string title, Inventory inventory) : base(padding, width, height, x, y, title) {
            this.inventory = inventory;
        }

        public override void draw() {
            // draw box with padding
            GUI.Box(new Rect(x - 10, y - 10, width + 20, (height * (numDataPoints + 2))), title);
            y += 10;

            // draw num coins
            GUI.Label(new Rect(x, y, width, height), "Coins: " + inventory.getNumItems(ItemType.COIN));
            y += height;

            // draw num wood
            GUI.Label(new Rect(x, y, width, height), "Wood: " + inventory.getNumItems(ItemType.WOOD));
            y += height;

            // draw num stone
            GUI.Label(new Rect(x, y, width, height), "Stone: " + inventory.getNumItems(ItemType.STONE));
            y += height;

            // draw num apples
            GUI.Label(new Rect(x, y, width, height), "Apples: " + inventory.getNumItems(ItemType.APPLE));
            y += height;

            // draw num saplings
            GUI.Label(new Rect(x, y, width, height), "Saplings: " + inventory.getNumItems(ItemType.SAPLING));
            y += height;
        }
    } 
}