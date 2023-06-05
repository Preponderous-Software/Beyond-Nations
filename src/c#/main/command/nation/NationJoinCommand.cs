namespace osg {

    public class NationJoinCommand {
        private NationRepository nationRepository;
        private EventProducer eventProducer;

        public NationJoinCommand(NationRepository nationRepository, EventProducer eventProducer) {
            this.nationRepository = nationRepository;
            this.eventProducer = eventProducer;
        }

        public void execute(Player player) {
            if (player.getNationId() != null) {
                Nation playerNation = nationRepository.getNation(player.getNationId());
                if (playerNation.getLeaderId() == player.getId()) {
                    player.getStatus().update("You are already the leader of " + playerNation.getName() + ".");
                }
                else {
                    player.getStatus().update("You are already a member of " + playerNation.getName() + ".");
                }
                return;
            }
            if (nationRepository.getNumberOfNations() == 0) {
                player.getStatus().update("There are no nations to join.");
                return;
            }
            Nation nation = nationRepository.getRandomNation();
            nation.addMember(player.getId());
            player.setNationId(nation.getId());
            player.setColor(nation.getColor());
            eventProducer.produceNationJoinEvent(nation, player.getId());
            player.getStatus().update("You joined nation " + nation.getName() + ". Members: " + nation.getNumberOfMembers() + ".");
        }
    }
}