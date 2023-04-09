using UnityEngine;

namespace osgtests {

    public static class Tests {
        
        public static void runTests() {
            // config
            TestGameConfig.runTests();

            // entity
            TestPawn.runTests();    
            TestRockEntity.runTests();
            TestTreeEntity.runTests();

            // inventory
            TestInventory.runTests();

            // nation
            TestNation.runTests();
            TestNationId.runTests();
            TestNationNameGenerator.runTests();
            TestNationRepository.runTests();

            // tick
            TestTickCounter.runTests();
        }
    }
}