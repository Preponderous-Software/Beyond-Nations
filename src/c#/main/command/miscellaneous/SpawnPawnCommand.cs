using UnityEngine;

namespace osg {

    public class SpawnPawnCommand {
        private Environment environment;
        private EventProducer eventProducer;

        public SpawnPawnCommand(Environment environment, EventProducer eventProducer) {
            this.environment = environment;
            this.eventProducer = eventProducer;
        }

        public void execute(Player player) {
            Vector3 position = player.getGameObject().transform.position;
            Pawn pawn = new Pawn(position, PawnNameGenerator.generate());
            eventProducer.producePawnSpawnEvent(position, pawn);

            Chunk playerChunk = environment.getChunkAtPosition(position);
            playerChunk.addEntity(pawn);
            environment.addEntityId(pawn.getId());
            pawn.getGameObject().transform.parent = playerChunk.getGameObject().transform;
        }
    }
}