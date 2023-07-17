using UnityEngine;

namespace osg {

    public class EnterSettlementCommand {
        private EntityRepository entityRepository;

        public EnterSettlementCommand(EntityRepository entityRepository) {
            this.entityRepository = entityRepository;
        }

        public void execute(Player player, Settlement settlement) {
            // enter settlement
            EntityId settlementId = settlement.getId();
            player.setCurrentSettlementId(settlementId);
            player.getStatus().update("Entered settlement.");

            // make camera a child of the settlement instead of the player
            player.getCamera().transform.parent = settlement.getGameObject().transform;

            // set game object to inactive
            player.getGameObject().SetActive(false);
        }
    }
}