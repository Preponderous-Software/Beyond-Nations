using UnityEngine;

using osg;

namespace osgtests {

    public static class TestTree {

        public static void runTests() {
            testInstantiation();
        }

        public static void testInstantiation() {
            // run
            int height = 5;
            AppleTree tree = new AppleTree(new Vector3(0, 0, 0), height);

            // check
            UnityEngine.Debug.Assert(tree.getType() == EntityType.TREE);
            UnityEngine.Debug.Assert(tree.getGameObject().name == "Tree");
            UnityEngine.Debug.Assert(tree.getGameObject().transform.position == new Vector3(0, 0, 0));
            UnityEngine.Debug.Assert(tree.getGameObject().transform.localScale == new Vector3(1, 1, 1));
            UnityEngine.Debug.Assert(tree.getGameObject().transform.childCount == 2);
            UnityEngine.Debug.Assert(tree.getGameObject().transform.GetChild(0).name == "Trunk");
            UnityEngine.Debug.Assert(tree.getGameObject().transform.GetChild(0).transform.localScale == new Vector3(1, height, 1));
            UnityEngine.Debug.Assert(tree.getGameObject().transform.GetChild(0).GetComponent<Renderer>().material.color == new Color(0.5f, 0.25f, 0));
            UnityEngine.Debug.Assert(tree.getGameObject().transform.GetChild(1).name == "Leaves");
            UnityEngine.Debug.Assert(tree.getGameObject().transform.GetChild(1).transform.localScale == new Vector3(3, 3, 3));
            UnityEngine.Debug.Assert(tree.getGameObject().transform.GetChild(1).GetComponent<Renderer>().material.color == Color.green);

            // clean up
            GameObject.Destroy(tree.getGameObject());
        }
    }
}