using UnityEngine;
using UnityEngine.UI;

namespace osg {
    public class SettlementInfoBox : InfoBox {
        private Settlement settlement;
        private NationRepository nationRepository;
        private EntityRepository entityRepository;
        private int numDataPoints = 4;

        public SettlementInfoBox(int padding, int width, int height, int x, int y, string title, Settlement settlement, NationRepository nationRepository, EntityRepository entityRepository) : base(padding, width, height, x, y, title) {
            this.settlement = settlement;
            this.nationRepository = nationRepository;
            this.entityRepository = entityRepository;
        }

        public override void draw() {
            Nation nation = nationRepository.getNation(settlement.getNationId());

            // draw box with padding
            GUI.Box(new Rect(x - 10, y - 10, width + 20, (height * (numDataPoints + 2))), title);
            y += 10;

            // draw nation name
            GUI.Label(new Rect(x, y, width, height), "Nation: " + nation.getName());
            y += height;

            // nation leader
            Entity leader = entityRepository.getEntity(nation.getLeaderId());
            if (leader is Player) {
                Player player = (Player) leader;
                GUI.Label(new Rect(x, y, width, height), "Leader: You");
            }
            else if (leader is Pawn) {
                Pawn pawn = (Pawn) leader;
                GUI.Label(new Rect(x, y, width, height), "Leader: " + pawn.getName());
            }
            else {
                GUI.Label(new Rect(x, y, width, height), "Leader: " + leader.getType());
            }
            y += height;

            // draw num pawns
            GUI.Label(new Rect(x, y, width, height), "PCIS: " + settlement.getCurrentlyPresentEntitiesCount());
            y += height;

            // draw settlement funds
            GUI.Label(new Rect(x, y, width, height), "Funds: " + settlement.getInventory().getNumItems(ItemType.COIN));
        }
    } 
}