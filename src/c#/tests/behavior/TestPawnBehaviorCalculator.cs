using UnityEngine;

using osg;

namespace osgtests {

    public static class TestPawnBehaviorCalculator{

        public static void runTests() {
            testComputeBehaviorType_Nationless_ShouldGatherResources();
            testComputeBehaviorType_Nationless_ShouldJoinNation();
            testComputeBehaviorType_Nationless_ShouldCreateSettlement();
            testComputeBehaviorType_InNation_ShouldJoinRandomSettlement();
            testComputeBehaviorType_IsLeaderAndHasEnoughWood_ShouldConstructSettlement();
            testComputeBehaviorType_IsLeaderAndHasEnoughWood_ShouldConstructStall();
            testComputeBehaviorType_IsLeaderAndHasEnoughWoodButMarketFull_ShouldExitOrDoNothing();
            testComputeBehaviorType_InSettlement_ShouldPurchaseFood();
            testComputeBehaviorType_InSettlement_ShouldSellResources();
            testComputeBehaviorType_InSettlement_ShouldCollectProfitFromStall();
            testComputeBehaviorType_InSettlementNoFoodAvailable_ShouldNotPurchaseFood();
            testComputeBehaviorType_InSettlementIsMerchantStallHasFood_ShouldCollectFoodFromStall();
        }

        /**
            * Input: pawn is nationless, no settlements nearby and does not have enough wood to create a settlement
            * Expected output: GATHER_RESOURCES
        */
        public static void testComputeBehaviorType_Nationless_ShouldGatherResources() {
            // prepare
            EntityRepository entityRepository = new EntityRepository();
            Environment environment = new Environment(5, 5, entityRepository);
            NationRepository nationRepository = new NationRepository();
            Pawn pawn = new Pawn(new Vector3(0, 0, 0), "test");
            entityRepository.addEntity(pawn);
            GameConfig gameConfig = new GameConfig();
            TickCounter tickCounter = new TickCounter();

            PawnBehaviorCalculator calculator = new PawnBehaviorCalculator(environment, entityRepository, nationRepository, gameConfig, tickCounter);

            // run
            BehaviorType behaviorType = calculator.computeBehaviorType(pawn);

            // check
            UnityEngine.Debug.Assert(behaviorType == BehaviorType.GATHER_RESOURCES);

            // cleanup
            environment.destroyGameObject();
            pawn.destroyGameObject();
        }

        /**
            * Input: pawn is nationless, settlement within range
            * Expected output: JOIN_NATION
        */
        public static void testComputeBehaviorType_Nationless_ShouldJoinNation() {
            // prepare world
            EntityRepository entityRepository = new EntityRepository();
            Environment environment = new Environment(5, 5, entityRepository);
            NationRepository nationRepository = new NationRepository();

            // prepare existing nation & settlement
            Pawn nationLeader = new Pawn(new Vector3(0, 0, 0), "test");
            entityRepository.addEntity(nationLeader);
            Nation nation = new Nation("test", nationLeader.getId());
            nationRepository.addNation(nation);
            Settlement settlement = new Settlement(new Vector3(0, 0, 0), nation.getId(), nation.getColor(), nation.getName());
            entityRepository.addEntity(settlement);

            // prepare pawn
            Pawn pawn = new Pawn(new Vector3(0, 0, 0), "test");
            entityRepository.addEntity(pawn);

            // prepare calculator
            GameConfig gameConfig = new GameConfig();
            TickCounter tickCounter = new TickCounter();
            PawnBehaviorCalculator calculator = new PawnBehaviorCalculator(environment, entityRepository, nationRepository, gameConfig, tickCounter);

            // execute
            BehaviorType behaviorType = calculator.computeBehaviorType(pawn);

            // verify
            UnityEngine.Debug.Assert(behaviorType == BehaviorType.JOIN_NATION);

            // cleanup
            environment.destroyGameObject();
            pawn.destroyGameObject();
            settlement.destroyGameObject();
            nationLeader.destroyGameObject();
        }

