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
                return BehaviorType.EXIT_SETTLEMENT;
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
                    if (pawn.getInventory().getNumItems(ItemType.WOOD) <= Stall.WOOD_COST_TO_BUILD) {
                        return BehaviorType.EXIT_SETTLEMENT;
                    }
                    return BehaviorType.CONSTRUCT_STALL;
                }
            }

            if (homeMarket.getNumStalls() < homeMarket.getMaxNumStalls() && role == NationRole.LEADER) {

            }

            return BehaviorType.NONE;
        }

        private BehaviorType computeBehaviorTypeOutsideSettlement(Pawn pawn) {
            if (pawnNeedsFood(pawn)) {
                return BehaviorType.GATHER_RESOURCES;
            }

            if (shouldPlantSapling(pawn)) {
                return BehaviorType.PLANT_SAPLING;
            }

            if (pawn.getNationId() == null) {
                return BehaviorType.WANDER;
            }

            Nation nation = nationRepository.getNation(pawn.getNationId());
            NationRole role = nation.getRole(pawn.getId());

            int numNationSettlements = nation.getNumberOfSettlements();

            if (numNationSettlements == 0) {
                if (role == NationRole.LEADER) {

                    if (pawn.getInventory().getNumItems(ItemType.WOOD) <= Settlement.WOOD_COST_TO_BUILD) {
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
                // join random settlement
                EntityId randomSettlementId = nation.getRandomSettlementId();
                if (randomSettlementId == null) {
                    return BehaviorType.NONE;
                }
                pawn.setHomeSettlementId(randomSettlementId);
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

            // 80% chance to skip planting sapling
            if (UnityEngine.Random.Range(0, 100) < 80) {
                return false;
            }

            // if no tree within x units, plant sapling
            int threshold = 25;
            AppleTree nearestTree = environment.getNearestTree(pawn.getGameObject().transform.position);
            Sapling nearestSapling = (Sapling)environment.getNearestEntityOfType(pawn.getGameObject().transform.position, EntityType.SAPLING);
            int distanceToNearestTree = nearestTree == null ? int.MaxValue : (int)Vector3.Distance(nearestTree.getGameObject().transform.position, pawn.getGameObject().transform.position);
            int distanceToNearestSapling = nearestSapling == null ? int.MaxValue : (int)Vector3.Distance(nearestSapling.getGameObject().transform.position, pawn.getGameObject().transform.position);

            return (nearestTree == null || distanceToNearestTree > threshold) && (nearestSapling == null || distanceToNearestSapling > threshold);
        }

        private bool nationLeaderHasApples(Pawn pawn) {
            NationId nationId = pawn.getNationId();
            if (nationId == null) {
                UnityEngine.Debug.LogWarning("nationId is null");
                return false;
            }

            Nation nation = nationRepository.getNation(nationId);
            if (nation == null) {
                UnityEngine.Debug.LogWarning("nation is null");
                return false;
            }

            if (nation.getLeaderId() == null) {
                UnityEngine.Debug.LogWarning("nation leader id is null");
                return false;
            }

            Entity nationLeader = entityRepository.getEntity(nation.getLeaderId());
            if (nationLeader == null) {
                UnityEngine.Debug.LogWarning("nation leader is null");
                return false;
            }

            return nationLeader.getInventory().getNumItems(ItemType.APPLE) > 0;
        }

        private bool pawnIsNationLeader(Pawn pawn) {
            return nationRepository.getNation(pawn.getNationId()).getLeaderId() == pawn.getId();
        }

        private bool pawnNeedsFood(Pawn pawn) {
            return pawn.getEnergy() < 80 && pawn.getInventory().getNumItems(ItemType.APPLE) == 0;
        }
    }
}