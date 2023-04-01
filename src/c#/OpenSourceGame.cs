using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Environment;
using static Chunk;
using static Location;
using static LandGenerator;
using static CanvasFactory;

/**
* The OpenSourceGame class is the main class of the game.
* It is the entry point of the game.
*/
public class OpenSourceGame : MonoBehaviour {
    private EventRepository eventRepository;
    private EventProducer eventProducer;
    private Environment environment;
    private LandGenerator landGenerator;
    private TickCounter tickCounter;
    private CanvasFactory canvasFactory;
    public Player player; // must be set in Unity Editor -- TODO: make this private and set it in the constructor (will require refactoring Player.cs)

    public int chunkSize = 9;
    public int locationScale = 9;
    public int updateInterval = 10; // update every x ticks

    private GameObject chunkPositionCanvasObject;

    // Initialization
    void Start() {
        Debug.Log("Starting game...");

        eventRepository = new EventRepository();
        Debug.Log("Event repository created.");

        eventProducer = new EventProducer(eventRepository);
        Debug.Log("Event producer created.");

        environment = new Environment(chunkSize, locationScale);
        Debug.Log("Environment created.");

        landGenerator = new LandGenerator(environment, player, eventProducer);
        Debug.Log("Land generator created.");

        tickCounter = new TickCounter(updateInterval);
        Debug.Log("Tick counter created.");

        canvasFactory = new CanvasFactory();

        createChunkPositionCanvas();
    }

    // Per-frame updates
    void Update() {
        tickCounter.increment();

        if (tickCounter.shouldUpdate()) {
            landGenerator.update();
            checkIfPlayerIsFallingIntoVoid();
            updateChunkPositionCanvas();
        }
    }

    void checkIfPlayerIsFallingIntoVoid() {
        float ypos = player.transform.position.y;
        if (ypos < -10) {
            eventProducer.producePlayerFallingIntoVoidEvent(player.transform.position);
            player.transform.position = new Vector3(0, 10, 0); 
        }
    }

    void createChunkPositionCanvas() {
        int x = 0;
        int y = Screen.height / 4;
        int fontSize = 20;
        chunkPositionCanvasObject = canvasFactory.createCanvasObject("Chunk: (0, 0)", fontSize, x, y);
    }

    void updateChunkPositionCanvas() {
        int x = landGenerator.getCurrentChunkX();
        int z = landGenerator.getCurrentChunkZ();
        updateText(chunkPositionCanvasObject, "Chunk: (" + x + ", " + z + ")");
    }

    public void updateText(GameObject canvasObject, string text) {
        Text textComponent = canvasObject.GetComponentInChildren<Text>();
        textComponent.text = text;
    }
}