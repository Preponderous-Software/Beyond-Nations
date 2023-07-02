using UnityEngine;

using osg;

namespace osgtests {

    public static class TestChunk {

        public static void runTests() {
            testInitialization();
            testGetLocation();
            testGetLocations();
            testGetGameObject();
            testGetPosition();
            testGetSize();
            testGetX();
            testGetZ();
        }

        public static void testInitialization() {
            // run
            Chunk chunk = new Chunk(0, 0, 1, 1);

            // verify
            UnityEngine.Debug.Assert(chunk != null);
            UnityEngine.Debug.Assert(chunk.getId() != null);
            UnityEngine.Debug.Assert(chunk.getSize() == 1);
            UnityEngine.Debug.Assert(chunk.getX() == 0);
            UnityEngine.Debug.Assert(chunk.getZ() == 0);
            UnityEngine.Debug.Assert(chunk.getPosition() == new Vector3(0, 0, 0));
            UnityEngine.Debug.Assert(chunk.getGameObject() != null);
            UnityEngine.Debug.Assert(chunk.getLocations() != null);
            UnityEngine.Debug.Assert(chunk.getLocations().Length == 1);
            UnityEngine.Debug.Assert(chunk.getLocations()[0, 0] != null);

            // clean up
            chunk.destroyGameObject();
        }

        public static void testGetLocation() {
            // prepare
            Chunk chunk = new Chunk(0, 0, 1, 1);

            // run
            Location location = chunk.getLocation(0, 0);

            // verify
            UnityEngine.Debug.Assert(location != null);

            // clean up
            chunk.destroyGameObject();
        }

        public static void testGetLocations() {
            // prepare
            Chunk chunk = new Chunk(0, 0, 1, 1);

            // run
            Location[,] locations = chunk.getLocations();

            // verify
            UnityEngine.Debug.Assert(locations != null);
            UnityEngine.Debug.Assert(locations.Length == 1);
            UnityEngine.Debug.Assert(locations[0, 0] != null);

            // clean up
            chunk.destroyGameObject();
        }

        public static void testGetGameObject() {
            // prepare
            Chunk chunk = new Chunk(0, 0, 1, 1);

            // run
            GameObject gameObject = chunk.getGameObject();

            // verify
            UnityEngine.Debug.Assert(gameObject != null);

            // clean up
            chunk.destroyGameObject();
        }

        public static void testGetPosition() {
            // prepare
            Chunk chunk = new Chunk(0, 0, 1, 1);

            // run
            Vector3 position = chunk.getPosition();

            // verify
            UnityEngine.Debug.Assert(position == new Vector3(0, 0, 0));

            // clean up
            chunk.destroyGameObject();
        }

        public static void testGetSize() {
            // prepare
            Chunk chunk = new Chunk(0, 0, 1, 1);

            // run
            int size = chunk.getSize();

            // verify
            UnityEngine.Debug.Assert(size == 1);

            // clean up
            chunk.destroyGameObject();
        }

        public static void testGetX() {
            // prepare
            Chunk chunk = new Chunk(0, 0, 1, 1);

            // run
            int x = chunk.getX();

            // verify
            UnityEngine.Debug.Assert(x == 0);

            // clean up
            chunk.destroyGameObject();
        }

        public static void testGetZ() {
            // prepare
            Chunk chunk = new Chunk(0, 0, 1, 1);

            // run
            int z = chunk.getZ();

            // verify
            UnityEngine.Debug.Assert(z == 0);

            // clean up
            chunk.destroyGameObject();
        }
    }
}