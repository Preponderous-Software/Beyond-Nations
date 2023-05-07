using System.Net;
using UnityEngine;
using System.Collections.Generic;

namespace osg {

    /**
     * A class that handles pawn actions.
     */
    public class PawnBehaviorExecutor {
        private Environment environment;
        private NationRepository nationRepository;
        private EventProducer eventProducer;

        public PawnBehaviorExecutor(Environment environment, NationRepository nationRepository, EventProducer eventProducer) {
            this.environment = environment;
            this.nationRepository = nationRepository;
            this.eventProducer = eventProducer;
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
            if (!pawn.hasTargetEntity()) {
                // target nation leader
                Nation nation = nationRepository.getNation(pawn.getNationId());
                if (nation != null) {
                    EntityId nationLeaderId = nation.getLeaderId();
                    Entity nationLeader = environment.getEntity(nationLeaderId);
                    if (nationLeader != null) {
                        pawn.setTargetEntity(nationLeader);
                    }
                }
            }
            else if (pawn.isAtTargetEntity()) {
                Inventory pawnInventory = pawn.getInventory();

                Entity targetEntity = pawn.getTargetEntity();
                EntityType targetEntityType = targetEntity.getType();
                Inventory targetInventory = null;
                if (targetEntityType == EntityType.PAWN) {
                    targetInventory = ((Pawn)targetEntity).getInventory();
                }
                else if (targetEntityType == EntityType.PLAYER) {
                    targetInventory = ((Player)targetEntity).getInventory();
                }
                else {
                    Debug.LogWarning("Pawn " + pawn + " has target entity " + targetEntity + " but it is not a pawn or player.");
                    pawn.setTargetEntity(null);
                    return;
                }

                int woodPrice = 2;
                int stonePrice = 3;
                int applePrice = 1;

                bool increaseRelationship = false;

                // if pawn has wood, sell 1
                if (pawnInventory.getNumItems(ItemType.WOOD) > 0 && targetInventory.getNumItems(ItemType.GOLD_COIN) >= woodPrice) {
                    pawnInventory.removeItem(ItemType.WOOD, 1);
                    targetInventory.addItem(ItemType.WOOD, 1);
                    pawnInventory.addItem(ItemType.GOLD_COIN, woodPrice);
                    targetInventory.removeItem(ItemType.GOLD_COIN, woodPrice);
                    increaseRelationship = true;
                }

                // if pawn has stone, sell 1
                if (pawnInventory.getNumItems(ItemType.STONE) > 0 && targetInventory.getNumItems(ItemType.GOLD_COIN) >= stonePrice) {
                    pawnInventory.removeItem(ItemType.STONE, 1);
                    targetInventory.addItem(ItemType.STONE, 1);
                    pawnInventory.addItem(ItemType.GOLD_COIN, stonePrice);
                    targetInventory.removeItem(ItemType.GOLD_COIN, stonePrice);
                    increaseRelationship = true;
                }

                // if pawn has apples, sell 1
                if (pawnInventory.getNumItems(ItemType.APPLE) > 0 && targetInventory.getNumItems(ItemType.GOLD_COIN) >= applePrice) {
                    pawnInventory.removeItem(ItemType.APPLE, 1);
                    targetInventory.addItem(ItemType.APPLE, 1);
                    pawnInventory.addItem(ItemType.GOLD_COIN, applePrice);
                    targetInventory.removeItem(ItemType.GOLD_COIN, applePrice);
                    increaseRelationship = true;
                }

                if (increaseRelationship) {
                    int increase = Random.Range(1, 5);
                    if (pawn.getRelationships().ContainsKey(targetEntity.getId())) {
                        pawn.getRelationships()[targetEntity.getId()] += increase;
                    }
                    else {
                        pawn.getRelationships().Add(targetEntity.getId(), increase);
                    }
                    
                    eventProducer.producePawnRelationshipIncreaseEvent(pawn, targetEntity, increase);
                    if (targetEntityType == EntityType.PLAYER) {
                        Player player = (Player)targetEntity;
                        player.getStatus().update(pawn.getName() + " sold resources to you. Relationship: " + pawn.getRelationships()[player.getId()]);
                    }
                }

            }
            else {
                // move towards target entity
                pawn.moveTowardsTargetEntity();
            }
        }

        private void executeWanderBehavior(Pawn pawn) {
            // 95% chance to skip
            if (Random.Range(0, 100) < 95) {
                return;
            }
            Vector3 currentPosition = pawn.getPosition();
            Vector3 targetPosition = currentPosition + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
            pawn.getGameObject().GetComponent<Rigidbody>().velocity = (targetPosition - currentPosition).normalized * pawn.getSpeed();
        }

        private void executePurchaseFoodBehavior(Pawn pawn) {
            // purchase food from nation leader

            if (!pawn.hasTargetEntity()) {
                // target nation leader
                Nation nation = nationRepository.getNation(pawn.getNationId());
                if (nation != null) {
                    EntityId nationLeaderId = nation.getLeaderId();
                    Entity nationLeader = environment.getEntity(nationLeaderId);
                    if (nationLeader != null) {
                        pawn.setTargetEntity(nationLeader);
                    }
                }
            }
            else if (pawn.isAtTargetEntity()) {
                Entity targetEntity = pawn.getTargetEntity();
                EntityType targetEntityType = targetEntity.getType();
                Inventory targetInventory = null;
                if (targetEntityType == EntityType.PAWN) {
                    targetInventory = ((Pawn)targetEntity).getInventory();
                }
                else if (targetEntityType == EntityType.PLAYER) {
                    targetInventory = ((Player)targetEntity).getInventory();
                }
                else {
                    Debug.LogWarning("Pawn " + pawn + " has target entity " + targetEntity + " but it is not a pawn or player.");
                    pawn.setTargetEntity(null);
                    return;
                }

                int applePrice = 5;
                int cost = applePrice;
                Inventory pawnInventory = pawn.getInventory();
                if (pawnInventory.getNumItems(ItemType.GOLD_COIN) >= cost) {
                    pawnInventory.removeItem(ItemType.GOLD_COIN, cost);
                    targetInventory.removeItem(ItemType.APPLE, 1);
                    pawnInventory.addItem(ItemType.APPLE, 1);
                    
                    // increase relationship
                    int increase = Random.Range(1, 5);
                    if (pawn.getRelationships().ContainsKey(targetEntity.getId())) {
                        pawn.getRelationships()[targetEntity.getId()] += increase;
                    }
                    else {
                        pawn.getRelationships().Add(targetEntity.getId(), increase);
                    }
                    
                    eventProducer.producePawnRelationshipIncreaseEvent(pawn, targetEntity, increase);
                    if (targetEntityType == EntityType.PLAYER) {
                        Player player = (Player)targetEntity;
                        player.getStatus().update(pawn.getName() + " bought an apple from you. Relationship: " + pawn.getRelationships()[player.getId()]);
                    }
                }
                else {
                    Debug.LogWarning(pawn.getName() + " tried to purchase food, but the target entity did not have enough gold coins. Cost: " + cost + ", Target entity gold coins: " + targetInventory.getNumItems(ItemType.GOLD_COIN));
                    pawn.setTargetEntity(null);
                }
            }
            else {
                // move towards target entity
                pawn.moveTowardsTargetEntity();
            }
        }
    }
}