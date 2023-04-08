using UnityEngine;

namespace osg {

    class LivingEntity : Entity {
        private GameObject gameObject;
        private GameObject targetObject;
        private Inventory inventory = new Inventory();

        public LivingEntity(Vector3 position, ChunkId chunkId) : base(EntityType.LIVING, chunkId) {
            gameObject = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            gameObject.transform.localScale = new Vector3(1, 1, 1);
            gameObject.GetComponent<Renderer>().material.color = Color.gray;
            gameObject.transform.position = position;
            gameObject.name = "LivingEntity";

            // add rigidbody
            Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>();

            // lock rotation
            rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        }

        public GameObject getGameObject() {
            return gameObject;
        }

        public int getSpeed() {
            return 20;
        }

        public bool hasTargetObject() {
            return targetObject != null;
        }

        public GameObject getTargetObject() {
            return targetObject;
        }

        public void setTargetObject(GameObject targetObject) {
            this.targetObject = targetObject;
        }

        public void moveTowardsTargetObject() {
            Vector3 targetPosition = targetObject.transform.position;
            Vector3 currentPosition = gameObject.transform.position;
            Vector3 direction = targetPosition - currentPosition;
            direction.Normalize();
            gameObject.GetComponent<Rigidbody>().velocity = direction * getSpeed();
        }

        public bool isAtTargetObject() {
            Vector3 targetPosition = targetObject.transform.position;
            Vector3 currentPosition = gameObject.transform.position;
            Vector3 direction = targetPosition - currentPosition;
            return direction.magnitude < 10;
        }

        public Inventory getInventory() {
            return inventory;
        }
    }
}