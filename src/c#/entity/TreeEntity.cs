using UnityEngine;

namespace osg {

    public class TreeEntity : Entity {
        private GameObject gameObject;
        private GameObject trunk;
        private GameObject leaves;
        
        public TreeEntity(Vector3 position, int height, ChunkId chunkId) : base("Tree", chunkId) {
            gameObject = new GameObject();
            gameObject.transform.position = position;
            gameObject.name = "Tree";

            trunk = GameObject.CreatePrimitive(PrimitiveType.Cube);
            trunk.transform.localScale = new Vector3(1, height, 1);
            trunk.GetComponent<Renderer>().material.color = new Color(0.5f, 0.25f, 0);
            trunk.transform.position = position;
            trunk.transform.parent = gameObject.transform;
            trunk.name = "Trunk";

            leaves = GameObject.CreatePrimitive(PrimitiveType.Cube);
            leaves.transform.localScale = new Vector3(3, 3, 3);
            leaves.GetComponent<Renderer>().material.color = Color.green;
            leaves.transform.position = position + new Vector3(0, height - 1, 0);
            leaves.transform.parent = gameObject.transform;
            leaves.name = "Leaves";
        }

        public GameObject getGameObject() {
            return gameObject;
        }
    }
}