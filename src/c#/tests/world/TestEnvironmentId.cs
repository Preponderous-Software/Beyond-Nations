using UnityEngine;

using osg;

namespace osgtests {

    public static class TestEnvironmentId {

        public static void runTests() {
            testInitialization();
            testEquality();
        }

        public static void testInitialization() {
            // run
            EnvironmentId environmentId = new EnvironmentId();

            // verify
            UnityEngine.Debug.Assert(environmentId != null);
        }

        public static void testEquality() {
            // prepare
            EnvironmentId environmentId1 = new EnvironmentId();
            EnvironmentId environmentId2 = new EnvironmentId();

            // verify
            UnityEngine.Debug.Assert(environmentId1 != environmentId2);
        }
    }
}