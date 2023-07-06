using System;
using UnityEngine;

namespace osg {

    public class AppleTree : Entity {
        private GameObject trunk;
        private GameObject leaves;
        private int height;
        
        public AppleTree(Vector3 position, int height) : base(EntityType.TREE) {
            this.height = height;
            createGameObject(position);
        }

        public override void createGameObject(Vector3 position) {
            GameObject gameObject = new GameObject();
            gameObject.transform.position = position;
            gameObject.name = "Tree";

            trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            trunk.transform.localScale = new Vector3(1, height, 1);
            trunk.GetComponent<Renderer>().material.color = new Color(0.5f, 0.25f, 0);
            trunk.transform.position = position;
            trunk.transform.parent = gameObject.transform;
            trunk.name = "Trunk";
            UnityEngine.Object.Destroy(trunk.GetComponent<CapsuleCollider>());

            leaves = GameObject.CreatePrimitive(PrimitiveType.Cube);
            leaves.transform.localScale = new Vector3(3, 3, 3);
            leaves.GetComponent<Renderer>().material.color = Color.green;
            leaves.transform.position = position + new Vector3(0, height - 1, 0);
            leaves.transform.parent = gameObject.transform;
            leaves.name = "Leaves";
            UnityEngine.Object.Destroy(leaves.GetComponent<BoxCollider>());
            
            setGameObject(gameObject);

            getInventory().addItem(ItemType.WOOD, UnityEngine.Random.Range(3, 6));
            getInventory().addItem(ItemType.APPLE, UnityEngine.Random.Range(0, 4));
            getInventory().addItem(ItemType.SAPLING, UnityEngine.Random.Range(0, 3));
        }

        public override void destroyGameObject() {
            UnityEngine.Object.Destroy(getGameObject());
        }
    }
}