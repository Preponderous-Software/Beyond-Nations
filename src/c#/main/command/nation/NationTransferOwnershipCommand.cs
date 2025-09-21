namespace beyondnations {

    public class NationTransferOwnershipCommand {
        private NationRepository nationRepository;
        private EntityRepository entityRepository;
        private EntityId newLeaderId;

        public NationTransferOwnershipCommand(NationRepository nationRepository, EntityRepository entityRepository, EntityId newLeaderId) {
            this.nationRepository = nationRepository;
            this.entityRepository = entityRepository;
            this.newLeaderId = newLeaderId;
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

            if (newLeaderId == null) {
                player.getStatus().update("Invalid new leader.");
                return;
            }

            if (!nation.isMember(newLeaderId)) {
                player.getStatus().update("The selected entity is not a member of this nation.");
                return;
            }

            if (newLeaderId.Equals(player.getId())) {
                player.getStatus().update("You are already the leader.");
                return;
            }

            Entity newLeader = entityRepository.getEntity(newLeaderId);
            if (newLeader == null) {
                player.getStatus().update("Selected entity not found.");
                return;
            }

            // Set old leader to serf role
            nation.setRole(player.getId(), NationRole.SERF);

            // Set new leader
            nation.setLeaderId(newLeaderId);
            nation.setRole(newLeaderId, NationRole.LEADER);

            string newLeaderName = newLeader.getName();
            player.getStatus().update("Transferred leadership of " + nation.getName() + " to " + newLeaderName + ".");
        }
    }
}