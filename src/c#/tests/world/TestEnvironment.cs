using UnityEngine;

using osg;

namespace osgtests {

    public static class TestEnvironment {

        public static void runTests() {
            testInitialization();
        }

        public static void testInitialization() {
            // run
            EntityRepository entityRepository = new EntityRepository();
            Environment environment = new Environment(1, 1, entityRepository);

            // verify
            UnityEngine.Debug.Assert(environment != null);
            UnityEngine.Debug.Assert(environment.getChunkAtPosition(new Vector3(0, 0, 0)) != null);

            // clean up
            environment.destroyGameObject();
        }

        // TODO: write more tests
    }
}