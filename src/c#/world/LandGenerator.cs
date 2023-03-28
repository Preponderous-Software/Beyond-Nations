using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Location;
using static Chunk;
using static Environment;

/**
 * A land generator is a component that generates land.
 * It is a part of the world.
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
        updateCurrentChunkBasedOnPlayerPosition();

        generateChunkIfNotExistent(currentChunkX, currentChunkZ);

        // generate surrounding chunks
        generateChunkIfNotExistent(currentChunkX + 1, currentChunkZ);
        generateChunkIfNotExistent(currentChunkX - 1, currentChunkZ);
        generateChunkIfNotExistent(currentChunkX, currentChunkZ + 1);
        generateChunkIfNotExistent(currentChunkX, currentChunkZ - 1);
    }

    private void generateChunkIfNotExistent(int chunkX, int chunkZ) {
        // check if chunk exists
        Chunk chunk = environment.getChunk(chunkX, chunkZ);
        if (chunk == null) {
            createNewChunkAt(chunkX, chunkZ);
        }
    }

    /**
     * Updates the current chunk.
     */
    private void updateCurrentChunkBasedOnPlayerPosition() {
        Vector3 playerPosition = player.transform.position;

        // calculate chunkX & chunkZ based on player position taking into account chunk size and location scale
        int chunkX = (int) (playerPosition.x / (chunkSize * locationScale));
        int chunkZ = (int) (playerPosition.z / (chunkSize * locationScale));

        // update current chunk
        currentChunkX = chunkX;
        currentChunkZ = chunkZ;
    }

    private void createNewChunkAt(int chunkX, int chunkZ) {
        // create new chunk
        Debug.Log("Creating new chunk at " + chunkX + ", " + chunkZ);
        Chunk chunk = new Chunk(chunkX, chunkZ, chunkSize, locationScale);
        environment.addChunk(chunk);
    }
}