        /**
            * Input: pawn is nationless, settlement not within range, enough wood to create settlement
            * Expected output: CREATE_NATION
        */
        public static void testComputeBehaviorType_Nationless_ShouldCreateSettlement() {
            // prepare world
            EntityRepository entityRepository = new EntityRepository();
            Environment environment = new Environment(5, 5, entityRepository);
            NationRepository nationRepository = new NationRepository();

            // prepare pawn
            Pawn pawn = new Pawn(new Vector3(0, 0, 0), "test");
            pawn.getInventory().addItem(ItemType.WOOD, Settlement.WOOD_COST_TO_BUILD);
            entityRepository.addEntity(pawn);

            // prepare calculator
            GameConfig gameConfig = new GameConfig();
            TickCounter tickCounter = new TickCounter();
            PawnBehaviorCalculator calculator = new PawnBehaviorCalculator(environment, entityRepository, nationRepository, gameConfig, tickCounter);

            // execute
            BehaviorType behaviorType = calculator.computeBehaviorType(pawn);

            // verify
            UnityEngine.Debug.Assert(behaviorType == BehaviorType.CREATE_NATION);

            // cleanup
            environment.destroyGameObject();
            pawn.destroyGameObject();
        }

        /**
            * Input: pawn is in nation but not in settlement
            * Expected output: JOIN_RANDOM_SETTLEMENT
        */
        private static void testComputeBehaviorType_InNation_ShouldJoinRandomSettlement() {
            // prepare world
            EntityRepository entityRepository = new EntityRepository();
            Environment environment = new Environment(5, 5, entityRepository);
            NationRepository nationRepository = new NationRepository();

            // prepare existing nation & settlement
            Pawn nationLeader = new Pawn(new Vector3(0, 0, 0), "test");
            entityRepository.addEntity(nationLeader);
            Nation nation = new Nation("test", nationLeader.getId());
            nationRepository.addNation(nation);
            Settlement settlement = new Settlement(new Vector3(0, 0, 0), nation.getId(), nation.getColor(), nation.getName());
            entityRepository.addEntity(settlement);
            nation.addSettlement(settlement.getId());

            // prepare pawn
            Pawn pawn = new Pawn(new Vector3(0, 0, 0), "test");
            entityRepository.addEntity(pawn);
            nation.addMember(pawn.getId());
            pawn.setNationId(nation.getId());

            // prepare calculator
            GameConfig gameConfig = new GameConfig();
            TickCounter tickCounter = new TickCounter();
            PawnBehaviorCalculator calculator = new PawnBehaviorCalculator(environment, entityRepository, nationRepository, gameConfig, tickCounter);

            // execute
            BehaviorType behaviorType = calculator.computeBehaviorType(pawn);

            // verify
            UnityEngine.Debug.Assert(behaviorType == BehaviorType.JOIN_RANDOM_SETTLEMENT);

            // cleanup
            environment.destroyGameObject();
            pawn.destroyGameObject();
            settlement.destroyGameObject();
            nationLeader.destroyGameObject();
        }

