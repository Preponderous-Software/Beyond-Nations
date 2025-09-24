using UnityEngine;

using beyondnations;

namespace beyondnationstests {

    public static class TestNationRenameCommand {

        public static void runTests() {
            testSuccessfulRename();
            testRenameWithoutNation();
            testRenameAsNonLeader();
            testRenameWithEmptyName();
        }

        public static void testSuccessfulRename() {
            // prepare
            NationRepository nationRepository = new NationRepository();
            Player player = new Player(1, 2, new TickCounter(), 100, 50);
            Nation nation = new Nation("Old Name", player.getId());
            nationRepository.addNation(nation);
            player.setNationId(nation.getId());
            
            NationRenameCommand command = new NationRenameCommand(nationRepository, "New Name");

            // run
            command.execute(player);

            // verify
            UnityEngine.Debug.Assert(nation.getName() == "New Name");
        }

        public static void testRenameWithoutNation() {
            // prepare
            NationRepository nationRepository = new NationRepository();
            Player player = new Player(1, 2, new TickCounter(), 100, 50);
            NationRenameCommand command = new NationRenameCommand(nationRepository, "New Name");

            // run
            command.execute(player);

            // verify - should not crash
            UnityEngine.Debug.Assert(true);
        }

        public static void testRenameAsNonLeader() {
            // prepare
            NationRepository nationRepository = new NationRepository();
            EntityId leaderId = new EntityId();
            Nation nation = new Nation("Test Nation", leaderId);
            
            Player player = new Player(1, 2, new TickCounter(), 100, 50);
            nation.addMember(player.getId());
            nationRepository.addNation(nation);
            player.setNationId(nation.getId());
            
            NationRenameCommand command = new NationRenameCommand(nationRepository, "New Name");

            // run
            command.execute(player);

            // verify - name should not change
            UnityEngine.Debug.Assert(nation.getName() == "Test Nation");
        }

        public static void testRenameWithEmptyName() {
            // prepare
            NationRepository nationRepository = new NationRepository();
            Player player = new Player(1, 2, new TickCounter(), 100, 50);
            Nation nation = new Nation("Old Name", player.getId());
            nationRepository.addNation(nation);
            player.setNationId(nation.getId());
            
            NationRenameCommand command = new NationRenameCommand(nationRepository, "");

            // run
            command.execute(player);

            // verify - name should not change
            UnityEngine.Debug.Assert(nation.getName() == "Old Name");
        }
    }
}