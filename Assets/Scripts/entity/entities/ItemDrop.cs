using UnityEngine;
using Vector3 = System.Numerics.Vector3;

namespace beyondnations {
    /// <summary>
    /// Represents an ItemStack that exists in the world as a physical entity.
    /// Players and pawns can interact with ItemDrops to pick them up.
    /// </summary>
    public class ItemDrop : Entity {
        private ItemStack itemStack;

        public ItemDrop(Vector3 position, ItemStack itemStack) : base(EntityType.ITEM_DROP, getItemDropName(itemStack)) {
            this.itemStack = itemStack;
            createGameObject(position);
        }

        private static string getItemDropName(ItemStack itemStack) {
            return itemStack.getItemType().ToString() + " x" + itemStack.getQuantity();
        }

        public ItemStack getItemStack() {
            return itemStack;
        }

        public override void createGameObject(Vector3 position) {
            GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            gameObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            
            // Color based on item type
            Color color = getColorForItemType(itemStack.getItemType());
            gameObject.GetComponent<Renderer>().material.color = color;
            
            gameObject.transform.position = position;
            gameObject.name = getName();
            
            // Remove collider for now - can be added later if needed
            UnityEngine.Object.Destroy(gameObject.GetComponent<SphereCollider>());
            
            setGameObject(gameObject);
        }

        public override void destroyGameObject() {
            UnityEngine.Object.Destroy(getGameObject());
        }

        private Color getColorForItemType(ItemType itemType) {
            switch (itemType) {
                case ItemType.COIN:
                    return new Color(1f, 0.84f, 0f); // Gold
                case ItemType.WOOD:
                    return new Color(0.6f, 0.4f, 0.2f); // Brown
                case ItemType.STONE:
                    return Color.gray;
                case ItemType.APPLE:
                    return Color.red;
                case ItemType.SAPLING:
                    return Color.green;
                default:
                    return Color.white;
            }
        }
    }
}
