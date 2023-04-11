using UnityEngine;

using osg;

namespace osgtests
{
    public static class TestRockEntity
    {
        public static void runTests()
        {
            testInstantiation();
        }

        public static void testInstantiation()
        {
            // run
            RockEntity rockEntity = new RockEntity(new Vector3(0, 0, 0));

            // check
            Debug.Assert(rockEntity.getType() == EntityType.ROCK);
            Debug.Assert(rockEntity.getGameObject().name == "Rock");
            Debug.Assert(rockEntity.getGameObject().transform.position == new Vector3(0, 0, 0));
            Debug.Assert(rockEntity.getGameObject().transform.localScale == new Vector3(1, 1, 1));
            Debug.Assert(
                rockEntity.getGameObject().GetComponent<Renderer>().material.color == Color.gray
            );

            // clean up
            GameObject.Destroy(rockEntity.getGameObject());
        }
    }
}
