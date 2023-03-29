using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Location;
using static Chunk;
using static EnvironmentId;

/*
* An environment is a collection of chunks.
* It is the world.
*/
public class Environment {
    private EnvironmentId id;
    private List<Chunk> chunks = new List<Chunk>();
    private GameObject gameObject;

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
}