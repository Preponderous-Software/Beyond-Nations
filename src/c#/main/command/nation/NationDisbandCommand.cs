namespace beyondnations {

    public class NationDisbandCommand {
        private NationRepository nationRepository;
        private EntityRepository entityRepository;
        private EventProducer eventProducer;

        public NationDisbandCommand(NationRepository nationRepository, EntityRepository entityRepository, EventProducer eventProducer) {
            this.nationRepository = nationRepository;
            this.entityRepository = entityRepository;
            this.eventProducer = eventProducer;
        }

        public void execute(Player player) {
            if (player.getNationId() == null) {
                player.getStatus().update("You are not in a nation.");
                return;
            }

            Nation nation = nationRepository.getNation(player.getNationId());
            if (nation.getLeaderId() != player.getId()) {
                player.getStatus().update("You are not the leader of this nation.");
                return;
            }

            string nationName = nation.getName();

            // Remove all members from nation
            foreach (EntityId memberId in nation.getMembers()) {
                Entity member = entityRepository.getEntity(memberId);
                if (member != null) {
                    if (member.getType() == EntityType.PLAYER) {
                        Player memberPlayer = (Player) member;
                        memberPlayer.setNationId(null);
                    } else if (member.getType() == EntityType.PAWN) {
                        Pawn memberPawn = (Pawn) member;
                        memberPawn.setNationId(null);
                    }
                }
            }

            // Remove settlements
            foreach (EntityId settlementId in nation.getSettlements()) {
                Settlement settlement = (Settlement) entityRepository.getEntity(settlementId);
                if (settlement != null) {
                    settlement.markForDeletion();
                }
            }

            // Remove nation from repository
            nationRepository.removeNation(nation);

            // Produce nation disband event
            eventProducer.produceNationDisbandEvent(nation);

            player.getStatus().update("Disbanded nation " + nationName + ".");
        }
    }
}