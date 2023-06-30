using UnityEngine;

namespace osg {

    public class TeleportHomeCommand {
        private EntityRepository entityRepository;

        public TeleportHomeCommand(EntityRepository entityRepository) {
            this.entityRepository = entityRepository;
        }

        public void execute(Player player) {
            EntityId homeSettlementId = player.getSettlementId();
            if (homeSettlementId == null) {
                player.getStatus().update("No home found.");
                return;
            }
            Entity homeSettlement = entityRepository.getEntity(homeSettlementId);
            player.getGameObject().transform.position = homeSettlement.getGameObject().transform.position + new Vector3(Random.Range(-20, 20), 0, Random.Range(-20, 20));
            player.getStatus().update("Welcome home!");
        }
    }
}