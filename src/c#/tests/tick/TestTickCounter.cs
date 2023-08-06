using UnityEngine;

using beyondnations;

namespace beyondnationstests {

    public static class TestTickCounter {

        public static void runTests() {
            testInstantiation();
            testIncrement();
        }

        public static void testInstantiation() {
            // run
            TickCounter tickCounter = new TickCounter();

            // check
            UnityEngine.Debug.Assert(tickCounter.getTick() == 0);
        }

        public static void testIncrement() {
            // prepare
            TickCounter tickCounter = new TickCounter();

            // run
            tickCounter.increment();

            // check
            UnityEngine.Debug.Assert(tickCounter.getTick() == 1);
        }
        
    }
}