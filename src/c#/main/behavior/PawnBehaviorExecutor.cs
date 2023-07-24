using System.Net;
using UnityEngine;
using System.Collections.Generic;

namespace osg {

    /**
     * A class that executes the behavior of a pawn.
     */
    public class PawnBehaviorExecutor {
        private Environment environment;
        private NationRepository nationRepository;
        private EventProducer eventProducer;
        private EntityRepository entityRepository;

        public PawnBehaviorExecutor(Environment environment, NationRepository nationRepository, EventProducer eventProducer, EntityRepository entityRepository) {
            this.environment = environment;
            this.nationRepository = nationRepository;
            this.eventProducer = eventProducer;
            this.entityRepository = entityRepository;
        }

        public void executeBehavior(Pawn pawn, BehaviorType behaviorType) {
            switch (behaviorType) {
                case BehaviorType.NONE:
                    break;
                case BehaviorType.GATHER_RESOURCES:
                    executeGatherResourcesBehavior(pawn);
                    break;
                case BehaviorType.SELL_RESOURCES:
                    executeSellResourcesBehavior(pawn);
                    break;
                case BehaviorType.WANDER:
                    executeWanderBehavior(pawn);
                    break;
                case BehaviorType.PURCHASE_FOOD:
                    executePurchaseFoodBehavior(pawn);
                    break;
                case BehaviorType.CONSTRUCT_SETTLEMENT:
                    executeConstructSettlementBehavior(pawn);
                    break;
                case BehaviorType.GO_TO_HOME_SETTLEMENT:
                    executeGoToHomeSettlementBehavior(pawn);
                    break;
                case BehaviorType.EXIT_SETTLEMENT:
                    executeExitSettlementBehavior(pawn);
                    break;
                case BehaviorType.PLANT_SAPLING:
                    executePlantSaplingBehavior(pawn);
                    break;
                case BehaviorType.CONSTRUCT_STALL:
                    executeConstructStallBehavior(pawn);
                    break;
                case BehaviorType.PURCHASE_STALL:
                    executePurchaseStallBehavior(pawn);
                    break;
                case BehaviorType.TRANSFER_ITEMS_TO_STALL:
                    executeTransferItemsToStallBehavior(pawn);
                    break;
                case BehaviorType.JOIN_NATION:
                    executeJoinNationBehavior(pawn);
                    break;
                case BehaviorType.CREATE_NATION:
                    executeCreateNationBehavior(pawn);
                    break;
                case BehaviorType.JOIN_RANDOM_SETTLEMENT:
                    executeJoinRandomSettlementBehavior(pawn);
                    break;
                case BehaviorType.COLLECT_PROFIT_FROM_STALL:
                    executeCollectProfitFromStallBehavior(pawn);
                    break;
                case BehaviorType.COLLECT_FOOD_FROM_STALL:
                    executeCollectFoodFromStallBehavior(pawn);
                    break;
                default:
                    Debug.LogError("Behavior type " + behaviorType + " is not implemented.");
                    break;
            }
        }

        private void executeGatherResourcesBehavior(Pawn pawn) {
            if (!pawn.hasTargetEntity() || (pawn.getTargetEntity().getType() != EntityType.TREE && pawn.getTargetEntity().getType() != EntityType.ROCK)) {
                // select nearest tree or rock
                Entity nearestTree = environment.getNearestTree(pawn.getPosition());
                Entity nearestRock = environment.getNearestRock(pawn.getPosition());
                if (nearestTree != null && nearestRock != null) {
                    if (Vector3.Distance(pawn.getPosition(), nearestTree.getPosition()) < Vector3.Distance(pawn.getPosition(), nearestRock.getPosition())) {
                        pawn.setTargetEntity(nearestTree);
                    } else {
                        pawn.setTargetEntity(nearestRock);
                    }
                } else if (nearestTree != null) {
                    pawn.setTargetEntity(nearestTree);
                } else if (nearestRock != null) {
                    pawn.setTargetEntity(nearestRock);
                }
            }

            Entity targetEntity = pawn.getTargetEntity();
            if (targetEntity == null) {
                Debug.LogWarning("Pawn " + pawn + " has no target entity in gather resources behavior.");
                return;
            }
            EntityType targetEntityType = targetEntity.getType();
            
            if (pawn.isAtTargetEntity()) {
                // gather
                if (targetEntity.getType() == EntityType.TREE || targetEntityType == EntityType.ROCK) {
                    targetEntity.markForDeletion();
                    pawn.getInventory().transferContentsOfInventory(targetEntity.getInventory());
                    pawn.setTargetEntity(null);

                    if (targetEntity.getType() == EntityType.TREE && pawn.getInventory().getNumItems(ItemType.SAPLING) > 0) {
                        pawn.setCurrentBehaviorType(BehaviorType.PLANT_SAPLING);
                    }
                    else {
                        pawn.setCurrentBehaviorType(BehaviorType.NONE);
                    }
                }
                else {
                    Debug.LogWarning("Pawn " + pawn + " is at target entity " + targetEntity + " but it is not a tree or rock.");
                    pawn.setTargetEntity(null);
                }
            }
            else {
                // move towards target entity
                pawn.moveTowardsTargetEntity();
            }
        }

