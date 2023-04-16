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
                // if population is 1, delete nation
                if (nation.getNumberOfMembers() == 1) {
                    nationRepository.removeNation(nation);
                    player.setNationId(null);
                    player.setColor(Color.white);
                    eventProducer.produceNationDisbandEvent(nation);
                    status.update("You disbanded nation " + nation.getName() + ".");
                    return;
                }
                else {
                    // if population is > 1, transfer leadership to another member
                    while (nation.getLeaderId() == player.getId()) {
                        nation.setLeaderId(nation.getRandomMemberId());
                    }
                    nation.removeMember(player.getId());
                    player.setNationId(null);
                    player.setColor(Color.white);
                    eventProducer.produceNationLeaveEvent(nation, player.getId());
                    status.update("You left nation " + nation.getName() + ". Members: " + nation.getNumberOfMembers() + ".");
                    return;
                }
            }
            nation.removeMember(player.getId());
            player.setNationId(null);
            player.setColor(Color.white);
            eventProducer.produceNationLeaveEvent(nation, player.getId());
            status.update("You left nation " + nation.getName() + ". Members: " + nation.getNumberOfMembers() + ".");
        }
    }
}