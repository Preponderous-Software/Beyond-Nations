using UnityEngine;
using Vector3 = System.Numerics.Vector3;

namespace beyondnations {

    public class TeleportHomeCommand {
        private RandomSource random;
        private EntityRepository entityRepository;

        public TeleportHomeCommand(EntityRepository entityRepository, RandomSource random) {
            this.random = random;
            this.entityRepository = entityRepository;
        }

        public void execute(Player player) {
            if (player.isCurrentlyInSettlement()) {
                player.getStatus().update("Cannot teleport while in settlement.");
                return;
            }
            
            EntityId homeSettlementId = player.getHomeSettlementId();
            if (homeSettlementId == null) {
                player.getStatus().update("No home found.");
                return;
            }
            Entity homeSettlement = entityRepository.getEntity(homeSettlementId);
            player.getGameObject().transform.position = homeSettlement.getGameObject().transform.position + new Vector3(random.range(-20, 20), 0, random.range(-20, 20));
            player.getStatus().update("Welcome home!");
        }
    }
}