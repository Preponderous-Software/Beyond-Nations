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
                default:
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
            Settlement settlement = (Settlement) entityRepository.getEntity(pawn.getHomeSettlementId());
            
            Market market = settlement.getMarket();
            market.sellResources(pawn);
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
            Settlement settlement = (Settlement) entityRepository.getEntity(pawn.getHomeSettlementId());
            if (settlement == null) {
                Debug.LogError("Pawn " + pawn + " is trying to purchase food from settlement " + pawn.getHomeSettlementId() + " but it does not exist.");
                return;
            }
            Market market = settlement.getMarket();
            bool result = market.purchaseFood(pawn);
            if (!result) {
                Debug.LogWarning("Pawn " + pawn + " tried to purchase food from settlement " + settlement + " but there was not enough food.");
            }
            else {
                Debug.Log("[PBE DEBUG] Pawn " + pawn + " purchased food from settlement " + settlement + ".");
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
            Nation nation = nationRepository.getNation(pawn.getNationId());
            Settlement settlement = null;
            if (nation != null && nation.getNumberOfSettlements() > 0) {
                EntityId settlementId = pawn.getHomeSettlementId();
                if (settlementId == null) {
                    Debug.LogError("Pawn " + pawn + " has no settlement id.");
                    return;
                }
                settlement = (Settlement) entityRepository.getEntity(settlementId);
            }

            if (!pawn.hasTargetEntity()) {
                if (settlement != null) {
                    pawn.setTargetEntity(settlement);
                }
            }
            else if (pawn.isAtTargetEntity(20)) {
                if (settlement == null) {
                    Debug.LogError("Pawn " + pawn + " has no settlement to go to.");
                    return;
                }
                settlement.addCurrentlyPresentEntity(pawn.getId());
                pawn.setCurrentlyInSettlement(true);
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

            pawn.setCurrentlyInSettlement(false);
            Settlement settlement = entityRepository.getEntity(pawn.getHomeSettlementId()) as Settlement;
            settlement.removeCurrentlyPresentEntity(pawn.getId());
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

            // if no home settlement
            EntityId homeSettlementId = pawn.getHomeSettlementId();
            if (homeSettlementId == null) {
                Debug.LogError("Pawn " + pawn + " has no home settlement id.");
                return;
            }

            // remove wood
            pawn.getInventory().removeItem(ItemType.WOOD, Stall.WOOD_COST_TO_BUILD);
            
            // build stall
            Settlement homeSettlement = (Settlement) entityRepository.getEntity(homeSettlementId);
            Market market = homeSettlement.getMarket();

            market.createStall();
            pawn.setCurrentBehaviorType(BehaviorType.NONE);       
        }

        private void executePurchaseStallBehavior(Pawn pawn) {
            // pawn is assumed to be in home settlement
            EntityId homeSettlementId = pawn.getHomeSettlementId();
            if (homeSettlementId == null) {
                Debug.LogError("Pawn " + pawn + " has no home settlement id.");
                return;
            }
            Settlement homeSettlement = (Settlement) entityRepository.getEntity(homeSettlementId);
            Market market = homeSettlement.getMarket();

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
            // pawn is assumed to be in home settlement
            EntityId homeSettlementId = pawn.getHomeSettlementId();
            if (homeSettlementId == null) {
                Debug.LogError("Pawn " + pawn + " has no home settlement id.");
                return;
            }
            Settlement homeSettlement = (Settlement) entityRepository.getEntity(homeSettlementId);
            Market market = homeSettlement.getMarket();

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
    }
}