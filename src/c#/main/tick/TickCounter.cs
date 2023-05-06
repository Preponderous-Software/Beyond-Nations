using System;

namespace osg {

    public class TickCounter {
        private int tick = 0;
        private int updateInterval = 10;
        
        // measured ticks per second
        private int mtps = 0;
        
        // last time tick was updated
        private DateTime lastTickUpdate = DateTime.Now;

        public TickCounter(int updateInterval) {
            this.updateInterval = updateInterval;
        }

        public int getTick() {
            return tick;
        }

        public void increment() {
            tick++;

            if (DateTime.Now - lastTickUpdate >= TimeSpan.FromSeconds(1)) {
                mtps = tick;
                tick = 0;
                lastTickUpdate = DateTime.Now;
            }
        }

        public bool shouldUpdate() {
            return tick % updateInterval == 0;
        }

        public int getMtps() {
            return mtps;
        }
    }
}