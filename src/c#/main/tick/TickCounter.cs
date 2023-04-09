namespace osg {

    public class TickCounter {
        private int tick = 0;
        private int updateInterval = 10;
        private int lastUpdateTick = 0;

        public TickCounter(int updateInterval) {
            this.updateInterval = updateInterval;
        }

        public int getTick() {
            return tick;
        }
        
        public int getLastUpdateTick() {
            return lastUpdateTick;
        }

        public void increment() {
            tick++;
        }

        public bool shouldUpdate() {
            if (tick - lastUpdateTick >= updateInterval) {
                lastUpdateTick = tick;
                return true;
            }
            return false;
        }
    }
}