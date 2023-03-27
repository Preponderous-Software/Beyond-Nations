using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Location;

/**
* A chunk is a 2D array of locations.
* It is a part of the environment.
*/
public class Chunk {
    private int size;
    private Location[,] locations;
    private Vector3 position;
    private int xpos;
    private int zpos;
    private string name;
    private GameObject gameObject;

    public Chunk(int xpos, int zpos, int size, int locationScale) {
        this.size = size;
        this.locations = new Location[size, size];
        this.xpos = xpos;
        this.zpos = zpos;
        this.name = "Chunk_" + xpos + "_" + zpos;
        this.position = calculatePosition(locationScale);
        initializeGameObject();
        generateLocations(locationScale);
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

    private Vector3 calculatePosition(int locationScale) {
        return new Vector3(xpos * size * locationScale, 0, zpos * size * locationScale);
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