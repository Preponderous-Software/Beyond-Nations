using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace osg {

    public class Player : MonoBehaviour
    {
        private PlayerId id = new PlayerId();
        private Rigidbody rigidBody = null;
        private bool isGrounded = false;
        private bool jumpKeyWasPressed = false;
        private float horizontalInput = 0;
        private float verticalInput = 0;
        private int walkSpeed = 1;
        private int jumpForce = 5;
        private bool zoomingIn = false;
        private bool zoomingOut = false;
        private Camera playerCamera = null;

        // Start is called before the first frame update
        void Start()
        {
            rigidBody = GetComponent<Rigidbody>();
            GameObject childCameraObject = transform.GetChild(0).gameObject;
            playerCamera = childCameraObject.GetComponent<Camera>();
        }

        // Update is called once per frame
        void Update()
        {
            // jump
            if (Input.GetKeyDown(KeyCode.Space))
            {
                jumpKeyWasPressed = true;
            }
            if (Input.GetKeyUp(KeyCode.Space))
            {
                jumpKeyWasPressed = false;
            }

            horizontalInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical");

            // toggle zooming in and out
            toggleZoomBasedOnInput();

            // reset player position if R pressed
            if (Input.GetKeyDown(KeyCode.R))
            {
                transform.position = new Vector3(0, 0, 0);
            }

            // modify speed if shift pressed
            if (Input.GetKey(KeyCode.LeftShift))
            {
                walkSpeed = 20;
            }
            else
            {
                walkSpeed = 10;
            }

        }

        // FixedUpdate is called once per physics frame
        void FixedUpdate()
        {
            // jump
            if (jumpKeyWasPressed && isGrounded)
            {
                rigidBody.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
            }

            // turn left and right
            if (horizontalInput != 0)
            {
                rigidBody.transform.Rotate(Vector3.up * horizontalInput * 2);
            }

            // move forward and back
            if (verticalInput != 0)
            {
                rigidBody.transform.Translate(Vector3.forward * verticalInput * walkSpeed * Time.deltaTime);
            }

            // zoom in and out
            // get distance 
            float distance = Vector3.Distance(playerCamera.transform.position, transform.position);
            if (zoomingIn)
            {
                // move camera closer to player
                Vector3 direction = playerCamera.transform.position - transform.position;
                playerCamera.transform.position -= direction * 0.1f;
            }
            else if (zoomingOut)
            {
                // move camera away from player
                Vector3 direction = playerCamera.transform.position - transform.position;
                playerCamera.transform.position += direction * 0.1f;
            }
        }

        public PlayerId getId()
        {
            return id;
        }

        // called when player collides with something
        private void OnCollisionEnter(Collision collision)
        {
            isGrounded = true;
        }

        // called when player stops colliding with something
        private void OnCollisionExit(Collision collision)
        {
            isGrounded = false;
        }

        // helper method to toggle zooming in and out
        void toggleZoomBasedOnInput()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                zoomingIn = true;
            }
            if (Input.GetKeyUp(KeyCode.I))
            {
                zoomingIn = false;
            }
            if (Input.GetKeyDown(KeyCode.O))
            {
                zoomingOut = true;
            }
            if (Input.GetKeyUp(KeyCode.O))
            {
                zoomingOut = false;
            }
        }
    }
}