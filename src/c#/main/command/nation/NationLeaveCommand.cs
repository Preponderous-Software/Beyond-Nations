using UnityEngine;

namespace osg {

    public class NationLeaveCommand {
        private NationRepository nationRepository;
        private EventProducer eventProducer;

        public NationLeaveCommand(NationRepository nationRepository, EventProducer eventProducer) {
            this.nationRepository = nationRepository;
            this.eventProducer = eventProducer;
        }

        public void execute(Player player) {
            Status status = player.getStatus();
            if (player.getNationId() == null) {
                status.update("You are not a member of a nation.");
                return;
            }
            Nation nation = nationRepository.getNation(player.getNationId());
            if (nation.getLeaderId() == player.getId()) {
                if (nation.getNumberOfMembers() == 1) {
                    deleteNation(nation, player, status);
                    return;
                }
                else {
                    transferLeadership(nation, player, status);
                    return;
                }
            }
            nation.removeMember(player.getId());
            player.setNationId(null);
            player.setColor(Color.white);
            eventProducer.produceNationLeaveEvent(nation, player.getId());
            status.update("You left nation " + nation.getName() + ". Members: " + nation.getNumberOfMembers() + ".");
        }

        private void deleteNation(Nation nation, Player player, Status status) {
            nationRepository.removeNation(nation);
            player.setNationId(null);
            player.setColor(Color.white);
            eventProducer.produceNationDisbandEvent(nation);
            status.update("You disbanded nation " + nation.getName() + ".");
        }

        private void transferLeadership(Nation nation, Player player, Status status) {
            while (nation.getLeaderId() == player.getId()) {
                nation.setLeaderId(nation.getRandomMemberId());
                nation.setRole(nation.getLeaderId(), NationRole.LEADER);
            }
            nation.removeMember(player.getId());
            player.setNationId(null);
            player.setColor(Color.white);
            eventProducer.produceNationLeaveEvent(nation, player.getId());
            status.update("You left nation " + nation.getName() + ". Members: " + nation.getNumberOfMembers() + ".");
            return;
        }
    }
}