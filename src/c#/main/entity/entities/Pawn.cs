using UnityEngine;

namespace osg {

    public class Pawn : Entity {
        private string name;
        private int speed = Random.Range(5, 20);
        private NationId nationId;
        private Entity targetEntity;
        private Inventory inventory;

        public Pawn(Vector3 position, string name) : base(EntityType.PAWN) {
            this.name = name;
            createGameObject(position);
            int startingGoldCoins = Random.Range(0, 100);
            this.inventory = new Inventory(startingGoldCoins);
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
            selectTarget(environment, nationRepository);

            if (hasTargetEntity()) {
                if (!isAtTargetEntity()) {
                    moveTowardsTargetEntity();
                }
                else {
                    interactWithTargetEntity();
                }
            }
            else {
                getGameObject().GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
        }

        public void setColor(Color color) {
            getGameObject().GetComponent<Renderer>().material.color = color;
        }

        private void selectTarget(Environment environment, NationRepository nationRepository) {
            int targetNumWood = 3;
            int targetNumStone = 2;

            if (inventory.getNumItems(ItemType.WOOD) < targetNumWood) {
                Entity nearestTree = environment.getNearestTree(getGameObject().transform.position);
                if (nearestTree == null) {
                    return;
                }
                setTargetEntity(nearestTree);
            }
            else if (inventory.getNumItems(ItemType.STONE) < targetNumStone) {
                Entity nearestRock = environment.getNearestRock(getGameObject().transform.position);
                if (nearestRock == null) {
                    return;
                }
                setTargetEntity(nearestRock);
            }
            else {
                if (getNationId() == null) {
                    return;
                }
                Nation nation = nationRepository.getNation(getNationId());
                if (nation == null) {
                    return;
                }
                EntityId leaderId = nation.getLeaderId();
                Entity leader = environment.getEntity(leaderId);
                setTargetEntity(leader);
            }
        }

        private void interactWithTargetEntity() {
            getGameObject().GetComponent<Rigidbody>().velocity = Vector3.zero;
            if (targetEntity.getType() == EntityType.TREE) {
                getTargetEntity().markForDeletion();
                setTargetEntity(null);
                inventory.addItem(ItemType.WOOD, 2);
            }
            else if (targetEntity.getType() == EntityType.ROCK) {
                getTargetEntity().markForDeletion();
                setTargetEntity(null);
                inventory.addItem(ItemType.STONE, 1);
            }
            else if (targetEntity.getType() == EntityType.PAWN) {
                Pawn targetPawn = (Pawn) targetEntity;
                attemptToSellResourcesTo(targetPawn);
            }
            else if (targetEntity.getType() == EntityType.PLAYER) {
                Player targetPlayer = (Player) targetEntity;
                attemptToSellResourcesTo(targetPlayer);
            }
            else {
                setTargetEntity(null);
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