using UnityEngine;

namespace osg {

    abstract public class Entity {
        private EntityId id;
        private EntityType type;
        private GameObject gameObject;
        private bool markedForDeletion = false;
        private Inventory inventory;
        
        public Entity(EntityType type) {
            this.id = new EntityId();
            this.type = type;
            this.inventory = new Inventory(0);
        }

        public EntityId getId() {
            return id;
        }

        public EntityType getType() {
            return type;
        }

        public GameObject getGameObject() {
            return gameObject;
        }

        public void setGameObject(GameObject gameObject) {
            this.gameObject = gameObject;
        }

        public bool doesGameObjectExist() {
            return gameObject != null;
        }

        public abstract void createGameObject(Vector3 position);
        
        public abstract void destroyGameObject();

        public void markForDeletion() {
            markedForDeletion = true;
        }

        public bool isMarkedForDeletion() {
            return markedForDeletion;
        }

        public Inventory getInventory() {
            return inventory;
        }

        public void setInventory(Inventory inventory) {
            this.inventory = inventory;
        }

        public Vector3 getPosition() {
            return getGameObject().transform.position;
        }
    }
}