        /**
            * Input: pawn is leader of nation and has enough wood to create a settlement
            * Expected output: CONSTRUCT_SETTLEMENT
        */
        private static void testComputeBehaviorType_IsLeaderAndHasEnoughWood_ShouldConstructSettlement() {
            // prepare world
            EntityRepository entityRepository = new EntityRepository();
            Environment environment = new Environment(5, 5, entityRepository);
            NationRepository nationRepository = new NationRepository();

            // prepare pawn & nation
            Pawn nationLeader = new Pawn(new Vector3(0, 0, 0), "test");
            nationLeader.getInventory().addItem(ItemType.WOOD, Settlement.WOOD_COST_TO_BUILD);
            entityRepository.addEntity(nationLeader);
            Nation nation = new Nation("test", nationLeader.getId());
            nationLeader.setNationId(nation.getId());
            nationRepository.addNation(nation);

            // prepare calculator
            GameConfig gameConfig = new GameConfig();
            TickCounter tickCounter = new TickCounter();
            PawnBehaviorCalculator calculator = new PawnBehaviorCalculator(environment, entityRepository, nationRepository, gameConfig, tickCounter);

            // execute
            BehaviorType behaviorType = calculator.computeBehaviorType(nationLeader);

            // verify
            UnityEngine.Debug.Assert(behaviorType == BehaviorType.CONSTRUCT_SETTLEMENT);

            // cleanup
            environment.destroyGameObject();
            nationLeader.destroyGameObject();
        }

        /**
            * Input: pawn is leader of nation, has enough wood to build a stall & market is not full
            * Expected output: CONSTRUCT_STALL
        */
        private static void testComputeBehaviorType_IsLeaderAndHasEnoughWood_ShouldConstructStall() {
            // prepare world
            EntityRepository entityRepository = new EntityRepository();
            Environment environment = new Environment(5, 5, entityRepository);
            NationRepository nationRepository = new NationRepository();

            // prepare pawn & nation
            Pawn nationLeader = new Pawn(new Vector3(0, 0, 0), "test");
            nationLeader.getInventory().addItem(ItemType.WOOD, Stall.WOOD_COST_TO_BUILD);
            entityRepository.addEntity(nationLeader);
            Nation nation = new Nation("test", nationLeader.getId());
            nationLeader.setNationId(nation.getId());
            nationRepository.addNation(nation);

            // prepare settlement
            Settlement settlement = new Settlement(new Vector3(0, 0, 0), nation.getId(), nation.getColor(), nation.getName());
            entityRepository.addEntity(settlement);
            nation.addSettlement(settlement.getId());
            nationLeader.setHomeSettlementId(settlement.getId());
            nationLeader.setCurrentlyInSettlement(true);

            // prepare calculator
            GameConfig gameConfig = new GameConfig();
            TickCounter tickCounter = new TickCounter();
            PawnBehaviorCalculator calculator = new PawnBehaviorCalculator(environment, entityRepository, nationRepository, gameConfig, tickCounter);

            // execute
            BehaviorType behaviorType = calculator.computeBehaviorType(nationLeader);

            // verify
            UnityEngine.Debug.Assert(behaviorType == BehaviorType.CONSTRUCT_STALL);

            // cleanup
            environment.destroyGameObject();
            nationLeader.destroyGameObject();
            settlement.destroyGameObject();
        }
        
        /**
            * Input: pawn is leader of nation, has enough wood to build a stall & market is full
            * Expected output: EXIT_SETTLEMENT or NONE
        */
        private static void testComputeBehaviorType_IsLeaderAndHasEnoughWoodButMarketFull_ShouldExitOrDoNothing() {
            // prepare world
            EntityRepository entityRepository = new EntityRepository();
            Environment environment = new Environment(5, 5, entityRepository);
            NationRepository nationRepository = new NationRepository();

            // prepare pawn & nation
            Pawn nationLeader = new Pawn(new Vector3(0, 0, 0), "test");
            nationLeader.getInventory().addItem(ItemType.WOOD, Stall.WOOD_COST_TO_BUILD);
            entityRepository.addEntity(nationLeader);
            Nation nation = new Nation("test", nationLeader.getId());
            nationLeader.setNationId(nation.getId());
            nationRepository.addNation(nation);

            // prepare settlement
            Settlement settlement = new Settlement(new Vector3(0, 0, 0), nation.getId(), nation.getColor(), nation.getName());
            entityRepository.addEntity(settlement);
            nation.addSettlement(settlement.getId());
            nationLeader.setHomeSettlementId(settlement.getId());
            nationLeader.setCurrentlyInSettlement(true);

            // prepare market
            for (int i = 0; i < settlement.getMarket().getMaxNumStalls(); i++) {
                settlement.getMarket().createStall();
            }

            // prepare calculator
            GameConfig gameConfig = new GameConfig();
            TickCounter tickCounter = new TickCounter();
            PawnBehaviorCalculator calculator = new PawnBehaviorCalculator(environment, entityRepository, nationRepository, gameConfig, tickCounter);

            // execute
            BehaviorType behaviorType = calculator.computeBehaviorType(nationLeader);

            // verify
            UnityEngine.Debug.Assert(behaviorType == BehaviorType.EXIT_SETTLEMENT || behaviorType == BehaviorType.NONE);

            // cleanup
            environment.destroyGameObject();
            nationLeader.destroyGameObject();
            settlement.destroyGameObject();
        }

