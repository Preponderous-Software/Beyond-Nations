using UnityEngine;

using beyondnations;

namespace beyondnationstests {

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
        }
    }
}