using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Environment;
using static Chunk;
using static Location;
using static LandGenerator;
using static GameConfig;
using static CanvasFactory;
using static EventRepository;
using static EventProducer;
using static TickCounter;
using static TextGameObject;

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
    private TextGameObject chunkPositionText;
    private Status status;
    public Player player; // must be set in Unity Editor -- TODO: make this private and set it in the constructor (will require refactoring Player.cs)

    // Initialization
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

        createChunkPositionText();
        status = new Status(tickCounter, gameConfig.getStatusExpirationTicks());
        status.update("Game started.");
    }

    // Per-frame updates
    void Update() {
        tickCounter.increment();

        if (tickCounter.shouldUpdate()) {
            landGenerator.update();
            checkIfPlayerIsFallingIntoVoid();
            updateChunkPositionText();

            status.clearStatusIfExpired();
        }
    }

    void checkIfPlayerIsFallingIntoVoid() {
        float ypos = player.transform.position.y;
        if (ypos < -10) {
            eventProducer.producePlayerFallingIntoVoidEvent(player.transform.position);
            player.transform.position = new Vector3(0, 10, 0); 
            status.update("You fell into the void. You have been teleported to the surface.");
        }
    }

    void createChunkPositionText() {
        int x = 0;
        int y = Screen.height / 4;
        int fontSize = 20;
        chunkPositionText = new TextGameObject("Chunk: (" + x + ", " + y + ")", fontSize, x, y);
    }

    void updateChunkPositionText() {
        int x = landGenerator.getCurrentChunkX();
        int z = landGenerator.getCurrentChunkZ();
        chunkPositionText.updateText("Chunk: (" + x + ", " + z + ")");
    }
}