using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Environment;
using static Chunk;
using static Location;
using static LandGenerator;
using static GameConfig;

/**
* The OpenSourceGame class is the main class of the game.
* It is the entry point of the game.
*/
public class OpenSourceGame : MonoBehaviour {
    private GameConfig gameConfig;
    private EventRepository eventRepository;
    private EventProducer eventProducer;
    private Environment environment;
    private LandGenerator landGenerator;
    private TickCounter tickCounter;
    public Player player; // must be set in Unity Editor -- TODO: make this private and set it in the constructor (will require refactoring Player.cs)

    // Start is called before the first frame update
    void Start() {
        Debug.Log("Starting game...");

        gameConfig = new GameConfig();
        Debug.Log("Game config created.");

        eventRepository = new EventRepository();
        Debug.Log("Event repository created.");

        eventProducer = new EventProducer(eventRepository);
        Debug.Log("Event producer created.");

        environment = new Environment(gameConfig.getChunkSize(), gameConfig.getLocationScale());
        Debug.Log("Environment created.");

        landGenerator = new LandGenerator(environment, player, eventProducer);
        Debug.Log("Land generator created.");

        tickCounter = new TickCounter(gameConfig.getUpdateInterval());
        Debug.Log("Tick counter created.");
    }

    // Update is called once per frame
    void Update() {
        tickCounter.increment();

        if (tickCounter.shouldUpdate()) {
            landGenerator.update();

            checkIfPlayerIsFallingIntoVoid();
        }
    }

    void checkIfPlayerIsFallingIntoVoid() {
        float ypos = player.transform.position.y;
        if (ypos < -10) {
            // produce event
            eventProducer.producePlayerFallingIntoVoidEvent(player.transform.position);

            // reset player position
            player.transform.position = new Vector3(0, 10, 0);            
        }
    }
}