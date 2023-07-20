namespace osg {

    public class GameConfig {
        private int chunkSize;
        private int locationScale;
        private int statusExpirationTicks;
        private int playerWalkSpeed;
        private int playerRunSpeed;
        private bool respawnPawns;
        private bool keepInventoryOnDeath;
        private int ticksBetweenBehaviorCalculations;
        private int ticksBetweenBehaviorExecutions;
        private int minDistanceBetweenSettlements;
        private int settlementJoinRange;
        private float settlementMetabolismMultiplier;
        private int renderDistance;
        private bool lagPreventionEnabled;
        private int maxNumEntities;
        private int maxNumChunks;

        public GameConfig() {
            chunkSize = 5;
            locationScale = 15;
            statusExpirationTicks = 500;
            playerWalkSpeed = 20;
            playerRunSpeed = 50;
            respawnPawns = false;
            keepInventoryOnDeath = true;
            ticksBetweenBehaviorCalculations = 1000;
            ticksBetweenBehaviorExecutions = 5;
            minDistanceBetweenSettlements = 250;
            settlementJoinRange = 500;
            settlementMetabolismMultiplier = 0.8f;
            renderDistance = 100;
            lagPreventionEnabled = true;
            maxNumEntities = 10000;
            maxNumChunks = 1000;
        }

        public int getChunkSize() {
            return chunkSize;
        }

        public int getLocationScale() {
            return locationScale;
        }

        public int getStatusExpirationTicks() {
            return statusExpirationTicks;
        }

        public int getPlayerWalkSpeed() {
            return playerWalkSpeed;
        }

        public int getPlayerRunSpeed() {
            return playerRunSpeed;
        }

        public bool getRespawnPawns() {
            return respawnPawns;
        }

        public bool getKeepInventoryOnDeath() {
            return keepInventoryOnDeath;
        }

        public int getTicksBetweenBehaviorCalculations() {
            return ticksBetweenBehaviorCalculations;
        }

        public int getTicksBetweenBehaviorExecutions() {
            return ticksBetweenBehaviorExecutions;
        }

        public int getMinDistanceBetweenSettlements() {
            return minDistanceBetweenSettlements;
        }

        public int getSettlementJoinRange() {
            return settlementJoinRange;
        }

        public float getSettlementMetabolismMultiplier() {
            return settlementMetabolismMultiplier;
        }

        public int getRenderDistance() {
            return renderDistance;
        }

        public bool getLagPreventionEnabled() {
            return lagPreventionEnabled;
        }

        public int getMaxNumEntities() {
            return maxNumEntities;
        }

        public int getMaxNumChunks() {
            return maxNumChunks;
        }
    }
}