using UnityEngine;

namespace beyondnations {

    public class CollectFoodFromStallCommand {
        private NationRepository nationRepository;
        private EntityRepository entityRepository;

        public CollectFoodFromStallCommand(NationRepository nationRepository, EntityRepository entityRepository) {
            this.nationRepository = nationRepository;
            this.entityRepository = entityRepository;
        }

        public void execute(Player player) {
            if (player.getNationId() == null) {
                player.getStatus().update("You are not in a nation.");
                return;
            }
            Nation nation = nationRepository.getNation(player.getNationId());
            if (nation.getRole(player.getId()) != NationRole.MERCHANT) {
                player.getStatus().update("You are not a merchant.");
                return;
            }

            EntityId currentSettlementId = player.getCurrentSettlementId();
            if (currentSettlementId == null) {
                player.getStatus().update("You are not inside a settlement.");
                return;
            }

            Settlement settlement = (Settlement) entityRepository.getEntity(currentSettlementId);

            Stall stall = settlement.getMarket().getStall(player.getId());
            if (stall == null) {
                player.getStatus().update("You do not own a stall.");
                return;
            }

            int applesToTransfer = stall.getInventory().getNumItems(ItemType.APPLE)/2;
            if (applesToTransfer == 0) {
                applesToTransfer = 1;
            }
            stall.getInventory().removeItem(ItemType.APPLE, applesToTransfer);
            player.getInventory().addItem(ItemType.APPLE, applesToTransfer);
            player.getStatus().update("Collected " + applesToTransfer + " apples from stall.");
        }
    }
}