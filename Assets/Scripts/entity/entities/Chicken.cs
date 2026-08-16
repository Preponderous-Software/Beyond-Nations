using UnityEngine;
using Vector3 = System.Numerics.Vector3;

namespace beyondnations {

    public class Chicken : Entity {
        private RandomSource random;
        private int speed;
        private Vector3 wanderTarget;
        private float wanderTimer = 0f;
        private float wanderInterval = 3f;
        private Rigidbody rigidbody;
        
        public Chicken(Vector3 position, RandomSource random) : base(EntityType.CHICKEN, "Chicken") {
            this.random = random;
            speed = random.range(5, 10);
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
            getInventory().addItem(ItemType.CHICKEN_MEAT, random.range(1, 3));
        }

        public override void destroyGameObject() {
            UnityEngine.Object.Destroy(getGameObject());
        }

        public void wander() {
            wanderTimer += Time.fixedDeltaTime;
            
            if (wanderTimer >= wanderInterval) {
                // Pick a new random direction
                wanderTarget = getGameObject().transform.position + new Vector3(
                    random.range(-10f, 10f),
                    0,
                    random.range(-10f, 10f)
                );
                wanderTimer = 0f;
            }

            // Move towards wander target
            Vector3 currentPosition = getGameObject().transform.position;
            Vector3 direction = wanderTarget - currentPosition;
            direction.Y = 0; // Keep movement horizontal
            
            if (direction.Length() > 0.5f) {
                direction = VectorMath.normalized(direction);
                rigidbody.velocity = direction * speed;
            } else {
                // Stop moving when close to target
                rigidbody.velocity = Vector3.Zero;
            }
        }
    }
}
