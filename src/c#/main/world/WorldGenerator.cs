using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace osg {

    /**
    * The WorldGenerator class is responsible for generating the land.
    */
    public class WorldGenerator {
        private Environment environment;
        private Player player;
        private EventProducer eventProducer;
        private int chunkSize = 5;
        private int locationScale = 3;
        private int currentChunkX = 0;
        private int currentChunkZ = 0;
        private EntityRepository entityRepository;

        public WorldGenerator(Environment environment, Player player, EventProducer eventProducer, EntityRepository entityRepository) {
            this.environment = environment;
            this.player = player;
            this.eventProducer = eventProducer;
            this.chunkSize = environment.getChunkSize();
            this.locationScale = environment.getLocationScale();
            this.entityRepository = entityRepository;
        }

        public void update() {
            calculateCurrentChunk();
            generateCurrentChunk();
            generateSurroundingChunksAt(currentChunkX, currentChunkZ);
        }

        public int getCurrentChunkX() {
            return currentChunkX;
        }

        public int getCurrentChunkZ() {
            return currentChunkZ;
        }

        private void generateCurrentChunk() {
            generateChunkIfNotExistent(currentChunkX, currentChunkZ);
        }

        public void generateSurroundingChunksAt(int chunkX, int chunkZ) {
            generateChunkIfNotExistent(chunkX - 1, chunkZ - 1);
            generateChunkIfNotExistent(chunkX - 1, chunkZ);
            generateChunkIfNotExistent(chunkX - 1, chunkZ + 1);
            generateChunkIfNotExistent(chunkX, chunkZ - 1);
            // generateChunkIfNotExistent(chunkX, chunkZ);
            generateChunkIfNotExistent(chunkX, chunkZ + 1);
            generateChunkIfNotExistent(chunkX + 1, chunkZ - 1);
            generateChunkIfNotExistent(chunkX + 1, chunkZ);
            generateChunkIfNotExistent(chunkX + 1, chunkZ + 1);
        }

        private bool generateChunkIfNotExistent(int chunkX, int chunkZ) {
            Chunk chunk = environment.getChunk(chunkX, chunkZ);
            if (chunk == null) {
                createNewChunkAt(chunkX, chunkZ);
                return true;
            }
            return false;
        }

        /**
        * Calculates the current chunk based on the player position.
        */
        private void calculateCurrentChunk() {
            Vector3 playerPosition = player.getGameObject().transform.position;
            int lengthOfChunk = chunkSize * locationScale;

            if (playerPosition.x >= 0) {
                currentChunkX = (int) (playerPosition.x / lengthOfChunk);
            } else {
                currentChunkX = (int) (playerPosition.x / lengthOfChunk) - 1;
            }

            if (playerPosition.z >= 0) {
                currentChunkZ = (int) (playerPosition.z / lengthOfChunk);
            } else {
                currentChunkZ = (int) (playerPosition.z / lengthOfChunk) - 1;
            }
        }

        private void createNewChunkAt(int chunkX, int chunkZ) {
            // produce event
            eventProducer.produceChunkGenerateEvent(chunkX, chunkZ);

            // create new chunk
            Chunk chunk = new Chunk(chunkX, chunkZ, chunkSize, locationScale);
            environment.addChunk(chunk);
            spawnTreeEntities(chunk);
            spawnRockEntities(chunk);
            spawnPawns(chunk);
        }

        private void spawnTreeEntities(Chunk chunk) {
            int numberOfTrees = UnityEngine.Random.Range(chunk.getSize(), chunk.getSize() * 2);
            for (int i = 0; i < numberOfTrees; i++) {
                Location randomLocation = chunk.getRandomLocation();
                if (randomLocation.getNumberOfEntities() > 0) {
                    continue;
                }

                Vector3 locationPosition = randomLocation.getPosition();

                // create tree
                Vector3 position = new Vector3(locationPosition.x, locationPosition.y + 1, locationPosition.z);
                AppleTree tree = new AppleTree(position, 5);
                entityRepository.addEntity(tree);
            }
        }

        private void spawnRockEntities(Chunk chunk) {
            int numberOfRocks = UnityEngine.Random.Range(chunk.getSize()/4, chunk.getSize()/2);
            for (int i = 0; i < numberOfRocks; i++) {
                Location randomLocation = chunk.getRandomLocation();
                if (randomLocation.getNumberOfEntities() > 0) {
                    continue;
                }

                Vector3 locationPosition = randomLocation.getPosition();

                // create rock
                Vector3 position = new Vector3(locationPosition.x, locationPosition.y + 1, locationPosition.z);
                Rock rock = new Rock(position);
                entityRepository.addEntity(rock);
            }
        }

        private void spawnPawns(Chunk chunk) {
            
            // 10% change to spawn a pawn
            bool shouldSpawnPawn = UnityEngine.Random.Range(0, 100) < 10;
            if (shouldSpawnPawn) {
                Location randomLocation = chunk.getRandomLocation();
                Vector3 locationPosition = randomLocation.getPosition();

                // create pawn
                Vector3 position = new Vector3(locationPosition.x, (float)(locationPosition.y + 1.5), locationPosition.z);
                Pawn pawn = new Pawn(position, PawnNameGenerator.generate());
                eventProducer.producePawnSpawnEvent(position, pawn);
                entityRepository.addEntity(pawn);
            }
        }

        public bool generateChunkAtPosition(Vector3 position) {
            int lengthOfChunk = chunkSize * locationScale;

            int chunkX = 0;
            if (position.x >= 0) {
                chunkX = (int) (position.x / lengthOfChunk);
            } else {
                chunkX = (int) (position.x / lengthOfChunk) - 1;
            }

            int chunkZ = 0;
            if (position.z >= 0) {
                chunkZ = (int) (position.z / lengthOfChunk);
            } else {
                chunkZ = (int) (position.z / lengthOfChunk) - 1;
            }

            return generateChunkIfNotExistent(chunkX, chunkZ);
        }

        public void generateSurroundingChunksAtPosition(Vector3 position) {
            int lengthOfChunk = chunkSize * locationScale;

            int chunkX = 0;
            if (position.x >= 0) {
                chunkX = (int) (position.x / lengthOfChunk);
            } else {
                chunkX = (int) (position.x / lengthOfChunk) - 1;
            }

            int chunkZ = 0;
            if (position.z >= 0) {
                chunkZ = (int) (position.z / lengthOfChunk);
            } else {
                chunkZ = (int) (position.z / lengthOfChunk) - 1;
            }

            generateSurroundingChunksAt(chunkX, chunkZ);
        }
    }
}