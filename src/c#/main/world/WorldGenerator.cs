using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace osg
{
    /**
    * The WorldGenerator class is responsible for generating the land.
    */
    public class WorldGenerator
    {
        private Environment environment;
        private Player player;
        private EventProducer eventProducer;
        private int chunkSize = 5;
        private int locationScale = 3;
        private int currentChunkX = 0;
        private int currentChunkZ = 0;

        public WorldGenerator(Environment environment, Player player, EventProducer eventProducer)
        {
            this.environment = environment;
            this.player = player;
            this.eventProducer = eventProducer;
            this.chunkSize = environment.getChunkSize();
            this.locationScale = environment.getLocationScale();
        }

        public void update()
        {
            calculateCurrentChunk();
            generateCurrentChunk();
            generateSurroundingChunks();
        }

        public int getCurrentChunkX()
        {
            return currentChunkX;
        }

        public int getCurrentChunkZ()
        {
            return currentChunkZ;
        }

        private void generateCurrentChunk()
        {
            generateChunkIfNotExistent(currentChunkX, currentChunkZ);
        }

        private void generateSurroundingChunks()
        {
            // sides
            generateChunkIfNotExistent(currentChunkX + 1, currentChunkZ);
            generateChunkIfNotExistent(currentChunkX - 1, currentChunkZ);
            generateChunkIfNotExistent(currentChunkX, currentChunkZ + 1);
            generateChunkIfNotExistent(currentChunkX, currentChunkZ - 1);

            // corners
            generateChunkIfNotExistent(currentChunkX + 1, currentChunkZ + 1);
            generateChunkIfNotExistent(currentChunkX - 1, currentChunkZ + 1);
            generateChunkIfNotExistent(currentChunkX + 1, currentChunkZ - 1);
            generateChunkIfNotExistent(currentChunkX - 1, currentChunkZ - 1);
        }

        private void generateChunkIfNotExistent(int chunkX, int chunkZ)
        {
            // check if chunk exists
            Chunk chunk = environment.getChunk(chunkX, chunkZ);
            if (chunk == null)
            {
                createNewChunkAt(chunkX, chunkZ);
            }
        }

        /**
        * Calculates the current chunk based on the player position.
        */
        private void calculateCurrentChunk()
        {
            Vector3 playerPosition = player.getGameObject().transform.position;
            int lengthOfChunk = chunkSize * locationScale;

            if (playerPosition.x >= 0)
            {
                currentChunkX = (int)(playerPosition.x / lengthOfChunk);
            }
            else
            {
                currentChunkX = (int)(playerPosition.x / lengthOfChunk) - 1;
            }

            if (playerPosition.z >= 0)
            {
                currentChunkZ = (int)(playerPosition.z / lengthOfChunk);
            }
            else
            {
                currentChunkZ = (int)(playerPosition.z / lengthOfChunk) - 1;
            }
        }

        private void createNewChunkAt(int chunkX, int chunkZ)
        {
            // produce event
            eventProducer.produceChunkGenerateEvent(chunkX, chunkZ);

            // create new chunk
            Chunk chunk = new Chunk(chunkX, chunkZ, chunkSize, locationScale);
            environment.addChunk(chunk);
            spawnTreeEntities(chunk);
            spawnRockEntities(chunk);
            spawnPawns(chunk);
        }

        private void spawnTreeEntities(Chunk chunk)
        {
            int numberOfTrees = Random.Range(chunk.getSize(), chunk.getSize() * 2);
            for (int i = 0; i < numberOfTrees; i++)
            {
                Location randomLocation = chunk.getRandomLocation();
                if (randomLocation.getNumberOfEntities() > 0)
                {
                    continue;
                }

                Vector3 locationPosition = randomLocation.getPosition();

                // create tree
                Vector3 position = new Vector3(
                    locationPosition.x,
                    locationPosition.y + 1,
                    locationPosition.z
                );
                TreeEntity tree = new TreeEntity(position, 5);

                // add tree to chunk
                chunk.addEntity(tree, randomLocation);
                environment.addEntityId(tree.getId());
                tree.getGameObject().transform.parent = randomLocation.getGameObject().transform;
            }
        }

        private void spawnRockEntities(Chunk chunk)
        {
            int numberOfRocks = Random.Range(chunk.getSize() / 4, chunk.getSize() / 2);
            for (int i = 0; i < numberOfRocks; i++)
            {
                Location randomLocation = chunk.getRandomLocation();
                if (randomLocation.getNumberOfEntities() > 0)
                {
                    continue;
                }

                Vector3 locationPosition = randomLocation.getPosition();

                // create rock
                Vector3 position = new Vector3(
                    locationPosition.x,
                    locationPosition.y + 1,
                    locationPosition.z
                );
                RockEntity rock = new RockEntity(position);

                // add rock to chunk if location is not occupied
                chunk.addEntity(rock, randomLocation);
                environment.addEntityId(rock.getId());
                rock.getGameObject().transform.parent = randomLocation.getGameObject().transform;
            }
        }

        private void spawnPawns(Chunk chunk)
        {
            // 10% change to spawn a pawn
            bool shouldSpawnPawn = Random.Range(0, 100) < 10;
            if (shouldSpawnPawn)
            {
                Location randomLocation = chunk.getRandomLocation();
                Vector3 locationPosition = randomLocation.getPosition();

                // create pawn
                Vector3 position = new Vector3(
                    locationPosition.x,
                    (float)(locationPosition.y + 1.5),
                    locationPosition.z
                );
                Pawn pawn = new Pawn(position, PawnNameGenerator.generate());
                eventProducer.producePawnSpawnEvent(position, pawn);

                chunk.addEntity(pawn, randomLocation);
                environment.addEntityId(pawn.getId());
                pawn.getGameObject().transform.parent = randomLocation.getGameObject().transform;
            }
        }

        public void generateChunkAtPosition(Vector3 position)
        {
            int lengthOfChunk = chunkSize * locationScale;

            int chunkX = 0;
            if (position.x >= 0)
            {
                chunkX = (int)(position.x / lengthOfChunk);
            }
            else
            {
                chunkX = (int)(position.x / lengthOfChunk) - 1;
            }

            int chunkZ = 0;
            if (position.z >= 0)
            {
                chunkZ = (int)(position.z / lengthOfChunk);
            }
            else
            {
                chunkZ = (int)(position.z / lengthOfChunk) - 1;
            }

            generateChunkIfNotExistent(chunkX, chunkZ);
        }
    }
}
