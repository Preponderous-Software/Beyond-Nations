namespace beyondnations {

    public class NationRenameCommand {
        private NationRepository nationRepository;
        private string newName;

        public NationRenameCommand(NationRepository nationRepository, string newName) {
            this.nationRepository = nationRepository;
            this.newName = newName;
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

            if (newName == null || newName.Trim().Length == 0) {
                player.getStatus().update("Name cannot be empty.");
                return;
            }

            string oldName = nation.getName();
            nation.setName(newName.Trim());
            player.getStatus().update("Renamed nation from " + oldName + " to " + newName.Trim() + ".");
        }
    }
}