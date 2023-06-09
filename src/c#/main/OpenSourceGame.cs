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
        private EntityRepository entityRepository;
        private EventProducer eventProducer;
        private PawnBehaviorCalculator pawnBehaviorCalculator;
        private PawnBehaviorExecutor pawnBehaviorExecutor;
        private Player player; // TODO: move to player repository
        private TextGameObject numGoldCoinsText;
        private TextGameObject numWoodText;
        private TextGameObject numStoneText;
        private TextGameObject numApplesText;
        private TextGameObject numSaplingsText;
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
            tickCounter = new TickCounter();
            player = new Player(playerGameObject, gameConfig.getPlayerWalkSpeed(), gameConfig.getPlayerRunSpeed(), tickCounter, gameConfig.getStatusExpirationTicks());
            eventRepository = new EventRepository();
            eventProducer = new EventProducer(eventRepository);
            entityRepository = new EntityRepository();
            environment = new Environment(gameConfig.getChunkSize(), gameConfig.getLocationScale(), entityRepository);
            worldGenerator = new WorldGenerator(environment, player, eventProducer, entityRepository);
            nationRepository = new NationRepository();
            pawnBehaviorCalculator = new PawnBehaviorCalculator(environment, entityRepository, nationRepository);
            pawnBehaviorExecutor = new PawnBehaviorExecutor(environment, nationRepository, eventProducer, entityRepository);

            // resources UI
            int resourcesX = -Screen.width / 4;
            int resourcesY = -Screen.height / 4;
            numGoldCoinsText = new TextGameObject("Gold Coins: 0", 20, resourcesX, resourcesY);
            numWoodText = new TextGameObject("Wood: 0", 20, resourcesX, resourcesY - 20);
            numStoneText = new TextGameObject("Stone: 0", 20, resourcesX, resourcesY - 40);
            numApplesText = new TextGameObject("Apples: 0", 20, resourcesX, resourcesY - 60);
            numSaplingsText = new TextGameObject("Saplings: 0", 20, resourcesX, resourcesY - 80);

            // other UI
            energyText = new TextGameObject("Energy: 100", 20, Screen.width / 4, -Screen.height / 4);

            // put in very top right corner
            mtpsText = new TextGameObject("0mtps", 20, Screen.width / 4, Screen.height / 4);

            entityRepository.addEntity(player);
            player.getStatus().update("Press N to create a nation.");
        }

        // Per-frame updates
        public void Update() {
            handleCommands();
            player.update();
        }

        // Fixed updates
        public void FixedUpdate() {
            tickCounter.increment();
            worldGenerator.update();
            checkIfPlayerIsFallingIntoVoid();
            numGoldCoinsText.updateText("Gold Coins: " + player.getInventory().getNumItems(ItemType.GOLD_COIN));
            numWoodText.updateText("Wood: " + player.getInventory().getNumItems(ItemType.WOOD));
            numStoneText.updateText("Stone: " + player.getInventory().getNumItems(ItemType.STONE));
            numApplesText.updateText("Apples: " + player.getInventory().getNumItems(ItemType.APPLE));
            numSaplingsText.updateText("Saplings: " + player.getInventory().getNumItems(ItemType.SAPLING));
            energyText.updateText("Energy: " + player.getEnergy());
            mtpsText.updateText(tickCounter.getMtps() + "mtps");
            player.getStatus().clearStatusIfExpired();

            // list of positions to generate chunks at
            List<Vector3> positionsToGenerateChunksAt = new List<Vector3>();

            foreach (Entity entity in entityRepository.getEntities()) {
                if (entity.getType() == EntityType.PAWN) {
                    Pawn pawn = (Pawn)entity;

                    // compute and execute behavior
                    BehaviorType currentBehavior = pawnBehaviorCalculator.computeBehaviorType(pawn);
                    pawnBehaviorExecutor.executeBehavior(pawn, currentBehavior);

                    // update energy
                    if (pawn.getEnergy() < 90 && pawn.getInventory().getNumItems(ItemType.APPLE) > 0) {
                        pawn.getInventory().removeItem(ItemType.APPLE, 1);
                        pawn.setEnergy(pawn.getEnergy() + 10);
                    }
                    pawn.setEnergy(pawn.getEnergy() - pawn.getMetabolism());

                    // set nametag to show energy and inventory contents
                    string nameTagText = pawn.getName() + " (" + (int)pawn.getEnergy() + ")";
                    // show wood, stone, apples and gold coins
                    if (pawn.getInventory().getNumItems(ItemType.WOOD) > 0) {
                        nameTagText += " W:" + pawn.getInventory().getNumItems(ItemType.WOOD);
                    }
                    if (pawn.getInventory().getNumItems(ItemType.STONE) > 0) {
                        nameTagText += " S:" + pawn.getInventory().getNumItems(ItemType.STONE);
                    }
                    if (pawn.getInventory().getNumItems(ItemType.APPLE) > 0) {
                        nameTagText += " A:" + pawn.getInventory().getNumItems(ItemType.APPLE);
                    }
                    if (pawn.getInventory().getNumItems(ItemType.GOLD_COIN) > 0) {
                        nameTagText += " G:" + pawn.getInventory().getNumItems(ItemType.GOLD_COIN);
                    }
                    pawn.setNameTag(nameTagText);

                    // create or join nation
                    if (pawn.getNationId() == null) {
                        createOrJoinNation(pawn);
                    }
                    Nation nation = nationRepository.getNation(pawn.getNationId());

                    // join settlement if not already in one
                    if (pawn.getSettlementId() == null) {
                        // choose random nation settlement
                        int numSettlements = nation.getSettlements().Count;
                        if (numSettlements != 0) {
                            int randomSettlementIndex = Random.Range(0, numSettlements);
                            EntityId randomSettlementId = nation.getSettlements()[randomSettlementIndex];
                            pawn.setSettlementId(randomSettlementId);
                        }
                    }

                    // check if pawn is falling into void
                    float ypos = pawn.getGameObject().transform.position.y;
                    if (ypos < -10) {
                        Debug.Log("Entity " + pawn.getId() + " fell into void. Teleporting.");
                        if (pawn.getSettlementId() != null) {
                            // pawn is in a settlement, so respawn at settlement
                            Settlement settlement = (Settlement)entityRepository.getEntity(pawn.getSettlementId());
                            Vector3 newPosition = settlement.getGameObject().transform.position;
                            newPosition = new Vector3(newPosition.x, newPosition.y + 1, newPosition.z);
                            pawn.getGameObject().transform.position = newPosition;
                        } else {
                            // pawn is not in a settlement, so respawn at spawn
                            pawn.getGameObject().transform.position = new Vector3(Random.Range(-100, 100), 100, Random.Range(-100, 100));
                        }
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
                        if (gameConfig.getKeepInventoryOnDeath() == false) {
                            pawn.getInventory().clear();
                        }
                        player.getStatus().update(pawn.getName() + " has died.");
                        if (gameConfig.getRespawnPawns()) {
                            if (pawn.getSettlementId() != null) {
                                // pawn is in a settlement, so respawn at settlement
                                Settlement settlement = (Settlement)entityRepository.getEntity(pawn.getSettlementId());
                                Vector3 newPosition = settlement.getGameObject().transform.position;
                                newPosition = new Vector3(newPosition.x + Random.Range(-20, 20), newPosition.y, newPosition.z + Random.Range(-20, 20));
                                pawn.getGameObject().transform.position = newPosition;
                            }
                            else {
                                // pawn is not in a settlement, so respawn at spawn
                                pawn.getGameObject().transform.position = new Vector3(0, 10, 0);
                            }
                        }
                        else {
                            pawn.markForDeletion();
                            nation.removeMember(pawn.getId());
                            if (nation.getLeaderId() == pawn.getId()) {
                                // transfer leadership to another pawn
                                if (nation.getNumberOfMembers() > 0) {
                                    nation.setLeaderId(nation.getOldestMemberId());
                                    if (pawn.getType() == EntityType.PAWN) {
                                        Pawn newLeader = (Pawn) entityRepository.getEntity(nation.getLeaderId());
                                        player.getStatus().update(newLeader.getName() + " is now the leader of " + nation.getName() + ".");
                                    }
                                    else if (pawn.getType() == EntityType.PLAYER) {
                                        Player newLeader = (Player) entityRepository.getEntity(nation.getLeaderId());
                                        player.getStatus().update("You are now the leader of " + nation.getName() + ".");
                                    }
                                    else {
                                        Debug.Log("ERROR: Oldest member of nation " + nation.getName() + " is not a pawn or player.");
                                    }
                                    
                                }
                                else {
                                    nationRepository.removeNation(nation);

                                    // remove settlements
                                    foreach (EntityId settlementId in nation.getSettlements()) {
                                        Settlement settlement = (Settlement) entityRepository.getEntity(settlementId);
                                        settlement.markForDeletion();
                                    }

                                    // clear settlements
                                    nation.getSettlements().Clear();

                                    player.getStatus().update(nation.getName() + " has been disbanded.");
                                }
                            }
                        }
                    }
                }
                else if (entity.getType() == EntityType.SAPLING) {
                    Sapling sapling = (Sapling)entity;
                    if (sapling.isGrown()) {
                        // replace with tree
                        TreeEntity tree = new TreeEntity(sapling.getGameObject().transform.position, 5);
                        entityRepository.addEntity(tree);
                        sapling.markForDeletion();
                    }
                }
            }

            foreach (Vector3 position in positionsToGenerateChunksAt) {
                worldGenerator.generateChunkAtPosition(position);
                worldGenerator.generateSurroundingChunksAtPosition(position);
            }
            
            player.fixedUpdate();
            if (player.getEnergy() <= 0) {
                eventProducer.producePlayerDeathEvent(player.getGameObject().transform.position, player);
                player.setEnergy(100);
                if (gameConfig.getKeepInventoryOnDeath() == false) {
                    player.getInventory().clear();
                }
                player.getStatus().update("You died.");
                
                if (player.getSettlementId() != null) {
                    // player is in a settlement, so respawn at settlement
                    Settlement settlement = (Settlement)entityRepository.getEntity(player.getSettlementId());
                    Vector3 newPosition = settlement.getGameObject().transform.position;
                    newPosition = new Vector3(newPosition.x + Random.Range(-20, 20), newPosition.y, newPosition.z + Random.Range(-20, 20));
                    player.getGameObject().transform.position = newPosition;
                }
                else {
                    player.getGameObject().transform.position = new Vector3(Random.Range(-100, 100), 10, Random.Range(-100, 100));
                }
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
                TeleportAllPawnsCommand command = new TeleportAllPawnsCommand(environment, entityRepository);
                command.execute(player);
            }
            else if (Input.GetKeyDown(KeyCode.Numlock)) {
                ToggleAutoWalkCommand command = new ToggleAutoWalkCommand();
                command.execute(player);
            }
            else if (Input.GetKeyDown(KeyCode.L)) {
                NationLeaveCommand command = new NationLeaveCommand(nationRepository, eventProducer, entityRepository);
                command.execute(player);
            }
            else if (Input.GetKeyDown(KeyCode.E)) {
                InteractCommand command = new InteractCommand(environment, nationRepository, eventProducer);
                command.execute(player);
            }
            else if (Input.GetKeyDown(KeyCode.F1)) {
                SpawnPawnCommand command = new SpawnPawnCommand(environment, eventProducer, entityRepository);
                command.execute(player);
            }
            else if (Input.GetKeyDown(KeyCode.F2)) {
                GenerateLandCommand command = new GenerateLandCommand(environment, worldGenerator);
                command.execute(player);
            }
            else if (Input.GetKeyDown(KeyCode.F3)) {
                SpawnMoneyCommand command = new SpawnMoneyCommand();
                command.execute(player);
            }
            else if (Input.GetKeyDown(KeyCode.F)) {
                FoundSettlementCommand command = new FoundSettlementCommand(environment, nationRepository, eventProducer, entityRepository);
                command.execute(player);
            }
            else if (Input.GetKeyDown(KeyCode.P)) {
                PlantSaplingCommand command = new PlantSaplingCommand(entityRepository);
                command.execute(player);
            }
        }

        private void checkIfPlayerIsFallingIntoVoid() {
            float ypos = player.getGameObject().transform.position.y;
            if (ypos < -10) {
                eventProducer.producePlayerFallingIntoVoidEvent(player.getGameObject().transform.position);
                if (player.getSettlementId() != null) {
                    // player is in a settlement, so respawn at settlement
                    Settlement settlement = (Settlement)entityRepository.getEntity(player.getSettlementId());
                    Vector3 newPosition = settlement.getGameObject().transform.position;
                    newPosition = new Vector3(newPosition.x + Random.Range(-20, 20), newPosition.y, newPosition.z + Random.Range(-20, 20));
                    player.getGameObject().transform.position = newPosition;
                }
                else {
                    player.getGameObject().transform.position = new Vector3(Random.Range(-100, 100), 10, Random.Range(-100, 100));
                }
                player.getStatus().update("You fell into the void. You have been teleported to the surface.");
            }
        }

        private void createOrJoinNation(Pawn pawn) {
            if (nationRepository.getNumberOfNations() <= gameConfig.getNumStartingNations()) {
                // create a new nation
                Nation nation = new Nation(NationNameGenerator.generate(), pawn.getId());
                nationRepository.addNation(nation);
                pawn.setNationId(nation.getId());
                pawn.setColor(nation.getColor());
                eventProducer.produceNationCreationEvent(nation);
                player.getStatus().update(pawn.getName() + " created nation " + nation.getName() + ".");
            }
            else {
                // join a random nation
                Nation nation = nationRepository.getRandomNation();
                nation.addMember(pawn.getId());
                pawn.setNationId(nation.getId());
                pawn.setColor(nation.getColor());
                eventProducer.produceNationJoinEvent(nation, pawn.getId());
                player.getStatus().update(pawn.getName() + " joined nation " + nation.getName() + ". Members: " + nation.getNumberOfMembers() + ".");
            }
        }

        private void deleteEntitiesMarkedForDeletion() {
            List<Entity> entitiesToDelete = new List<Entity>();
            foreach (Entity entity in entityRepository.getEntities()) {
                if (entity.isMarkedForDeletion()) {
                    entitiesToDelete.Add(entity);
                }
            }
            foreach (Entity entity in entitiesToDelete) {
                entity.destroyGameObject();
                entityRepository.removeEntity(entity);
            }
        }
    }
}