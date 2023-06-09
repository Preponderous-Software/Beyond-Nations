using UnityEngine;
using System.Collections.Generic;

namespace osg {

    public class Pawn : Entity {
        private string name;
        private int speed = Random.Range(10, 20);
        private NationId nationId;
        private EntityId settlementId;
        private Entity targetEntity;
        private BehaviorType currentBehaviorType = BehaviorType.NONE;

        private int targetNumWood = 3;
        private int targetNumStone = 3;
        private int targetNumApples = 3;
        
        private int distanceThreshold = 10;

        private GameObject nameTag;
        private float energy = 100.00f;
        private float metabolism = Random.Range(0.01f, 0.10f);

        // map of entity id to integer representing relationship strength
        private Dictionary<EntityId, int> relationships = new Dictionary<EntityId, int>();

        public Pawn(Vector3 position, string name) : base(EntityType.PAWN) {
            this.name = name;
            createGameObject(position);
            int startingGoldCoins = Random.Range(50, 200);
            getInventory().addItem(ItemType.GOLD_COIN, startingGoldCoins);

            // create text object above head
            initializeNameTag();
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

        public EntityId getSettlementId() {
            return settlementId;
        }

        public void setSettlementId(EntityId settlementId) {
            this.settlementId = settlementId;
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
    }
}