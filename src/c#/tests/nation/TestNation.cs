using UnityEngine;

using beyondnations;

namespace beyondnationstests {

    public static class TestNation {

        public static void runTests() {
            testInitialization();
            testAddMember();
            testRemoveMember();
        }

        public static void testInitialization() {
            // prepare
            string name = "Test Nation";
            EntityId leaderId = new EntityId();

            // run
            Nation nation = new Nation(name, leaderId);

            // verify
            UnityEngine.Debug.Assert(nation.getId() != null);
            UnityEngine.Debug.Assert(nation.getName() == name);
            UnityEngine.Debug.Assert(nation.getLeaderId() == leaderId);
            UnityEngine.Debug.Assert(nation.getNumberOfMembers() == 1);
            UnityEngine.Debug.Assert(nation.isMember(leaderId));
            UnityEngine.Debug.Assert(nation.getColor() != null);
        }

        public static void testAddMember() {
            // prepare
            string name = "Test Nation";
            EntityId leaderId = new EntityId();
            Nation nation = new Nation(name, leaderId);

            // run
            EntityId memberId = new EntityId();
            nation.addMember(memberId);

            // verify
            UnityEngine.Debug.Assert(nation.getNumberOfMembers() == 2);
            UnityEngine.Debug.Assert(nation.isMember(memberId));
        }

        public static void testRemoveMember() {
            // prepare
            string name = "Test Nation";
            EntityId leaderId = new EntityId();
            Nation nation = new Nation(name, leaderId);

            // run
            EntityId memberId = new EntityId();
            nation.addMember(memberId);
            nation.removeMember(memberId);

            // verify
            UnityEngine.Debug.Assert(nation.getNumberOfMembers() == 1);
            UnityEngine.Debug.Assert(!nation.isMember(memberId));
        }
    }
}