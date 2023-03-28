using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Environment;
using static Chunk;
using static Location;
using static LandGenerator;

/**
* The OpenSourceGame class is the main class of the game.
* It is the entry point of the game.
*/
public class OpenSourceGame : MonoBehaviour {
    private Environment environment;
    private LandGenerator landGenerator;
    private TickCounter tickCounter;
    public Player player; // must be set in Unity Editor -- TODO: make this private and set it in the constructor (will require refactoring Player.cs)

    public int chunkSize = 9;
    public int locationScale = 9;
    public int updateInterval = 10; // update every x ticks

    // Start is called before the first frame update
    void Start() {
        Debug.Log("Starting game...");

        environment = new Environment(chunkSize, locationScale);
        Debug.Log("Environment created.");

        landGenerator = new LandGenerator(environment, player);
        Debug.Log("Land generator created.");

        tickCounter = new TickCounter(updateInterval);
        Debug.Log("Tick counter created.");
    }

    // Update is called once per frame
    void Update() {
        tickCounter.increment();

        if (tickCounter.shouldUpdate()) {
            landGenerator.update();
        }
    }
}