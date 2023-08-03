using UnityEngine;

using beyondnations;

namespace beyondnationstests {

    public static class TestNationRepository {

        public static void runTests() {
            testInitialization();
            testAddNation();
            testRemoveNation();
            testGetNationByNationId();
        }

        public static void testInitialization() {
            // run
            NationRepository repository = new NationRepository();

            // verify
            UnityEngine.Debug.Assert(repository != null);
        }

        public static void testAddNation() {
            // prepare
            NationRepository repository = new NationRepository();
            EntityId leaderId = new EntityId();
            Nation nation = new Nation("Test Nation", leaderId);

            // run
            repository.addNation(nation);

            // verify
            UnityEngine.Debug.Assert(repository.getNation(nation.getId()) == nation);     
        }

        public static void testRemoveNation() {
            // prepare
            NationRepository repository = new NationRepository();
            EntityId leaderId = new EntityId();
            Nation nation = new Nation("Test Nation", leaderId);
            repository.addNation(nation);

            // run
            repository.removeNation(nation);

            // verify
            UnityEngine.Debug.Assert(repository.getNation(nation.getId()) == null);
        }

        public static void testGetNationByNationId() {
            // prepare
            NationRepository repository = new NationRepository();
            EntityId leaderId = new EntityId();
            Nation nation = new Nation("Test Nation", leaderId);
            repository.addNation(nation);

            // run
            Nation retrievedNation = repository.getNation(nation.getId());

            // verify
            UnityEngine.Debug.Assert(retrievedNation.getId() == nation.getId());
        }
    }
}