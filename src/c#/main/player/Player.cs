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
        private Status status = null;
        private bool autoWalk = false;
        private float energy = 100;
        private float metabolism = 0.01f;

        public Player(GameObject gameObject, int walkSpeed, int runSpeed, TickCounter tickCounter, int statusExpirationTicks) : base(EntityType.PLAYER){
            setGameObject(gameObject);
            this.rigidBody = gameObject.GetComponent<Rigidbody>();
            this.walkSpeed = walkSpeed;
            this.runSpeed = runSpeed;
            status = new Status(tickCounter, statusExpirationTicks);
            this.currentSpeed = walkSpeed;
            GameObject childCameraObject = gameObject.transform.GetChild(0).gameObject;
            this.playerCamera = childCameraObject.GetComponent<Camera>();
            getInventory().addItem(ItemType.GOLD_COIN, Random.Range(100, 400));
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
            throw new System.NotImplementedException();
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