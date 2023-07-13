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

        public GameConfig() {
            chunkSize = 9;
            locationScale = 9;
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
    }
}