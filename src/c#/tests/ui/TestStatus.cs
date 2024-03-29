using UnityEngine;
using UnityEngine.UI;

using beyondnations;

namespace beyondnationstests {

    public static class TestStatus {

        public static void runTests() {
            testInstantiation();
            testUpdate();
            testClearStatusIfExpiredNotExpired();
            testClearStatusIfExpiredExpired();
        }
        
        private static void testInstantiation() {
            // prepare
            TickCounter tickCounter = new TickCounter();
            int ticksToExpire = 10;

            // execute
            Status status = new Status(tickCounter, ticksToExpire);

            // verify
            UnityEngine.Debug.Assert(status.getTicksToExpire() == ticksToExpire);
            UnityEngine.Debug.Assert(status.getTickLastSet() == 0);
            UnityEngine.Debug.Assert(status.getStatus() == "Game started.");

            // cleanup
            GameObject.Destroy(status.getTextGameObject().getCanvasObject());
        }

        private static void testUpdate() {
            // prepare
            TickCounter tickCounter = new TickCounter();
            int ticksToExpire = 10;
            Status status = new Status(tickCounter, ticksToExpire);

            // execute
            status.update("test");

            // verify
            UnityEngine.Debug.Assert(status.getTickLastSet() == 0);
            UnityEngine.Debug.Assert(status.getStatus() == "test");

            // cleanup
            GameObject.Destroy(status.getTextGameObject().getCanvasObject());
        }

        private static void testClearStatusIfExpiredNotExpired() {
            // prepare
            TickCounter tickCounter = new TickCounter();
            int ticksToExpire = 10;
            Status status = new Status(tickCounter, ticksToExpire);
            status.update("test");

            // execute
            tickCounter.increment();
            status.clearStatusIfExpired();

            // verify
            UnityEngine.Debug.Assert(status.getStatus() == "test");

            // cleanup
            GameObject.Destroy(status.getTextGameObject().getCanvasObject());
        }

        private static void testClearStatusIfExpiredExpired() {
            // prepare
            TickCounter tickCounter = new TickCounter();
            int ticksToExpire = 10;
            Status status = new Status(tickCounter, ticksToExpire);
            status.update("test");

            // execute
            for (int i = 0; i < ticksToExpire + 1; i++) {
                tickCounter.increment();
            }
            status.clearStatusIfExpired();

            // verify
            UnityEngine.Debug.Assert(status.getStatus() == "");

            // cleanup
            GameObject.Destroy(status.getTextGameObject().getCanvasObject());
        }
    }
}