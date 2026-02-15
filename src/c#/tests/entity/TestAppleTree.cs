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
            Transform trunkTransform = tree.getGameObject().transform.GetChild(0);
            Transform leavesTransform = tree.getGameObject().transform.GetChild(1);
            
            MeshFilter trunkMeshFilter = trunkTransform.GetComponent<MeshFilter>();
            MeshRenderer trunkMeshRenderer = trunkTransform.GetComponent<MeshRenderer>();
            MeshFilter leavesMeshFilter = leavesTransform.GetComponent<MeshFilter>();
            MeshRenderer leavesMeshRenderer = leavesTransform.GetComponent<MeshRenderer>();
            
            // Components should exist
            UnityEngine.Debug.Assert(trunkMeshFilter != null);
            UnityEngine.Debug.Assert(trunkMeshRenderer != null);
            UnityEngine.Debug.Assert(leavesMeshFilter != null);
            UnityEngine.Debug.Assert(leavesMeshRenderer != null);
            
            // Meshes should be assigned
            UnityEngine.Debug.Assert(trunkMeshFilter.mesh != null);
            UnityEngine.Debug.Assert(leavesMeshFilter.mesh != null);
            
            // Materials should be assigned and have expected colors
            UnityEngine.Debug.Assert(trunkMeshRenderer.material != null);
            UnityEngine.Debug.Assert(leavesMeshRenderer.material != null);
            UnityEngine.Debug.Assert(trunkMeshRenderer.material.color == new Color(0.5f, 0.25f, 0));
            UnityEngine.Debug.Assert(leavesMeshRenderer.material.color == Color.green);

            // clean up
            GameObject.Destroy(tree.getGameObject());
        }
    }
}