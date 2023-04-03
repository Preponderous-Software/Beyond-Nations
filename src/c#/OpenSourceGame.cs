using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace osg {

    /**
    * The OpenSourceGame class is the main class of the game.
    * It is the entry point of the game.
    */
    public class OpenSourceGame : MonoBehaviour {
        private GameConfig gameConfig;
        private EventRepository eventRepository;
        private EventProducer eventProducer;
        private Environment environment;
        private WorldGenerator worldGenerator;
        private TickCounter tickCounter;
        private TextGameObject chunkPositionText;
        private Status status;
        private NationRepository nationRepository;
        public Player player; // must be set in Unity Editor -- TODO: make this private and set it in the constructor (will require refactoring Player.cs)

        // Initialization
        void Start() {
            gameConfig = new GameConfig();
            eventRepository = new EventRepository();
            eventProducer = new EventProducer(eventRepository);
            environment = new Environment(gameConfig.getChunkSize(), gameConfig.getLocationScale());
            worldGenerator = new WorldGenerator(environment, player, eventProducer);
            tickCounter = new TickCounter(gameConfig.getUpdateInterval());
            chunkPositionText = new TextGameObject("Chunk: (0, 0)", 20, 0, Screen.height / 4);
            status = new Status(tickCounter, gameConfig.getStatusExpirationTicks());
            nationRepository = new NationRepository();

            status.update("Entered world.");
        }

        // Per-frame updates
        void Update() {
            tickCounter.increment();

            // if N pressed, create nation
            if (Input.GetKeyDown(KeyCode.N)) {
                if (nationRepository.getNation(player.getId()) != null) {
                    status.update("You already have a nation.");
                    return;
                }
                Nation nation = new Nation("Test Nation", player.getId());
                nationRepository.addNation(nation);
                eventProducer.produceNationCreationEvent(nation);
                status.update("Created nation " + nation.getName() + ".");
            }
        }

        // Fixed updates
        void FixedUpdate() {
            if (tickCounter.shouldUpdate()) {
                worldGenerator.update();
                checkIfPlayerIsFallingIntoVoid();
                chunkPositionText.updateText("Chunk: (" + worldGenerator.getCurrentChunkX() + ", " + worldGenerator.getCurrentChunkZ() + ")");
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
    }
}