        private static void testComputeBehaviorType_InSettlement_ShouldPurchaseFood() {
            // prepare world
            EntityRepository entityRepository = new EntityRepository();
            Environment environment = new Environment(5, 5, entityRepository);
            NationRepository nationRepository = new NationRepository();

            // prepare existing nation & settlement
            Pawn nationLeader = new Pawn(new Vector3(0, 0, 0), "test");
            entityRepository.addEntity(nationLeader);
            Nation nation = new Nation("test", nationLeader.getId());
            nationRepository.addNation(nation);
            Settlement settlement = new Settlement(new Vector3(0, 0, 0), nation.getId(), nation.getColor(), nation.getName());
            entityRepository.addEntity(settlement);

            // prepare existing stall & merchant
            Pawn merchant = new Pawn(new Vector3(0, 0, 0), "test");
            entityRepository.addEntity(merchant);
            settlement.getMarket().createStall();
            Stall stall = settlement.getMarket().getStallForSale();
            stall.setOwnerId(merchant.getId());
            stall.getInventory().addItem(ItemType.APPLE, 10);

            // prepare pawn
            Pawn pawn = new Pawn(new Vector3(0, 0, 0), "test");
            pawn.setEnergy(10);
            pawn.getInventory().addItem(ItemType.COIN, 10);
            entityRepository.addEntity(pawn);
            pawn.setHomeSettlementId(settlement.getId());
            pawn.setCurrentlyInSettlement(true);

            // prepare calculator
            GameConfig gameConfig = new GameConfig();
            TickCounter tickCounter = new TickCounter();
            PawnBehaviorCalculator calculator = new PawnBehaviorCalculator(environment, entityRepository, nationRepository, gameConfig, tickCounter);

            // execute
            BehaviorType behaviorType = calculator.computeBehaviorType(pawn);

            // verify
            UnityEngine.Debug.Assert(behaviorType == BehaviorType.PURCHASE_FOOD);

            // cleanup
            environment.destroyGameObject();
            pawn.destroyGameObject();
            settlement.destroyGameObject();
            nationLeader.destroyGameObject();
            merchant.destroyGameObject();
        }

