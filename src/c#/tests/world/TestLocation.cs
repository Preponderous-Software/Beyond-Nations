using UnityEngine;

using osg;

namespace osgtests {

    public static class TestLocation {
        
        public static void runTests() {
            testInitialization();
        }

        public static void testInitialization() {
            // run
            Location location = new Location(0, 0, 1);

            // verify
            UnityEngine.Debug.Assert(location != null);
            UnityEngine.Debug.Assert(location.getId() != null);
            UnityEngine.Debug.Assert(location.getPosition() == new Vector3(0, 0, 0));
            UnityEngine.Debug.Assert(location.getScale() == 1);
            UnityEngine.Debug.Assert(location.getGameObject() != null);
            UnityEngine.Debug.Assert(location.getNumberOfEntities() == 0);

            // clean up
            location.destroyGameObject();
        }
    }

}