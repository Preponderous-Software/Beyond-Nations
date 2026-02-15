using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace beyondnations {

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
        private EntityId homeSettlementId = null;
        private Status status = null;
        private bool autoWalk = false;
        private float energy = 100;
        private float metabolism = UnityEngine.Random.Range(0.001f, 0.010f);

        // First-person camera variables
        private float mouseSensitivity = 2.0f;
        private float verticalRotation = 0f;
        private float maxVerticalAngle = 80f;

        // map of entity id to integer representing relationship strength
        private Dictionary<EntityId, int> relationships = new Dictionary<EntityId, int>();
        private EntityId currentSettlementId = null;

        public Player(int walkSpeed, int runSpeed, TickCounter tickCounter, int statusExpirationTicks, int renderDistance) : base(EntityType.PLAYER, "Player"){
            createGameObject(new Vector3(0, 2, 0));
            setupCamera(renderDistance);
            this.rigidBody = getGameObject().GetComponent<Rigidbody>();
            this.walkSpeed = walkSpeed;
            this.runSpeed = runSpeed;
            status = new Status(tickCounter, statusExpirationTicks);
            this.currentSpeed = walkSpeed;
            getInventory().addItem(ItemType.COIN, UnityEngine.Random.Range(100, 400));
            
            // Lock cursor for first-person view
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void update() {
            // Handle movement input
            horizontalInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical");

            // Handle mouse input for camera rotation immediately for responsive control
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            // Rotate player horizontally (Y-axis)
            if (mouseX != 0) {
                rigidBody.transform.Rotate(Vector3.up * mouseX);
            }

            // Rotate camera vertically (X-axis) with clamping
            if (mouseY != 0) {
                verticalRotation -= mouseY;
                verticalRotation = Mathf.Clamp(verticalRotation, -maxVerticalAngle, maxVerticalAngle);
                playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
            }

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
            if (isCurrentlyInSettlement()) {
                return;
            }
            
            // Use A/D for strafing instead of rotation
            if (horizontalInput != 0) {
                rigidBody.transform.Translate(Vector3.right * horizontalInput * currentSpeed * Time.fixedDeltaTime);
            }

            if (verticalInput != 0 && !autoWalk) {
                rigidBody.transform.Translate(Vector3.forward * verticalInput * currentSpeed * Time.fixedDeltaTime);
            }

            if (jumpKeyWasPressed) {
                jump();
                jumpKeyWasPressed = false;
            }

            if (autoWalk) {
                rigidBody.transform.Translate(Vector3.forward * currentSpeed * Time.fixedDeltaTime);
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
            // Only freeze X and Z rotation to allow Y-axis rotation for mouse look
            rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            
            // Hide the player capsule renderer for first-person view
            Renderer renderer = gameObject.GetComponent<Renderer>();
            if (renderer != null) {
                renderer.enabled = false;
            }
            
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

        public EntityId getHomeSettlementId() {
            return homeSettlementId;
        }

        public void setHomeSettlementId(EntityId settlementId) {
            this.homeSettlementId = settlementId;
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

        public bool isCurrentlyInSettlement() {
            return getCurrentSettlementId() != null;
        }

        public EntityId getCurrentSettlementId() {
            return currentSettlementId;
        }

        public void setCurrentSettlementId(EntityId currentSettlementId) {
            this.currentSettlementId = currentSettlementId;
        }

        public void clearCurrentSettlementId() {
            setCurrentSettlementId(null);
        }
        
        private void setupCamera(int renderDistance) {
            GameObject cameraObject = GameObject.Find("/Camera");      
            cameraObject.transform.SetParent(getGameObject().transform);
            // Position camera at eye level for first-person view
            cameraObject.transform.localPosition = new Vector3(0, 0.5f, 0);
            cameraObject.transform.localRotation = Quaternion.identity;
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