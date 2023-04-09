using UnityEngine;

using osg;

namespace osgtests {

    public static class TestLivingEntity {

        public static void runTests() {
            testInstantiation();
            testGetSpeed();
            testHasTargetEntity();
            testGetTargetEntity();
            testSetTargetEntity();
        }

        public static void testInstantiation() {
            // run
            LivingEntity livingEntity = new LivingEntity(new Vector3(0, 0, 0), new ChunkId());

            // check
            Debug.Assert(livingEntity.getType() == EntityType.LIVING);
            Debug.Assert(livingEntity.getGameObject().name == "LivingEntity");
            Debug.Assert(livingEntity.getGameObject().transform.position == new Vector3(0, 0, 0));
            Debug.Assert(livingEntity.getGameObject().transform.localScale == new Vector3(1, 1, 1));
            Debug.Assert(livingEntity.getGameObject().GetComponent<Renderer>().material.color == Color.gray);

            // clean up
            GameObject.Destroy(livingEntity.getGameObject());
        }

        public static void testGetSpeed() {
            // run
            LivingEntity livingEntity = new LivingEntity(new Vector3(0, 0, 0), new ChunkId());

            // check
            Debug.Assert(livingEntity.getSpeed() > 0);

            // clean up
            GameObject.Destroy(livingEntity.getGameObject());
        }

        public static void testHasTargetEntity() {
            // run
            LivingEntity livingEntity = new LivingEntity(new Vector3(0, 0, 0), new ChunkId());

            // check
            Debug.Assert(!livingEntity.hasTargetEntity());

            // clean up
            GameObject.Destroy(livingEntity.getGameObject());
        }

        public static void testGetTargetEntity() {
            // run
            LivingEntity livingEntity = new LivingEntity(new Vector3(0, 0, 0), new ChunkId());

            // check
            Debug.Assert(livingEntity.getTargetEntity() == null);

            // clean up
            GameObject.Destroy(livingEntity.getGameObject());
        }

        public static void testSetTargetEntity() {
            // run
            LivingEntity livingEntity = new LivingEntity(new Vector3(0, 0, 0), new ChunkId());
            LivingEntity targetEntity = new LivingEntity(new Vector3(0, 0, 0), new ChunkId());
            livingEntity.setTargetEntity(targetEntity);

            // check
            Debug.Assert(livingEntity.getTargetEntity() == targetEntity);

            // clean up
            GameObject.Destroy(livingEntity.getGameObject());
            GameObject.Destroy(targetEntity.getGameObject());
        }
    }
}