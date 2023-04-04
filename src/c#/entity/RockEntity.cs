using UnityEngine;

namespace osg {

    public class RockEntity : Entity {
        private GameObject gameObject;
            
        public RockEntity(Vector3 position, ChunkId chunkId) : base(EntityType.ROCK, chunkId) {
            gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            gameObject.transform.localScale = new Vector3(1, 1, 1);
            gameObject.GetComponent<Renderer>().material.color = Color.gray;
            gameObject.transform.position = position;
            gameObject.name = "Rock";
        }

        public GameObject getGameObject() {
            return gameObject;
        }

    }
}