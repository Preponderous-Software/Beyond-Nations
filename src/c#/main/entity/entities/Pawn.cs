using UnityEngine;

namespace osg {

    public class Pawn : Entity {
        private string name;
        private int speed = Random.Range(10, 20);
        private NationId nationId;
        private Entity targetEntity;
        private Inventory inventory;
        private BehaviorType currentBehaviorType = BehaviorType.NONE;

        private int targetNumWood = 3;
        private int targetNumStone = 3;
        
        private int distanceThreshold = 10;

        private GameObject nameTag;

        public Pawn(Vector3 position, string name) : base(EntityType.PAWN) {
            this.name = name;
            createGameObject(position);
            int startingGoldCoins = Random.Range(50, 200);
            this.inventory = new Inventory(startingGoldCoins);

            // create text object above head
            nameTag = new GameObject();
            nameTag.transform.parent = getGameObject().transform;
            nameTag.transform.localPosition = new Vector3(0, 2, 0);
            TextMesh textMesh = nameTag.AddComponent<TextMesh>();
            textMesh.text = getName();
            textMesh.fontSize = 64;
            textMesh.color = Color.black;
            textMesh.alignment = TextAlignment.Center;
            textMesh.anchor = TextAnchor.MiddleCenter;
            textMesh.characterSize = 0.1f;
            textMesh.GetComponent<Renderer>().material.color = Color.black;
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
            if (targetEntity == null) {
                Debug.LogWarning("target entity is null in moveTowardsTargetEntity()");
                return;
            }
            if (targetEntity.isMarkedForDeletion()) {
                Debug.LogWarning("target entity is marked for deletion in moveTowardsTargetEntity()");
                setTargetEntity(null);
                return;
            }
            if (targetEntity.getGameObject() == null) {
                setTargetEntity(null);
                Debug.LogWarning("target entity game object is null in moveTowardsTargetEntity()");
                return;
            }
            Vector3 targetPosition = targetEntity.getGameObject().transform.position;
            Vector3 currentPosition = getGameObject().transform.position;
            Vector3 direction = targetPosition - currentPosition;
            direction.Normalize();
            getGameObject().GetComponent<Rigidbody>().velocity = direction * getSpeed();
        }

        public bool isAtTargetEntity() {
            if (targetEntity == null) {
                Debug.LogWarning("target entity is null in isAtTargetEntity()");
                return false;
            }
            if (targetEntity.isMarkedForDeletion()) {
                Debug.LogWarning("target entity is marked for deletion in isAtTargetEntity()");
                setTargetEntity(null);
                return false;
            }
            if (targetEntity.getGameObject() == null) {
                setTargetEntity(null);
                Debug.LogWarning("target entity game object is null in isAtTargetEntity()");
                return false;
            }
            Vector3 targetPosition = targetEntity.getGameObject().transform.position;
            Vector3 currentPosition = getGameObject().transform.position;
            Vector3 direction = targetPosition - currentPosition;
            return direction.magnitude < distanceThreshold;
        }

        public Inventory getInventory() {
            return inventory;
        }

        public override void createGameObject(Vector3 position) {
            GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            gameObject.transform.localScale = new Vector3(1, 1, 1);
            gameObject.GetComponent<Renderer>().material.color = Color.gray;
            gameObject.transform.position = position;
            gameObject.name = getName();
            Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>();
            rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            setGameObject(gameObject);
        }

        public override void destroyGameObject() {
            UnityEngine.Object.Destroy(getGameObject());
        }

        public void fixedUpdate(Environment environment, NationRepository nationRepository) {
            computeBehaviorType(environment, nationRepository);

            if (currentBehaviorType == BehaviorType.GATHER_RESOURCES) {
                gatherResources(environment);
            }
            else if (currentBehaviorType == BehaviorType.SELL_RESOURCES) {
                sellResources(environment, nationRepository);
            }
            else if (currentBehaviorType == BehaviorType.WANDER) {
                wander(environment);
            }
            else if (currentBehaviorType == BehaviorType.NONE) {
                // do nothing
            }
            else {
                Debug.LogWarning("unknown behavior type in fixedUpdate(): " + currentBehaviorType);
            }
        }

        public void setColor(Color color) {
            getGameObject().GetComponent<Renderer>().material.color = color;
        }

