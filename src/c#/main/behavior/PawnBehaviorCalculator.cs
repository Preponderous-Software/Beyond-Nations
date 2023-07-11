using System.Diagnostics;
using UnityEngine;

namespace osg {

    /**
     * A class that computes the behavior type of a pawn.
     */
    public class PawnBehaviorCalculator {
        private Environment environment;
        private EntityRepository entityRepository;
        private NationRepository nationRepository;
        private GameConfig gameConfig = new GameConfig();

        public PawnBehaviorCalculator(Environment environment, EntityRepository entityRepository, NationRepository nationRepository, GameConfig gameConfig) {
            this.environment = environment;
            this.entityRepository = entityRepository;
            this.nationRepository = nationRepository;
            this.gameConfig = gameConfig;
        }
        
        public BehaviorType computeBehaviorType(Pawn pawn) {
            if (pawn.isMarkedForDeletion()) {
                UnityEngine.Debug.Log("[PBC] Pawn '" + pawn.getName() + " is marked for deletion. Returning NONE.");
                return BehaviorType.NONE;
            }

            if (pawn.isCurrentlyInSettlement()) {
                return computeBehaviorTypeInSettlement(pawn);
            }
            else {
                return computeBehaviorTypeOutsideSettlement(pawn);
            }
        }

        private BehaviorType computeBehaviorTypeInSettlement(Pawn pawn) {
            if (pawnNeedsFood(pawn)) {
                int expectedFoodCost = 1;
                if (pawn.getInventory().getNumItems(ItemType.COIN) >= expectedFoodCost) {
                    UnityEngine.Debug.Log("[PBC DEBUG] Pawn '" + pawn.getName() + "' has enough coins to purchase food. Returning PURCHASE_FOOD.");
                    return BehaviorType.PURCHASE_FOOD;
                }
                else {
                    return BehaviorType.EXIT_SETTLEMENT;
                }
            }

            Nation nation = nationRepository.getNation(pawn.getNationId());
            NationRole role = nation.getRole(pawn.getId());

            Settlement homeSettlement = (Settlement)entityRepository.getEntity(pawn.getHomeSettlementId());
            if (homeSettlement == null) {
                return BehaviorType.NONE;
            }

            Market homeMarket = homeSettlement.getMarket();

            if (role == NationRole.LEADER) {
                if (homeMarket.getNumStalls() < homeMarket.getMaxNumStalls()) {
                    // if not enough wood
                    if (pawn.getInventory().getNumItems(ItemType.WOOD) < Stall.WOOD_COST_TO_BUILD) {
                        return BehaviorType.EXIT_SETTLEMENT;
                    }
                    else {
                        return BehaviorType.CONSTRUCT_STALL;
                    }
                }
                else {
                    // 10% chance to exit settlement
                    if (Random.Range(0, 100) < 10) {
                        return BehaviorType.EXIT_SETTLEMENT;
                    }
                    else {
                        return BehaviorType.NONE;
                    }
                }
            }
            else if (role == NationRole.SERF) {
                // if enough coins and stall for sale
                if (pawn.getInventory().getNumItems(ItemType.COIN) >= Stall.COIN_COST_TO_PURCHASE && homeMarket.getNumStallsForSale() > 0) {
                    return BehaviorType.PURCHASE_STALL;
                }
                else {

                    // if pawn has an abundance of resources, sell them
                    if (pawn.getInventory().containsAbundanceOfResources()) {
                        return BehaviorType.SELL_RESOURCES;
                    }

                    // 10% chance to exit settlement
                    if (Random.Range(0, 100) < 10) {
                        return BehaviorType.EXIT_SETTLEMENT;
                    }
                    else {
                        return BehaviorType.NONE;
                    }
                }
            }
            else if (role == NationRole.MERCHANT) {
                // transfer items to stall if pawn has wood or stone
                if (pawn.getInventory().getNumItems(ItemType.WOOD) > 0 && pawn.getInventory().getNumItems(ItemType.STONE) > 0) {
                    return BehaviorType.TRANSFER_ITEMS_TO_STALL;
                }

                Market market = homeSettlement.getMarket();
                Stall stall = market.getStall(pawn.getId());
                if (stall.getInventory().hasItem(ItemType.COIN)) {
                    return BehaviorType.COLLECT_PROFIT_FROM_STALL;
                }
                
                if (Random.Range(0, 100) < 10) {
                    return BehaviorType.EXIT_SETTLEMENT;
                }
                else {
                    return BehaviorType.NONE;
                }
            }
            else {
                return BehaviorType.NONE;
            }            
        }

