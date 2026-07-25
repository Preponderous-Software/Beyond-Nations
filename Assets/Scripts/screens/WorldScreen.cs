using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace beyondnations {

    /**
    * The world screen of the game.
    */
    public class WorldScreen {
        private GameConfig gameConfig;
        private bool debugMode = false;

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
        private LagPreventer lagPreventer;

        private int numPawnDeaths = 0;
        private int numPlayerDeaths = 0;
        private bool inventoryInfoBoxEnabled = true;

        public WorldScreen(GameConfig gameConfig, bool debugMode) {
            this.gameConfig = gameConfig;
            this.debugMode = debugMode;

            tickCounter = new TickCounter();
            player = new Player(gameConfig.getPlayerWalkSpeed(), gameConfig.getPlayerRunSpeed(), tickCounter, gameConfig.getStatusExpirationTicks(), gameConfig.getRenderDistance());
            eventRepository = new EventRepository();
            eventProducer = new EventProducer(eventRepository);
            entityRepository = new EntityRepository();
            environment = new Environment(gameConfig.getChunkSize(), gameConfig.getLocationScale(), entityRepository);
            worldGenerator = new WorldGenerator(environment, player, eventProducer, entityRepository, gameConfig);
            nationRepository = new NationRepository();
            pawnBehaviorCalculator = new PawnBehaviorCalculator(environment, entityRepository, nationRepository, gameConfig, tickCounter);
            pawnBehaviorExecutor = new PawnBehaviorExecutor(environment, nationRepository, eventProducer, entityRepository);
            entityRepository.addEntity(player);
            lagPreventer = new LagPreventer(gameConfig, tickCounter, entityRepository, environment);
            player.getStatus().update("Press " + KeyBindings.createNewNation + " to create a nation.");
        }

        public void Update() {
            handleCommands();
            player.update();
        }

        public void FixedUpdate() {
            tickCounter.increment();
            worldGenerator.update();
            checkIfPlayerIsFallingIntoVoid();
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
                        pawn.setEnergy(pawn.getEnergy() - pawn.getMetabolism() * gameConfig.getSettlementMetabolismMultiplier());
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
                            EntityId homeSettlementId = pawn.getHomeSettlementId();
                            if (homeSettlementId != null) {
                                // pawn has home settlement, so respawn at settlement
                                Settlement settlement = (Settlement)entityRepository.getEntity(homeSettlementId);
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
                        eventProducer.producePawnDeathEvent(pawn);
                        numPawnDeaths++;
                        player.getStatus().update(pawn.getName() + " has died.");
                        if (gameConfig.getRespawnPawns()) {
                            pawn.setEnergy(100);
                            if (gameConfig.getKeepInventoryOnDeath() == false) {
                                pawn.getInventory().clear();
                            }

                            if (!pawn.isCurrentlyInSettlement()) {
                                // teleport pawn's game object
                                EntityId homeSettlementId = pawn.getHomeSettlementId();
                                if (homeSettlementId != null) {
                                    // pawn has home settlement, so respawn at settlement
                                    Settlement settlement = (Settlement)entityRepository.getEntity(homeSettlementId);
                                    Vector3 newPosition = settlement.getGameObject().transform.position;
                                    newPosition = new Vector3(newPosition.x + UnityEngine.Random.Range(-20, 20), newPosition.y, newPosition.z + UnityEngine.Random.Range(-20, 20));
                                    pawn.getGameObject().transform.position = newPosition;
                                }
                                else {
                                    // pawn is not in a settlement, so respawn at spawn
                                    pawn.getGameObject().transform.position = new Vector3(0, 10, 0);
                                }
                            }                            
                        }
                        else {
                            pawn.markForDeletion();

                            if (pawn.getNationId() != null) {
                                Nation nation = nationRepository.getNation(pawn.getNationId());
                                NationRole role = nation.getRole(pawn.getId());
                                if (role == NationRole.LEADER) {
                                    // transfer leadership to another pawn
                                    if (nation.getNumberOfMembers() > 0) {
                                        nation.setLeaderId(nation.getOldestMemberId());
                                        if (pawn.getType() == EntityType.PAWN) {
                                            Pawn newLeader = (Pawn) entityRepository.getEntity(nation.getLeaderId());
                                            player.getStatus().update(newLeader.getName() + " is now the leader of " + nation.getName() + ".");
                                            nation.setRole(newLeader.getId(), NationRole.LEADER);
                                        }
                                        else if (pawn.getType() == EntityType.PLAYER) {
                                            Player newLeader = (Player) entityRepository.getEntity(nation.getLeaderId());
                                            player.getStatus().update("You are now the leader of " + nation.getName() + ".");
                                            nation.setRole(newLeader.getId(), NationRole.LEADER);
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
                                else if (role == NationRole.MERCHANT) {
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

                                nation.removeMember(pawn.getId());
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

            int maxChunks = gameConfig.getMaxNumChunks();
            int numChunks = environment.getNumChunks();

            if (numChunks < maxChunks) {
                foreach (Vector3 position in positionsToGenerateChunksAt) {
                    worldGenerator.generateChunkAtPosition(position);
                    worldGenerator.generateSurroundingChunksAtPosition(position);
                }
            }
            
            player.fixedUpdate();
            if (player.getEnergy() <= 0) {
                eventProducer.producePlayerDeathEvent(player);
                numPlayerDeaths++;
                player.setEnergy(100);
                if (gameConfig.getKeepInventoryOnDeath() == false) {
                    player.getInventory().clear();
                }
                player.getStatus().update("You died.");
                
                if (player.getHomeSettlementId() != null) {
                    // player has home settlement, so respawn at settlement
                    Settlement homeSettlement = (Settlement)entityRepository.getEntity(player.getHomeSettlementId());
                    Vector3 newPosition = homeSettlement.getGameObject().transform.position;
                    newPosition = new Vector3(newPosition.x + UnityEngine.Random.Range(-20, 20), newPosition.y, newPosition.z + UnityEngine.Random.Range(-20, 20));
                    player.getGameObject().transform.position = newPosition;
                }
                else {
                    player.getGameObject().transform.position = new Vector3(UnityEngine.Random.Range(-100, 100), 10, UnityEngine.Random.Range(-100, 100));
                }
            }

            if (gameConfig.getLagPreventionEnabled()) {
                lagPreventer.markGameObjectsForDeletion();
            }

            deleteEntitiesMarkedForDeletion();
        }

        // on gui
        public void OnGUI() {
            drawCommandButtons();
            
            if (debugMode) {
                drawDebugInfo();
            }

            if (player.isCurrentlyInSettlement()) {
                Settlement settlement = (Settlement) entityRepository.getEntity(player.getCurrentSettlementId());
                drawSettlementInfo(settlement);

                Market market = settlement.getMarket();
                drawMarketInfo(market);
            }

            if (inventoryInfoBoxEnabled) {
                drawInventoryInfo(player.getInventory());
            }

            drawPlayerInfo();
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
                Nation nation = nationRepository.getNation(player.getNationId());

                // draw leave nation
                if (GUI.Button(new Rect(buttonX, buttonY, buttonWidth, buttonHeight), "Leave Nation")) {
                    NationLeaveCommand command = new NationLeaveCommand(nationRepository, eventProducer, entityRepository);
                    command.execute(player);
                }
                buttonX += buttonWidth + buttonSpacing;

                // if leader and no settlements and enough resources, draw found settlement
                if (player.getId() == nation.getLeaderId() && nation.getNumberOfSettlements() == 0 && player.getInventory().getNumItems(ItemType.WOOD) >= Settlement.WOOD_COST_TO_BUILD) {
                    // draw found settlement
                    if (GUI.Button(new Rect(buttonX, buttonY, buttonWidth, buttonHeight), "Found Settlement")) {
                        FoundSettlementCommand command = new FoundSettlementCommand(nationRepository, eventProducer, entityRepository, gameConfig);
                        command.execute(player);
                    }
                    buttonX += buttonWidth + buttonSpacing;
                }

                // teleport home
                if (!player.isCurrentlyInSettlement() && player.getHomeSettlementId() != null) {
                    Settlement homeSettlement = (Settlement) entityRepository.getEntity(player.getHomeSettlementId());
                    if (GUI.Button(new Rect(buttonX, buttonY, buttonWidth, buttonHeight), "Teleport Home")) {
                        TeleportHomeCommand command = new TeleportHomeCommand(entityRepository);
                        command.execute(player);
                    }
                    buttonX += buttonWidth + buttonSpacing;
                }

                // if leader and no stalls and enough resources, draw build stall
                if (player.isCurrentlyInSettlement()) {
                    Settlement settlement = (Settlement) entityRepository.getEntity(player.getCurrentSettlementId());
                    if (settlement.getNationId() == player.getNationId()) {
                        if (player.getId() == nation.getLeaderId() && settlement.getMarket().getNumStalls() < settlement.getMarket().getMaxNumStalls() && player.getInventory().getNumItems(ItemType.WOOD) >= Stall.WOOD_COST_TO_BUILD) {
                            if (GUI.Button(new Rect(buttonX, buttonY, buttonWidth, buttonHeight), "Build Stall")) {
                                BuildStallCommand command = new BuildStallCommand(nationRepository, entityRepository);
                                command.execute(player);
                            }
                            buttonX += buttonWidth + buttonSpacing;
                        }
                    }
                    
                }

                // if serf, enough gold and stall for sale, draw purchase stall
                if (player.isCurrentlyInSettlement()) {
                    Settlement settlement = (Settlement) entityRepository.getEntity(player.getCurrentSettlementId());
                    if (settlement.getNationId() == player.getNationId()) {
                        if (nation.getRole(player.getId()) == NationRole.SERF && player.getInventory().getNumItems(ItemType.COIN) >= Stall.COIN_COST_TO_PURCHASE && settlement.getMarket().getNumStallsForSale() > 0) {
                            if (GUI.Button(new Rect(buttonX, buttonY, buttonWidth, buttonHeight), "Purchase Stall")) {
                                PurchaseStallCommand command = new PurchaseStallCommand(nationRepository, entityRepository);
                                command.execute(player);
                            }
                            buttonX += buttonWidth + buttonSpacing;
                        }
                    }
                    
                }

                // if merchant, draw transfer items & collect profit
                if (player.isCurrentlyInSettlement()) {
                    Settlement settlement = (Settlement) entityRepository.getEntity(player.getCurrentSettlementId());
                    if (settlement.getNationId() == player.getNationId()) {
                        Market market = settlement.getMarket();
                        Stall stall = market.getStall(player.getId());
                        if (stall != null) {
                            Inventory stallInventory = stall.getInventory();
                            if (GUI.Button(new Rect(buttonX, buttonY, buttonWidth, buttonHeight), "Transfer Items")) {
                                TransferItemsToStallCommand command = new TransferItemsToStallCommand(nationRepository, entityRepository);
                                command.execute(player);
                            }
                            buttonX += buttonWidth + buttonSpacing;

                            int numCoins = stallInventory.getNumItems(ItemType.COIN);
                            if (numCoins > 0) {
                                if (GUI.Button(new Rect(buttonX, buttonY, buttonWidth, buttonHeight), "Collect Coins (" + numCoins + ")" )) {
                                    CollectProfitFromStallCommand command = new CollectProfitFromStallCommand(nationRepository, entityRepository);
                                    command.execute(player);
                                }
                                buttonX += buttonWidth + buttonSpacing;
                            }

                            int numApples = stallInventory.getNumItems(ItemType.APPLE);
                            if (numApples > 0) {
                                if (GUI.Button(new Rect(buttonX, buttonY, buttonWidth, buttonHeight), "Collect Food (" + numApples + ")")) {
                                    CollectFoodFromStallCommand command = new CollectFoodFromStallCommand(nationRepository, entityRepository);
                                    command.execute(player);
                                }
                                buttonX += buttonWidth + buttonSpacing;
                            }
                        }
                    }
                }

                // if leader, draw withdraw settlement funds
                if (player.isCurrentlyInSettlement()) {
                    Settlement settlement = (Settlement) entityRepository.getEntity(player.getCurrentSettlementId());
                    if (settlement.getNationId() == player.getNationId()) {
                        if (player.getId() == nation.getLeaderId()) {
                            if (GUI.Button(new Rect(buttonX, buttonY, buttonWidth, buttonHeight), "Withdraw Funds")) {
                                WithdrawSettlementFundsCommand command = new WithdrawSettlementFundsCommand(nationRepository, entityRepository);
                                command.execute(player);
                            }
                            buttonX += buttonWidth + buttonSpacing;
                        }
                    }
                }

            }

            // if saplings, plant sapling
            if (player.getInventory().getNumItems(ItemType.SAPLING) > 0 && !player.isCurrentlyInSettlement()) {
                if (GUI.Button(new Rect(buttonX, buttonY, buttonWidth, buttonHeight), "Plant Sapling")) {
                    PlantSaplingCommand command = new PlantSaplingCommand(entityRepository);
                    command.execute(player);
                }
                buttonX += buttonWidth + buttonSpacing;
            }

            if (!player.isCurrentlyInSettlement()) {
                Settlement nearestSettlement = (Settlement) environment.getNearestEntityOfType(player.getGameObject().transform.position, EntityType.SETTLEMENT);
                if (nearestSettlement != null) {
                    int distanceToNearestSettlement = (int) Vector3.Distance(player.getGameObject().transform.position, nearestSettlement.getGameObject().transform.position);
                    int distanceThreshold = 50;
                    if (distanceToNearestSettlement < distanceThreshold) {
                        if (GUI.Button(new Rect(buttonX, buttonY, buttonWidth, buttonHeight), "Enter Settlement")) {
                            EnterSettlementCommand command = new EnterSettlementCommand(entityRepository);
                            command.execute(player, nearestSettlement);
                        }
                        buttonX += buttonWidth + buttonSpacing;
                    }
                }
            }
            else {
                // draw exit settlement
                if (GUI.Button(new Rect(buttonX, buttonY, buttonWidth, buttonHeight), "Exit Settlement")) {
                    ExitSettlementCommand command = new ExitSettlementCommand(entityRepository);
                    command.execute(player);
                }
                buttonX += buttonWidth + buttonSpacing;

                // draw purchase food
                if (player.getInventory().getNumItems(ItemType.COIN) > 0) {
                    if (GUI.Button(new Rect(buttonX, buttonY, buttonWidth, buttonHeight), "Purchase Food")) {
                        PurchaseFoodFromMarketCommand command = new PurchaseFoodFromMarketCommand(nationRepository, entityRepository);
                        command.execute(player);
                    }
                    buttonX += buttonWidth + buttonSpacing;
                }

                // draw sell resources
                if (player.getInventory().getNumItems(ItemType.WOOD) > 0 || player.getInventory().getNumItems(ItemType.STONE) > 0) {
                    if (GUI.Button(new Rect(buttonX, buttonY, buttonWidth, buttonHeight), "Sell Resources")) {
                        SellResourcesAtMarketCommand command = new SellResourcesAtMarketCommand(nationRepository, entityRepository);
                        command.execute(player);
                    }
                    buttonX += buttonWidth + buttonSpacing;
                }
            }
        }

        private void drawDebugInfo() {
            int padding = 10;
            int width = 150;
            int height = 20;
            int xPos = 10;
            int yPos = 10;
            DebugInfoBox debugInfoBox = new DebugInfoBox(xPos, yPos, width, height, padding, tickCounter, environment, entityRepository, nationRepository, player, eventRepository);
            debugInfoBox.updateDeathCounts(numPawnDeaths, numPlayerDeaths);
            debugInfoBox.draw();
        }

        private void drawInventoryInfo(Inventory inventory) {
            int padding = 10;
            int width = 150;
            int height = 20;
            int x = 10;
            int y = 500;
            InfoBox inventoryInfoBox = new InventoryInfoBox(x, y, width, height, padding, "Inventory Info (I)", inventory);
            inventoryInfoBox.draw();
        }

        private void drawSettlementInfo(Settlement settlement) {
            int padding = 10;
            int width = 150;
            int height = 20;
            int x = Screen.width - width - padding;
            int y = 30;
            InfoBox settlementInfoBox = new SettlementInfoBox(x, y, width, height, padding, "Settlement Info ", settlement, nationRepository, entityRepository);
            settlementInfoBox.draw();
        }

        private void drawMarketInfo(Market market) {
            int padding = 10;
            int width = 150;
            int height = 20;
            int x = Screen.width - width - padding;
            int y = 200;
            InfoBox marketInfoBox = new MarketInfoBox(x, y, width, height, padding, "Market Info", market, entityRepository);
            marketInfoBox.draw();
        }

        private void drawPlayerInfo() {
            int padding = 10;
            int width = 150;
            int height = 20;
            int x = Screen.width - width - padding;
            int y = 500;
            InfoBox playerInfoBox = new PlayerInfoBox(x, y, width, height, padding, "Player Info", player, nationRepository);
            playerInfoBox.draw();
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
                if (!debugMode) {
                    player.getStatus().update("Debug mode must be enabled to teleport all pawns to player. Press " + KeyBindings.toggleDebugMode + " to enable debug mode.");
                    return;
                }
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
                GenerateLandCommand command = new GenerateLandCommand(environment, worldGenerator, gameConfig);
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
            else if (Input.GetKeyDown(KeyBindings.increaseRenderDistance)) {
                IncreaseRenderDistanceCommand command = new IncreaseRenderDistanceCommand();
                command.execute(player);
            }
            else if (Input.GetKeyDown(KeyBindings.decreaseRenderDistance)) {
                DecreaseRenderDistanceCommand command = new DecreaseRenderDistanceCommand();
                command.execute(player);
            }
            else if (Input.GetKeyDown(KeyBindings.toggleInventory)) {
                inventoryInfoBoxEnabled = !inventoryInfoBoxEnabled;
            }
        }

        private void checkIfPlayerIsFallingIntoVoid() {
            float ypos = player.getGameObject().transform.position.y;
            if (ypos < -10) {
                eventProducer.producePlayerFallingIntoVoidEvent(player.getGameObject().transform.position);
                if (player.getHomeSettlementId() != null) {
                    // player has home settlement, so respawn at settlement
                    Settlement homeSettlement = (Settlement)entityRepository.getEntity(player.getHomeSettlementId());
                    Vector3 newPosition = homeSettlement.getGameObject().transform.position;
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