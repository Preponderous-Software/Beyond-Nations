using UnityEngine;

namespace osg {

    public class NationLeaveCommand {
        private NationRepository nationRepository;
        private EventProducer eventProducer;
        private EntityRepository entityRepository;

        public NationLeaveCommand(NationRepository nationRepository, EventProducer eventProducer, EntityRepository entityRepository) {
            this.nationRepository = nationRepository;
            this.eventProducer = eventProducer;
            this.entityRepository = entityRepository;
        }

        public void execute(Player player) {
            if (player.getNationId() == null) {
                player.getStatus().update("You are not a member of a nation.");
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
            player.getStatus().update("You left nation " + nation.getName() + ". Members: " + nation.getNumberOfMembers() + ".");
        }

        private void deleteNation(Nation nation, Player player, Status status) {
            nationRepository.removeNation(nation);
            player.setNationId(null);
            player.setColor(Color.white);
            eventProducer.produceNationDisbandEvent(nation);
            player.getStatus().update("You disbanded nation " + nation.getName() + ".");

            // remove settlements
            foreach (EntityId settlementId in nation.getSettlements()) {
                Settlement settlement = (Settlement) entityRepository.getEntity(settlementId);
                settlement.markForDeletion();
            }

            // clear settlements
            nation.getSettlements().Clear();
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
            player.getStatus().update("You left nation " + nation.getName() + ". Members: " + nation.getNumberOfMembers() + ".");
            return;
        }
    }
}