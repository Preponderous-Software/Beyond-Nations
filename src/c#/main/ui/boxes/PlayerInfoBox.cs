using UnityEngine;
using UnityEngine.UI;

namespace osg {
    public class PlayerInfoBox : InfoBox {
        private Player player;
        private NationRepository nationRepository;
        private int numDataPoints = 4;

        public PlayerInfoBox(int padding, int width, int height, int x, int y, string title, Player player, NationRepository nationRepository) : base(padding, width, height, x, y, title) {
            this.player = player;
            this.nationRepository = nationRepository;
        }

        public override void draw() {
            // draw box with padding
            GUI.Box(new Rect(x - 10, y - 10, width + 20, (height * (numDataPoints + 2))), title);
            y += 10;

            // draw energy
            GUI.Label(new Rect(x, y, width, height), "Energy: " + (int) player.getEnergy());
            y += height;

            // draw num relationships
            GUI.Label(new Rect(x, y, width, height), "Relationships: " + player.getRelationships().Count);
            y += height;

            // if in nation
            if (player.getNationId() != null) {
                Nation nation = nationRepository.getNation(player.getNationId());

                // draw nation name
                GUI.Label(new Rect(x, y, width, height), "Nation: " + nation.getName());
                y += height;

                // draw role
                GUI.Label(new Rect(x, y, width, height), "Role: " + nation.getRole(player.getId()));
            }
            else {
                // draw nation name
                GUI.Label(new Rect(x, y, width, height), "Nation: none");
                y += height;

                // draw role
                GUI.Label(new Rect(x, y, width, height), "Role: none");
            }
        }
    } 
}