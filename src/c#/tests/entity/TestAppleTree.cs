using UnityEngine;

using beyondnations;

namespace beyondnationstests {

    public static class TestTree {

        public static void runTests() {
            testInstantiation();
        }

        public static void testInstantiation() {
            // run
            int height = 5;
            AppleTree tree = new AppleTree(new Vector3(0, 0, 0), height);

            // check - verify tree is created with proper structure
            UnityEngine.Debug.Assert(tree.getType() == EntityType.TREE);
            UnityEngine.Debug.Assert(tree.getGameObject().name == "AppleTree");
            UnityEngine.Debug.Assert(tree.getGameObject().transform.position == new Vector3(0, 0, 0));
            UnityEngine.Debug.Assert(tree.getGameObject().transform.childCount == 2);
            UnityEngine.Debug.Assert(tree.getGameObject().transform.GetChild(0).name == "Trunk");
            UnityEngine.Debug.Assert(tree.getGameObject().transform.GetChild(1).name == "Leaves");
            
            // Verify trunk and leaves have MeshFilter and MeshRenderer components (3D model)
            UnityEngine.Debug.Assert(tree.getGameObject().transform.GetChild(0).GetComponent<MeshFilter>() != null);
            UnityEngine.Debug.Assert(tree.getGameObject().transform.GetChild(0).GetComponent<MeshRenderer>() != null);
            UnityEngine.Debug.Assert(tree.getGameObject().transform.GetChild(1).GetComponent<MeshFilter>() != null);
            UnityEngine.Debug.Assert(tree.getGameObject().transform.GetChild(1).GetComponent<MeshRenderer>() != null);

            // clean up
            GameObject.Destroy(tree.getGameObject());
        }
    }
}