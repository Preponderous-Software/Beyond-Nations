using UnityEngine;
using UnityEngine.UI;

using osg;

namespace osgtests {

    public static class TestScreenOverlay {

        public static void runTests() {
            testInitialization();
        }
        
        private static void testInitialization() {
            // execute
            ScreenOverlay screenOverlay = new ScreenOverlay(null, null);
            
            // verify
            Debug.Assert(screenOverlay != null);

            // cleanup
            screenOverlay.destroy();
        }

        // TODO: test update method (requires mocking the Player class)
    }
}