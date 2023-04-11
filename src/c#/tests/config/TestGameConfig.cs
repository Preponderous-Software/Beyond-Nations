using UnityEngine;

using osg;

namespace osgtests
{
    public static class TestGameConfig
    {
        public static void runTests()
        {
            testInstantiation();
        }

        public static void testInstantiation()
        {
            // run
            GameConfig config = new GameConfig();

            // check
            Debug.Assert(config.getChunkSize() == 9);
            Debug.Assert(config.getLocationScale() == 9);
            Debug.Assert(config.getUpdateInterval() == 10);
            Debug.Assert(config.getStatusExpirationTicks() == 500);
            Debug.Assert(config.getPlayerWalkSpeed() == 20);
            Debug.Assert(config.getPlayerRunSpeed() == 50);
        }
    }
}
