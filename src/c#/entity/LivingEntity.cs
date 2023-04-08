using UnityEngine;

namespace osg {

    class LivingEntity : Entity {
        private GameObject targetObject;
        private Inventory inventory = new Inventory();

        public LivingEntity(Vector3 position, ChunkId chunkId) : base(EntityType.LIVING, chunkId) {
            createGameObject(position);
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
            Vector3 currentPosition = getGameObject().transform.position;
            Vector3 direction = targetPosition - currentPosition;
            direction.Normalize();
            getGameObject().GetComponent<Rigidbody>().velocity = direction * getSpeed();
        }

        public bool isAtTargetObject() {
            Vector3 targetPosition = targetObject.transform.position;
            Vector3 currentPosition = getGameObject().transform.position;
            Vector3 direction = targetPosition - currentPosition;
            return direction.magnitude < 10;
        }

        public Inventory getInventory() {
            return inventory;
        }

        public override void createGameObject(Vector3 position) {
            GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            gameObject.transform.localScale = new Vector3(1, 1, 1);
            gameObject.GetComponent<Renderer>().material.color = Color.gray;
            gameObject.transform.position = position;
            gameObject.name = "LivingEntity";

            // add rigidbody
            Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>();

            // lock rotation
            rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

            setGameObject(gameObject);
        }

        public override void destroyGameObject() {
            UnityEngine.Object.Destroy(getGameObject());
        }
    }
}