using UnityEngine;

namespace osg {

    public class Pawn : Entity {
        private string name;
        private int speed = Random.Range(5, 20);
        private NationId nationId;
        private Entity targetEntity;
        private Inventory inventory;
        private BehaviorType currentBehaviorType = BehaviorType.NONE;

        public Pawn(Vector3 position, string name) : base(EntityType.PAWN) {
            this.name = name;
            createGameObject(position);
            int startingGoldCoins = Random.Range(500, 2000);
            this.inventory = new Inventory(startingGoldCoins);

            // create text object above head
            GameObject textObject = new GameObject();
            textObject.transform.parent = getGameObject().transform;
            textObject.transform.localPosition = new Vector3(0, 2, 0);
            TextMesh textMesh = textObject.AddComponent<TextMesh>();
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
            gameObject.name = getName();
            Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>();
            rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            setGameObject(gameObject);
        }

        public override void destroyGameObject() {
            UnityEngine.Object.Destroy(getGameObject());
        }

        public void fixedUpdate(Environment environment, NationRepository nationRepository) {
            if (currentBehaviorType == BehaviorType.NONE) {
                computeBehaviorType();
            }
            else if (currentBehaviorType == BehaviorType.GATHER_RESOURCES) {
                gatherResources(environment);
            }
            else if (currentBehaviorType == BehaviorType.SELL_RESOURCES) {
                sellResources(environment, nationRepository);
            }
        }

        private void gatherResources(Environment environment) {
            if (!hasTargetEntity()) {
                // select nearest tree or rock
                int targetNumWood = 3;
                int targetNumStone = 2;

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
                    inventory.addItem(ItemType.WOOD, 2);
                }
                else if (targetEntity.getType() == EntityType.ROCK) {
                    targetEntity.markForDeletion();
                    setTargetEntity(null);
                    inventory.addItem(ItemType.STONE, 1);
                }
            }
            else {
                moveTowardsTargetEntity();
            }
        }

        private void sellResources(Environment environment, NationRepository nationRepository) {
            if (!hasTargetEntity()) {
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

        public void setColor(Color color) {
            getGameObject().GetComponent<Renderer>().material.color = color;
        }

        private void computeBehaviorType() {
            if (inventory.getNumItems(ItemType.WOOD) >= 3 && inventory.getNumItems(ItemType.STONE) >= 2) {
                currentBehaviorType = BehaviorType.SELL_RESOURCES;
            }
            else {
                currentBehaviorType = BehaviorType.GATHER_RESOURCES;
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

        public bool isLeaderOfNation(Nation nation) {
            return nation.getLeaderId() == getId();
        }
    }

    public enum BehaviorType {
        NONE,
        GATHER_RESOURCES,
        SELL_RESOURCES
    }
}