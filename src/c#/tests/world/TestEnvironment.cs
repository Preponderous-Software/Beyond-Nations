using UnityEngine;

using osg;

namespace osgtests {

    public static class TestEnvironment {

        public static void runTests() {
            testInitialization();
        }

        public static void testInitialization() {
            // run
            Environment environment = new Environment(1, 1);

            // verify
            Debug.Assert(environment != null);
            Debug.Assert(environment.getId() != null);
            Debug.Assert(environment.getChunks() != null);
            Debug.Assert(environment.getChunkAtPosition(new Vector3(0, 0, 0)) != null);

            // clean up
            environment.destroyGameObject();
        }

        // TODO: write more tests
    }
}