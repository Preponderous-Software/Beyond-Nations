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
        private ScreenOverlay screenOverlay;
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
            player = new Player(gameConfig.getPlayerWalkSpeed(), gameConfig.getPlayerRunSpeed(), tickCounter, gameConfig.getStatusExpirationTicks());
            screenOverlay = new ScreenOverlay(player, tickCounter);
            eventRepository = new EventRepository();
            eventProducer = new EventProducer(eventRepository);
            entityRepository = new EntityRepository();
            environment = new Environment(gameConfig.getChunkSize(), gameConfig.getLocationScale(), entityRepository);
            worldGenerator = new WorldGenerator(environment, player, eventProducer, entityRepository);
            nationRepository = new NationRepository();
            pawnBehaviorCalculator = new PawnBehaviorCalculator(environment, entityRepository, nationRepository);
            pawnBehaviorExecutor = new PawnBehaviorExecutor(environment, nationRepository, eventProducer, entityRepository);
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
            screenOverlay.update();
            player.getStatus().clearStatusIfExpired();

            // list of positions to generate chunks at
            List<Vector3> positionsToGenerateChunksAt = new List<Vector3>();

            foreach (Entity entity in entityRepository.getEntities()) {
                if (entity.getType() == EntityType.PAWN) {
                    Pawn pawn = (Pawn) entity;

                    // update energy
                    if (pawn.getEnergy() < 90 && pawn.getInventory().getNumItems(ItemType.APPLE) > 0) {
                        pawn.getInventory().removeItem(ItemType.APPLE, 1);
                        pawn.setEnergy(pawn.getEnergy() + 10);
                    }

                    if (pawn.isCurrentlyInSettlement()) {
                        pawn.setEnergy(pawn.getEnergy() - pawn.getMetabolism() / 10f);
                        if (pawn.getEnergy() < 50) {
                            pawn.setCurrentlyInSettlement(false);
                            Settlement settlement = entityRepository.getEntity(pawn.getHomeSettlementId()) as Settlement;
                            settlement.removeCurrentlyPresentEntity(pawn.getId());
                            pawn.createGameObject(settlement.getGameObject().transform.position + new Vector3(Random.Range(-20, 20), 0, Random.Range(-20, 20)));
                            pawn.setColor(settlement.getColor());
                        }
                        continue;
                    }
                    else {
                        pawn.setEnergy(pawn.getEnergy() - pawn.getMetabolism());
                    }

                    int ticksBetweenBehaviorCalculations = gameConfig.getTicksBetweenBehaviorCalculations();
                    BehaviorType currentBehavior = BehaviorType.NONE;
                    if (tickCounter.getTick() % ticksBetweenBehaviorCalculations == 0) {
                        currentBehavior = pawnBehaviorCalculator.computeBehaviorType(pawn);
                        pawn.setCurrentBehaviorType(currentBehavior);
                    }

                    int ticksBetweenBehaviorExecutions = gameConfig.getTicksBetweenBehaviorExecutions();
                    if (tickCounter.getTick() % ticksBetweenBehaviorExecutions == 0) {
                        pawnBehaviorExecutor.executeBehavior(pawn, currentBehavior);
                    }

                    string nameTagText = pawn.getName() + "\n" + pawn.getCurrentBehaviorDescription();
                    pawn.setNameTag(nameTagText);

                    // create or join nation
                    if (pawn.getNationId() == null) {
                        createOrJoinNation(pawn);
                    }
                    Nation nation = nationRepository.getNation(pawn.getNationId());

                    // join settlement if not already in one
                    if (pawn.getHomeSettlementId() == null) {
                        // choose random nation settlement
                        int numSettlements = nation.getSettlements().Count;
                        if (numSettlements != 0) {
                            int randomSettlementIndex = Random.Range(0, numSettlements);
                            EntityId randomSettlementId = nation.getSettlements()[randomSettlementIndex];
                            pawn.setHomeSettlementId(randomSettlementId);
                        }
                    }

                    // check if pawn is falling into void
                    float ypos = pawn.getGameObject().transform.position.y;
                    if (ypos < -10) {
                        Debug.Log("Entity " + pawn.getId() + " fell into void. Teleporting.");
                        if (pawn.getHomeSettlementId() != null) {
                            // pawn is in a settlement, so respawn at settlement
                            Settlement settlement = (Settlement)entityRepository.getEntity(pawn.getHomeSettlementId());
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
                        player.getStatus().update(pawn.getName() + " has died.");
                        if (gameConfig.getRespawnPawns()) {
                            pawn.setEnergy(100);
                            if (gameConfig.getKeepInventoryOnDeath() == false) {
                                pawn.getInventory().clear();
                            }
                            if (pawn.getHomeSettlementId() != null) {
                                // pawn is in a settlement, so respawn at settlement
                                Settlement settlement = (Settlement)entityRepository.getEntity(pawn.getHomeSettlementId());
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
                TeleportAllPawnsCommand command = new TeleportAllPawnsCommand(entityRepository);
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
                InteractCommand command = new InteractCommand(environment, nationRepository, eventProducer, entityRepository);
                command.execute(player);
            }
            else if (Input.GetKeyDown(KeyCode.F1)) {
                SpawnPawnCommand command = new SpawnPawnCommand(eventProducer, entityRepository);
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
                FoundSettlementCommand command = new FoundSettlementCommand(nationRepository, eventProducer, entityRepository);
                command.execute(player);
            }
            else if (Input.GetKeyDown(KeyCode.P)) {
                PlantSaplingCommand command = new PlantSaplingCommand(entityRepository);
                command.execute(player);
            }
            else if (Input.GetKeyDown(KeyCode.H)) {
                TeleportHomeCommand command = new TeleportHomeCommand(entityRepository);
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