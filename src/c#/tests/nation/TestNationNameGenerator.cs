using UnityEngine;

using osg;

namespace osgtests {

    public static class TestNationNameGenerator {

        public static void runTests() {
            testGenerate();
        }

        public static void testGenerate() {
            // run
            string name = NationNameGenerator.generate();

            // verify
            Debug.Assert(name != null);
            Debug.Assert(name.Length > 0);
        }
    }
}