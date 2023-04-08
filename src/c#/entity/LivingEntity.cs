using UnityEngine;

namespace osg {

    class LivingEntity : Entity {
        private Entity targetEntity;
        private Inventory inventory = new Inventory();

        public LivingEntity(Vector3 position, ChunkId chunkId) : base(EntityType.LIVING, chunkId) {
            createGameObject(position);
        }

        public int getSpeed() {
            return 20;
        }

        public bool hasTargetEntity() {
            return targetEntity != null;
        }

        public Entity getTargetEntity() {
            return targetEntity;
        }

        public void setTargetEntity(Entity targetEntity) {
            this.targetEntity = targetEntity;
        }

        public void moveTowardsTargetEntity() {
            Vector3 targetPosition = targetEntity.getGameObject().transform.position;
            Vector3 currentPosition = getGameObject().transform.position;
            Vector3 direction = targetPosition - currentPosition;
            direction.Normalize();
            getGameObject().GetComponent<Rigidbody>().velocity = direction * getSpeed();
        }

        public bool isAtTargetEntity() {
            Vector3 targetPosition = targetEntity.getGameObject().transform.position;
            Vector3 currentPosition = getGameObject().transform.position;
            Vector3 direction = targetPosition - currentPosition;
            int threshold = 5;
            return direction.magnitude < threshold;
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

        public void fixedUpdate(Environment environment) {
            // target nearest tree
            Entity nearestTree = environment.getNearestTree(getGameObject().transform.position);
            if (nearestTree == null) {
                return;
            }

            setTargetEntity(nearestTree);
            if (!isAtTargetEntity()) {
                moveTowardsTargetEntity();
            }
            else {
                getGameObject().GetComponent<Rigidbody>().velocity = Vector3.zero;
                getGameObject().GetComponent<Renderer>().material.color = Color.blue;
            }
        }
    }
}