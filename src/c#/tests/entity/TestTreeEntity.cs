using UnityEngine;

using osg;

namespace osgtests {

    public static class TestTreeEntity {

        public static void runTests() {
            Debug.Log("Running TreeEntity tests...");
            testInstantiation();
        }

        public static void testInstantiation() {
            // run
            int height = 5;
            TreeEntity treeEntity = new TreeEntity(new Vector3(0, 0, 0), height, new ChunkId());

            // check
            Debug.Assert(treeEntity.getType() == EntityType.TREE);
            Debug.Assert(treeEntity.getGameObject().name == "Tree");
            Debug.Assert(treeEntity.getGameObject().transform.position == new Vector3(0, 0, 0));
            Debug.Assert(treeEntity.getGameObject().transform.localScale == new Vector3(1, 1, 1));
            Debug.Assert(treeEntity.getGameObject().transform.childCount == 2);
            Debug.Assert(treeEntity.getGameObject().transform.GetChild(0).name == "Trunk");
            Debug.Assert(treeEntity.getGameObject().transform.GetChild(0).transform.localScale == new Vector3(1, height, 1));
            Debug.Assert(treeEntity.getGameObject().transform.GetChild(0).GetComponent<Renderer>().material.color == new Color(0.5f, 0.25f, 0));
            Debug.Assert(treeEntity.getGameObject().transform.GetChild(1).name == "Leaves");
            Debug.Assert(treeEntity.getGameObject().transform.GetChild(1).transform.localScale == new Vector3(3, 3, 3));
            Debug.Assert(treeEntity.getGameObject().transform.GetChild(1).GetComponent<Renderer>().material.color == Color.green);

            // clean up
            GameObject.Destroy(treeEntity.getGameObject());
        }
    }
}