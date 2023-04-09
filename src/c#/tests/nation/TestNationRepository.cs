using UnityEngine;

using osg;

namespace osgtests {

    public static class TestNationRepository {

        public static void runTests() {
            testInitialization();
            testAddNation();
            testRemoveNation();
            testGetNation();
        }

        public static void testInitialization() {
            // run
            NationRepository repository = new NationRepository();

            // verify
            Debug.Assert(repository != null);
        }

        public static void testAddNation() {
            // prepare
            NationRepository repository = new NationRepository();
            EntityId leaderId = new EntityId();
            Nation nation = new Nation("Test Nation", leaderId);

            // run
            repository.addNation(nation);

            // verify
            Debug.Assert(repository.getNation(nation.getId()) == nation);
            Debug.Assert(repository.getNation(leaderId) == nation);            
        }

        public static void testRemoveNation() {
            // prepare
            NationRepository repository = new NationRepository();
            EntityId leaderId = new EntityId();
            Nation nation = new Nation("Test Nation", leaderId);
            repository.addNation(nation);

            // run
            repository.removeNation(nation.getId());

            // verify
            Debug.Assert(repository.getNation(nation.getId()) == null);
            Debug.Assert(repository.getNation(leaderId) == null);
        }

        public static void testGetNation() {
            // prepare
            NationRepository repository = new NationRepository();
            EntityId leaderId = new EntityId();
            Nation nation = new Nation("Test Nation", leaderId);
            repository.addNation(nation);

            // run
            Nation nation1 = repository.getNation(nation.getId());
            Nation nation2 = repository.getNation(leaderId);

            // verify
            Debug.Assert(nation1 == nation);
            Debug.Assert(nation2 == nation);
        }
    }
}