        private BehaviorType computeBehaviorTypeOutsideSettlement(Pawn pawn) {
            if (pawnNeedsFood(pawn)) {
                int expectedFoodCost = 1;
                if (pawn.getInventory().getNumItems(ItemType.COIN) >= expectedFoodCost) {
                    return BehaviorType.GO_TO_HOME_SETTLEMENT;
                }
            }

            if (shouldPlantSapling(pawn)) {
                return BehaviorType.PLANT_SAPLING;
            }

            if (pawn.getNationId() == null) {
                Settlement nearestSettlement = (Settlement) environment.getNearestEntityOfType(pawn.getGameObject().transform.position, EntityType.SETTLEMENT);
                if (nearestSettlement != null && Vector3.Distance(pawn.getGameObject().transform.position, nearestSettlement.getGameObject().transform.position) < gameConfig.getSettlementJoinRange()) {
                    return BehaviorType.JOIN_NATION;
                }
                else {
                    // if enough wood to build settlement, create nation
                    if (pawn.getInventory().getNumItems(ItemType.WOOD) >= Settlement.WOOD_COST_TO_BUILD) {
                        return BehaviorType.CREATE_NATION;
                    }
                    else {
                        return BehaviorType.GATHER_RESOURCES;
                    }
                }
            }

            Nation nation = nationRepository.getNation(pawn.getNationId());
            NationRole role = nation.getRole(pawn.getId());

            int numNationSettlements = nation.getNumberOfSettlements();

            if (numNationSettlements == 0) {
                if (role == NationRole.LEADER) {

                    if (pawn.getInventory().getNumItems(ItemType.WOOD) < Settlement.WOOD_COST_TO_BUILD) {
                        return BehaviorType.GATHER_RESOURCES;
                    }

                    // if no settlements within x units, create settlement
                    Entity nearestSettlement = environment.getNearestEntityOfType(pawn.getGameObject().transform.position, EntityType.SETTLEMENT);
                    int distanceToNearestSettlement = nearestSettlement == null ? int.MaxValue : (int)Vector3.Distance(nearestSettlement.getGameObject().transform.position, pawn.getGameObject().transform.position);
                    if (nearestSettlement == null || distanceToNearestSettlement > gameConfig.getMinDistanceBetweenSettlements()) {
                        return BehaviorType.CONSTRUCT_SETTLEMENT;
                    }
                    else {
                        return BehaviorType.GATHER_RESOURCES;
                    }
                }
            }

            if (pawn.getHomeSettlementId() == null && numNationSettlements > 0) {
                return BehaviorType.JOIN_RANDOM_SETTLEMENT;
            }

            if (role == NationRole.LEADER) {
                // if not enough wood for stall, gather resources
                if (pawn.getInventory().getNumItems(ItemType.WOOD) <= Stall.WOOD_COST_TO_BUILD) {
                    return BehaviorType.GATHER_RESOURCES;
                }
            }
            
            if (pawn.getInventory().containsAbundanceOfResources() && numNationSettlements > 0) {
                return BehaviorType.GO_TO_HOME_SETTLEMENT;
            }
            else {
                return BehaviorType.GATHER_RESOURCES;
            }
        }


        // helper methods
        private bool shouldPlantSapling(Pawn pawn) {

            if (pawn.getInventory().getNumItems(ItemType.SAPLING) == 0) {
                return false;
            }

            // if no tree or sapling within x units, plant sapling
            int threshold = 25;
            AppleTree nearestTree = environment.getNearestTree(pawn.getGameObject().transform.position);
            Sapling nearestSapling = (Sapling)environment.getNearestEntityOfType(pawn.getGameObject().transform.position, EntityType.SAPLING);
            int distanceToNearestTree = nearestTree == null ? int.MaxValue : (int)Vector3.Distance(nearestTree.getGameObject().transform.position, pawn.getGameObject().transform.position);
            int distanceToNearestSapling = nearestSapling == null ? int.MaxValue : (int)Vector3.Distance(nearestSapling.getGameObject().transform.position, pawn.getGameObject().transform.position);

            return (nearestTree == null || distanceToNearestTree > threshold) && (nearestSapling == null || distanceToNearestSapling > threshold);
        }

        private bool pawnNeedsFood(Pawn pawn) {
            return pawn.getEnergy() < 80 && pawn.getInventory().getNumItems(ItemType.APPLE) == 0;
        }
    }
}