        private void gatherResources(Environment environment) {
            if (!hasTargetEntity()) {
                // select nearest tree or rock

                Entity nearestTree = environment.getNearestTree(getGameObject().transform.position);
                Entity nearestRock = environment.getNearestRock(getGameObject().transform.position);
                if (nearestTree == null && nearestRock == null) {
                    return;
                }
                if (nearestTree == null) {
                    setTargetEntity(nearestRock);
                }
                else if (nearestRock == null) {
                    setTargetEntity(nearestTree);
                }
                else {
                    float distanceToTree = Vector3.Distance(getGameObject().transform.position, nearestTree.getGameObject().transform.position);
                    float distanceToRock = Vector3.Distance(getGameObject().transform.position, nearestRock.getGameObject().transform.position);
                    if (distanceToTree < distanceToRock) {
                        setTargetEntity(nearestTree);
                    }
                    else {
                        setTargetEntity(nearestRock);
                    }
                }
            }
            else if (isAtTargetEntity()) {
                // gather
                if (targetEntity.getType() == EntityType.TREE) {
                    targetEntity.markForDeletion();
                    setTargetEntity(null);
                    inventory.addItem(ItemType.WOOD, 1);
                }
                else if (targetEntity.getType() == EntityType.ROCK) {
                    targetEntity.markForDeletion();
                    setTargetEntity(null);
                    inventory.addItem(ItemType.STONE, 1);
                }
                else {
                    Debug.LogWarning("target entity is not a tree or rock");
                    setTargetEntity(null);
                }
            }
            else {
                moveTowardsTargetEntity();
            }
        }

        private void sellResources(Environment environment, NationRepository nationRepository) {
            if (!hasTargetEntity() && getNationId() != null) {
                // target nation leader
                Nation nation = nationRepository.getNation(getNationId());
                EntityId leaderId = nation.getLeaderId();
                Entity leader = environment.getEntity(leaderId);
                setTargetEntity(leader);
            }
            else if (isAtTargetEntity()) {
                // sell
                attemptToSellResourcesTo(getTargetEntity());
            }
            else {
                moveTowardsTargetEntity();
            }
        }

        private void wander(Environment environment) {
            Vector3 currentPosition = getGameObject().transform.position;
            Vector3 targetPosition = currentPosition + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
            getGameObject().GetComponent<Rigidbody>().velocity = (targetPosition - currentPosition).normalized * getSpeed();
        }

        private void computeBehaviorType(Environment environment, NationRepository nationRepository) {
            // if leader
            if (getNationId() == null) {
                currentBehaviorType = BehaviorType.WANDER;
                return;
            }
            Nation nation = nationRepository.getNation(getNationId());
            NationRole role = nation.getRole(getId());
            if (role == NationRole.LEADER) {
                currentBehaviorType = BehaviorType.WANDER;
                return;
            }
            else if (role == NationRole.CITIZEN) {
                if (inventory.getNumItems(ItemType.WOOD) >= targetNumWood && inventory.getNumItems(ItemType.STONE) >= targetNumStone) {
                    currentBehaviorType = BehaviorType.SELL_RESOURCES;
                }
                else {
                    currentBehaviorType = BehaviorType.GATHER_RESOURCES;
                }
            }

        }

        private void attemptToSellResourcesTo(Entity targetEntity) {
            int numWood = inventory.getNumItems(ItemType.WOOD);
            int numStone = inventory.getNumItems(ItemType.STONE);

            if (targetEntity.getType() == EntityType.PAWN) {
                Pawn targetPawn = (Pawn) targetEntity;

                int cost = numWood * 2 + numStone * 3;
                if (targetPawn.getInventory().getNumItems(ItemType.GOLD_COIN) >= cost) {
                    targetPawn.getInventory().removeItem(ItemType.GOLD_COIN, cost);
                    inventory.removeItem(ItemType.WOOD, numWood);
                    inventory.removeItem(ItemType.STONE, numStone);
                    targetPawn.getInventory().addItem(ItemType.WOOD, numWood);
                    targetPawn.getInventory().addItem(ItemType.STONE, numStone);
                }
                else {
                    setTargetEntity(null);
                }
            }
            else if (targetEntity.getType() == EntityType.PLAYER) {
                Player targetPlayer = (Player) targetEntity;

                int cost = numWood * 2 + numStone * 3;
                if (targetPlayer.getInventory().getNumItems(ItemType.GOLD_COIN) >= cost) {
                    targetPlayer.getInventory().removeItem(ItemType.GOLD_COIN, cost);
                    inventory.removeItem(ItemType.WOOD, numWood);
                    inventory.removeItem(ItemType.STONE, numStone);
                    targetPlayer.getInventory().addItem(ItemType.WOOD, numWood);
                    targetPlayer.getInventory().addItem(ItemType.STONE, numStone);
                    targetPlayer.getStatus().update("You bought " + numWood + " wood and " + numStone + " stone from " + getName() + " for " + cost + " gold coins.");
                }
                else {
                    setTargetEntity(null);
                }
            }
        }
    }
}