        private void executeSellResourcesBehavior(Pawn pawn) {
            Settlement settlement = (Settlement) entityRepository.getEntity(pawn.getCurrentSettlementId());
            
            Market market = settlement.getMarket();
            EntityId stallOwnerId = market.sellResources(pawn);
            if (stallOwnerId == null) {
                Debug.LogWarning("Pawn " + pawn + " tried to sell resources but was unable to.");
                pawn.setCurrentBehaviorType(BehaviorType.NONE);
                return;
            }
            else {
                // increase relationship with stall owner
                Entity stallOwner = entityRepository.getEntity(stallOwnerId);
                if (stallOwner is Pawn) {
                    Pawn stallOwnerPawn = (Pawn) stallOwner;
                    increaseRelationship(pawn, stallOwnerPawn, 1);
                }
                else if (stallOwner is Player) {
                    Player stallOwnerPlayer = (Player) stallOwner;
                    increaseRelationship(pawn, stallOwnerPlayer, 1);
                }
                else {
                    Debug.LogError("Stall owner " + stallOwner + " is not a pawn or player.");
                }
            }
            pawn.setCurrentBehaviorType(BehaviorType.NONE);
        }

        private void executeWanderBehavior(Pawn pawn) {
            // 80% chance to skip
            if (UnityEngine.Random.Range(0, 100) < 80) {
                return;
            }

            if (pawn.isCurrentlyInSettlement()) {
                Debug.LogError("Pawn " + pawn + " is currently in a settlement but is trying to wander.");
                return;
            }
            Vector3 currentPosition = pawn.getPosition();
            Vector3 targetPosition = currentPosition + new Vector3(UnityEngine.Random.Range(-1f, 1f), 0, UnityEngine.Random.Range(-1f, 1f));
            pawn.getGameObject().GetComponent<Rigidbody>().velocity = (targetPosition - currentPosition).normalized * pawn.getSpeed();
        }

        private void executePurchaseFoodBehavior(Pawn pawn) {
            // pawn is assumed to be in a settlement
            if (!pawn.isCurrentlyInSettlement()) {
                Debug.LogError("Pawn " + pawn + " is not currently in a settlement but is trying to purchase food.");
                return;
            }

            // purchase food from settlement market
            Settlement settlement = (Settlement) entityRepository.getEntity(pawn.getCurrentSettlementId());
            if (settlement == null) {
                Debug.LogError("Pawn " + pawn.getName() + " is trying to purchase food from settlement market " + pawn.getCurrentSettlementId() + " but it does not exist.");
                return;
            }
            Market market = settlement.getMarket();
            EntityId stallOwnerId = market.purchaseFood(pawn);
            if (stallOwnerId == null) {
                Debug.LogWarning("Pawn " + pawn.getName() + " tried to purchase food from settlement market " + settlement + " but there was not enough food.");
            }
            else {
                // increase relationship with stall owner
                Entity stallOwner = entityRepository.getEntity(stallOwnerId);
                if (stallOwner is Pawn) {
                    Pawn stallOwnerPawn = (Pawn) stallOwner;
                    increaseRelationship(pawn, stallOwnerPawn, 1);
                }
                else if (stallOwner is Player) {
                    Player stallOwnerPlayer = (Player) stallOwner;
                    increaseRelationship(pawn, stallOwnerPlayer, 1);
                }
                else {
                    Debug.LogError("Stall owner " + stallOwner + " is not a pawn or player.");
                }
            }
            pawn.setCurrentBehaviorType(BehaviorType.NONE);
        }

