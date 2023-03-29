using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Location;
using static Chunk;
using static Environment;

/**
 * The LandGenerator class is responsible for generating the land.
 */
public class LandGenerator {
    private Environment environment;
    private Player player;
    private int chunkSize = 5;
    private int locationScale = 3;
    private int currentChunkX = 0;
    private int currentChunkZ = 0;

    public LandGenerator(Environment environment, Player player) {
        this.environment = environment;
        this.player = player;
        this.chunkSize = environment.getChunkSize();
        this.locationScale = environment.getLocationScale();
    }

    public void update() {
        calculateCurrentChunk();
        generateCurrentChunk();
        generateSurroundingChunks();
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

    private void generateSurroundingChunks() {
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

    private void generateChunkIfNotExistent(int chunkX, int chunkZ) {
        // check if chunk exists
        Chunk chunk = environment.getChunk(chunkX, chunkZ);
        if (chunk == null) {
            createNewChunkAt(chunkX, chunkZ);
        }
    }

    /**
     * Calculates the current chunk based on the player position.
     */
    private void calculateCurrentChunk() {
        Vector3 playerPosition = player.transform.position;
        int lengthOfChunk = chunkSize * locationScale;

        if (playerPosition.x >= 0) {
            currentChunkX = (int) (playerPosition.x / lengthOfChunk);
        } else {
            currentChunkX = (int) (playerPosition.x / lengthOfChunk) - 1; // this 
        }

        if (playerPosition.z >= 0) {
            currentChunkZ = (int) (playerPosition.z / lengthOfChunk);
        } else {
            currentChunkZ = (int) (playerPosition.z / lengthOfChunk) - 1;
        }
    }

    private void createNewChunkAt(int chunkX, int chunkZ) {
        // create new chunk
        Debug.Log("Creating new chunk at " + chunkX + ", " + chunkZ);
        Chunk chunk = new Chunk(chunkX, chunkZ, chunkSize, locationScale);
        environment.addChunk(chunk);
    }
}