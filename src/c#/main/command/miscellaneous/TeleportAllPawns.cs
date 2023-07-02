using UnityEngine;

namespace osg {

    public class TeleportAllPawnsCommand {
        private EntityRepository entityRepository;

        public TeleportAllPawnsCommand(EntityRepository entityRepository) {
            this.entityRepository = entityRepository;
        }

        public void execute(Player player) {
            foreach (Entity entity in entityRepository.getEntities()) {
                if (entity.getType() == EntityType.PAWN) {
                    Pawn pawn = (Pawn)entity;

                    if (pawn.isCurrentlyInSettlement()) {
                            pawn.setCurrentlyInSettlement(false);
                            Settlement settlement = entityRepository.getEntity(pawn.getHomeSettlementId()) as Settlement;
                            settlement.removeCurrentlyPresentEntity(pawn.getId());
                            pawn.createGameObject(player.getGameObject().transform.position + new Vector3(UnityEngine.Random.Range(-20, 20), 0, UnityEngine.Random.Range(-20, 20)));
                            pawn.setColor(settlement.getColor());
                    }
                    else {
                        pawn.getGameObject().transform.position = player.getGameObject().transform.position + new Vector3(UnityEngine.Random.Range(-20, 20), 0, UnityEngine.Random.Range(-20, 20));
                    }

                    pawn.setTargetEntity(null);
                }
            }
        }
    }
}