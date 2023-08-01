using UnityEngine;

using osg;

namespace osgtests {

    public static class TestLocationId {
        
        public static void runTests() {
            testInitialization();
            testEquality();
        }

        public static void testInitialization() {
            // run
            LocationId locationId = new LocationId();

            // verify
            UnityEngine.Debug.Assert(locationId != null);
        }

        public static void testEquality() {
            // prepare
            LocationId locationId1 = new LocationId();
            LocationId locationId2 = new LocationId();

            // verify
            UnityEngine.Debug.Assert(locationId1 != locationId2);
        }
    }
}