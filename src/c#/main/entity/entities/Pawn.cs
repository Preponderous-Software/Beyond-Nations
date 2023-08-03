using UnityEngine;
using System.Collections.Generic;

namespace beyondnations {

    public class Pawn : Entity {
        private string name;
        private int speed = UnityEngine.Random.Range(10, 20);
        private NationId nationId;
        private EntityId homeSettlementId;
        private Entity targetEntity;
        private BehaviorType currentBehaviorType = BehaviorType.NONE;
        
        private int distanceThreshold = 10;

        private GameObject nameTag;
        private float energy = 100.00f;
        private float metabolism = UnityEngine.Random.Range(0.001f, 0.010f);

        // map of entity id to integer representing relationship strength
        private Dictionary<EntityId, int> relationships = new Dictionary<EntityId, int>();
        private EntityId currentSettlementId;

        public Pawn(Vector3 position, string name) : base(EntityType.PAWN) {
            this.name = name;
            createGameObject(position);
            int startingGoldCoins = UnityEngine.Random.Range(50, 200);
            getInventory().addItem(ItemType.COIN, startingGoldCoins);
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

        public EntityId getHomeSettlementId() {
            return homeSettlementId;
        }

        public void setHomeSettlementId(EntityId settlementId) {
            this.homeSettlementId = settlementId;
        }

        public Dictionary<EntityId, int> getRelationships() {
            return relationships;
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
                return;
            }
            if (targetEntity.isMarkedForDeletion()) {
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
            return isAtTargetEntity(distanceThreshold);
        }

        public bool isAtTargetEntity(int distanceThreshold) {
            if (targetEntity == null) {
                return false;
            }
            if (targetEntity.isMarkedForDeletion()) {
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

            bool toReturn = direction.magnitude < distanceThreshold;

            return toReturn;
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
            initializeNameTag();
        }

        public override void destroyGameObject() {
            UnityEngine.Object.Destroy(getGameObject());
        }

        public void setColor(Color color) {
            getGameObject().GetComponent<Renderer>().material.color = color;
        }

        public float getEnergy() {
            return energy;
        }

        public void setEnergy(float energy) {
            this.energy = energy;
        }

        private void initializeNameTag() {
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

        public BehaviorType getCurrentBehaviorType() {
            return currentBehaviorType;
        }

        public void setCurrentBehaviorType(BehaviorType currentBehaviorType) {
            this.currentBehaviorType = currentBehaviorType;
        }

        public void setNameTag(string name) {
            nameTag.GetComponent<TextMesh>().text = name;
        }

        public float getMetabolism() {
            return metabolism;
        }

        public void increaseRelationship(Entity entity, int amount) {
            if (entity == null) {
                Debug.LogError("entity is null in increaseRelationship()");
                return;
            }
            if (entity.getId() == getId()) {
                Debug.LogError("entity is self in increaseRelationship()");
                return;
            }
            if (getRelationships().ContainsKey(entity.getId())) {
                getRelationships()[entity.getId()] += amount;
            }
            else {
                getRelationships().Add(entity.getId(), amount);
            }
        }

        public void decreaseRelationship(Entity entity, int amount) {
            if (entity == null) {
                Debug.LogError("entity is null in decreaseRelationship()");
                return;
            }
            if (entity.getId() == getId()) {
                Debug.LogError("entity is self in decreaseRelationship()");
                return;
            }
            if (getRelationships().ContainsKey(entity.getId())) {
                getRelationships()[entity.getId()] -= amount;
            }
            else {
                getRelationships().Add(entity.getId(), -amount);
            }
        }

        public bool isCurrentlyInSettlement() {
            return currentSettlementId != null;
        }

        public EntityId getCurrentSettlementId() {
            return currentSettlementId;
        }

        public void setCurrentSettlementId(EntityId currentSettlementId) {
            this.currentSettlementId = currentSettlementId;
        }

        public void clearCurrentSettlementId() {
            currentSettlementId = null;
        }

        public string getCurrentBehaviorDescription() {
            if (getCurrentBehaviorType() == BehaviorType.NONE) {
                return "(doing nothing)";
            }
            else if (getCurrentBehaviorType() == BehaviorType.GATHER_RESOURCES) {
                return "(gathering resources)";
            }
            else if (getCurrentBehaviorType() == BehaviorType.SELL_RESOURCES) {
                return "(selling resources)";
            }
            else if (getCurrentBehaviorType() == BehaviorType.WANDER) {
                return "(wandering)";
            }
            else if (getCurrentBehaviorType() == BehaviorType.PURCHASE_FOOD) {
                return "(purchasing food)";
            }
            else if (getCurrentBehaviorType() == BehaviorType.CONSTRUCT_SETTLEMENT) {
                return "(creating settlementing)";
            }
            else if (getCurrentBehaviorType() == BehaviorType.GO_TO_HOME_SETTLEMENT) {
                return "(going home)";
            }
            else if (getCurrentBehaviorType() == BehaviorType.PLANT_SAPLING) {
                return "(planting sapling)";
            }
            else {
                return "(?)";
            }
        }
    }
}