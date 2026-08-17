using System.Collections.Generic;

namespace beyondnations.desktop.ui.boxes {

    /**
    * The player's inventory, ported from
    * Assets/Scripts/ui/boxes/InventoryInfoBox.cs. Same six item types, same
    * labels, same order. Toggled with I, which is why the key is in the title.
    */
    public class InventoryInfoBox : InfoBox {
        private readonly Inventory inventory;

        public InventoryInfoBox(Inventory inventory) : base("Inventory Info (I)") {
            this.inventory = inventory;
        }

        public override List<string> getLines() {
            return new List<string> {
                "Coins: " + inventory.getNumItems(ItemType.COIN),
                "Wood: " + inventory.getNumItems(ItemType.WOOD),
                "Stone: " + inventory.getNumItems(ItemType.STONE),
                "Apples: " + inventory.getNumItems(ItemType.APPLE),
                "Saplings: " + inventory.getNumItems(ItemType.SAPLING),
                "Chicken Meat: " + inventory.getNumItems(ItemType.CHICKEN_MEAT)
            };
        }
    }
}
