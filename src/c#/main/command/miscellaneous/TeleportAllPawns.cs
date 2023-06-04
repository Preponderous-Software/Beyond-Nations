namespace osg {

    public class TeleportAllPawnsCommand {
        private Environment environment;

        public TeleportAllPawnsCommand(Environment environment) {
            this.environment = environment;
        }

        public void execute(Player player) {
            foreach (Chunk chunk in environment.getChunks()) {
                foreach (Entity entity in chunk.getEntities()) {
                    if (entity.getType() == EntityType.PAWN) {
                        Pawn pawn = (Pawn)entity;
                        pawn.getGameObject().transform.position = player.getGameObject().transform.position;
                        pawn.setTargetEntity(null);
                    }
                }
            }
        }
    }
}