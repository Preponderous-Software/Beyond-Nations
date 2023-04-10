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
        private Player player;
        private TextGameObject numWoodText;
        private TextGameObject numStoneText;

        public GameObject playerGameObject; // must be set in Unity Editor -- TODO: make this private and set it in the constructor (will require refactoring Player.cs)
        public bool runTests = false;

        // Initialization
        void Start() {
            if (runTests) {
                Debug.Log("Running tests...");
                osgtests.Tests.runTests();
                Debug.Log("Tests complete. Pausing.");
                Debug.Break();
            }
            else {
                Debug.Log("Not running tests. Set `runTests` to true to run tests.");
            }

            gameConfig = new GameConfig();
            tickCounter = new TickCounter(gameConfig.getUpdateInterval());
            status = new Status(tickCounter, gameConfig.getStatusExpirationTicks());
            player = new Player(playerGameObject, gameConfig.getPlayerWalkSpeed(), gameConfig.getPlayerRunSpeed(), new ChunkId(), status);
            eventRepository = new EventRepository();
            eventProducer = new EventProducer(eventRepository);
            environment = new Environment(gameConfig.getChunkSize(), gameConfig.getLocationScale());
            worldGenerator = new WorldGenerator(environment, player, eventProducer);
            chunkPositionText = new TextGameObject("Chunk: (0, 0)", 20, 0, Screen.height / 4);
            nationRepository = new NationRepository();
            numWoodText = new TextGameObject("Wood: 0", 20, -Screen.width / 4, 0);
            numStoneText = new TextGameObject("Stone: 0", 20, Screen.width / 4, 0);

            environment.getChunk(0, 0).addEntity(player);
            environment.addEntityId(player.getId());
            status.update("Press N to create a nation.");
        }

        // Per-frame updates
        void Update() {
            tickCounter.increment();

            handleCommands();

            player.update();
        }

        // Fixed updates
        void FixedUpdate() {
            if (tickCounter.shouldUpdate()) {
                worldGenerator.update();
                checkIfPlayerIsFallingIntoVoid();
                chunkPositionText.updateText("Chunk: (" + worldGenerator.getCurrentChunkX() + ", " + worldGenerator.getCurrentChunkZ() + ")");
                numWoodText.updateText("Wood: " + player.getInventory().getNumWood());
                numStoneText.updateText("Stone: " + player.getInventory().getNumStone());
                status.clearStatusIfExpired();

                foreach (Chunk chunk in environment.getChunks()) {
                    foreach (Entity entity in chunk.getEntities()) {
                        if (entity.getType() == EntityType.LIVING) {
                            Pawn pawn = (Pawn)entity;

                            pawn.fixedUpdate(environment, nationRepository);

                            if (pawn.getNationId() == null) {
                                createOrJoinNation(pawn);
                            }

                            float ypos = pawn.getGameObject().transform.position.y;
                            if (ypos < -10) {
                                Debug.Log("Entity " + pawn.getId() + " fell into void. Teleporting to spawn.");
                                pawn.getGameObject().transform.position = new Vector3(0, 10, 0);
                            }
                        }
                    }
                }
            }
            
            player.fixedUpdate();
            deleteEntitiesMarkedForDeletion();
        }

        void handleCommands() {
            // if N pressed, create nation
            if (Input.GetKeyDown(KeyCode.N)) {
                if (player.getNationId() != null) {
                    Nation playerNation = nationRepository.getNation(player.getNationId());
                    if (playerNation.getLeaderId() == player.getId()) {
                        status.update("You are already the leader of " + playerNation.getName() + ".");
                    }
                    else {
                        status.update("You are already a member of " + playerNation.getName() + ".");
                    }
                    return;
                }
                Nation nation = new Nation(NationNameGenerator.generate(), player.getId());
                nationRepository.addNation(nation);
                player.setNationId(nation.getId());
                player.setColor(nation.getColor());
                eventProducer.produceNationCreationEvent(nation);
                status.update("Created nation " + nation.getName() + ".");
            }

            // if J pressed, join nation
            if (Input.GetKeyDown(KeyCode.J)) {
                if (player.getNationId() != null) {
                    Nation playerNation = nationRepository.getNation(player.getNationId());
                    if (playerNation.getLeaderId() == player.getId()) {
                        status.update("You are already the leader of " + playerNation.getName() + ".");
                    }
                    else {
                        status.update("You are already a member of " + playerNation.getName() + ".");
                    }
                    return;
                }
                Nation nation = nationRepository.getRandomNation();
                if (nation == null) {
                    status.update("There are no nations to join.");
                    return;
                }
                nation.addMember(player.getId());
                player.setNationId(nation.getId());
                player.setColor(nation.getColor());
                eventProducer.produceNationJoinEvent(nation, player.getId());
                status.update("You joined nation " + nation.getName() + ". Members: " + nation.getNumberOfMembers() + ".");
            }

            // if T pressed, teleport all living entities to player
            if (Input.GetKeyDown(KeyCode.T)) {
                foreach (Chunk chunk in environment.getChunks()) {
                    foreach (Entity entity in chunk.getEntities()) {
                        if (entity.getType() == EntityType.LIVING) {
                            Pawn pawn = (Pawn)entity;
                            pawn.getGameObject().transform.position = player.getGameObject().transform.position;
                        }
                    }
                }
            }
        }

        void checkIfPlayerIsFallingIntoVoid() {
            float ypos = player.getGameObject().transform.position.y;
            if (ypos < -10) {
                eventProducer.producePlayerFallingIntoVoidEvent(player.getGameObject().transform.position);
                player.getGameObject().transform.position = new Vector3(0, 10, 0); 
                status.update("You fell into the void. You have been teleported to the surface.");
            }
        }

        void createOrJoinNation(Pawn pawn) {
            // if less than 4 nations, create a new nation
            if (nationRepository.getNumberOfNations() < 4) {
                Nation nation = new Nation(NationNameGenerator.generate(), pawn.getId());
                nationRepository.addNation(nation);
                pawn.setNationId(nation.getId());
                pawn.setColor(nation.getColor());
                eventProducer.produceNationCreationEvent(nation);
                status.update(pawn.getName() + " created nation " + nation.getName() + ".");
            }
            else {
                // join a random nation
                Nation nation = nationRepository.getRandomNation();
                nation.addMember(pawn.getId());
                pawn.setNationId(nation.getId());
                pawn.setColor(nation.getColor());
                eventProducer.produceNationJoinEvent(nation, pawn.getId());
                status.update(pawn.getName() + " joined nation " + nation.getName() + ". Members: " + nation.getNumberOfMembers() + ".");
            }
        }

        void deleteEntitiesMarkedForDeletion() {
            List<Entity> entitiesToDelete = new List<Entity>();
            foreach (Chunk chunk in environment.getChunks()) {
                foreach (Entity entity in chunk.getEntities()) {
                    if (entity.isMarkedForDeletion()) {
                        entitiesToDelete.Add(entity);
                    }
                }
            }
            foreach (Entity entity in entitiesToDelete) {
                entity.destroyGameObject();
                environment.removeEntity(entity);
            }
        }
    }
}