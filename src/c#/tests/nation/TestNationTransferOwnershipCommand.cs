using UnityEngine;

using beyondnations;

namespace beyondnationstests {

    public static class TestNationTransferOwnershipCommand {

        public static void runTests() {
            testSuccessfulTransfer();
            testTransferWithoutNation();
            testTransferAsNonLeader();
            testTransferToNonMember();
            testTransferToSelf();
        }

        public static void testSuccessfulTransfer() {
            // prepare
            NationRepository nationRepository = new NationRepository();
            EntityRepository entityRepository = new EntityRepository();
            
            Player player = new Player(1, 2, new TickCounter(), 100, 50);
            Nation nation = new Nation("Test Nation", player.getId());
            
            Player newLeader = new Player(1, 2, new TickCounter(), 100, 50);
            nation.addMember(newLeader.getId());
            nationRepository.addNation(nation);
            
            player.setNationId(nation.getId());
            entityRepository.addEntity(player);
            entityRepository.addEntity(newLeader);
            
            NationTransferOwnershipCommand command = new NationTransferOwnershipCommand(nationRepository, entityRepository, newLeader.getId());

            // run
            command.execute(player);

            // verify
            UnityEngine.Debug.Assert(nation.getLeaderId().Equals(newLeader.getId()));
            UnityEngine.Debug.Assert(nation.getRole(newLeader.getId()) == NationRole.LEADER);
            UnityEngine.Debug.Assert(nation.getRole(player.getId()) == NationRole.SERF);
        }

        public static void testTransferWithoutNation() {
            // prepare
            NationRepository nationRepository = new NationRepository();
            EntityRepository entityRepository = new EntityRepository();
            Player newLeader = new Player(1, 2, new TickCounter(), 100, 50);
            
            Player player = new Player(1, 2, new TickCounter(), 100, 50);
            
            NationTransferOwnershipCommand command = new NationTransferOwnershipCommand(nationRepository, entityRepository, newLeader.getId());

            // run
            command.execute(player);

            // verify - should not crash
            UnityEngine.Debug.Assert(true);
        }

        public static void testTransferAsNonLeader() {
            // prepare
            NationRepository nationRepository = new NationRepository();
            EntityRepository entityRepository = new EntityRepository();
            
            EntityId leaderId = new EntityId();
            Nation nation = new Nation("Test Nation", leaderId);
            
            Player player = new Player(1, 2, new TickCounter(), 100, 50);
            Player newLeader = new Player(1, 2, new TickCounter(), 100, 50);
            
            nation.addMember(player.getId());
            nation.addMember(newLeader.getId());
            nationRepository.addNation(nation);
            player.setNationId(nation.getId());
            
            NationTransferOwnershipCommand command = new NationTransferOwnershipCommand(nationRepository, entityRepository, newLeader.getId());

            // run
            command.execute(player);

            // verify - leadership should not change
            UnityEngine.Debug.Assert(nation.getLeaderId().Equals(leaderId));
        }

        public static void testTransferToNonMember() {
            // prepare
            NationRepository nationRepository = new NationRepository();
            EntityRepository entityRepository = new EntityRepository();
            
            Player player = new Player(1, 2, new TickCounter(), 100, 50);
            Player nonMember = new Player(1, 2, new TickCounter(), 100, 50);
            Nation nation = new Nation("Test Nation", player.getId());
            nationRepository.addNation(nation);
            player.setNationId(nation.getId());
            
            NationTransferOwnershipCommand command = new NationTransferOwnershipCommand(nationRepository, entityRepository, nonMember.getId());

            // run
            command.execute(player);

            // verify - leadership should not change
            UnityEngine.Debug.Assert(nation.getLeaderId().Equals(player.getId()));
        }

        public static void testTransferToSelf() {
            // prepare
            NationRepository nationRepository = new NationRepository();
            EntityRepository entityRepository = new EntityRepository();
            
            Player player = new Player(1, 2, new TickCounter(), 100, 50);
            Nation nation = new Nation("Test Nation", player.getId());
            nationRepository.addNation(nation);
            player.setNationId(nation.getId());
            
            NationTransferOwnershipCommand command = new NationTransferOwnershipCommand(nationRepository, entityRepository, player.getId());

            // run
            command.execute(player);

            // verify - leadership should not change
            UnityEngine.Debug.Assert(nation.getLeaderId().Equals(player.getId()));
        }
    }
}