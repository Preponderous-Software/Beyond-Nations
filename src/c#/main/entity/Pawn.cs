using UnityEngine;

namespace osg {

    class Pawn : Entity {
        private string name;
        private int speed = Random.Range(5, 20);
        private NationId nationId;
        private Entity targetEntity;
        private Inventory inventory = new Inventory();

        // names
        public static string[] names = new string[] {
            "Bob",
            "Alice",
            "Charlie",
            "Dave",
            "Eve",
            "Frank",
            "Grace",
            "Hank",
            "Irene",
            "Judy",
            "Karl",
            "Linda",
            "Mike",
            "Nancy",
            "Oscar",
            "Peggy",
            "Quinn",
            "Ruth",
            "Stan",
            "Tina",
            "Ursula",
            "Victor",
            "Wendy",
            "Xavier",
            "Yvonne",
            "Zach"
        };

        public Pawn(Vector3 position, ChunkId chunkId) : base(EntityType.LIVING, chunkId) {
            createGameObject(position);
            name = names[Random.Range(0, names.Length)];
        }

        public string getName() {
            return name;
        }

        public int getSpeed() {
            return speed;
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
            gameObject.name = "Pawn";
            Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>();
            rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            setGameObject(gameObject);
        }

        public override void destroyGameObject() {
            UnityEngine.Object.Destroy(getGameObject());
        }

        public void fixedUpdate(Environment environment, NationRepository nationRepository) {
            int targetNumWood = 3;
            int targetNumStone = 2;
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
                Nation nation = nationRepository.getNation(nationId);
                if (nation == null) {
                    return;
                }
                EntityId leaderId = nation.getLeaderId();
                Entity leader = environment.getEntity(leaderId);
                setTargetEntity(leader);
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
                    else if (targetEntity.getType() == EntityType.LIVING) {
                        // deposit resources
                        Pawn pawn = (Pawn)targetEntity;
                        pawn.getInventory().addWood(inventory.getNumWood());
                        pawn.getInventory().addStone(inventory.getNumStone());
                        inventory.setNumWood(0);
                        inventory.setNumStone(0);
                        setTargetEntity(null);
                    }
                    else if (targetEntity.getType() == EntityType.PLAYER) {
                        // deposit resources
                        Player player = (Player)targetEntity;
                        player.getInventory().addWood(inventory.getNumWood());
                        player.getInventory().addStone(inventory.getNumStone());
                        inventory.setNumWood(0);
                        inventory.setNumStone(0);
                        setTargetEntity(null);
                    }
                    else {
                        setTargetEntity(null);
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