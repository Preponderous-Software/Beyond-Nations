using UnityEngine;
using Vector3 = System.Numerics.Vector3;

namespace beyondnations {
    public class PlantSaplingCommand {
        private RandomSource random;
        private EntityRepository entityRepository;

        public PlantSaplingCommand(EntityRepository entityRepository, RandomSource random) {
            this.random = random;
            this.entityRepository = entityRepository;
        }

        public void execute(Player player) {
            if (player.getInventory().getNumItems(ItemType.SAPLING) > 0) {
                player.getInventory().removeItem(ItemType.SAPLING, 1);
                Sapling sapling = new Sapling(player.getGameObject().transform.position, 3, random);
                sapling.getGameObject().transform.position += new Vector3(random.range(-5, 5), 0, random.range(-5, 5));
                sapling.getGameObject().transform.position = new Vector3(sapling.getGameObject().transform.position.X, 2, sapling.getGameObject().transform.position.Z);
                entityRepository.addEntity(sapling);
            }
            else {
                player.getStatus().update("You don't have any saplings.");
            }
        }
    }
}