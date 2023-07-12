using System;
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
        public bool debugMode = false;

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
            pawnBehaviorCalculator = new PawnBehaviorCalculator(environment, entityRepository, nationRepository, gameConfig);
            pawnBehaviorExecutor = new PawnBehaviorExecutor(environment, nationRepository, eventProducer, entityRepository);
            entityRepository.addEntity(player);
            player.getStatus().update("Press " + KeyBindings.createNewNation + " to create a nation.");
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
            player.getStatus().clearStatusIfExpired();
            screenOverlay.update();

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
                    }
                    else {
                        pawn.setEnergy(pawn.getEnergy() - pawn.getMetabolism());
                    }

                    int ticksBetweenBehaviorCalculations = gameConfig.getTicksBetweenBehaviorCalculations();
                    if (tickCounter.getTick() % ticksBetweenBehaviorCalculations == 0) {
                        pawn.setCurrentBehaviorType(pawnBehaviorCalculator.computeBehaviorType(pawn));
                    }

                    int ticksBetweenBehaviorExecutions = gameConfig.getTicksBetweenBehaviorExecutions();
                    if (tickCounter.getTick() % ticksBetweenBehaviorExecutions == 0) {
                        pawnBehaviorExecutor.executeBehavior(pawn, pawn.getCurrentBehaviorType());
                    }

                    if (!pawn.isCurrentlyInSettlement()) {
                        string nameTagText = pawn.getName() + "\n" + pawn.getCurrentBehaviorDescription();
                        pawn.setNameTag(nameTagText);
                    }

                    if (!pawn.isCurrentlyInSettlement()) {
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
                                pawn.getGameObject().transform.position = new Vector3(UnityEngine.Random.Range(-100, 100), 100, UnityEngine.Random.Range(-100, 100));
                            }
                        }

                        // check if pawn is in a new chunk
                        Chunk retrievedChunk = environment.getChunkAtPosition(pawn.getGameObject().transform.position);
                        if (retrievedChunk == null) {
                            positionsToGenerateChunksAt.Add(pawn.getGameObject().transform.position);
                        }
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
                                newPosition = new Vector3(newPosition.x + UnityEngine.Random.Range(-20, 20), newPosition.y, newPosition.z + UnityEngine.Random.Range(-20, 20));
                                pawn.getGameObject().transform.position = newPosition;
                            }
                            else {
                                // pawn is not in a settlement, so respawn at spawn
                                pawn.getGameObject().transform.position = new Vector3(0, 10, 0);
                            }
                        }
                        else {
                            pawn.markForDeletion();

                            if (player.getNationId() != null) {
                                Nation nation = nationRepository.getNation(player.getNationId());

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
                                else if (nation.getRole(pawn.getId()) == NationRole.MERCHANT) {
                                    // remove stall ownership
                                    foreach (EntityId settlementId in nation.getSettlements()) {
                                        Settlement settlement = (Settlement)entityRepository.getEntity(settlementId);
                                        foreach (Stall stall in settlement.getMarket().getStalls()) {
                                            if (stall.getOwnerId() == pawn.getId()) {
                                                stall.setOwnerId(null);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else if (entity.getType() == EntityType.SAPLING) {
                    Sapling sapling = (Sapling)entity;
                    if (sapling.isGrown()) {
                        // replace with tree
                        AppleTree tree = new AppleTree(sapling.getGameObject().transform.position, 5);
                        entityRepository.addEntity(tree);
                        sapling.markForDeletion();
                    }
                }
                else if (entity.getType() == EntityType.SETTLEMENT) {
                    Settlement settlement = (Settlement)entity;

                    int totalTicks = tickCounter.getTotalTicks();
                    if (totalTicks % 1000 == 0) {
                        int numCoinsToGenerate = settlement.getMarket().getNumStalls();
                        if (numCoinsToGenerate > 0) {
                            settlement.getInventory().addItem(ItemType.COIN, numCoinsToGenerate);
                        }
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
                    newPosition = new Vector3(newPosition.x + UnityEngine.Random.Range(-20, 20), newPosition.y, newPosition.z + UnityEngine.Random.Range(-20, 20));
                    player.getGameObject().transform.position = newPosition;
                }
                else {
                    player.getGameObject().transform.position = new Vector3(UnityEngine.Random.Range(-100, 100), 10, UnityEngine.Random.Range(-100, 100));
                }
            }
            deleteEntitiesMarkedForDeletion();
        }

        // on gui
        public void OnGUI() {
            drawCommandButtons();
            
            if (debugMode) {
                GUI.color = Color.black;
                drawDebugInfo();
            }
        }

        private void drawCommandButtons() {
            int buttonHeight = Screen.height / 20;
            int buttonWidth = Screen.width / 10;
            int buttonSpacing = 10;
            int buttonX = 100;
            int buttonY = Screen.height - buttonHeight - 10;
            
            if (player.getNationId() == null) {
                // draw create nation
                if (GUI.Button(new Rect(buttonX, buttonY, buttonWidth, buttonHeight), "Create Nation")) {
                    NationCreateCommand command = new NationCreateCommand(nationRepository, eventProducer);
                    command.execute(player);
                }
                buttonX += buttonWidth + buttonSpacing;

                // draw join nation
                if (GUI.Button(new Rect(buttonX, buttonY, buttonWidth, buttonHeight), "Join Nation")) {
                    NationJoinCommand command = new NationJoinCommand(nationRepository, eventProducer);
                    command.execute(player);
                }
                buttonX += buttonWidth + buttonSpacing;
            }
            else {
                // draw leave nation
                if (GUI.Button(new Rect(buttonX, buttonY, buttonWidth, buttonHeight), "Leave Nation")) {
                    NationLeaveCommand command = new NationLeaveCommand(nationRepository, eventProducer, entityRepository);
                    command.execute(player);
                }
                buttonX += buttonWidth + buttonSpacing;

                // if leader and no settlements and enough resources, draw found settlement
                Nation nation = nationRepository.getNation(player.getNationId());
                if (player.getId() == nation.getLeaderId() && nation.getNumberOfSettlements() == 0 && player.getInventory().getNumItems(ItemType.WOOD) >= Settlement.WOOD_COST_TO_BUILD) {
                    // draw found settlement
                    if (GUI.Button(new Rect(buttonX, buttonY, buttonWidth, buttonHeight), "Found Settlement")) {
                        FoundSettlementCommand command = new FoundSettlementCommand(nationRepository, eventProducer, entityRepository, gameConfig);
                        command.execute(player);
                    }
                    buttonX += buttonWidth + buttonSpacing;
                }

                // teleport home
                if (player.getSettlementId() != null) {
                    Settlement settlement = (Settlement) entityRepository.getEntity(player.getSettlementId());
                    if (GUI.Button(new Rect(buttonX, buttonY, buttonWidth, buttonHeight), "Teleport Home")) {
                        TeleportHomeCommand command = new TeleportHomeCommand(entityRepository);
                        command.execute(player);
                    }
                    buttonX += buttonWidth + buttonSpacing;
                }

                // if leader and no stalls and enough resources, draw build stall
                if (player.getSettlementId() != null) {
                    Settlement settlement = (Settlement) entityRepository.getEntity(player.getSettlementId());
                    if (player.getId() == nation.getLeaderId() && settlement.getMarket().getNumStalls() < settlement.getMarket().getMaxNumStalls() && player.getInventory().getNumItems(ItemType.WOOD) >= Stall.WOOD_COST_TO_BUILD) {
                        if (GUI.Button(new Rect(buttonX, buttonY, buttonWidth, buttonHeight), "Build Stall")) {
                            BuildStallCommand command = new BuildStallCommand(nationRepository, entityRepository);
                            command.execute(player);
                        }
                        buttonX += buttonWidth + buttonSpacing;
                    }
                }

                // if serf, enough gold and stall for sale, draw purchase stall
                if (player.getSettlementId() != null) {
                    Settlement settlement = (Settlement) entityRepository.getEntity(player.getSettlementId());
                    if (nation.getRole(player.getId()) == NationRole.SERF && player.getInventory().getNumItems(ItemType.COIN) >= Stall.COIN_COST_TO_PURCHASE && settlement.getMarket().getNumStallsForSale() > 0) {
                        if (GUI.Button(new Rect(buttonX, buttonY, buttonWidth, buttonHeight), "Purchase Stall")) {
                            PurchaseStallCommand command = new PurchaseStallCommand(nationRepository, entityRepository);
                            command.execute(player);
                        }
                        buttonX += buttonWidth + buttonSpacing;
                    }
                }

                // if merchant, draw transfer items & collect profit
                if (player.getSettlementId() != null) {
                    Settlement settlement = (Settlement) entityRepository.getEntity(player.getSettlementId());
                    if (nation.getRole(player.getId()) == NationRole.MERCHANT) {
                        if (GUI.Button(new Rect(buttonX, buttonY, buttonWidth, buttonHeight), "Transfer Items to Stall")) {
                            TransferItemsToStallCommand command = new TransferItemsToStallCommand(nationRepository, entityRepository);
                            command.execute(player);
                        }
                        buttonX += buttonWidth + buttonSpacing;

                        if (GUI.Button(new Rect(buttonX, buttonY, buttonWidth, buttonHeight), "Collect Profit from Stall")) {
                            CollectProfitFromStallCommand command = new CollectProfitFromStallCommand(nationRepository, entityRepository);
                            command.execute(player);
                        }
                        buttonX += buttonWidth + buttonSpacing;
                    }
                }
            }

            // if saplings, plant sapling
            if (player.getInventory().getNumItems(ItemType.SAPLING) > 0) {
                if (GUI.Button(new Rect(buttonX, buttonY, buttonWidth, buttonHeight), "Plant Sapling")) {
                    PlantSaplingCommand command = new PlantSaplingCommand(entityRepository);
                    command.execute(player);
                }
                buttonX += buttonWidth + buttonSpacing;
            }
        }

        private void drawDebugInfo() {
            int yPos = 10;
            int width = 500;
            int height = 20;
                
            // fps                
            GUI.Label(new Rect(10, yPos, width, height), "FPS: " + (int)(1.0f / Time.smoothDeltaTime));
            yPos += 20;
            
            // mtps
            GUI.Label(new Rect(10, yPos, width, height), "MTPS: " + tickCounter.getMtps());
            yPos += 20;

            // tick
            GUI.Label(new Rect(10, yPos, width, height), "Total ticks: " + tickCounter.getTotalTicks());
            yPos += 20;

            // current chunk
            Chunk currentChunk = environment.getChunkAtPosition(player.getGameObject().transform.position);
            if (currentChunk != null) {
                GUI.Label(new Rect(10, yPos, width, height), "Chunk: " + currentChunk.getX() + ", " + currentChunk.getZ());
            }
            else {
                GUI.Label(new Rect(10, yPos, width, height), "Chunk: null");
            }
            yPos += 20;

            // number of entities
            GUI.Label(new Rect(10, yPos, width, height), "Entities: " + entityRepository.getNumberOfEntities());
            yPos += 20;

            // number of chunks
            GUI.Label(new Rect(10, yPos, width, height), "Chunks: " + environment.getNumberOfChunks());
            yPos += 20;

            // number of pawns
            int numPawns = entityRepository.getEntitiesOfType(EntityType.PAWN).Count;
            GUI.Label(new Rect(10, yPos, width, height), "Pawns: " + numPawns);
            yPos += 20;

            // number of nations
            GUI.Label(new Rect(10, yPos, width, height), "Nations: " + nationRepository.getNumberOfNations());
            yPos += 20;

            // number of settlements
            GUI.Label(new Rect(10, yPos, width, height), "Settlements: " + entityRepository.getEntitiesOfType(EntityType.SETTLEMENT).Count);
            yPos += 20;

            // number of trees
            GUI.Label(new Rect(10, yPos, width, height), "Trees: " + entityRepository.getEntitiesOfType(EntityType.TREE).Count);
            yPos += 20;

            // number of saplings
            GUI.Label(new Rect(10, yPos, width, height), "Saplings: " + entityRepository.getEntitiesOfType(EntityType.SAPLING).Count);
            yPos += 20;

            // number of rocks
            GUI.Label(new Rect(10, yPos, width, height), "Rocks: " + entityRepository.getEntitiesOfType(EntityType.ROCK).Count);
            yPos += 20;

            // events stored
            GUI.Label(new Rect(10, yPos, width, height), "Events: " + eventRepository.getTotalNumberOfEvents());
            yPos += 20;

            // total num stalls
            int totalNumStalls = 0;
            foreach (Settlement settlement in entityRepository.getEntitiesOfType(EntityType.SETTLEMENT)) {
                totalNumStalls += settlement.getMarket().getNumStalls();
            }
            GUI.Label(new Rect(10, yPos, width, height), "Stalls: " + totalNumStalls);
            yPos += 20;

            // pawns currently in settlement
            int numPawnsCurrentlyInSettlement = 0;
            foreach (Pawn pawn in entityRepository.getEntitiesOfType(EntityType.PAWN)) {
                if (pawn.isCurrentlyInSettlement()) {
                    numPawnsCurrentlyInSettlement++;
                }
            }
            GUI.Label(new Rect(10, yPos, width, height), "PCIS: " + numPawnsCurrentlyInSettlement + " / " + numPawns);
            yPos += 20;

            // nationless pawns
            int numNationlessPawns = 0;
            foreach (Pawn pawn in entityRepository.getEntitiesOfType(EntityType.PAWN)) {
                if (pawn.getNationId() == null) {
                    numNationlessPawns++;
                }
            }
            GUI.Label(new Rect(10, yPos, width, height), "Nationless: " + numNationlessPawns + " / " + numPawns);
            yPos += 20;

            // num leaders/merchants/serfs
            int numLeaders = 0;
            int numMerchants = 0;
            int numSerfs = 0;
            foreach (Pawn pawn in entityRepository.getEntitiesOfType(EntityType.PAWN)) {
                if (pawn.getNationId() == null) {
                    continue;
                }
                Nation nation = nationRepository.getNation(pawn.getNationId());
                NationRole role = nation.getRole(pawn.getId());
                if (role == NationRole.LEADER) {
                    numLeaders++;
                }
                else if (role == NationRole.MERCHANT) {
                    numMerchants++;
                }
                else if (role == NationRole.SERF) {
                    numSerfs++;
                }
            }
            GUI.Label(new Rect(10, yPos, width, height), "Leaders: " + numLeaders);
            yPos += 20;
            GUI.Label(new Rect(10, yPos, width, height), "Merchants: " + numMerchants);
            yPos += 20;
            GUI.Label(new Rect(10, yPos, width, height), "Serfs: " + numSerfs);
            yPos += 20;
        }

        private void handleCommands() {
            if (Input.GetKeyDown(KeyBindings.createNewNation)) {
                NationCreateCommand command = new NationCreateCommand(nationRepository, eventProducer);
                command.execute(player);
            }
            else if (Input.GetKeyDown(KeyBindings.joinNation)) {
                NationJoinCommand command = new NationJoinCommand(nationRepository, eventProducer);
                command.execute(player);
            }
            else if (Input.GetKeyDown(KeyBindings.teleportAllToPlayer)) {
                TeleportAllPawnsCommand command = new TeleportAllPawnsCommand(entityRepository);
                command.execute(player);
            }
            else if (Input.GetKeyDown(KeyBindings.toggleAutoWalk)) {
                ToggleAutoWalkCommand command = new ToggleAutoWalkCommand();
                command.execute(player);
            }
            else if (Input.GetKeyDown(KeyBindings.leaveNation)) {
                NationLeaveCommand command = new NationLeaveCommand(nationRepository, eventProducer, entityRepository);
                command.execute(player);
            }
            else if (Input.GetKeyDown(KeyBindings.interact)) {
                InteractCommand command = new InteractCommand(environment, nationRepository, eventProducer, entityRepository);
                command.execute(player);
            }
            else if (Input.GetKeyDown(KeyBindings.foundSettlement)) {
                FoundSettlementCommand command = new FoundSettlementCommand(nationRepository, eventProducer, entityRepository, gameConfig);
                command.execute(player);
            }
            else if (Input.GetKeyDown(KeyBindings.plantSapling)) {
                PlantSaplingCommand command = new PlantSaplingCommand(entityRepository);
                command.execute(player);
            }
            else if (Input.GetKeyDown(KeyBindings.teleportToHomeSettlement)) {
                TeleportHomeCommand command = new TeleportHomeCommand(entityRepository);
                command.execute(player);
            }
            else if (Input.GetKeyDown(KeyBindings.buildStall)) {
                BuildStallCommand command = new BuildStallCommand(nationRepository, entityRepository);
                command.execute(player);
            }
            else if (Input.GetKeyDown(KeyBindings.toggleDebugMode)) {
                debugMode = !debugMode;
            }
            else if (Input.GetKeyDown(KeyBindings.spawnNewPawn)) {
                if (!debugMode) {
                    player.getStatus().update("Debug mode must be enabled to spawn a pawn. Press " + KeyBindings.toggleDebugMode + " to enable debug mode.");
                    return;
                }
                SpawnPawnCommand command = new SpawnPawnCommand(eventProducer, entityRepository);
                command.execute(player);
            }
            else if (Input.GetKeyDown(KeyBindings.generateNearbyLand)) {
                if (!debugMode) {
                    player.getStatus().update("Debug mode must be enabled to generate nearby land. Press " + KeyBindings.toggleDebugMode + " to enable debug mode.");
                    return;
                }
                GenerateLandCommand command = new GenerateLandCommand(environment, worldGenerator);
                command.execute(player);
            }
            else if (Input.GetKeyDown(KeyBindings.spawnMoney)) {
                if (!debugMode) {
                    player.getStatus().update("Debug mode must be enabled to spawn money. Press " + KeyBindings.toggleDebugMode + " to enable debug mode.");
                    return;
                }
                SpawnMoneyCommand command = new SpawnMoneyCommand();
                command.execute(player);
            }
            else if (Input.GetKeyDown(KeyBindings.spawnWood)) {
                if (!debugMode) {
                    player.getStatus().update("Debug mode must be enabled to spawn wood. Press " + KeyBindings.toggleDebugMode + " to enable debug mode.");
                    return;
                }
                SpawnWoodCommand command = new SpawnWoodCommand();
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
                    newPosition = new Vector3(newPosition.x + UnityEngine.Random.Range(-20, 20), newPosition.y, newPosition.z + UnityEngine.Random.Range(-20, 20));
                    player.getGameObject().transform.position = newPosition;
                }
                else {
                    player.getGameObject().transform.position = new Vector3(UnityEngine.Random.Range(-100, 100), 10, UnityEngine.Random.Range(-100, 100));
                }
                player.getStatus().update("You fell into the void. You have been teleported to the surface.");
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