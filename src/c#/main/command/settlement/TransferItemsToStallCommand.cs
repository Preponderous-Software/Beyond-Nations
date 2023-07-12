using UnityEngine;

namespace osg {

    public class TransferItemsToStallCommand {
        private NationRepository nationRepository;
        private EntityRepository entityRepository;

        public TransferItemsToStallCommand(NationRepository nationRepository, EntityRepository entityRepository) {
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

            if (player.getSettlementId() == null) {
                player.getStatus().update("You are not in a settlement.");
                return;
            }

            Settlement settlement = (Settlement) entityRepository.getEntity(player.getSettlementId());

            Stall stall = settlement.getMarket().getStall(player.getId());
            if (stall == null) {
                player.getStatus().update("You do not own a stall.");
                return;
            }

            stall.getInventory().transferContentsOfInventory(player.getInventory());
            player.getStatus().update("Transferred contents of inventory to stall.");
        }
    }
}