using System.Diagnostics;
using UnityEngine;

using beyondnations;

namespace beyondnationstests {

    public static class TestMarket {

        public static void runTests() {
            testInstantiation();
            testCreateStall();
            testGetStallForSale();
            testGetStall();
        }

        private static void testInstantiation() {
            Market market = new Market(10);
            UnityEngine.Debug.Assert(market != null);
            UnityEngine.Debug.Assert(market.getNumStalls() == 0);
            UnityEngine.Debug.Assert(market.getMaxNumStalls() == 10);
        }

        private static void testCreateStall() {
            // prepare
            Market market = new Market(10);

            // execute
            bool result = market.createStall();

            // verify
            UnityEngine.Debug.Assert(result == true);
            UnityEngine.Debug.Assert(market.getNumStalls() == 1);
            UnityEngine.Debug.Assert(market.getNumStallsForSale() == 1);
        }

        private static void testGetStallForSale() {
            // prepare
            Market market = new Market(10);
            market.createStall();

            // execute
            Stall stall = market.getStallForSale();

            // verify
            UnityEngine.Debug.Assert(stall != null);
            UnityEngine.Debug.Assert(stall.getOwnerId() == null);
        }

        private static void testGetStall() {
            // prepare
            Market market = new Market(10);
            Pawn pawn = new Pawn(new Vector3(0, 0, 0), "test");
            market.createStall();
            market.getStallForSale().setOwnerId(pawn.getId());

            // execute
            Stall stall = market.getStall(pawn.getId());

            // verify
            UnityEngine.Debug.Assert(stall != null);

            // cleanup
            pawn.destroyGameObject();
        }
    }
}