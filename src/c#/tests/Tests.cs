using UnityEngine;

namespace osgtests {

    public static class Tests {
        
        public static void runTests() {
            // config
            TestGameConfig.runTests();

            // entity
            TestLivingEntity.runTests();    
            TestRockEntity.runTests();
            TestTreeEntity.runTests();

            // tick
            TestTickCounter.runTests();
        }
    }
}