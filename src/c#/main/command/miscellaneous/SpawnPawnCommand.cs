using UnityEngine;

namespace beyondnations {

    public class SpawnPawnCommand {
        private EventProducer eventProducer;
        private EntityRepository entityRepository;

        public SpawnPawnCommand(EventProducer eventProducer, EntityRepository entityRepository) {
            this.eventProducer = eventProducer;
            this.entityRepository = entityRepository;
        }

        public void execute(Player player) {
            Vector3 position = player.getGameObject().transform.position;
            position += new Vector3(UnityEngine.Random.Range(-5f, 5f), 0, UnityEngine.Random.Range(-5f, 5f));
            Pawn pawn = new Pawn(position, PawnNameGenerator.generate());
            eventProducer.producePawnSpawnEvent(position, pawn);
            entityRepository.addEntity(pawn);
        }
    }
}