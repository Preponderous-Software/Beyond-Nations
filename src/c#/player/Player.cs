using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace osg {

    public class Player {
        private PlayerId id = new PlayerId();
        private GameObject gameObject = null;
        private Rigidbody rigidBody = null;
        private bool jumpKeyWasPressed = false;
        private float horizontalInput = 0;
        private float verticalInput = 0;
        private int walkSpeed;
        private int runSpeed;
        private int currentSpeed;
        private Camera playerCamera = null;
        private Inventory inventory = new Inventory();

        public Player(GameObject gameObject, int walkSpeed, int runSpeed) {
            this.gameObject = gameObject;
            this.rigidBody = gameObject.GetComponent<Rigidbody>();
            this.walkSpeed = walkSpeed;
            this.runSpeed = runSpeed;
            this.currentSpeed = walkSpeed;
            GameObject childCameraObject = gameObject.transform.GetChild(0).gameObject;
            this.playerCamera = childCameraObject.GetComponent<Camera>();
        }

        public void update() {
            horizontalInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical");

            // modify speed if shift pressed
            if (Input.GetKey(KeyCode.LeftShift)) {
                currentSpeed = runSpeed;
            } else {
                currentSpeed = walkSpeed;
            }

            // jump if space pressed
            if (Input.GetKeyDown(KeyCode.Space)) {
                jumpKeyWasPressed = true;
            }
        }

        public void fixedUpdate() {
            // turn left and right
            if (horizontalInput != 0) {
                rigidBody.transform.Rotate(Vector3.up * horizontalInput * 2);
            }

            // move forward and backward
            if (verticalInput != 0) {
                rigidBody.transform.Translate(Vector3.forward * verticalInput * currentSpeed * Time.deltaTime);
            }

            if (jumpKeyWasPressed) {
                jump();
                jumpKeyWasPressed = false;
            }
        }

        private void jump() {
            if (isGrounded()) {
                rigidBody.AddForce(Vector3.up * 10, ForceMode.Impulse);
            }
        }

        public PlayerId getId() {
            return id;
        }

        public GameObject getGameObject() {
            return gameObject;
        }

        public Camera getCamera() {
            return playerCamera;
        }

        public bool isGrounded() {
            int minY = 0;
            int maxY = 2;
            return gameObject.transform.position.y > minY && gameObject.transform.position.y < maxY;
        }

        public Inventory getInventory() {
            return inventory;
        }
    }
}