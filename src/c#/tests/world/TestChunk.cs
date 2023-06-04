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
            Debug.Assert(chunk != null);
            Debug.Assert(chunk.getId() != null);
            Debug.Assert(chunk.getSize() == 1);
            Debug.Assert(chunk.getX() == 0);
            Debug.Assert(chunk.getZ() == 0);
            Debug.Assert(chunk.getPosition() == new Vector3(0, 0, 0));
            Debug.Assert(chunk.getGameObject() != null);
            Debug.Assert(chunk.getLocations() != null);
            Debug.Assert(chunk.getLocations().Length == 1);
            Debug.Assert(chunk.getLocations()[0, 0] != null);

            // clean up
            chunk.destroyGameObject();
        }

        public static void testGetLocation() {
            // prepare
            Chunk chunk = new Chunk(0, 0, 1, 1);

            // run
            Location location = chunk.getLocation(0, 0);

            // verify
            Debug.Assert(location != null);

            // clean up
            chunk.destroyGameObject();
        }

        public static void testGetLocations() {
            // prepare
            Chunk chunk = new Chunk(0, 0, 1, 1);

            // run
            Location[,] locations = chunk.getLocations();

            // verify
            Debug.Assert(locations != null);
            Debug.Assert(locations.Length == 1);
            Debug.Assert(locations[0, 0] != null);

            // clean up
            chunk.destroyGameObject();
        }

        public static void testGetGameObject() {
            // prepare
            Chunk chunk = new Chunk(0, 0, 1, 1);

            // run
            GameObject gameObject = chunk.getGameObject();

            // verify
            Debug.Assert(gameObject != null);

            // clean up
            chunk.destroyGameObject();
        }

        public static void testGetPosition() {
            // prepare
            Chunk chunk = new Chunk(0, 0, 1, 1);

            // run
            Vector3 position = chunk.getPosition();

            // verify
            Debug.Assert(position == new Vector3(0, 0, 0));

            // clean up
            chunk.destroyGameObject();
        }

        public static void testGetSize() {
            // prepare
            Chunk chunk = new Chunk(0, 0, 1, 1);

            // run
            int size = chunk.getSize();

            // verify
            Debug.Assert(size == 1);

            // clean up
            chunk.destroyGameObject();
        }

        public static void testGetX() {
            // prepare
            Chunk chunk = new Chunk(0, 0, 1, 1);

            // run
            int x = chunk.getX();

            // verify
            Debug.Assert(x == 0);

            // clean up
            chunk.destroyGameObject();
        }

        public static void testGetZ() {
            // prepare
            Chunk chunk = new Chunk(0, 0, 1, 1);

            // run
            int z = chunk.getZ();

            // verify
            Debug.Assert(z == 0);

            // clean up
            chunk.destroyGameObject();
        }
    }
}