namespace osg {

    public class NationCreateCommand {
        private NationRepository nationRepository;
        private EventProducer eventProducer;

        public NationCreateCommand(NationRepository nationRepository, EventProducer eventProducer) {
            this.nationRepository = nationRepository;
            this.eventProducer = eventProducer;
        }

        public void execute(Player player) {
            Status status = player.getStatus();
            if (player.getNationId() != null) {
                Nation playerNation = nationRepository.getNation(player.getNationId());
                if (playerNation.getLeaderId() == player.getId()) {
                    status.update("You are already the leader of " + playerNation.getName() + ".");
                }
                else {
                    status.update("You are already a member of " + playerNation.getName() + ".");
                }
                return;
            }
            Nation nation = new Nation(NationNameGenerator.generate(), player.getId());
            nationRepository.addNation(nation);
            player.setNationId(nation.getId());
            player.setColor(nation.getColor());
            eventProducer.produceNationCreationEvent(nation);
            status.update("Created nation " + nation.getName() + ".");
        }
    }
}