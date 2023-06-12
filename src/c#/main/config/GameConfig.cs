namespace osg {

    class GameConfig {
        private int chunkSize;
        private int locationScale;
        private int statusExpirationTicks;
        private int playerWalkSpeed;
        private int playerRunSpeed;
        private bool respawnPawns;
        private int numStartingNations;
        private bool keepInventoryOnDeath;
        private int ticksBetweenBehaviorCalculations;
        private int ticksBetweenBehaviorExecutions;

        public GameConfig() {
            chunkSize = 9;
            locationScale = 9;
            statusExpirationTicks = 500;
            playerWalkSpeed = 20;
            playerRunSpeed = 50;
            respawnPawns = true;
            numStartingNations = 5;
            keepInventoryOnDeath = true;
            ticksBetweenBehaviorCalculations = 25;
            ticksBetweenBehaviorExecutions = 5;
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

        public int getNumStartingNations() {
            return numStartingNations;
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
    }
}