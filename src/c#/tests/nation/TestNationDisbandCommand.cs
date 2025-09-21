using UnityEngine;

using beyondnations;

namespace beyondnationstests {

    public static class TestNationDisbandCommand {

        public static void runTests() {
            testSuccessfulDisband();
            testDisbandWithoutNation();
            testDisbandAsNonLeader();
        }

        public static void testSuccessfulDisband() {
            // prepare
            NationRepository nationRepository = new NationRepository();
            EntityRepository entityRepository = new EntityRepository();
            EventRepository eventRepository = new EventRepository();
            EventProducer eventProducer = new EventProducer(eventRepository);
            
            Player player = new Player(1.0f, 2.0f, new TickCounter(), 100, 50);
            Nation nation = new Nation("Test Nation", player.getId());
            nationRepository.addNation(nation);
            player.setNationId(nation.getId());
            entityRepository.addEntity(player);
            
            NationDisbandCommand command = new NationDisbandCommand(nationRepository, entityRepository, eventProducer);

            // run
            command.execute(player);

            // verify
            UnityEngine.Debug.Assert(nationRepository.getNation(nation.getId()) == null);
            UnityEngine.Debug.Assert(player.getNationId() == null);
        }

        public static void testDisbandWithoutNation() {
            // prepare
            NationRepository nationRepository = new NationRepository();
            EntityRepository entityRepository = new EntityRepository();
            EventRepository eventRepository = new EventRepository();
            EventProducer eventProducer = new EventProducer(eventRepository);
            
            Player player = new Player(1.0f, 2.0f, new TickCounter(), 100, 50);
            
            NationDisbandCommand command = new NationDisbandCommand(nationRepository, entityRepository, eventProducer);

            // run
            command.execute(player);

            // verify - should not crash
            UnityEngine.Debug.Assert(true);
        }

        public static void testDisbandAsNonLeader() {
            // prepare
            NationRepository nationRepository = new NationRepository();
            EntityRepository entityRepository = new EntityRepository();
            EventRepository eventRepository = new EventRepository();
            EventProducer eventProducer = new EventProducer(eventRepository);
            
            EntityId leaderId = new EntityId();
            Nation nation = new Nation("Test Nation", leaderId);
            
            Player player = new Player(1.0f, 2.0f, new TickCounter(), 100, 50);
            nation.addMember(player.getId());
            nationRepository.addNation(nation);
            player.setNationId(nation.getId());
            
            NationDisbandCommand command = new NationDisbandCommand(nationRepository, entityRepository, eventProducer);

            // run
            command.execute(player);

            // verify - nation should still exist
            UnityEngine.Debug.Assert(nationRepository.getNation(nation.getId()) != null);
        }
    }
}