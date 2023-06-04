namespace osg {

    public class TeleportAllPawnsCommand {
        private Environment environment;
        private EntityRepository entityRepository;

        public TeleportAllPawnsCommand(Environment environment, EntityRepository entityRepository) {
            this.environment = environment;
            this.entityRepository = entityRepository;
        }

        public void execute(Player player) {
            foreach (Entity entity in entityRepository.getEntities()) {
                if (entity.getType() == EntityType.PAWN) {
                    Pawn pawn = (Pawn)entity;
                    pawn.getGameObject().transform.position = player.getGameObject().transform.position;
                    pawn.setTargetEntity(null);
                }
            }
        }
    }
}