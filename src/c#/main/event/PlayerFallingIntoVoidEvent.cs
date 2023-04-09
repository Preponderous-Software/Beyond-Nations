using UnityEngine;

namespace osg {

    public class PlayerFallingIntoVoidEvent : Event {
        private Vector3 position;

        public PlayerFallingIntoVoidEvent(Vector3 position) : base(EventType.PlayerFallingIntoVoid, "The player has fallen into the void at (" + position.x + ", " + position.y + ", " + position.z + ").") {
            this.position = position;
        }

        public Vector3 Position {
            get { return position; }
            set { position = value; }
        }

        public override string ToString() {
            return "PlayerFallingIntoVoidEvent [position=" + position + ", type=" + getType() + ", description=" + getDescription() + ", date=" + getDate() + "]";
        }
    }
}