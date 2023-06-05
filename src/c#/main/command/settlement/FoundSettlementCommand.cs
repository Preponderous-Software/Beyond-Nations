namespace osg {

    public class FoundSettlementCommand {
        private Environment environment;
        private NationRepository nationRepository;
        private EventProducer eventProducer;
        private EntityRepository entityRepository;

        public FoundSettlementCommand(Environment environment, NationRepository nationRepository, EventProducer eventProducer, EntityRepository entityRepository) {
            this.environment = environment;
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

            player.getInventory().removeItem(ItemType.GOLD_COIN, 100);
            Settlement settlement = new Settlement(player.getGameObject().transform.position, player.getNationId(), nation.getColor(), nation.getName());
            entityRepository.addEntity(settlement);
            nationRepository.getNation(player.getNationId()).addSettlement(settlement.getId());
            player.getStatus().update("Settlement founded.");
        }
    }
}