        private void executeConstructSettlementBehavior(Pawn pawn) {
            Vector3 targetPosition = pawn.getPosition();

            // get nation color
            Nation nation = nationRepository.getNation(pawn.getNationId());
            Color nationColor = nation.getColor();

            // remove wood
            Inventory inventory = pawn.getInventory();
            if (inventory.getNumItems(ItemType.WOOD) < Settlement.WOOD_COST_TO_BUILD) {
                Debug.LogError("Pawn " + pawn + " does not have enough wood to build a settlement but is trying to.");
                return;
            }
            inventory.removeItem(ItemType.WOOD, Settlement.WOOD_COST_TO_BUILD);

            // create settlement
            Settlement settlement = new Settlement(targetPosition, nation.getId(), nationColor, nation.getName());
            nation.addSettlement(settlement.getId());
            entityRepository.addEntity(settlement);
            pawn.setHomeSettlementId(settlement.getId());
            pawn.setCurrentBehaviorType(BehaviorType.WANDER);
        }

        private void executeGoToHomeSettlementBehavior(Pawn pawn) {
            NationId nationId = pawn.getNationId();
            if (nationId == null) {
                Debug.LogError("Pawn " + pawn + " has no nation id.");
                return;
            }
            Nation nation = nationRepository.getNation(nationId);
            Settlement homeSettlement = null;
            if (nation != null && nation.getNumberOfSettlements() > 0) {
                EntityId homeSettlementId = pawn.getHomeSettlementId();
                if (homeSettlementId == null) {
                    Debug.LogError("Pawn " + pawn + " has no settlement id.");
                    return;
                }
                homeSettlement = (Settlement) entityRepository.getEntity(homeSettlementId);
            }

            if (!pawn.hasTargetEntity()) {
                if (homeSettlement != null) {
                    pawn.setTargetEntity(homeSettlement);
                }
            }
            else if (pawn.isAtTargetEntity(20)) {
                if (homeSettlement == null) {
                    Debug.LogError("Pawn " + pawn + " has no settlement to go to.");
                    return;
                }
                homeSettlement.addCurrentlyPresentEntity(pawn.getId());
                pawn.setCurrentSettlementId(homeSettlement.getId());
                pawn.destroyGameObject();
                pawn.setTargetEntity(null);
                pawn.setCurrentBehaviorType(BehaviorType.NONE);
                return;
            }
            else {
                // move towards target entity
                pawn.moveTowardsTargetEntity();
            }
        }

        private void executeExitSettlementBehavior(Pawn pawn) {
            // if not in settlement
            if (!pawn.isCurrentlyInSettlement()) {
                Debug.LogError("Pawn " + pawn + " is not currently in a settlement but is trying to exit one.");
                return;
            }

            Settlement settlement = entityRepository.getEntity(pawn.getCurrentSettlementId()) as Settlement;
            settlement.removeCurrentlyPresentEntity(pawn.getId());
            pawn.clearCurrentSettlementId();
            pawn.createGameObject(settlement.getGameObject().transform.position + new Vector3(UnityEngine.Random.Range(-20, 20), 0, UnityEngine.Random.Range(-20, 20)));
            pawn.setColor(settlement.getColor());
            pawn.setCurrentBehaviorType(BehaviorType.NONE);
        }

        private void executePlantSaplingBehavior(Pawn pawn) {
            // if pawn has no saplings, skip
            if (pawn.getInventory().getNumItems(ItemType.SAPLING) == 0) {
                Debug.LogWarning("Pawn " + pawn + " has no saplings to plant.");
                return;
            }

            Vector3 position = pawn.getPosition();
            position += new Vector3(UnityEngine.Random.Range(-5f, 5f), 0, UnityEngine.Random.Range(-5f, 5f));
            position.y = 2;
            Sapling tree = new Sapling(position, 3);
            entityRepository.addEntity(tree);
            pawn.getInventory().removeItem(ItemType.SAPLING, 1);
            pawn.setCurrentBehaviorType(BehaviorType.WANDER);
        }

