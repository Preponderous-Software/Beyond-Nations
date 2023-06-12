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

            // 10% chance to consider planting a sapling
            if (Random.Range(0, 100) < 5) {
                if (pawn.getInventory().getNumItems(ItemType.SAPLING) > 0) {
                    // if no tree within x units, plant sapling
                    TreeEntity nearestTree = environment.getNearestTree(pawn.getGameObject().transform.position);
                    Sapling nearestSapling = (Sapling)environment.getNearestEntityOfType(pawn.getGameObject().transform.position, EntityType.SAPLING);
                    int distanceToNearestTree = nearestTree == null ? int.MaxValue : (int)Vector3.Distance(nearestTree.getGameObject().transform.position, pawn.getGameObject().transform.position);
                    int distanceToNearestSapling = nearestSapling == null ? int.MaxValue : (int)Vector3.Distance(nearestSapling.getGameObject().transform.position, pawn.getGameObject().transform.position);
                    if (nearestTree == null || distanceToNearestTree > 50 && distanceToNearestSapling > 20) {
                        return BehaviorType.PLANT_SAPLING;
                    }
                }
            }      

            if (pawn.getEnergy() < 80 && pawn.getInventory().getNumItems(ItemType.APPLE) == 0) {
                // if nation leader has apples, purchase apples from leader
                if (pawn.getNationId() != null) {
                    Nation nation1 = nationRepository.getNation(pawn.getNationId());
                    Entity nationLeader = entityRepository.getEntity(nation1.getLeaderId());
                    if (nationLeader.getId() == pawn.getId()) {
                        return BehaviorType.GATHER_RESOURCES;
                    }
                    if (nationLeader.getInventory().getNumItems(ItemType.APPLE) > 0 && pawn.getInventory().getNumItems(ItemType.GOLD_COIN) >= 5) {
                        return BehaviorType.PURCHASE_FOOD;
                    }
                }

                return BehaviorType.GATHER_RESOURCES;
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
    }
}