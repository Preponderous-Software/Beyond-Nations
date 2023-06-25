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
            Debug.Assert(location != null);
            Debug.Assert(location.getId() != null);
            Debug.Assert(location.getPosition() == new Vector3(0, 0, 0));
            Debug.Assert(location.getScale() == 1);
            Debug.Assert(location.getGameObject() != null);
            Debug.Assert(location.getNumberOfEntities() == 0);

            // clean up
            location.destroyGameObject();
        }
    }

}