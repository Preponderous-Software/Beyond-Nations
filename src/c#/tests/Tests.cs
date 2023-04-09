using UnityEngine;

namespace osgtests {

    public static class Tests {
        
        public static void runTests() {
            TestTickCounter.runTests();
            TestGameConfig.runTests();
            TestRockEntity.runTests();
            TestTreeEntity.runTests();
            TestLivingEntity.runTests();
        }
    }
}