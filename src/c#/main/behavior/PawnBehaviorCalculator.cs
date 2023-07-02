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

        public PawnBehaviorCalculator(Environment environment, EntityRepository entityRepository, NationRepository nationRepository) {
            this.environment = environment;
            this.entityRepository = entityRepository;
            this.nationRepository = nationRepository;
        }
        
        public BehaviorType computeBehaviorType(Pawn pawn) {
            if (pawn.isMarkedForDeletion()) {
                UnityEngine.Debug.Log("[PBC] Pawn '" + pawn.getName() + " is marked for deletion. Returning NONE.");
                return BehaviorType.NONE;
            }

            bool foodNeeded = pawnNeedsFood(pawn);
            if (pawn.isCurrentlyInSettlement()) {
                if (foodNeeded) {
                    UnityEngine.Debug.Log("[PBC] Pawn '" + pawn.getName() + "' needs food and is in settlement. Returning EXIT_SETTLEMENT.");
                    return BehaviorType.EXIT_SETTLEMENT;
                }
                else {
                    UnityEngine.Debug.Log("[PBC] Pawn '" + pawn.getName() + "' is in settlement and does not need food. Returning NONE.");
                    return BehaviorType.NONE;
                }
            }

            if (foodNeeded) {
                UnityEngine.Debug.Log("[PBC] Pawn '" + pawn.getName() + "' needs food. Returning GATHER_RESOURCES.");
                return BehaviorType.GATHER_RESOURCES;
            }

            if (shouldPlantSapling(pawn)) {
                UnityEngine.Debug.Log("[PBC] Pawn '" + pawn.getName() + "' should plant sapling. Returning PLANT_SAPLING.");
                return BehaviorType.PLANT_SAPLING;
            }

            if (pawn.getNationId() == null) {
                UnityEngine.Debug.Log("[PBC] Pawn '" + pawn.getName() + "' has no nation. Returning WANDER.");
                return BehaviorType.WANDER;
            }

            Nation nation = nationRepository.getNation(pawn.getNationId());
            NationRole role = nation.getRole(pawn.getId());

            int numNationSettlements = nation.getNumberOfSettlements();

            if (numNationSettlements == 0) {
                if (role == NationRole.LEADER) {

                    if (pawn.getInventory().getNumItems(ItemType.WOOD) <= Settlement.WOOD_COST_TO_BUILD) {
                        UnityEngine.Debug.Log("[PBC] Pawn '" + pawn.getName() + "' is nation leader and not enough wood to build settlement. Returning GATHER_RESOURCES.");
                        return BehaviorType.GATHER_RESOURCES;
                    }

                    // if no settlements within x units, create settlement
                    Entity nearestSettlement = environment.getNearestEntityOfType(pawn.getGameObject().transform.position, EntityType.SETTLEMENT);
                    int distanceToNearestSettlement = nearestSettlement == null ? int.MaxValue : (int)Vector3.Distance(nearestSettlement.getGameObject().transform.position, pawn.getGameObject().transform.position);
                    if (nearestSettlement == null || distanceToNearestSettlement > 200) {
                        UnityEngine.Debug.Log("[PBC] Pawn '" + pawn.getName() + "' is nation leader and no settlements within 200 units. Returning CONSTRUCT_SETTLEMENT.");
                        return BehaviorType.CONSTRUCT_SETTLEMENT;
                    }
                    else {
                        UnityEngine.Debug.Log("[PBC] Pawn '" + pawn.getName() + "' is nation leader and settlement within 200 units. Returning GATHER_RESOURCES.");
                        return BehaviorType.GATHER_RESOURCES;
                    }
                }
            }

            if (pawn.getHomeSettlementId() == null && numNationSettlements > 0) {
                // join random settlement
                EntityId randomSettlementId = nation.getRandomSettlementId();
                if (randomSettlementId == null) {
                    UnityEngine.Debug.LogError("[PBC] Random settlement id is null. Returning NONE.");
                    return BehaviorType.NONE;
                }
                pawn.setHomeSettlementId(randomSettlementId);
            }

            if (pawn.getHomeSettlementId() == null) {
                UnityEngine.Debug.Log("[PBC] Pawn '" + pawn.getName() + "' has no home settlement. Returning WANDER.");
                return BehaviorType.WANDER;
            }
            Settlement homeSettlement = (Settlement)entityRepository.getEntity(pawn.getHomeSettlementId());
            if (homeSettlement == null) {
                UnityEngine.Debug.LogError("[PBC] Home settlement is null. Returning NONE.");
                return BehaviorType.NONE;
            }

            Market homeMarket = homeSettlement.getMarket();
            if (homeMarket.getNumStalls() < homeMarket.getMaxNumStalls() && role == NationRole.LEADER) {
                // if not enough wood
                if (pawn.getInventory().getNumItems(ItemType.WOOD) <= Stall.WOOD_COST_TO_BUILD) {
                    UnityEngine.Debug.Log("[PBC] Pawn '" + pawn.getName() + "' is nation leader and not enough wood to build stall. Returning GATHER_RESOURCES.");
                    return BehaviorType.GATHER_RESOURCES;
                }
                UnityEngine.Debug.Log("[PBC] Pawn '" + pawn.getName() + "' is nation leader and home settlement has room for more stalls. Returning CONSTRUCT_STALL.");
                return BehaviorType.CONSTRUCT_STALL;
            }

            if (pawn.getInventory().containsAbundanceOfResources() && numNationSettlements > 0) {
                UnityEngine.Debug.Log("[PBC] Pawn '" + pawn.getName() + "' has an abundance of resources. Returning GO_HOME.");
                return BehaviorType.GO_TO_HOME_SETTLEMENT;
            }
            else {
                UnityEngine.Debug.Log("[PBC] Pawn '" + pawn.getName() + "' does not have an abundance of resources. Returning GATHER_RESOURCES.");
                return BehaviorType.GATHER_RESOURCES;
            }

            UnityEngine.Debug.LogError("[PBC] Pawn '" + pawn.getName() + "' behavior type could not be determined. Returning NONE.");
            return BehaviorType.NONE;
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