using UnityEngine;

namespace beyondnations {

    public class Chicken : Entity {
        private int speed;
        private Vector3 wanderTarget;
        private float wanderTimer = 0f;
        private float wanderInterval = 3f;
        private Rigidbody rigidbody;
        
        public Chicken(Vector3 position) : base(EntityType.CHICKEN, "Chicken") {
            speed = UnityEngine.Random.Range(5, 10);
            createGameObject(position);
        }

        public int getSpeed() {
            return speed;
        }

        public override void createGameObject(Vector3 position) {
            GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            gameObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            gameObject.GetComponent<Renderer>().material.color = new Color(1f, 0.9f, 0.8f); // Light beige/tan color
            gameObject.transform.position = position;
            gameObject.name = getName();
            rigidbody = gameObject.AddComponent<Rigidbody>();
            rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            setGameObject(gameObject);

            // Add chicken meat to inventory
            getInventory().addItem(ItemType.CHICKEN_MEAT, UnityEngine.Random.Range(1, 3));
        }

        public override void destroyGameObject() {
            UnityEngine.Object.Destroy(getGameObject());
        }

        public void wander() {
            wanderTimer += Time.fixedDeltaTime;
            
            if (wanderTimer >= wanderInterval) {
                // Pick a new random direction
                wanderTarget = getGameObject().transform.position + new Vector3(
                    UnityEngine.Random.Range(-10f, 10f),
                    0,
                    UnityEngine.Random.Range(-10f, 10f)
                );
                wanderTimer = 0f;
            }

            // Move towards wander target
            Vector3 currentPosition = getGameObject().transform.position;
            Vector3 direction = wanderTarget - currentPosition;
            direction.y = 0; // Keep movement horizontal
            
            if (direction.magnitude > 0.5f) {
                direction.Normalize();
                rigidbody.velocity = direction * speed;
            } else {
                // Stop moving when close to target
                rigidbody.velocity = Vector3.zero;
            }
        }
    }
}
