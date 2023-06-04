using UnityEngine;

namespace osg {

    public class SpawnMoneyCommand {

        public void execute(Player player) {
            player.getInventory().addItem(ItemType.GOLD_COIN, 100);
            player.getStatus().update("Spawned 100 gold coins.");
        }
    }
}