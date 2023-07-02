using UnityEngine;

namespace osg {
    public class PlantSaplingCommand {
        private EntityRepository entityRepository;

        public PlantSaplingCommand(EntityRepository entityRepository) {
            this.entityRepository = entityRepository;
        }

        public void execute(Player player) {
            if (player.getInventory().getNumItems(ItemType.SAPLING) > 0) {
                player.getInventory().removeItem(ItemType.SAPLING, 1);
                Sapling sapling = new Sapling(player.getGameObject().transform.position, 3);
                sapling.getGameObject().transform.position += new Vector3(UnityEngine.Random.Range(-5, 5), 0, UnityEngine.Random.Range(-5, 5));
                entityRepository.addEntity(sapling);
            }
            else {
                player.getStatus().update("You don't have any saplings.");
            }
        }
    }
}