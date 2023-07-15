using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace osg {

    public class Sapling : Entity {
        private GameObject trunk;
        private GameObject leaves;
        private int height;
        
        private DateTime planted;
        private int growTime;
        
        public Sapling(Vector3 position, int height) : base(EntityType.SAPLING) {
            this.height = height;
            createGameObject(position);
            planted = DateTime.Now;
            growTime = UnityEngine.Random.Range(60, 600);
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

            getInventory().addItem(ItemType.WOOD, UnityEngine.Random.Range(3, 6));
            getInventory().addItem(ItemType.APPLE, UnityEngine.Random.Range(2, 4));
            getInventory().addItem(ItemType.SAPLING, UnityEngine.Random.Range(1, 3));
        }

        public override void destroyGameObject() {
            UnityEngine.Object.Destroy(getGameObject());
        }
        
        public bool isGrown() {
            return DateTime.Now.Subtract(planted).TotalSeconds > growTime;
        }
    }
}