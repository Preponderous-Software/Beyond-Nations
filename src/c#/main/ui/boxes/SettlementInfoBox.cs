using UnityEngine;
using UnityEngine.UI;

namespace osg {
    public class SettlementInfoBox : InfoBox {
        private Settlement settlement;
        private NationRepository nationRepository;
        private int numDataPoints = 3;

        public SettlementInfoBox(int padding, int width, int height, int x, int y, string title, Settlement settlement, NationRepository nationRepository) : base(padding, width, height, x, y, title) {
            this.settlement = settlement;
            this.nationRepository = nationRepository;
        }

        public override void draw() {
            // draw box with padding
            GUI.Box(new Rect(x - 10, y - 10, width + 20, (height * (numDataPoints + 2))), title);
            y += 10;

            // draw nation name
            GUI.Label(new Rect(x, y, width, height), "Nation: " + nationRepository.getNation(settlement.getNationId()).getName());
            y += height;

            // draw num pawns
            GUI.Label(new Rect(x, y, width, height), "PCIS: " + settlement.getCurrentlyPresentEntitiesCount());
            y += height;

            // draw settlement funds
            GUI.Label(new Rect(x, y, width, height), "Funds: " + settlement.getInventory().getNumItems(ItemType.COIN));
        }
    } 
}