using System.Numerics;

namespace beyondnations {

    /**
    * An ItemStack that exists in the world as a physical entity.
    * Players and pawns can interact with ItemDrops to pick them up.
    */
    public class ItemDrop : Entity {
        private ItemStack itemStack;

        public ItemDrop(Vector3 position, ItemStack itemStack) : base(EntityType.ITEM_DROP, getItemDropName(itemStack)) {
            this.itemStack = itemStack;
            setPosition(position);
            setAppearance(new Appearance(PrimitiveKind.Sphere, new Vector3(0.5f, 0.5f, 0.5f), colorForItemType(itemStack.getItemType())));
        }

        private static string getItemDropName(ItemStack itemStack) {
            return itemStack.getItemType().ToString() + " x" + itemStack.getQuantity();
        }

        public ItemStack getItemStack() {
            return itemStack;
        }

        private static Rgba colorForItemType(ItemType itemType) {
            switch (itemType) {
                case ItemType.COIN:
                    return new Rgba(1f, 0.84f, 0f); // gold
                case ItemType.WOOD:
                    return new Rgba(0.6f, 0.4f, 0.2f); // brown
                case ItemType.STONE:
                    return Rgba.Gray;
                case ItemType.APPLE:
                    return Rgba.Red;
                case ItemType.SAPLING:
                    return Rgba.Green;
                default:
                    return Rgba.White;
            }
        }
    }
}
