using UnityEngine;

namespace osg {

    /**
     * A class that handles pawn actions.
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
                return BehaviorType.NONE;
            }

            if (pawnNeedsFood(pawn)) {
                // if nation leader has apples, purchase apples from leader
                if (nationLeaderHasApples(pawn) && !pawnIsNationLeader(pawn)) {
                    return BehaviorType.PURCHASE_FOOD;
                }
                else {
                    return BehaviorType.GATHER_RESOURCES;
                }
            }

            if (shouldPlantSapling(pawn)) {
                return BehaviorType.PLANT_SAPLING;
            }

            if (pawn.getNationId() == null) {
                return BehaviorType.WANDER;
            }

            Nation nation = nationRepository.getNation(pawn.getNationId());
            NationRole role = nation.getRole(pawn.getId());
            if (role == NationRole.LEADER) {
                if (nation.getNumberOfSettlements() == 0) {
                    // if no settlements within x units, create settlement
                    Entity nearestSettlement = environment.getNearestEntityOfType(pawn.getGameObject().transform.position, EntityType.SETTLEMENT);
                    int distanceToNearestSettlement = nearestSettlement == null ? int.MaxValue : (int)Vector3.Distance(nearestSettlement.getGameObject().transform.position, pawn.getGameObject().transform.position);
                    if (nearestSettlement == null || distanceToNearestSettlement > 200) {
                        return BehaviorType.CREATE_SETTLEMENT;
                    }
                    else {
                        return BehaviorType.GATHER_RESOURCES;
                    }
                    
                }
                else {
                    return BehaviorType.GO_HOME;
                }
                
            }
            else if (role == NationRole.CITIZEN) {
                // if pawn has at least 1 of each resource, consider selling resources
                if (pawn.getInventory().getNumItems(ItemType.WOOD) >= 1 && pawn.getInventory().getNumItems(ItemType.STONE) >= 1 && pawn.getInventory().getNumItems(ItemType.APPLE) >= 1) {
                    if (Random.Range(0, 100) < 2) {
                        Entity nationLeader = entityRepository.getEntity(nation.getLeaderId());

                        int numWood = pawn.getInventory().getNumItems(ItemType.WOOD);
                        int numStone = pawn.getInventory().getNumItems(ItemType.STONE);
                        int numApples = pawn.getInventory().getNumItems(ItemType.APPLE);
                        if (nationLeader.getInventory().getNumItems(ItemType.GOLD_COIN) < numWood * 2 + numStone * 3 + numApples * 1) {
                            // leader doesn't have enough money to buy resources
                            return BehaviorType.GO_HOME;
                        }
                        return BehaviorType.SELL_RESOURCES;
                    }
                    else {
                        return BehaviorType.GO_HOME;
                    }
                }
                else {
                    // if pawn doesn't have an abundance of resources, gather resources
                    if (pawn.getInventory().getNumItems(ItemType.WOOD) < 10 && pawn.getInventory().getNumItems(ItemType.STONE) < 10 && pawn.getInventory().getNumItems(ItemType.APPLE) < 10) {
                        return BehaviorType.GATHER_RESOURCES;
                    }
                    else {
                        return BehaviorType.GO_HOME;
                    }
                }
            }

            return BehaviorType.NONE;
        }


        // helper methods
        private bool shouldPlantSapling(Pawn pawn) {

            if (pawn.getInventory().getNumItems(ItemType.SAPLING) == 0) {
                return false;
            }

            // 95% chance to skip planting sapling
            if (Random.Range(0, 100) < 95) {
                return false;
            }

            // if no tree within x units, plant sapling
            TreeEntity nearestTree = environment.getNearestTree(pawn.getGameObject().transform.position);
            Sapling nearestSapling = (Sapling)environment.getNearestEntityOfType(pawn.getGameObject().transform.position, EntityType.SAPLING);
            int distanceToNearestTree = nearestTree == null ? int.MaxValue : (int)Vector3.Distance(nearestTree.getGameObject().transform.position, pawn.getGameObject().transform.position);
            int distanceToNearestSapling = nearestSapling == null ? int.MaxValue : (int)Vector3.Distance(nearestSapling.getGameObject().transform.position, pawn.getGameObject().transform.position);

            return (nearestTree == null || distanceToNearestTree > 50) && (nearestSapling == null || distanceToNearestSapling > 20);
        }

        private bool nationLeaderHasApples(Pawn pawn) {
            NationId nationId = pawn.getNationId();
            if (nationId == null) {
                Debug.LogWarning("nationId is null");
                return false;
            }

            Nation nation = nationRepository.getNation(nationId);
            if (nation == null) {
                Debug.LogWarning("nation is null");
                return false;
            }

            if (nation.getLeaderId() == null) {
                Debug.LogWarning("nation leader id is null");
                return false;
            }

            Pawn nationLeader = (Pawn)entityRepository.getEntity(nation.getLeaderId());
            if (nationLeader == null) {
                Debug.LogWarning("nation leader is null");
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