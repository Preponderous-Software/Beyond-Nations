using UnityEngine;

using beyondnations;

namespace beyondnationstests {

    public static class TestRock {

        public static void runTests() {
            testInstantiation();
        }

        public static void testInstantiation() {
            // run
            Rock Rock = new Rock(new Vector3(0, 0, 0));

            // check
            UnityEngine.Debug.Assert(Rock.getType() == EntityType.ROCK);
            UnityEngine.Debug.Assert(Rock.getGameObject().name == "Rock");
            UnityEngine.Debug.Assert(Rock.getGameObject().transform.position == new Vector3(0, 0, 0));
            UnityEngine.Debug.Assert(Rock.getGameObject().transform.localScale == new Vector3(1, 1, 1));
            UnityEngine.Debug.Assert(Rock.getGameObject().GetComponent<Renderer>().material.color == Color.gray);

            // clean up
            GameObject.Destroy(Rock.getGameObject());
        }

    }
}