        private void executeConstructStallBehavior(Pawn pawn) {
            // if not enough wood
            if (pawn.getInventory().getNumItems(ItemType.WOOD) < Stall.WOOD_COST_TO_BUILD) {
                Debug.LogError("Pawn " + pawn + " does not have enough wood to build a stall, but is trying to.");
                return;
            }

            // if not leader
            Nation nation = nationRepository.getNation(pawn.getNationId());
            if (nation.getRole(pawn.getId()) != NationRole.LEADER) {
                Debug.LogError("Pawn " + pawn + " is not a leader but is trying to build a stall.");
                return;
            }

            // if not currently in a settlement
            EntityId currentSettlementId = pawn.getCurrentSettlementId();
            if (currentSettlementId == null) {
                Debug.LogError("Pawn " + pawn + " is not in a settlement but is trying to build a stall.");
                return;
            }

            // remove wood
            pawn.getInventory().removeItem(ItemType.WOOD, Stall.WOOD_COST_TO_BUILD);
            
            // build stall
            Settlement currentSettlement = (Settlement) entityRepository.getEntity(currentSettlementId);
            Market market = currentSettlement.getMarket();

            market.createStall();
            pawn.setCurrentBehaviorType(BehaviorType.NONE);       
        }

        private void executePurchaseStallBehavior(Pawn pawn) {
            EntityId currentSettlementId = pawn.getCurrentSettlementId();
            if (currentSettlementId == null) {
                Debug.LogError("Pawn " + pawn + " is not in a settlement but is trying to purchase a stall.");
                return;
            }
            Settlement currentSettlement = (Settlement) entityRepository.getEntity(currentSettlementId);
            Market market = currentSettlement.getMarket();

            // if no stalls for sale
            if (market.getNumStallsForSale() == 0) {
                Debug.LogWarning("Pawn " + pawn + " is trying to purchase a stall but there are none for sale.");
                return;
            }

            // if not enough money
            if (pawn.getInventory().getNumItems(ItemType.COIN) < Stall.COIN_COST_TO_PURCHASE) {
                Debug.LogWarning("Pawn " + pawn + " is trying to purchase a stall but does not have enough money.");
                return;
            }

            // remove money
            pawn.getInventory().removeItem(ItemType.COIN, Stall.COIN_COST_TO_PURCHASE);

            // assign ownership of stall to pawn
            Stall stall = market.getStallForSale();
            stall.setOwnerId(pawn.getId());
            Nation nation = nationRepository.getNation(pawn.getNationId());
            nation.setRole(pawn.getId(), NationRole.MERCHANT);
            pawn.setCurrentBehaviorType(BehaviorType.NONE);
        }

        private void executeTransferItemsToStallBehavior(Pawn pawn) {
            EntityId currentSettlementId = pawn.getCurrentSettlementId();
            if (currentSettlementId == null) {
                Debug.LogError("Pawn " + pawn + " has no home settlement id.");
                return;
            }
            Settlement currentSettlement = (Settlement) entityRepository.getEntity(currentSettlementId);
            Market market = currentSettlement.getMarket();

            // if no stalls owned
            if (market.getStall(pawn.getId()) == null) {
                Debug.LogWarning("Pawn " + pawn + " is trying to transfer items to a stall but does not own any stalls.");
                return;
            }

            // if no wood or stone
            if (pawn.getInventory().getNumItems(ItemType.WOOD) == 0 && pawn.getInventory().getNumItems(ItemType.STONE) == 0) {
                Debug.LogWarning("Pawn " + pawn + " is trying to transfer items to a stall but has no wood or stone.");
                return;
            }

            // transfer items
            Stall stall = market.getStall(pawn.getId());
            stall.transferContentsFromEntity(pawn);

            // get half of coins from stall
            int numCoins = stall.getInventory().getNumItems(ItemType.COIN);
            int numCoinsToTransfer = numCoins / 2;
            stall.getInventory().removeItem(ItemType.COIN, numCoinsToTransfer);
            pawn.getInventory().addItem(ItemType.COIN, numCoinsToTransfer);

            Debug.Log("Pawn " + pawn.getName() + " transferred items to their stall.");
            pawn.setCurrentBehaviorType(BehaviorType.NONE);
        }

        private void executeJoinNationBehavior(Pawn pawn) {
            Settlement nearestSettlement = (Settlement) environment.getNearestEntityOfType(pawn.getGameObject().transform.position, EntityType.SETTLEMENT);
            Nation nation = nationRepository.getNation(nearestSettlement.getNationId());
            nation.addMember(pawn.getId());
            pawn.setNationId(nation.getId());
            pawn.setColor(nation.getColor());
            eventProducer.produceNationJoinEvent(nation, pawn.getId());
            pawn.setCurrentBehaviorType(BehaviorType.NONE);
        }

