using UnityEngine;

namespace osg {

    public class Rock : Entity {
            
        public Rock(Vector3 position) : base(EntityType.ROCK) {
            createGameObject(position);
        }

        public override void createGameObject(Vector3 position) {
            GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            gameObject.transform.localScale = new Vector3(1, 1, 1);
            gameObject.GetComponent<Renderer>().material.color = Color.gray;
            gameObject.transform.position = position;
            gameObject.name = "Rock";
            UnityEngine.Object.Destroy(gameObject.GetComponent<BoxCollider>());
            setGameObject(gameObject);

            getInventory().addItem(ItemType.STONE, 1);
        }

        public override void destroyGameObject() {
            UnityEngine.Object.Destroy(getGameObject());
        }

    }
}