using UnityEngine;

namespace osg {

    public class BuildStallCommand {
        private NationRepository nationRepository;
        private EntityRepository entityRepository;

        public BuildStallCommand(NationRepository nationRepository, EntityRepository entityRepository) {
            this.nationRepository = nationRepository;
            this.entityRepository = entityRepository;
        }

        public void execute(Player player) {
            if (player.getNationId() == null) {
                player.getStatus().update("You are not in a nation.");
                return;
            }
            Nation nation = nationRepository.getNation(player.getNationId());
            if (nation.getRole(player.getId()) != NationRole.LEADER) {
                player.getStatus().update("You are not the leader of a nation.");
                return;
            }

            if (player.getSettlementId() == null) {
                player.getStatus().update("You are not in a settlement.");
                return;
            }

            Settlement settlement = (Settlement) entityRepository.getEntity(player.getSettlementId());

            if (settlement.getMarket().getNumStalls() >= settlement.getMarket().getMaxNumStalls()) {
                player.getStatus().update("Market is full. Cannot build stall.");
                return;
            }

            if (player.getInventory().getNumItems(ItemType.WOOD) < Stall.WOOD_COST_TO_BUILD) {
                player.getStatus().update("Not enough wood. Need " + (Stall.WOOD_COST_TO_BUILD - player.getInventory().getNumItems(ItemType.WOOD)) + " more.");
                return;
            }

            settlement.getMarket().createStall();
            player.getInventory().removeItem(ItemType.WOOD, Stall.WOOD_COST_TO_BUILD);
            player.getStatus().update("Stall built.");
        }
    }
}