        public static void testComputeBehaviorType_InSettlement_ShouldSellResources() {
            // prepare world
            EntityRepository entityRepository = new EntityRepository();
            Environment environment = new Environment(5, 5, entityRepository);
            NationRepository nationRepository = new NationRepository();

            // prepare existing nation & settlement
            Pawn nationLeader = new Pawn(new Vector3(0, 0, 0), "test");
            entityRepository.addEntity(nationLeader);
            Nation nation = new Nation("test", nationLeader.getId());
            nationRepository.addNation(nation);
            Settlement settlement = new Settlement(new Vector3(0, 0, 0), nation.getId(), nation.getColor(), nation.getName());
            entityRepository.addEntity(settlement);

            // prepare existing stall & merchant
            Pawn merchant = new Pawn(new Vector3(0, 0, 0), "test");
            entityRepository.addEntity(merchant);
            settlement.getMarket().createStall();
            Stall stall = settlement.getMarket().getStallForSale();
            stall.setOwnerId(merchant.getId());

            // prepare pawn
            Pawn pawn = new Pawn(new Vector3(0, 0, 0), "test");
            Inventory inventory = pawn.getInventory();
            inventory.addItem(ItemType.WOOD, 100);
            inventory.addItem(ItemType.STONE, 100);
            inventory.addItem(ItemType.COIN, 100);
            inventory.addItem(ItemType.APPLE, 100);
            entityRepository.addEntity(pawn);
            nation.addMember(pawn.getId());
            pawn.setNationId(nation.getId());
            pawn.setHomeSettlementId(settlement.getId());
            pawn.setCurrentlyInSettlement(true);

            // prepare calculator
            GameConfig gameConfig = new GameConfig();
            TickCounter tickCounter = new TickCounter();
            PawnBehaviorCalculator calculator = new PawnBehaviorCalculator(environment, entityRepository, nationRepository, gameConfig, tickCounter);

            // execute
            BehaviorType behaviorType = calculator.computeBehaviorType(pawn);

            // verify
            UnityEngine.Debug.Assert(behaviorType == BehaviorType.SELL_RESOURCES);

            // cleanup
            environment.destroyGameObject();
            pawn.destroyGameObject();
            settlement.destroyGameObject();
            nationLeader.destroyGameObject();
            merchant.destroyGameObject();
        }

        private static void testComputeBehaviorType_InSettlement_ShouldCollectProfitFromStall() {
            // prepare world
            EntityRepository entityRepository = new EntityRepository();
            Environment environment = new Environment(5, 5, entityRepository);
            NationRepository nationRepository = new NationRepository();

            // prepare existing nation & settlement
            Pawn nationLeader = new Pawn(new Vector3(0, 0, 0), "test");
            entityRepository.addEntity(nationLeader);
            Nation nation = new Nation("test", nationLeader.getId());
            nationRepository.addNation(nation);
            Settlement settlement = new Settlement(new Vector3(0, 0, 0), nation.getId(), nation.getColor(), nation.getName());
            entityRepository.addEntity(settlement);

            // prepare existing stall & merchant
            Pawn merchant = new Pawn(new Vector3(0, 0, 0), "test");
            nation.addMember(merchant.getId());
            nation.setRole(merchant.getId(), NationRole.MERCHANT);
            merchant.setNationId(nation.getId());
            merchant.setHomeSettlementId(settlement.getId());
            merchant.setCurrentlyInSettlement(true);
            entityRepository.addEntity(merchant);
            settlement.getMarket().createStall();
            Stall stall = settlement.getMarket().getStallForSale();
            stall.setOwnerId(merchant.getId());
            stall.getInventory().addItem(ItemType.COIN, 10);

            // prepare calculator
            GameConfig gameConfig = new GameConfig();
            TickCounter tickCounter = new TickCounter();
            PawnBehaviorCalculator calculator = new PawnBehaviorCalculator(environment, entityRepository, nationRepository, gameConfig, tickCounter);

            // execute
            BehaviorType behaviorType = calculator.computeBehaviorType(merchant);

            // verify
            UnityEngine.Debug.Assert(behaviorType == BehaviorType.COLLECT_PROFIT_FROM_STALL);

            // cleanup
            environment.destroyGameObject();
            merchant.destroyGameObject();
            settlement.destroyGameObject();
            nationLeader.destroyGameObject();
        }

