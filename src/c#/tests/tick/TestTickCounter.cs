using UnityEngine;

using osg;

namespace osgtests {

    public static class TestTickCounter {

        public static void runTests() {
            testInstantiation();
            testIncrement();
            testShouldUpdate();
        }

        public static void testInstantiation() {
            // run
            int updateInterval = 10;
            TickCounter tickCounter = new TickCounter(updateInterval);

            // check
            Debug.Assert(tickCounter.getTick() == 0);
            Debug.Assert(tickCounter.shouldUpdate() == false);
        }

        public static void testIncrement() {
            // prepare
            int updateInterval = 10;
            TickCounter tickCounter = new TickCounter(updateInterval);

            // run
            tickCounter.increment();

            // check
            Debug.Assert(tickCounter.getTick() == 1);
        }

        public static void testShouldUpdate() {
            // prepare
            int updateInterval = 2;
            TickCounter tickCounter = new TickCounter(updateInterval);

            // run
            tickCounter.increment();
            tickCounter.increment();

            // check
            Debug.Assert(tickCounter.shouldUpdate() == true);
        }
        
    }
}