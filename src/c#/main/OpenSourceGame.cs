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

        private Environment environment;
        private WorldGenerator worldGenerator;
        private TickCounter tickCounter;
        private EventRepository eventRepository;
        private NationRepository nationRepository;
        private EventProducer eventProducer;
        private PawnBehaviorExecutor pawnBehaviorExecutor;
        private Player player;

        private Status status;
        // private TextGameObject chunkPositionText;
        private TextGameObject numGoldCoinsText;
        private TextGameObject numWoodText;
        private TextGameObject numStoneText;
        private TextGameObject numApplesText;
        private TextGameObject energyText;
        private TextGameObject mtpsText;

        public GameObject playerGameObject; // must be set in Unity Editor -- TODO: make this private and set it in the constructor (will require refactoring Player.cs)
        public bool runTests = false;

        // Initialization
        public void Start() {
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
            player = new Player(playerGameObject, gameConfig.getPlayerWalkSpeed(), gameConfig.getPlayerRunSpeed(), status);
            eventRepository = new EventRepository();
            eventProducer = new EventProducer(eventRepository);
            environment = new Environment(gameConfig.getChunkSize(), gameConfig.getLocationScale());
            worldGenerator = new WorldGenerator(environment, player, eventProducer);
            nationRepository = new NationRepository();
            pawnBehaviorExecutor = new PawnBehaviorExecutor(environment, nationRepository, eventProducer);

            // resources UI
            int resourcesX = -Screen.width / 4;
            int resourcesY = -Screen.height / 4;
            numGoldCoinsText = new TextGameObject("Gold Coins: 0", 20, resourcesX, resourcesY);
            numWoodText = new TextGameObject("Wood: 0", 20, resourcesX, resourcesY - 20);
            numStoneText = new TextGameObject("Stone: 0", 20, resourcesX, resourcesY - 40);
            numApplesText = new TextGameObject("Apples: 0", 20, resourcesX, resourcesY - 60);

            // other UI
            energyText = new TextGameObject("Energy: 100", 20, Screen.width / 4, -Screen.height / 4);
            // chunkPositionText = new TextGameObject("Chunk: (0, 0)", 20, 0, Screen.height / 4);

            // put in very top right corner
            mtpsText = new TextGameObject("0mtps", 20, Screen.width / 4, Screen.height / 4);

            environment.getChunk(0, 0).addEntity(player);
            environment.addEntityId(player.getId());
            status.update("Press N to create a nation.");
        }

        // Per-frame updates
        public void Update() {
            tickCounter.increment();

            handleCommands();

            player.update();
        }

        // Fixed updates
        public void FixedUpdate() {
            if (tickCounter.shouldUpdate()) {
                worldGenerator.update();
                checkIfPlayerIsFallingIntoVoid();
                // chunkPositionText.updateText("Chunk: (" + worldGenerator.getCurrentChunkX() + ", " + worldGenerator.getCurrentChunkZ() + ")");
                numGoldCoinsText.updateText("Gold Coins: " + player.getInventory().getNumItems(ItemType.GOLD_COIN));
                numWoodText.updateText("Wood: " + player.getInventory().getNumItems(ItemType.WOOD));
                numStoneText.updateText("Stone: " + player.getInventory().getNumItems(ItemType.STONE));
                numApplesText.updateText("Apples: " + player.getInventory().getNumItems(ItemType.APPLE));
                energyText.updateText("Energy: " + player.getEnergy());
                mtpsText.updateText(tickCounter.getMtps() + "mtps");
                status.clearStatusIfExpired();

                // list of positions to generate chunks at
                List<Vector3> positionsToGenerateChunksAt = new List<Vector3>();

                foreach (Chunk chunk in environment.getChunks()) {
                    foreach (Entity entity in chunk.getEntities()) {
                        if (entity.getType() == EntityType.PAWN) {
                            Pawn pawn = (Pawn)entity;

                            // compute and execute behavior
                            pawn.computeBehaviorType(environment, nationRepository);
                            pawnBehaviorExecutor.executeBehavior(pawn, pawn.getCurrentBehaviorType());

                            // update energy
                            if (pawn.getEnergy() < 90 && pawn.getInventory().getNumItems(ItemType.APPLE) > 0) {
                                pawn.getInventory().removeItem(ItemType.APPLE, 1);
                                pawn.setEnergy(pawn.getEnergy() + 10);
                            }
                            pawn.setEnergy(pawn.getEnergy() - pawn.getMetabolism());
                            pawn.setNameTag(pawn.getName() + " (" + pawn.getEnergy() + ")");

                            // create or join nation
                            if (pawn.getNationId() == null) {
                                createOrJoinNation(pawn);
                            }

                            // check if pawn is falling into void
                            float ypos = pawn.getGameObject().transform.position.y;
                            if (ypos < -10) {
                                Debug.Log("Entity " + pawn.getId() + " fell into void. Teleporting to spawn.");
                                pawn.getGameObject().transform.position = new Vector3(0, 10, 0);
                            }

                            // check if pawn is in a new chunk
                            Chunk retrievedChunk = environment.getChunkAtPosition(pawn.getGameObject().transform.position);
                            if (retrievedChunk == null) {
                                positionsToGenerateChunksAt.Add(pawn.getGameObject().transform.position);
                            }
                            
                            // check if pawn is dead
                            if (pawn.getEnergy() <= 0) {
                                eventProducer.producePawnDeathEvent(pawn.getGameObject().transform.position, pawn);
                                pawn.setEnergy(100);
                                pawn.getInventory().clear();
                                status.update(pawn.getName() + " has died.");
                                pawn.getGameObject().transform.position = new Vector3(Random.Range(-100, 100), 10, Random.Range(-100, 100));
                            }
                        }
                    }
                }

                foreach (Vector3 position in positionsToGenerateChunksAt) {
                    worldGenerator.generateChunkAtPosition(position);
                    worldGenerator.generateSurroundingChunksAtPosition(position);
                }
            }
            
            player.fixedUpdate();
            if (player.getEnergy() <= 0) {
                eventProducer.producePlayerDeathEvent(player.getGameObject().transform.position, player);
                player.setEnergy(100);
                player.getInventory().clear();
                status.update("You died.");
                player.getGameObject().transform.position = new Vector3(Random.Range(-100, 100), 10, Random.Range(-100, 100));
            }
            deleteEntitiesMarkedForDeletion();
        }

        private void handleCommands() {
            if (Input.GetKeyDown(KeyCode.N)) {
                NationCreateCommand command = new NationCreateCommand(nationRepository, eventProducer);
                command.execute(player);
            }
            else if (Input.GetKeyDown(KeyCode.J)) {
                NationJoinCommand command = new NationJoinCommand(nationRepository, eventProducer);
                command.execute(player);
            }
            else if (Input.GetKeyDown(KeyCode.T)) {
                TeleportAllPawnsCommand command = new TeleportAllPawnsCommand(environment);
                command.execute(player);
            }
            else if (Input.GetKeyDown(KeyCode.Numlock)) {
                ToggleAutoWalkCommand command = new ToggleAutoWalkCommand();
                command.execute(player);
            }
            else if (Input.GetKeyDown(KeyCode.L)) {
                NationLeaveCommand command = new NationLeaveCommand(nationRepository, eventProducer);
                command.execute(player);
            }
            else if (Input.GetKeyDown(KeyCode.E)) {
                InteractCommand command = new InteractCommand(environment, nationRepository, eventProducer);
                command.execute(player);
            }
            else if (Input.GetKeyDown(KeyCode.F1)) {
                SpawnPawnCommand command = new SpawnPawnCommand(environment, eventProducer);
                command.execute(player);
            }
        }

        private void checkIfPlayerIsFallingIntoVoid() {
            float ypos = player.getGameObject().transform.position.y;
            if (ypos < -10) {
                eventProducer.producePlayerFallingIntoVoidEvent(player.getGameObject().transform.position);
                player.getGameObject().transform.position = new Vector3(0, 10, 0); 
                status.update("You fell into the void. You have been teleported to the surface.");
            }
        }

        private void createOrJoinNation(Pawn pawn) {
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

        private void deleteEntitiesMarkedForDeletion() {
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