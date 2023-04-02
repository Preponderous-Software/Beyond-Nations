namespace osg {

    public class Entity {
        private EntityId id;
        private string name;
        private ChunkId chunkId;

        public Entity(string name, ChunkId chunkId) {
            this.id = new EntityId();
            this.name = name;
            this.chunkId = chunkId;
        }

        public EntityId getId() {
            return id;
        }

        public string getName() {
            return name;
        }

        public ChunkId getChunkId() {
            return chunkId;
        }
    }
}