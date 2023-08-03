using UnityEngine;

using beyondnations;

namespace beyondnationstests {

    public static class TestGameConfig {

        public static void runTests() {
            testInstantiation();
        }

        public static void testInstantiation() {
            // run
            GameConfig config = new GameConfig();
            
            // check
            UnityEngine.Debug.Assert(config.getChunkSize() > 0);
            UnityEngine.Debug.Assert(config.getLocationScale() > 0);
            UnityEngine.Debug.Assert(config.getStatusExpirationTicks() > 0);
            UnityEngine.Debug.Assert(config.getPlayerWalkSpeed() > 0);
            UnityEngine.Debug.Assert(config.getPlayerRunSpeed() > config.getPlayerWalkSpeed());
            UnityEngine.Debug.Assert(config.getTicksBetweenBehaviorCalculations() > 0);
            UnityEngine.Debug.Assert(config.getTicksBetweenBehaviorExecutions() > 0);
            UnityEngine.Debug.Assert(config.getMinDistanceBetweenSettlements() > 0);
            UnityEngine.Debug.Assert(config.getSettlementJoinRange() > 0);
        }
    }    
}