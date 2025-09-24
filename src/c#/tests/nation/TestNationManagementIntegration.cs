using UnityEngine;

using beyondnations;

namespace beyondnationstests {

    public static class TestNationManagementIntegration {

        public static void runTests() {
            testFullNationManagementWorkflow();
        }

        public static void testFullNationManagementWorkflow() {
            // prepare
            NationRepository nationRepository = new NationRepository();
            EntityRepository entityRepository = new EntityRepository();
            EventRepository eventRepository = new EventRepository();
            EventProducer eventProducer = new EventProducer(eventRepository);
            
            Player leader = new Player(1, 2, new TickCounter(), 100, 50);
            Player member1 = new Player(1, 2, new TickCounter(), 100, 50);
            Player member2 = new Player(1, 2, new TickCounter(), 100, 50);
            
            // Create a nation
            Nation nation = new Nation("Original Nation", leader.getId());
            nation.addMember(member1.getId());
            nation.addMember(member2.getId());
            nationRepository.addNation(nation);
            leader.setNationId(nation.getId());
            
            entityRepository.addEntity(leader);
            entityRepository.addEntity(member1);
            entityRepository.addEntity(member2);

            // Test 1: Rename nation
            NationRenameCommand renameCommand = new NationRenameCommand(nationRepository, "Renamed Nation");
            renameCommand.execute(leader);
            UnityEngine.Debug.Assert(nation.getName() == "Renamed Nation");

            // Test 2: Transfer ownership
            NationTransferOwnershipCommand transferCommand = new NationTransferOwnershipCommand(nationRepository, entityRepository, member1.getId());
            transferCommand.execute(leader);
            UnityEngine.Debug.Assert(nation.getLeaderId().Equals(member1.getId()));
            UnityEngine.Debug.Assert(nation.getRole(member1.getId()) == NationRole.LEADER);
            UnityEngine.Debug.Assert(nation.getRole(leader.getId()) == NationRole.SERF);

            // Test 3: New leader can disband nation
            NationDisbandCommand disbandCommand = new NationDisbandCommand(nationRepository, entityRepository, eventProducer);
            disbandCommand.execute(member1);
            UnityEngine.Debug.Assert(nationRepository.getNation(nation.getId()) == null);
            
            UnityEngine.Debug.Log("Nation management integration test passed!");
        }
    }
}