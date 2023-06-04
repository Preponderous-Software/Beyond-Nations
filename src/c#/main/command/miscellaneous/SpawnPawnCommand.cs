using UnityEngine;

namespace osg {

    public class SpawnPawnCommand {
        private Environment environment;
        private EventProducer eventProducer;
        private EntityRepository entityRepository;

        public SpawnPawnCommand(Environment environment, EventProducer eventProducer, EntityRepository entityRepository) {
            this.environment = environment;
            this.eventProducer = eventProducer;
            this.entityRepository = entityRepository;
        }

        public void execute(Player player) {
            Vector3 position = player.getGameObject().transform.position;
            Pawn pawn = new Pawn(position, PawnNameGenerator.generate());
            eventProducer.producePawnSpawnEvent(position, pawn);
            entityRepository.addEntity(pawn);
        }
    }
}