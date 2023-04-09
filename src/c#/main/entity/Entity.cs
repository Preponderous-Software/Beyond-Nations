using UnityEngine;

namespace osg {

    abstract public class Entity {
        private EntityId id;
        private EntityType type;
        private ChunkId chunkId;
        private GameObject gameObject;
        private bool markedForDeletion = false;

        public Entity(EntityType type, ChunkId chunkId) {
            this.id = new EntityId();
            this.type = type;
            this.chunkId = chunkId;
        }

        public EntityId getId() {
            return id;
        }

        public EntityType getType() {
            return type;
        }

        public ChunkId getChunkId() {
            return chunkId;
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
    }

}