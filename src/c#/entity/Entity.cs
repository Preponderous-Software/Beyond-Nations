namespace osg {

    abstract public class Entity {
        private EntityId id;
        private EntityType type;
        private ChunkId chunkId;

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
    }
}