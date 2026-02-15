using UnityEngine;

namespace beyondnations {

    public class Chicken : Entity {
        private int speed = UnityEngine.Random.Range(5, 10);
        private Vector3 wanderTarget;
        private float wanderTimer = 0f;
        private float wanderInterval = 3f;
        
        public Chicken(Vector3 position) : base(EntityType.CHICKEN, "Chicken") {
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
            Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>();
            rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            setGameObject(gameObject);

            // Add chicken meat to inventory
            getInventory().addItem(ItemType.CHICKEN_MEAT, UnityEngine.Random.Range(1, 3));
        }

        public override void destroyGameObject() {
            UnityEngine.Object.Destroy(getGameObject());
        }

        public void wander() {
            wanderTimer += Time.deltaTime;
            
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
            direction.Normalize();

            if (direction.magnitude > 0.1f) {
                getGameObject().GetComponent<Rigidbody>().velocity = direction * speed;
            }
        }
    }
}
