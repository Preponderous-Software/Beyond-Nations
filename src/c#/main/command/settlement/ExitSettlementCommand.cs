using UnityEngine;

namespace osg {

    public class ExitSettlementCommand {
        private EntityRepository entityRepository;

        public ExitSettlementCommand(EntityRepository entityRepository) {
            this.entityRepository = entityRepository;
        }

        public void execute(Player player) {
            player.getStatus().update("Exited settlement.");
                player.setCurrentSettlementId(null);
                player.getGameObject().SetActive(true);

                // make camera a child of the player instead of the settlement
                player.getCamera().transform.parent = player.getGameObject().transform;
        }
    }
}