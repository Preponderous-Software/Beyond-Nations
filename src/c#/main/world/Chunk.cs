using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace osg {

    /**
    * A chunk is a 2D array of locations.
    * It is a part of the environment.
    */
    public class Chunk {
        private ChunkId id;
        private int size;
        private Location[,] locations;
        private Vector3 position;
        private int xpos;
        private int zpos;
        private string name;
        private GameObject gameObject;
        private List<Entity> entities = new List<Entity>();

        public Chunk(int xpos, int zpos, int size, int locationScale) {
            this.id = new ChunkId();
            this.size = size;
            this.locations = new Location[size, size];
            this.xpos = xpos;
            this.zpos = zpos;
            this.name = "Chunk_" + xpos + "_" + zpos;
            this.position = calculatePosition(locationScale);
            initializeGameObject();
            generateLocations(locationScale);
        }

        public ChunkId getId() {
            return id;
        }

        public int getSize() {
            return size;
        }

        public int getX() {
            return xpos;
        }

        public int getZ() {
            return zpos;
        }

        public Vector3 getPosition() {
            return position;
        }

        public Location[,] getLocations() {
            return locations;
        }

        public Location getLocation(int x, int z) {
            return locations[x, z];
        }

        public GameObject getGameObject() {
            return gameObject;
        }

        public int getLocationScale() {
            return locations[0, 0].getScale();
        }

        public void addEntity(Entity entity) {
            entities.Add(entity);
            Location location = getRandomLocation();
            location.addEntityId(entity.getId());
        }

        public void addEntity(Entity entity, Location location) {
            entities.Add(entity);
            location.addEntityId(entity.getId());
        }

        public void removeEntity(Entity entity) {
            entities.Remove(entity);
            // get location
            Location location = null;
            for (int x = 0; x < size; x++) {
                for (int z = 0; z < size; z++) {
                    if (locations[x, z].isEntityPresent(entity)) {
                        location = locations[x, z];
                        break;
                    }
                }
            }
            if (location == null) {
                throw new System.Exception("Entity not found in chunk");
            }
            location.removeEntityId(entity.getId());
        }

        public bool isEntityPresent(Entity entity) {
            return entities.Contains(entity);
        }

        public Entity getEntity(EntityId entityId) {
            foreach (Entity entity in entities) {
                if (entity.getId().Equals(entityId)) {
                    return entity;
                }
            }
            throw new System.Exception("Entity not found in chunk");
        }

        public int getNumberOfEntities() {
            return entities.Count;
        }

        public Location getRandomLocation() {
            int x = Random.Range(0, size);
            int z = Random.Range(0, size);
            return locations[x, z];
        }

        public List<Entity> getEntities() {
            return entities;
        }

        private Vector3 calculatePosition(int locationScale) {
            int lengthOfChunk = size * locationScale;
            int x = xpos * lengthOfChunk;
            int z = zpos * lengthOfChunk;
            return new Vector3(x, 0, z);
        }

        private void initializeGameObject() {
            this.gameObject = new GameObject(name);
            this.gameObject.transform.position = position;
        }

        private void generateLocations(int locationScale) {
            for (int x = 0; x < size; x++) {
                for (int z = 0; z < size; z++) {
                    locations[x, z] = new Location(xpos * size + x, zpos * size + z, locationScale);
                    locations[x, z].getGameObject().transform.parent = gameObject.transform;
                }
            }
        }
    }
}