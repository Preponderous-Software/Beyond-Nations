using UnityEngine;

using osg;

namespace osgtests {

    public static class TestPawn {

        public static void runTests() {
            testInstantiation();
            testGetSpeed();
            testHasTargetEntity();
            testGetTargetEntity();
            testSetTargetEntity();
        }

        public static void testInstantiation() {
            // run
            Pawn pawn = new Pawn(new Vector3(0, 0, 0), "Pawn");

            // check
            UnityEngine.Debug.Assert(pawn.getType() == EntityType.PAWN);
            UnityEngine.Debug.Assert(pawn.getGameObject().name == "Pawn");
            UnityEngine.Debug.Assert(pawn.getGameObject().transform.position == new Vector3(0, 0, 0));
            UnityEngine.Debug.Assert(pawn.getGameObject().transform.localScale == new Vector3(1, 1, 1));
            UnityEngine.Debug.Assert(pawn.getGameObject().GetComponent<Renderer>().material.color == Color.gray);

            // clean up
            GameObject.Destroy(pawn.getGameObject());
        }

        public static void testGetSpeed() {
            // run
            Pawn pawn = new Pawn(new Vector3(0, 0, 0), "Pawn");

            // check
            UnityEngine.Debug.Assert(pawn.getSpeed() > 0);

            // clean up
            GameObject.Destroy(pawn.getGameObject());
        }

        public static void testHasTargetEntity() {
            // run
            Pawn pawn = new Pawn(new Vector3(0, 0, 0), "Pawn");

            // check
            UnityEngine.Debug.Assert(!pawn.hasTargetEntity());

            // clean up
            GameObject.Destroy(pawn.getGameObject());
        }

        public static void testGetTargetEntity() {
            // run
            Pawn pawn = new Pawn(new Vector3(0, 0, 0), "Pawn");

            // check
            UnityEngine.Debug.Assert(pawn.getTargetEntity() == null);

            // clean up
            GameObject.Destroy(pawn.getGameObject());
        }

        public static void testSetTargetEntity() {
            // run
            Pawn pawn = new Pawn(new Vector3(0, 0, 0), "Pawn");
            Pawn targetEntity = new Pawn(new Vector3(0, 0, 0), "Target Entity");
            pawn.setTargetEntity(targetEntity);

            // check
            UnityEngine.Debug.Assert(pawn.getTargetEntity() == targetEntity);

            // clean up
            GameObject.Destroy(pawn.getGameObject());
            GameObject.Destroy(targetEntity.getGameObject());
        }
    }
}