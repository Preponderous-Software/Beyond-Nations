using UnityEngine;

using osg;

namespace osgtests {

    public static class TestNationId {

        public static void runTests() {
            testInitialization();
            testEquality();
        }

        public static void testInitialization() {
            // run
            NationId id = new NationId();

            // verify
            UnityEngine.Debug.Assert(id != null);
        }

        public static void testEquality() {
            // prepare
            NationId id1 = new NationId();
            NationId id2 = new NationId();

            // verify
            UnityEngine.Debug.Assert(id1 != id2);
            UnityEngine.Debug.Assert(id1 == id1);
            UnityEngine.Debug.Assert(id2 == id2);
        }
    }
}