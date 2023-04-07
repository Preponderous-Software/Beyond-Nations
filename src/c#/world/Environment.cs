using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace osg {

    /*
    * An environment is a collection of chunks.
    * It is the world.
    */
    public class Environment {
        private EnvironmentId id;
        private List<Chunk> chunks = new List<Chunk>();
        private GameObject gameObject;
        private List<EntityId> entityIds = new List<EntityId>();

        public Environment(int chunkSize, int locationScale) {
            this.id = new EnvironmentId();
            gameObject = new GameObject("Environment");
            gameObject.transform.parent = GameObject.Find("Open Source Game").transform;
            gameObject.transform.position = new Vector3(0, 0, 0);

            // create initial chunk
            Chunk chunk = new Chunk(0, 0, chunkSize, locationScale);
            addChunk(chunk);
        }

        public EnvironmentId getId() {
            return id;
        }

        public void addChunk(Chunk chunk) {
            chunk.getGameObject().transform.parent = gameObject.transform;
            chunks.Add(chunk);
        }

        public Chunk getChunk(int xpos, int zpos) {
            foreach (Chunk chunk in chunks) {
                if (chunk.getX() == xpos && chunk.getZ() == zpos) {
                    return chunk;
                }
            }
            return null;
        }

        public int getSize() {
            return chunks.Count;
        }

        public int getChunkSize() {
            return chunks[0].getSize();
        }

        public int getLocationScale() {
            return chunks[0].getLocationScale();
        }

        public bool isEntityPresent(Entity entity) {
            for (int i = 0; i < chunks.Count; i++) {
                if (chunks[i].isEntityPresent(entity)) {
                    return true;
                }
            }
            return false;
        }

        public List<Chunk> getChunks() {
            return chunks;
        }

        public List<EntityId> getEntityIds() {
            return entityIds;
        }

        public Entity getEntity(EntityId entityId) {
            foreach (Chunk chunk in chunks) {
                try {
                    return chunk.getEntity(entityId);
                } catch (System.Exception e) {
                    // do nothing
                }
            }
            throw new System.Exception("Entity not found in environment");
        }

        public void addEntityId(EntityId entityId) {
            entityIds.Add(entityId);
        }

        public void removeEntityId(EntityId entityId) {
            entityIds.Remove(entityId);
        }
    }
}