using UnityEngine;

using osg;

namespace osgtests {

    public static class TestLocation {
        
        public static void runTests() {
            testInitialization();
            testAddEntityId();
            testRemoveEntityId();
            testIsEntityPresent();
            testGetNumberOfEntities();
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

        public static void testAddEntityId() {
            // prepare
            Location location = new Location(0, 0, 1);
            Entity entity = new RockEntity(new Vector3(0, 0, 0));

            // run
            location.addEntityId(entity.getId());

            // verify
            Debug.Assert(location.getNumberOfEntities() == 1);

            // clean up
            location.destroyGameObject();
        }

        public static void testRemoveEntityId() {
            // prepare
            Location location = new Location(0, 0, 1);
            Entity entity = new RockEntity(new Vector3(0, 0, 0));
            location.addEntityId(entity.getId());

            // run
            location.removeEntityId(entity.getId());

            // verify
            Debug.Assert(location.getNumberOfEntities() == 0);

            // clean up
            location.destroyGameObject();
        }

        public static void testIsEntityPresent() {
            // prepare
            Location location = new Location(0, 0, 1);
            Entity entity = new RockEntity(new Vector3(0, 0, 0));
            location.addEntityId(entity.getId());

            // verify
            Debug.Assert(location.isEntityPresent(entity));

            // clean up
            location.destroyGameObject();
        }

        public static void testGetNumberOfEntities() {
            // prepare
            Location location = new Location(0, 0, 1);
            Entity entity1 = new RockEntity(new Vector3(0, 0, 0));
            Entity entity2 = new RockEntity(new Vector3(0, 0, 0));
            location.addEntityId(entity1.getId());
            location.addEntityId(entity2.getId());

            // verify
            Debug.Assert(location.getNumberOfEntities() == 2);

            // clean up
            location.destroyGameObject();
        }
    }

}