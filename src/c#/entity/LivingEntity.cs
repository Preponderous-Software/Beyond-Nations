using UnityEngine;

namespace osg {

    class LivingEntity : Entity {
        private string name;
        private NationId nationId;
        private Entity targetEntity;
        private Inventory inventory = new Inventory();

        public LivingEntity(Vector3 position, ChunkId chunkId) : base(EntityType.LIVING, chunkId) {
            createGameObject(position);
            name = "LivingEntity";
        }

        public string getName() {
            return name;
        }

        public int getSpeed() {
            return 20;
        }

        public NationId getNationId() {
            return nationId;
        }

        public void setNationId(NationId nationId) {
            this.nationId = nationId;
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
            if (targetEntity == null) {
                return false;
            }
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
            Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>();
            rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            setGameObject(gameObject);
        }

        public override void destroyGameObject() {
            UnityEngine.Object.Destroy(getGameObject());
        }

        public void fixedUpdate(Environment environment, Player player) {
            int targetNumWood = 5;
            int targetNumStone = 3;
            if (inventory.getNumWood() < targetNumWood) {
                Entity nearestTree = environment.getNearestTree(getGameObject().transform.position);
                if (nearestTree == null) {
                    return;
                }
                setTargetEntity(nearestTree);
            }
            else if (inventory.getNumStone() < targetNumStone) {
                Entity nearestRock = environment.getNearestRock(getGameObject().transform.position);
                if (nearestRock == null) {
                    return;
                }
                setTargetEntity(nearestRock);
            }
            else {
                setTargetEntity(player);
            }

            if (hasTargetEntity()) {
                if (!isAtTargetEntity()) {
                    moveTowardsTargetEntity();
                }
                else {
                    getGameObject().GetComponent<Rigidbody>().velocity = Vector3.zero;
                    if (targetEntity.getType() == EntityType.TREE) {
                        getTargetEntity().markForDeletion();
                        setTargetEntity(null);
                        inventory.addWood(1);
                    }
                    else if (targetEntity.getType() == EntityType.ROCK) {
                        getTargetEntity().markForDeletion();
                        setTargetEntity(null);
                        inventory.addStone(1);
                    }
                }
            }
            else {
                getGameObject().GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
        }

        public void setColor(Color color) {
            getGameObject().GetComponent<Renderer>().material.color = color;
        }
    }
}