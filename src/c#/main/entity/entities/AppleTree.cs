using System;
using UnityEngine;

namespace beyondnations {

    public class AppleTree : Entity {
        private int height;
        
        public AppleTree(Vector3 position, int height) : base(EntityType.TREE, "Tree") {
            this.height = height;
            createGameObject(position);
        }

        public override void createGameObject(Vector3 position) {
            // Use the new 3D tree model instead of primitives
            GameObject gameObject = TreeModel.CreateTree(position, height);
            gameObject.name = "AppleTree";
            
            setGameObject(gameObject);

            getInventory().addItem(ItemType.WOOD, UnityEngine.Random.Range(3, 6));
            getInventory().addItem(ItemType.APPLE, UnityEngine.Random.Range(0, 3));
            getInventory().addItem(ItemType.SAPLING, UnityEngine.Random.Range(0, 3));
        }

        public override void destroyGameObject() {
            UnityEngine.Object.Destroy(getGameObject());
        }
    }
}