using UnityEngine;

using osg;

namespace osgtests
{
    public static class TestNation
    {
        public static void runTests()
        {
            testInitialization();
            testAddMember();
            testRemoveMember();
        }

        public static void testInitialization()
        {
            // prepare
            string name = "Test Nation";
            EntityId leaderId = new EntityId();

            // run
            Nation nation = new Nation(name, leaderId);

            // verify
            Debug.Assert(nation.getId() != null);
            Debug.Assert(nation.getName() == name);
            Debug.Assert(nation.getLeaderId() == leaderId);
            Debug.Assert(nation.getNumberOfMembers() == 1);
            Debug.Assert(nation.isMember(leaderId));
            Debug.Assert(nation.getColor() != null);
        }

        public static void testAddMember()
        {
            // prepare
            string name = "Test Nation";
            EntityId leaderId = new EntityId();
            Nation nation = new Nation(name, leaderId);

            // run
            EntityId memberId = new EntityId();
            nation.addMember(memberId);

            // verify
            Debug.Assert(nation.getNumberOfMembers() == 2);
            Debug.Assert(nation.isMember(memberId));
        }

        public static void testRemoveMember()
        {
            // prepare
            string name = "Test Nation";
            EntityId leaderId = new EntityId();
            Nation nation = new Nation(name, leaderId);

            // run
            EntityId memberId = new EntityId();
            nation.addMember(memberId);
            nation.removeMember(memberId);

            // verify
            Debug.Assert(nation.getNumberOfMembers() == 1);
            Debug.Assert(!nation.isMember(memberId));
        }
    }
}
