namespace osg {

    class GameConfig {
        private int chunkSize;
        private int locationScale;
        private int statusExpirationTicks;
        private int playerWalkSpeed;
        private int playerRunSpeed;
        private bool respawnPawns;
        private int numStartingNations;

        public GameConfig() {
            chunkSize = 9;
            locationScale = 9;
            statusExpirationTicks = 500;
            playerWalkSpeed = 20;
            playerRunSpeed = 50;
            respawnPawns = true;
            numStartingNations = 5;
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
    }
}