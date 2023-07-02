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
            UnityEngine.Debug.Assert(name != null);
            UnityEngine.Debug.Assert(name.Length > 0);
        }
    }
}