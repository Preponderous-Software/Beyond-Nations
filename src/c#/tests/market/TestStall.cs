using UnityEngine;

using beyondnations;

namespace beyondnationstests {

    public static class TestStall {

        public static void runTests() {
            testInstantiation();
            testSetOwnerId();
        }

        private static void testInstantiation() {
            Stall stall = new Stall();
            UnityEngine.Debug.Assert(stall != null);
            UnityEngine.Debug.Assert(stall.getOwnerId() == null);
            UnityEngine.Debug.Assert(stall.getInventory() != null);
        }

        private static void testSetOwnerId() {
            // prepare
            Stall stall = new Stall();
            Pawn pawn = new Pawn(new Vector3(0, 0, 0), "test");

            // execute
            stall.setOwnerId(pawn.getId());

            // verify
            UnityEngine.Debug.Assert(stall.getOwnerId() == pawn.getId());
        
            // cleanup
            pawn.destroyGameObject();
        }
    }
}