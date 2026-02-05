using UnityEngine;
using UnityEngine.UI;

namespace beyondnations {
    public class DebugInfoBox : InfoBox {
        private TickCounter tickCounter;
        private Environment environment;
        private EntityRepository entityRepository;
        private NationRepository nationRepository;
        private Player player;
        private EventRepository eventRepository;
        private int numPawnDeaths = 0;
        private int numPlayerDeaths = 0;
        private int numDataPoints = 21;

        public DebugInfoBox(int x, int y, int width, int height, int padding, TickCounter tickCounter, Environment environment, EntityRepository entityRepository, NationRepository nationRepository, Player player, EventRepository eventRepository) : base(x, y, width, height, padding, "Debug Info (F1)") {
            this.tickCounter = tickCounter;
            this.environment = environment;
            this.entityRepository = entityRepository;
            this.nationRepository = nationRepository;
            this.player = player;
            this.eventRepository = eventRepository;
        }

        public void updateDeathCounts(int numPawnDeaths, int numPlayerDeaths) {
            this.numPawnDeaths = numPawnDeaths;
            this.numPlayerDeaths = numPlayerDeaths;
        }

        public override void draw() {
            // draw box with padding
            GUI.Box(new Rect(x - 10, y - 10, width + 20, (height * (numDataPoints + 2))), title);
            y += 10;
                
            // fps                
            GUI.Label(new Rect(x, y, width, height), "FPS: " + (int)(1.0f / Time.smoothDeltaTime));
            y += 20;
            
            // mtps
            GUI.Label(new Rect(x, y, width, height), "MTPS: " + tickCounter.getMtps());
            y += 20;

            // tick
            GUI.Label(new Rect(x, y, width, height), "Total ticks: " + tickCounter.getTotalTicks());
            y += 20;

            // current chunk
            Chunk currentChunk = environment.getChunkAtPosition(player.getGameObject().transform.position);
            if (currentChunk != null) {
                GUI.Label(new Rect(x, y, width, height), "Chunk: " + currentChunk.getX() + ", " + currentChunk.getZ());
            }
            else {
                GUI.Label(new Rect(x, y, width, height), "Chunk: null");
            }
            y += 20;

            // number of entities
            GUI.Label(new Rect(x, y, width, height), "Entities: " + entityRepository.getNumEntities());
            y += 20;

            // number of chunks
            GUI.Label(new Rect(x, y, width, height), "Chunks: " + environment.getNumChunks());
            y += 20;

            // number of pawns
            int numPawns = entityRepository.getEntitiesOfType(EntityType.PAWN).Count;
            GUI.Label(new Rect(x, y, width, height), "Pawns: " + numPawns);
            y += 20;

            // number of nations
            GUI.Label(new Rect(x, y, width, height), "Nations: " + nationRepository.getNumberOfNations());
            y += 20;

            // number of settlements
            GUI.Label(new Rect(x, y, width, height), "Settlements: " + entityRepository.getEntitiesOfType(EntityType.SETTLEMENT).Count);
            y += 20;

            // number of trees
            GUI.Label(new Rect(x, y, width, height), "Trees: " + entityRepository.getEntitiesOfType(EntityType.TREE).Count);
            y += 20;

            // number of saplings
            GUI.Label(new Rect(x, y, width, height), "Saplings: " + entityRepository.getEntitiesOfType(EntityType.SAPLING).Count);
            y += 20;

            // number of rocks
            GUI.Label(new Rect(x, y, width, height), "Rocks: " + entityRepository.getEntitiesOfType(EntityType.ROCK).Count);
            y += 20;

            // events stored
            GUI.Label(new Rect(x, y, width, height), "Events: " + eventRepository.getTotalNumberOfEvents());
            y += 20;

            // total num stalls
            int totalNumStalls = 0;
            foreach (Settlement settlement in entityRepository.getEntitiesOfType(EntityType.SETTLEMENT)) {
                totalNumStalls += settlement.getMarket().getNumStalls();
            }
            GUI.Label(new Rect(x, y, width, height), "Stalls: " + totalNumStalls);
            y += 20;

            // pawns currently in settlement
            int numPawnsCurrentlyInSettlement = 0;
            foreach (Pawn pawn in entityRepository.getEntitiesOfType(EntityType.PAWN)) {
                if (pawn.isCurrentlyInSettlement()) {
                    numPawnsCurrentlyInSettlement++;
                }
            }
            GUI.Label(new Rect(x, y, width, height), "PCIS: " + numPawnsCurrentlyInSettlement + " / " + numPawns);
            y += 20;

            // nationless pawns
            int numNationlessPawns = 0;
            foreach (Pawn pawn in entityRepository.getEntitiesOfType(EntityType.PAWN)) {
                if (pawn.getNationId() == null) {
                    numNationlessPawns++;
                }
            }
            GUI.Label(new Rect(x, y, width, height), "Nationless: " + numNationlessPawns + " / " + numPawns);
            y += 20;

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
            GUI.Label(new Rect(x, y, width, height), "Leaders: " + numLeaders);
            y += 20;
            GUI.Label(new Rect(x, y, width, height), "Merchants: " + numMerchants);
            y += 20;
            GUI.Label(new Rect(x, y, width, height), "Serfs: " + numSerfs);
            y += 20;

            // pawn deaths
            GUI.Label(new Rect(x, y, width, height), "Pawn Deaths: " + numPawnDeaths);
            y += 20;

            // player deaths
            GUI.Label(new Rect(x, y, width, height), "Player Deaths: " + numPlayerDeaths);
            y += 20;

            
            // perform discrepency checks
            if (numPawns != numLeaders + numMerchants + numSerfs + numNationlessPawns) {
                Debug.LogError("Discrepency in pawn count!");
            }
        }
    } 
}