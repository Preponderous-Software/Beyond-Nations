using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace osg {

    public class Player : Entity {
        private Rigidbody rigidBody = null;
        private bool jumpKeyWasPressed = false;
        private float horizontalInput = 0;
        private float verticalInput = 0;
        private int walkSpeed;
        private int runSpeed;
        private int currentSpeed;
        private Camera playerCamera = null;
        private NationId nationId = null;
        private EntityId settlementId = null;
        private Status status = null;
        private bool autoWalk = false;
        private float energy = 100;
        private float metabolism = UnityEngine.Random.Range(0.001f, 0.010f);

        // map of entity id to integer representing relationship strength
        private Dictionary<EntityId, int> relationships = new Dictionary<EntityId, int>();

        public Player(int walkSpeed, int runSpeed, TickCounter tickCounter, int statusExpirationTicks, int renderDistance) : base(EntityType.PLAYER){
            createGameObject(new Vector3(0, 2, 0));
            setupCamera(renderDistance);
            this.rigidBody = getGameObject().GetComponent<Rigidbody>();
            this.walkSpeed = walkSpeed;
            this.runSpeed = runSpeed;
            status = new Status(tickCounter, statusExpirationTicks);
            this.currentSpeed = walkSpeed;
            getInventory().addItem(ItemType.COIN, UnityEngine.Random.Range(100, 400));
        }

        public void update() {
            horizontalInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical");

            if (Input.GetKey(KeyCode.LeftShift)) {
                currentSpeed = runSpeed;
            } else {
                currentSpeed = walkSpeed;
            }

            if (Input.GetKeyDown(KeyCode.Space)) {
                jumpKeyWasPressed = true;
            }
        }

        public void fixedUpdate() {
            if (horizontalInput != 0) {
                rigidBody.transform.Rotate(Vector3.up * horizontalInput * 2);
            }

            if (verticalInput != 0 && !autoWalk) {
                rigidBody.transform.Translate(Vector3.forward * verticalInput * currentSpeed * Time.deltaTime);
            }

            if (jumpKeyWasPressed) {
                jump();
                jumpKeyWasPressed = false;
            }

            if (autoWalk) {
                rigidBody.transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
            }

            if (energy < 90 && getInventory().getNumItems(ItemType.APPLE) > 0) {
                eatApple();
            }

            energy -= metabolism;
        }

        public Camera getCamera() {
            return playerCamera;
        }

        public bool isGrounded() {
            int minY = 0;
            int maxY = 2;
            return getGameObject().transform.position.y > minY && getGameObject().transform.position.y < maxY;
        }

        public override void createGameObject(Vector3 position) {
            GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            gameObject.transform.localScale = new Vector3(1, 1, 1);
            gameObject.transform.position = position;
            gameObject.name = "Player";
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

        public Status getStatus() {
            return status;
        }

        public void toggleAutoWalk() {
            autoWalk = !autoWalk;
        }

        public float getEnergy() {
            return energy;
        }

        public void setEnergy(float energy) {
            this.energy = energy;
        }

        public int getRenderDistance() {
            return (int) playerCamera.farClipPlane;
        }

        public void increaseRenderDistance() {
            playerCamera.farClipPlane += 10;

            int maxRenderDistance = 1000;
            if (playerCamera.farClipPlane > maxRenderDistance) {
                playerCamera.farClipPlane = maxRenderDistance;
            }
        }

        public void decreaseRenderDistance() {
            playerCamera.farClipPlane -= 10;

            int minRenderDistance = 50;
            if (playerCamera.farClipPlane < minRenderDistance) {
                playerCamera.farClipPlane = minRenderDistance;
            }
        }

        public Dictionary<EntityId, int> getRelationships() {
            return relationships;
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

        private void setupCamera(int renderDistance) {
            GameObject cameraObject = GameObject.Find("/Camera");      
            cameraObject.transform.SetParent(getGameObject().transform);
            cameraObject.transform.position = new Vector3(0, 5, -10);
            this.playerCamera = cameraObject.GetComponent<Camera>();
            this.playerCamera.farClipPlane = renderDistance;
        }

        private void jump() {
            if (isGrounded()) {
                rigidBody.AddForce(Vector3.up * 10, ForceMode.Impulse);
            }
        }

        private void eatApple() {
            getInventory().removeItem(ItemType.APPLE, 1);
            energy += 10;
        }
    }
}