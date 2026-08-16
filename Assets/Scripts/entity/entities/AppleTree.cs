using System;
using UnityEngine;
using Vector3 = System.Numerics.Vector3;

namespace beyondnations {

    public class AppleTree : Entity {
        private RandomSource random;
        private GameObject trunk;
        private GameObject leaves;
        private int height;
        
        public AppleTree(Vector3 position, int height, RandomSource random) : base(EntityType.TREE, "Tree") {
            this.random = random;
            this.height = height;
            createGameObject(position);
        }

        public override void createGameObject(Vector3 position) {
            GameObject gameObject = new GameObject();
            gameObject.transform.position = position;
            gameObject.name = "AppleTree";

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

            getInventory().addItem(ItemType.WOOD, random.range(3, 6));
            getInventory().addItem(ItemType.APPLE, random.range(0, 3));
            getInventory().addItem(ItemType.SAPLING, random.range(0, 3));
        }

        public override void destroyGameObject() {
            UnityEngine.Object.Destroy(getGameObject());
        }
    }
}