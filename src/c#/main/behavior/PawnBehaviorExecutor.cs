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
            // TODO: go to settlement market and sell resources
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
            // TODO: go to settlement market and purchase food
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

        // ---

        private void buyItem(Pawn buyer, Entity seller, ItemType itemType, int numItems) {
            Inventory buyerInventory = buyer.getInventory();
            Inventory sellerInventory = seller.getInventory();

            // check if seller has item
            if (sellerInventory.getNumItems(itemType) < numItems) {
                Debug.LogWarning("Seller " + seller + " does not have " + numItems + " of item type " + itemType + ".");
                return;
            }
            
            // decide price
            int price = 0;
            switch (itemType) {
                case ItemType.WOOD:
                    price = 1;
                    break;
                case ItemType.STONE:
                    price = 2;
                    break;
                case ItemType.APPLE:
                    price = 5;
                    break;
                default:
                    Debug.LogWarning("Seller " + seller + " tried to sell item type " + itemType + " but it is not a valid item type.");
                    return;
            }

            // check if buyer has enough gold coins
            if (buyerInventory.getNumItems(ItemType.GOLD_COIN) < price) {
                Debug.LogWarning("Buyer " + buyer + " does not have enough gold coins to buy item type " + itemType + ". Price: " + price + ", Buyer gold coins: " + buyerInventory.getNumItems(ItemType.GOLD_COIN));
                return;
            }

            // transfer items
            sellerInventory.removeItem(itemType, numItems);
            buyerInventory.addItem(itemType, numItems);
            buyerInventory.removeItem(ItemType.GOLD_COIN, price);
            sellerInventory.addItem(ItemType.GOLD_COIN, price);

            // increase relationship
            int increase = UnityEngine.Random.Range(1, 5);
            buyer.increaseRelationship(seller, increase);
            eventProducer.producePawnRelationshipIncreaseEvent(buyer, seller, increase);

            // update status
            if (seller.getType() == EntityType.PLAYER) {
                Player player = (Player)seller;
                player.getStatus().update(buyer.getName() + " bought " + numItems + " " + itemType + " from you. Relationship: " + buyer.getRelationships()[player.getId()]);
            }
        }

        private void sellItem(Pawn seller, Entity buyer, ItemType itemType, int numItems) {
            Inventory sellerInventory = seller.getInventory();
            Inventory buyerInventory = buyer.getInventory();

            // check if seller has item
            if (sellerInventory.getNumItems(itemType) < numItems) {
                Debug.LogWarning("Seller " + seller + " does not have " + numItems + " of item type " + itemType + ".");
                return;
            }
            
            // decide price
            int price = 0;
            switch (itemType) {
                case ItemType.WOOD:
                    price = 1;
                    break;
                case ItemType.STONE:
                    price = 2;
                    break;
                case ItemType.APPLE:
                    price = 5;
                    break;
                default:
                    Debug.LogWarning("Seller " + seller + " tried to sell item type " + itemType + " but it is not a valid item type.");
                    return;
            }

            // check if buyer has enough gold coins
            if (buyerInventory.getNumItems(ItemType.GOLD_COIN) < price * numItems) {
                Debug.LogWarning("Buyer " + buyer + " does not have enough gold coins to purchase " + numItems + " of item type " + itemType + ".");
                return;
            }

            // transfer items
            sellerInventory.removeItem(itemType, numItems);
            buyerInventory.addItem(itemType, numItems);
            buyerInventory.removeItem(ItemType.GOLD_COIN, price * numItems);
            sellerInventory.addItem(ItemType.GOLD_COIN, price * numItems);

            // increase relationship
            int increase = UnityEngine.Random.Range(1, 5);
            seller.increaseRelationship(buyer, increase);
            eventProducer.producePawnRelationshipIncreaseEvent(seller, buyer, increase);

            if (buyer.getType() == EntityType.PLAYER) {
                Player player = (Player)buyer;
                player.getStatus().update(seller.getName() + " sold " + numItems + " " + itemType + " to you. Relationship: " + seller.getRelationships()[player.getId()]);
            }
        }
    }
}