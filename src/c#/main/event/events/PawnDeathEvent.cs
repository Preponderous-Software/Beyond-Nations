using UnityEngine;

namespace osg {

    public class PawnDeathEvent : Event {

        private Vector3 position;
        private Pawn pawn;

        public PawnDeathEvent(Vector3 position, Pawn pawn) : base(EventType.PawnDeath, pawn.getName() + " has died at (" + position.x + ", " + position.y + ", " + position.z + ").") {
            this.position = position;
            this.pawn = pawn;
        }

        public Vector3 getPosition() {
            return position;
        }

        public Pawn getPawn() {
            return pawn;
        }

        public override string ToString() {
            return "PawnDeathEvent [position=" + position + ", pawn=" + pawn + ", type=" + getType() + ", description=" + getDescription() + ", date=" + getDate() + "]";
        }
    }
}