        private void executeCreateNationBehavior(Pawn pawn) {
            Nation nation = new Nation(NationNameGenerator.generate(), pawn.getId());
            nationRepository.addNation(nation);
            pawn.setNationId(nation.getId());
            pawn.setColor(nation.getColor());
            eventProducer.produceNationCreationEvent(nation);
            pawn.setCurrentBehaviorType(BehaviorType.NONE);
        }

        private void executeJoinRandomSettlementBehavior(Pawn pawn) {
            // get nation
            Nation nation = nationRepository.getNation(pawn.getNationId());

            // if no settlements
            if (nation.getNumberOfSettlements() == 0) {
                Debug.LogWarning("Pawn " + pawn + " is trying to join a random settlement but their nation has no settlements.");
                return;
            }

            // get random settlement
            EntityId settlementId = nation.getRandomSettlementId();
            Settlement settlement = (Settlement) entityRepository.getEntity(settlementId);

            // join settlement
            pawn.setHomeSettlementId(settlementId);
            pawn.setCurrentBehaviorType(BehaviorType.NONE);
        }

        private void executeCollectProfitFromStallBehavior(Pawn pawn) {
            EntityId currentSettlementId = pawn.getCurrentSettlementId();
            if (currentSettlementId == null) {
                Debug.LogError("Pawn " + pawn + " is not in a settlement but is trying to collect profit from a stall.");
                return;
            }
            Settlement currentSettlement = (Settlement) entityRepository.getEntity(currentSettlementId);
            Market market = currentSettlement.getMarket();

            // if no stalls owned
            if (market.getStall(pawn.getId()) == null) {
                Debug.LogWarning("Pawn " + pawn + " is trying to collect profit from a stall but does not own any stalls.");
                return;
            }

            // collect profit
            Stall stall = market.getStall(pawn.getId());
            int profit = stall.getInventory().getNumItems(ItemType.COIN);
            pawn.getInventory().addItem(ItemType.COIN, profit);
            stall.getInventory().removeItem(ItemType.COIN, profit);
            Debug.Log("Pawn " + pawn.getName() + " collected " + profit + " coins from their stall.");
            pawn.setCurrentBehaviorType(BehaviorType.NONE);
        }

        private void executeCollectFoodFromStallBehavior(Pawn pawn) {
            EntityId currentSettlementId = pawn.getCurrentSettlementId();
            if (currentSettlementId == null) {
                Debug.LogError("Pawn " + pawn + " has no home settlement id.");
                return;
            }
            Settlement currentSettlement = (Settlement) entityRepository.getEntity(currentSettlementId);
            Market market = currentSettlement.getMarket();

            // if no stalls owned
            if (market.getStall(pawn.getId()) == null) {
                Debug.LogWarning("Pawn " + pawn + " is trying to collect food from a stall but does not own any stalls.");
                return;
            }

            // collect food
            Stall stall = market.getStall(pawn.getId());
            int food = stall.getInventory().getNumItems(ItemType.APPLE);
            int foodToTransfer = food / 2;
            if (foodToTransfer == 0) {
                foodToTransfer = 1;
            }
            pawn.getInventory().addItem(ItemType.APPLE, foodToTransfer);
            stall.getInventory().removeItem(ItemType.APPLE, foodToTransfer);
            Debug.Log("Pawn " + pawn.getName() + " collected " + foodToTransfer + " food from their stall.");
            pawn.setCurrentBehaviorType(BehaviorType.NONE);
        }

        // helpers -----------------------------------------------------------------------------------------------

        private void increaseRelationship(Pawn pawn, Pawn otherPawn, int amount) {
            pawn.increaseRelationship(otherPawn, amount);
            otherPawn.increaseRelationship(pawn, amount);
        }

        private void increaseRelationship(Pawn pawn, Player player, int amount) {
            pawn.increaseRelationship(player, amount);
            player.increaseRelationship(pawn, amount);
            player.getStatus().update("Relationship with " + pawn.getName() + " is now " + player.getRelationships()[pawn.getId()] + ".");
        }
    }
}