using UnityEngine;

namespace beyondnationstests {

    public static class Tests {
        
        public static void runTests() {
            // behavior
            TestPawnBehaviorCalculator.runTests();

            // config
            TestGameConfig.runTests();

            // entity
            TestPawn.runTests();    
            TestRock.runTests();
            TestTree.runTests();

            // inventory
            TestInventory.runTests();

            // market
            TestMarket.runTests();
            TestStall.runTests();

            // nation
            TestNation.runTests();
            TestNationId.runTests();
            TestNationNameGenerator.runTests();
            TestNationRepository.runTests();

            // tick
            TestTickCounter.runTests();

            // ui
            TestCanvasFactory.runTests();
            TestStatus.runTests();
            TestTextGameObject.runTests();

            // world
            TestLocationId.runTests();
            TestLocation.runTests();
            TestChunkId.runTests();
            TestChunk.runTests();
            TestEnvironmentId.runTests();
            TestEnvironment.runTests();
        }
    }
}