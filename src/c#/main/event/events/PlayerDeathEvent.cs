using UnityEngine;

namespace osg {

    public class PlayerDeathEvent : Event {

        private Vector3 position;
        private Player player;

        public PlayerDeathEvent(Vector3 position, Player player) : base(EventType.PlayerDeath, "Player " + player.getId() + " has died at (" + position.x + ", " + position.y + ", " + position.z + ").") {
            this.position = position;
            this.player = player;
        }

        public Vector3 getPosition() {
            return position;
        }

        public Player getPlayer() {
            return player;
        }

        public override string ToString() {
            return "PlayerDeathEvent [position=" + position + ", player=" + player + ", type=" + getType() + ", description=" + getDescription() + ", date=" + getDate() + "]";
        }
    }
}