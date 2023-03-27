using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
* A location is a single point in the world.
* It is a part of a chunk.
*/
public class Location {
    private Vector3 position;
    private int scale;
    private string name;
    private GameObject gameObject;

    public Location(int xpos, int zpos, int scale) {
        this.position = new Vector3(xpos * scale, 0, zpos * scale);
        this.scale = scale;
        this.name = "Location_" + xpos + "_" + zpos;
        initializeGameObject();
    }

    public Vector3 getPosition() {
        return position;
    }

    public int getScale() {
        return scale;
    }

    public GameObject getGameObject() {
        return gameObject;
    }

    private void initializeGameObject() {
        this.gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        this.gameObject.name = name;
        this.gameObject.transform.position = position;
        this.gameObject.transform.localScale = new Vector3(scale, 1, scale);
        // random green color
        setColor(new Color(0, Random.value, 0));
    }

    private void setColor(Color color) {
        this.gameObject.GetComponent<Renderer>().material.color = color;
    }
}