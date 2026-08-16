using System;
using UnityEngine;
using Vector3 = System.Numerics.Vector3;

namespace beyondnations {

    public class Sapling : Entity {
        private RandomSource random;
        private GameObject trunk;
        private GameObject leaves;
        private int height;
        
        private DateTime planted;
        private int growTime;
        
        public Sapling(Vector3 position, int height, RandomSource random) : base(EntityType.SAPLING, "Sapling") {
            this.random = random;
            this.height = height;
            createGameObject(position);
            planted = DateTime.Now;
            growTime = random.range(60, 600);
        }

        public override void createGameObject(Vector3 position) {
            GameObject gameObject = new GameObject();
            gameObject.transform.position = position;
            gameObject.name = "Sapling";

            trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            trunk.transform.localScale = new Vector3(0.5f, height, 0.5f);
            trunk.GetComponent<Renderer>().material.color = new Color(0.5f, 0.25f, 0);
            trunk.transform.position = position;
            trunk.transform.parent = gameObject.transform;
            UnityEngine.Object.Destroy(trunk.GetComponent<CapsuleCollider>());
            trunk.name = "Trunk";

            leaves = GameObject.CreatePrimitive(PrimitiveType.Cube);
            leaves.transform.localScale = new Vector3(1, 1, 1);
            leaves.GetComponent<Renderer>().material.color = Color.green;
            leaves.transform.position = position + new Vector3(0, height - 1, 0);
            leaves.transform.parent = gameObject.transform;
            UnityEngine.Object.Destroy(leaves.GetComponent<BoxCollider>());
            leaves.name = "Leaves";
            
            setGameObject(gameObject);

            getInventory().addItem(ItemType.WOOD, random.range(3, 6));
            getInventory().addItem(ItemType.APPLE, random.range(2, 4));
            getInventory().addItem(ItemType.SAPLING, random.range(1, 3));
        }

        public override void destroyGameObject() {
            UnityEngine.Object.Destroy(getGameObject());
        }
        
        public bool isGrown() {
            return DateTime.Now.Subtract(planted).TotalSeconds > growTime;
        }
    }
}