using System.Collections.Generic;

namespace osg {

    public class Stall {
        private EntityId ownerId;
        private Inventory inventory;

        public Stall() {
            ownerId = null;
            inventory = new Inventory(0);
        }

        public EntityId getOwnerId() {
            return ownerId;
        }

        public void setOwnerId(EntityId ownerId) {
            this.ownerId = ownerId;
        }

        public Inventory getInventory() {
            return inventory;
        }

        public void transferContentsToEntity(Entity entity) {
            entity.getInventory().transferContentsOfInventory(inventory);
        }

        public void transferContentsFromEntity(Entity entity) {
            inventory.transferContentsOfInventory(entity.getInventory());
        }
    }
}