        private static void testComputeBehaviorType_InSettlementNoFoodAvailable_ShouldNotPurchaseFood() {
            // prepare world
            EntityRepository entityRepository = new EntityRepository();
            Environment environment = new Environment(5, 5, entityRepository);
            NationRepository nationRepository = new NationRepository();

            // prepare existing nation & settlement
            Pawn nationLeader = new Pawn(new Vector3(0, 0, 0), "test");
            entityRepository.addEntity(nationLeader);
            Nation nation = new Nation("test", nationLeader.getId());
            nationRepository.addNation(nation);
            Settlement settlement = new Settlement(new Vector3(0, 0, 0), nation.getId(), nation.getColor(), nation.getName());
            entityRepository.addEntity(settlement);

            // prepare existing stall & merchant
            Pawn merchant = new Pawn(new Vector3(0, 0, 0), "test");
            nation.addMember(merchant.getId());
            nation.setRole(merchant.getId(), NationRole.MERCHANT);
            entityRepository.addEntity(merchant);
            settlement.getMarket().createStall();
            Stall stall = settlement.getMarket().getStallForSale();
            stall.setOwnerId(merchant.getId());

            // prepare pawn
            Pawn pawn = new Pawn(new Vector3(0, 0, 0), "test");
            pawn.setEnergy(10);
            pawn.getInventory().addItem(ItemType.COIN, 10);
            entityRepository.addEntity(pawn);
            pawn.setHomeSettlementId(settlement.getId());
            pawn.setCurrentlyInSettlement(true);

            // prepare calculator
            GameConfig gameConfig = new GameConfig();
            TickCounter tickCounter = new TickCounter();
            PawnBehaviorCalculator calculator = new PawnBehaviorCalculator(environment, entityRepository, nationRepository, gameConfig, tickCounter);

            // execute
            BehaviorType behaviorType = calculator.computeBehaviorType(pawn);

            // verify
            UnityEngine.Debug.Assert(behaviorType != BehaviorType.PURCHASE_FOOD);

            // cleanup
            environment.destroyGameObject();
            pawn.destroyGameObject();
            settlement.destroyGameObject();
            nationLeader.destroyGameObject();
            merchant.destroyGameObject();
        }

        private static void testComputeBehaviorType_InSettlementIsMerchantStallHasFood_ShouldCollectFoodFromStall() {
            // prepare world
            EntityRepository entityRepository = new EntityRepository();
            Environment environment = new Environment(5, 5, entityRepository);
            NationRepository nationRepository = new NationRepository();

            // prepare existing nation & settlement
            Pawn nationLeader = new Pawn(new Vector3(0, 0, 0), "test");
            entityRepository.addEntity(nationLeader);
            Nation nation = new Nation("test", nationLeader.getId());
            nationRepository.addNation(nation);
            Settlement settlement = new Settlement(new Vector3(0, 0, 0), nation.getId(), nation.getColor(), nation.getName());
            entityRepository.addEntity(settlement);

            // prepare existing stall & merchant
            Pawn merchant = new Pawn(new Vector3(0, 0, 0), "test");
            nation.addMember(merchant.getId());
            nation.setRole(merchant.getId(), NationRole.MERCHANT);
            merchant.setNationId(nation.getId());
            merchant.setEnergy(10);
            merchant.setHomeSettlementId(settlement.getId());
            merchant.setCurrentlyInSettlement(true);
            entityRepository.addEntity(merchant);
            settlement.getMarket().createStall();
            Stall stall = settlement.getMarket().getStallForSale();
            stall.setOwnerId(merchant.getId());
            stall.getInventory().addItem(ItemType.APPLE, 10);

            // prepare calculator
            GameConfig gameConfig = new GameConfig();
            TickCounter tickCounter = new TickCounter();
            PawnBehaviorCalculator calculator = new PawnBehaviorCalculator(environment, entityRepository, nationRepository, gameConfig, tickCounter);

            // execute
            BehaviorType behaviorType = calculator.computeBehaviorType(merchant);

            // verify
            UnityEngine.Debug.Assert(behaviorType == BehaviorType.COLLECT_FOOD_FROM_STALL);

            // cleanup
            environment.destroyGameObject();
            settlement.destroyGameObject();
            nationLeader.destroyGameObject();
            merchant.destroyGameObject();
        }
    }
}