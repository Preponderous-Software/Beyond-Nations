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
        private TextGameObject numGoldCoinsText;
        private TextGameObject numWoodText;
        private TextGameObject numStoneText;
        private TextGameObject numApplesText;

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
            player = new Player(playerGameObject, gameConfig.getPlayerWalkSpeed(), gameConfig.getPlayerRunSpeed(), status);
            eventRepository = new EventRepository();
            eventProducer = new EventProducer(eventRepository);
            environment = new Environment(gameConfig.getChunkSize(), gameConfig.getLocationScale());
            worldGenerator = new WorldGenerator(environment, player, eventProducer);
            chunkPositionText = new TextGameObject("Chunk: (0, 0)", 20, 0, Screen.height / 4);
            nationRepository = new NationRepository();
            numGoldCoinsText = new TextGameObject("Gold Coins: 0", 20, -Screen.width / 4, Screen.height / 4);
            numWoodText = new TextGameObject("Wood: 0", 20, -Screen.width / 4, 0);
            numStoneText = new TextGameObject("Stone: 0", 20, Screen.width / 4, 0);
            numApplesText = new TextGameObject("Apples: 0", 20, Screen.width / 4, Screen.height / 4);

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
                numGoldCoinsText.updateText("Gold Coins: " + player.getInventory().getNumItems(ItemType.GOLD_COIN));
                numWoodText.updateText("Wood: " + player.getInventory().getNumItems(ItemType.WOOD));
                numStoneText.updateText("Stone: " + player.getInventory().getNumItems(ItemType.STONE));
                numApplesText.updateText("Apples: " + player.getInventory().getNumItems(ItemType.APPLE));
                status.clearStatusIfExpired();

                // list of positions to generate chunks at
                List<Vector3> positionsToGenerateChunksAt = new List<Vector3>();

                foreach (Chunk chunk in environment.getChunks()) {
                    foreach (Entity entity in chunk.getEntities()) {
                        if (entity.getType() == EntityType.PAWN) {
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

                            Chunk retrievedChunk = environment.getChunkAtPosition(pawn.getGameObject().transform.position);
                            if (retrievedChunk == null) {
                                positionsToGenerateChunksAt.Add(pawn.getGameObject().transform.position);
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
                if (nationRepository.getNumberOfNations() == 0) {
                    status.update("There are no nations to join.");
                    return;
                }
                Nation nation = nationRepository.getRandomNation();
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
                        if (entity.getType() == EntityType.PAWN) {
                            Pawn pawn = (Pawn)entity;
                            pawn.getGameObject().transform.position = player.getGameObject().transform.position;
                        }
                    }
                }
            }

            // if num lock pressed, toggle auto walk
            if (Input.GetKeyDown(KeyCode.Numlock)) {
                player.toggleAutoWalk();
            }

            // if L pressed, leave nation
            if (Input.GetKeyDown(KeyCode.L)) {
                if (player.getNationId() == null) {
                    status.update("You are not a member of a nation.");
                    return;
                }
                Nation nation = nationRepository.getNation(player.getNationId());
                if (nation.getLeaderId() == player.getId()) {
                    // if population is 1, delete nation
                    if (nation.getNumberOfMembers() == 1) {
                        nationRepository.removeNation(nation);
                        player.setNationId(null);
                        player.setColor(Color.white);
                        eventProducer.produceNationDisbandEvent(nation);
                        status.update("You disbanded nation " + nation.getName() + ".");
                        return;
                    }
                    else {
                        // if population is > 1, transfer leadership to another member
                        while (nation.getLeaderId() == player.getId()) {
                            nation.setLeaderId(nation.getRandomMemberId());
                        }
                        nation.removeMember(player.getId());
                        player.setNationId(null);
                        player.setColor(Color.white);
                        eventProducer.produceNationLeaveEvent(nation, player.getId());
                        status.update("You left nation " + nation.getName() + ". Members: " + nation.getNumberOfMembers() + ".");
                        return;
                    }
                }
                nation.removeMember(player.getId());
                player.setNationId(null);
                player.setColor(Color.white);
                eventProducer.produceNationLeaveEvent(nation, player.getId());
                status.update("You left nation " + nation.getName() + ". Members: " + nation.getNumberOfMembers() + ".");
            }

            // if E pressed, gather resources from tree/rock
            if (Input.GetKeyDown(KeyCode.E)) {
                TreeEntity tree = environment.getNearestTree(player.getGameObject().transform.position);
                RockEntity rock = environment.getNearestRock(player.getGameObject().transform.position);
                Pawn pawn = (Pawn) environment.getNearestEntityOfType(player.getGameObject().transform.position, EntityType.PAWN);

                // if within range of tree or rock, gather resources
                if (tree != null && Vector3.Distance(player.getGameObject().transform.position, tree.getGameObject().transform.position) < 5) {
                    tree.markForDeletion();
                    player.getInventory().transferContentsOfInventory(tree.getInventory());
                    status.update("Gathered wood from tree.");
                }
                else if (rock != null && Vector3.Distance(player.getGameObject().transform.position, rock.getGameObject().transform.position) < 5) {
                    rock.markForDeletion();
                    player.getInventory().transferContentsOfInventory(rock.getInventory());
                    status.update("Gathered stone from rock.");
                }
                else if (pawn != null && Vector3.Distance(player.getGameObject().transform.position, pawn.getGameObject().transform.position) < 5) {
                    Nation pawnsNation = nationRepository.getNation(pawn.getNationId());
                    EntityId randomMemberId = pawnsNation.getRandomMemberId();

                    // make pawn say random phrase to player
                    List <string> phrases = new List<string>() {
                        "Hello!",
                        "How are you?",
                        "Glory to " + pawnsNation.getName() + "!",
                        "What's your name?",
                        "I'm " + pawn.getName() + ".",
                        "Have you heard of " + pawnsNation.getName() + "?",
                        "Nice to meet you!",
                        "I'm hungry.",
                        "The weather is nice today."
                    };
                    if (pawnsNation.getNumberOfMembers() > 1) {
                        phrases.Add("There are " + pawnsNation.getNumberOfMembers() + " members in " + pawnsNation.getName() + ".");

                        if (pawnsNation.getLeaderId() == player.getId()) {
                            phrases.Add("Hey boss!");
                            phrases.Add("How's it going boss?");
                            phrases.Add("What's up boss?");
                            phrases.Add("I'm glad to be a member of " + pawnsNation.getName() + ".");
                            phrases.Add("Please don't kick me out of " + pawnsNation.getName() + ".");
                            phrases.Add("When are we going to build more houses?");
                        }
                    }
                    if (player.getNationId() != null) {
                        Nation playersNation = nationRepository.getNation(player.getNationId());
                        if (playersNation.getLeaderId() == pawn.getId()) {
                            // sell items to pawn
                            int numWood = player.getInventory().getNumItems(ItemType.WOOD);
                            int numStone = player.getInventory().getNumItems(ItemType.STONE);
                            int numApples = player.getInventory().getNumItems(ItemType.APPLE);

                            int cost = (numWood * 5) + (numStone * 10) + (numApples * 1);

                            if (cost == 0) {
                                status.update("You don't have anything to sell.");
                                return;
                            }

                            if (pawn.getInventory().getNumItems(ItemType.GOLD_COIN) < cost) {
                                status.update(pawn.getName() + " doesn't have enough gold coins to buy your items.");
                                return;
                            }

                            player.getInventory().removeItem(ItemType.WOOD, numWood);
                            player.getInventory().removeItem(ItemType.STONE, numStone);
                            player.getInventory().removeItem(ItemType.APPLE, numApples);
                            pawn.getInventory().addItem(ItemType.WOOD, numWood);
                            pawn.getInventory().addItem(ItemType.STONE, numStone);
                            pawn.getInventory().addItem(ItemType.APPLE, numApples);
                            player.getInventory().addItem(ItemType.GOLD_COIN, cost);
                            status.update("Sold " + numWood + " wood, " + numStone + " stone, and " + numApples + " apples to " + pawn.getName() + " for " + cost + " gold coins.");
                            return;
                        }
                    }
                    string phrase = phrases[Random.Range(0, phrases.Count)];
                    status.update(pawn.getName() + ": \"" + phrase + "\"");
                }
                else {
                    status.update("No entities within range to interact with.");
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