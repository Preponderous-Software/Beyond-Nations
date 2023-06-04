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
        private EntityRepository entityRepository;

        public Environment(int chunkSize, int locationScale, EntityRepository entityRepository) {
            this.entityRepository = entityRepository;
            this.id = new EnvironmentId();
            gameObject = new GameObject("Environment");
            gameObject.transform.parent = GameObject.Find("Open Source Game").transform;
            gameObject.transform.position = new Vector3(0, 0, 0);

            // create initial chunk
            Chunk chunk = new Chunk(0, 0, chunkSize, locationScale);
            addChunk(chunk);
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

        public int getChunkSize() {
            return chunks[0].getSize();
        }

        public int getLocationScale() {
            return chunks[0].getLocationScale();
        }

        public TreeEntity getNearestTree(Vector3 position) {
            return (TreeEntity)getNearestEntityOfType(position, EntityType.TREE);
        }

        public RockEntity getNearestRock(Vector3 position) {
            return (RockEntity)getNearestEntityOfType(position, EntityType.ROCK);
        }

        public Entity getNearestEntityOfType(Vector3 position, EntityType type) {
            Entity nearestEntity = null;
            float nearestDistance = float.MaxValue;
            foreach (Entity entity in entityRepository.getEntities()) {
                if (entity.getType() == type) {
                    float distance = Vector3.Distance(position, entity.getGameObject().transform.position);
                    if (distance < nearestDistance) {
                        nearestDistance = distance;
                        nearestEntity = entity;
                    }
                }
            }
            return nearestEntity;
        }

        public Chunk getChunkAtPosition(Vector3 position) {
            int chunkSize = getChunkSize();
            int locationScale = getLocationScale();
            int xpos = (int)(position.x / (chunkSize * locationScale));
            int zpos = (int)(position.z / (chunkSize * locationScale));
            return getChunk(xpos, zpos);
        }

        public void destroyGameObject() {
            UnityEngine.Object.Destroy(gameObject);
        }
    }
}