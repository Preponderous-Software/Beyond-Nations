using UnityEngine;

namespace osg {

    public class FoundSettlementCommand {
        private NationRepository nationRepository;
        private EventProducer eventProducer;
        private EntityRepository entityRepository;

        public FoundSettlementCommand(NationRepository nationRepository, EventProducer eventProducer, EntityRepository entityRepository) {
            this.nationRepository = nationRepository;
            this.eventProducer = eventProducer;
            this.entityRepository = entityRepository;
        }

        public void execute(Player player) {
            // if not in a nation
            if (player.getNationId() == null) {
                player.getStatus().update("You are not in a nation.");
                return;
            }
            Nation nation = nationRepository.getNation(player.getNationId());
            // if not the leader of a nation
            if (nation.getLeaderId() != player.getId()) {
                player.getStatus().update("You are not the leader of a nation.");
                return;
            }
            // if not enough money
            if (player.getInventory().getNumItems(ItemType.GOLD_COIN) < 100) {
                player.getStatus().update("You don't have enough money.");
                return;
            }

            // if another settlement within 200 units
            foreach (Entity entity in entityRepository.getEntities()) {
                if (entity.getType() == EntityType.SETTLEMENT) {
                    Settlement settlementToCheck = (Settlement)entity;
                    int distance = (int)Vector3.Distance(player.getGameObject().transform.position, settlementToCheck.getPosition());
                    if (distance < 200) {
                        player.getStatus().update("Too close to another settlement.");
                        return;
                    }
                }
            }

            player.getInventory().removeItem(ItemType.GOLD_COIN, 100);
            Settlement settlement = new Settlement(player.getGameObject().transform.position, player.getNationId(), nation.getColor(), nation.getName());
            entityRepository.addEntity(settlement);
            nationRepository.getNation(player.getNationId()).addSettlement(settlement.getId());
            player.getStatus().update("Settlement founded.");
            player.setSettlementId(settlement.getId());
        }
    }
}