using System;
using UnityEngine;

namespace osg {

    public class TreeEntity : Entity {
        private GameObject trunk;
        private GameObject leaves;
        private int height;
        
        public TreeEntity(Vector3 position, int height) : base(EntityType.TREE) {
            this.height = height;
            createGameObject(position);
        }

        public override void createGameObject(Vector3 position) {
            GameObject gameObject = new GameObject();
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
            
            setGameObject(gameObject);

            getInventory().addItem(ItemType.WOOD, UnityEngine.Random.Range(3, 6));
            getInventory().addItem(ItemType.APPLE, UnityEngine.Random.Range(2, 4));
        }

        public override void destroyGameObject() {
            UnityEngine.Object.Destroy(getGameObject());
        }
    }
}