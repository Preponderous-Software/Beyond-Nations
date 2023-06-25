using UnityEngine;

using osg;

namespace osgtests {

    public static class TestPawnBehaviorCalculator{

        public static void runTests() {
            testComputeBehaviorTypePawnMarkedForDeletionShouldDoNothing();
            testComputeBehaviorTypePawnNeedsFoodNationLeaderHasApplesShouldPurchaseFood();
            testComputeBehaviorTypePawnNeedsFoodNationLeaderDoesNotHaveApplesShouldGatherResources();
            testComputeBehaviorTypeNoNationShouldWander();
            testComputeBehaviorTypeNationLeaderNoSettlementsShouldCreateSettlement();
            testComputeBehaviorTypeNationLeaderHasSettlementsShouldGoHome();
        }

        /**
         * == Scenario 1 ==
         * Input: pawn marked for deletion
         * Expected output: NONE
        */
        public static void testComputeBehaviorTypePawnMarkedForDeletionShouldDoNothing() {
            // prepare
            EntityRepository entityRepository = new EntityRepository();
            Environment environment = new Environment(5, 5, entityRepository);
            NationRepository nationRepository = new NationRepository();
            PawnBehaviorCalculator calculator = new PawnBehaviorCalculator(environment, entityRepository, nationRepository);
            Pawn pawn = new Pawn(new Vector3(0, 0, 0), "test");
            pawn.markForDeletion();

            // run
            BehaviorType behaviorType = calculator.computeBehaviorType(pawn);

            // check
            Debug.Assert(behaviorType == BehaviorType.NONE);

            // cleanup
            environment.destroyGameObject();
            pawn.destroyGameObject();
        }

        /**
         * == Scenario 2 ==
            * Input: pawn needs food but is nation leader
            * Expected output: GATHER_RESOURCES
        */
        // TODO: implement

        /**
         * == Scenario 3 ==
            * Input: pawn needs food, nation leader has apples
            * Expected output: PURCHASE_FOOD
        */
        public static void testComputeBehaviorTypePawnNeedsFoodNationLeaderHasApplesShouldPurchaseFood() {
            // prepare
            EntityRepository entityRepository = new EntityRepository();
            Environment environment = new Environment(5, 5, entityRepository);
            NationRepository nationRepository = new NationRepository();
            Pawn pawn = new Pawn(new Vector3(0, 0, 0), "test");
            pawn.setEnergy(50);
            entityRepository.addEntity(pawn);

            Pawn nationLeader = new Pawn(new Vector3(0, 0, 0), "testleader");
            nationLeader.getInventory().addItem(ItemType.APPLE, 10);
            entityRepository.addEntity(nationLeader);

            Nation nation = new Nation("testnation", nationLeader.getId());
            nationLeader.setNationId(nation.getId());
            nation.addMember(pawn.getId());
            pawn.setNationId(nation.getId());
            nationRepository.addNation(nation);

            PawnBehaviorCalculator calculator = new PawnBehaviorCalculator(environment, entityRepository, nationRepository);

            // run
            BehaviorType behaviorType = calculator.computeBehaviorType(pawn);

            // check
            Debug.Assert(behaviorType == BehaviorType.PURCHASE_FOOD);

            // cleanup
            environment.destroyGameObject();
            pawn.destroyGameObject();
            nationLeader.destroyGameObject();
        }

        /**
         * == Scenario 4 ==
            * Input: pawn needs food, nation leader does not have apples
            * Expected output: GATHER_RESOURCES
        */
        public static void testComputeBehaviorTypePawnNeedsFoodNationLeaderDoesNotHaveApplesShouldGatherResources() {
            // prepare
            EntityRepository entityRepository = new EntityRepository();
            Environment environment = new Environment(5, 5, entityRepository);
            NationRepository nationRepository = new NationRepository();
            Pawn pawn = new Pawn(new Vector3(0, 0, 0), "test");
            pawn.setEnergy(50);
            entityRepository.addEntity(pawn);

            Pawn nationLeader = new Pawn(new Vector3(0, 0, 0), "testleader");
            entityRepository.addEntity(nationLeader);

            Nation nation = new Nation("testnation", nationLeader.getId());
            nationLeader.setNationId(nation.getId());
            nation.addMember(pawn.getId());
            pawn.setNationId(nation.getId());
            nationRepository.addNation(nation);

            PawnBehaviorCalculator calculator = new PawnBehaviorCalculator(environment, entityRepository, nationRepository);

            // run
            BehaviorType behaviorType = calculator.computeBehaviorType(pawn);

            // check
            Debug.Assert(behaviorType == BehaviorType.GATHER_RESOURCES);

            // cleanup
            environment.destroyGameObject();
            pawn.destroyGameObject();
            nationLeader.destroyGameObject();
        }

        /**
         * == Scenario 5 ==
            * Input: pawn has saplings, no trees/saplings within x units
            * Expected output: PLANT_SAPLING
        */
        // TODO: implement

        /**
         * == Scenario 6 ==
            * Input: pawn has no nation
            * Expected output: WANDER
        */
        public static void testComputeBehaviorTypeNoNationShouldWander() {
            // prepare
            EntityRepository entityRepository = new EntityRepository();
            Environment environment = new Environment(5, 5, entityRepository);
            NationRepository nationRepository = new NationRepository();
            Pawn pawn = new Pawn(new Vector3(0, 0, 0), "test");
            entityRepository.addEntity(pawn);

            PawnBehaviorCalculator calculator = new PawnBehaviorCalculator(environment, entityRepository, nationRepository);

            // run
            BehaviorType behaviorType = calculator.computeBehaviorType(pawn);

            // check
            Debug.Assert(behaviorType == BehaviorType.WANDER);

            // cleanup
            environment.destroyGameObject();
            pawn.destroyGameObject();
        }

        /**
         * == Scenario 7 ==
            * Input: pawn has nation, is leader, has no settlements and no settlement within x units
            * Expected output: create settlement
        */
        public static void testComputeBehaviorTypeNationLeaderNoSettlementsShouldCreateSettlement() {
            // prepare
            EntityRepository entityRepository = new EntityRepository();
            Environment environment = new Environment(5, 5, entityRepository);
            NationRepository nationRepository = new NationRepository();
            Pawn pawn = new Pawn(new Vector3(0, 0, 0), "test");
            entityRepository.addEntity(pawn);

            Pawn nationLeader = new Pawn(new Vector3(0, 0, 0), "testleader");
            entityRepository.addEntity(nationLeader);

            Nation nation = new Nation("testnation", nationLeader.getId());
            nationLeader.setNationId(nation.getId());
            nation.addMember(pawn.getId());
            pawn.setNationId(nation.getId());
            nationRepository.addNation(nation);

            PawnBehaviorCalculator calculator = new PawnBehaviorCalculator(environment, entityRepository, nationRepository);

            // run
            BehaviorType behaviorType = calculator.computeBehaviorType(nationLeader);

            // check
            Debug.Assert(behaviorType == BehaviorType.CREATE_SETTLEMENT);

            // cleanup
            environment.destroyGameObject();
            pawn.destroyGameObject();
            nationLeader.destroyGameObject();
        }

        /**
         * == Scenario 8 ==
            * Input: pawn has nation, is leader, has no settlements and settlement within x units
            * Expected output: GATHER_RESOURCES
        */
        // TODO: implement

        /**
         * == Scenario 9 ==
            * Input: pawn has nation, is leader, has settlements
            * Expected output: GO_HOME
        */
        public static void testComputeBehaviorTypeNationLeaderHasSettlementsShouldGoHome() {
            // prepare
            EntityRepository entityRepository = new EntityRepository();
            Environment environment = new Environment(5, 5, entityRepository);
            NationRepository nationRepository = new NationRepository();
            Pawn pawn = new Pawn(new Vector3(0, 0, 0), "test");
            entityRepository.addEntity(pawn);

            Pawn nationLeader = new Pawn(new Vector3(0, 0, 0), "testleader");
            entityRepository.addEntity(nationLeader);

            Nation nation = new Nation("testnation", nationLeader.getId());
            nationLeader.setNationId(nation.getId());
            nation.addMember(pawn.getId());
            pawn.setNationId(nation.getId());
            nationRepository.addNation(nation);

            Settlement settlement = new Settlement(new Vector3(0, 0, 0), nation.getId(), nation.getColor(), "testsettlement");
            entityRepository.addEntity(settlement);
            nation.addSettlement(settlement.getId());

            PawnBehaviorCalculator calculator = new PawnBehaviorCalculator(environment, entityRepository, nationRepository);

            // run
            BehaviorType behaviorType = calculator.computeBehaviorType(nationLeader);

            // check
            Debug.Assert(behaviorType == BehaviorType.GO_HOME);

            // cleanup
            environment.destroyGameObject();
            pawn.destroyGameObject();
            nationLeader.destroyGameObject();
            settlement.destroyGameObject();
        }

        /**
         * == Scenario 10 ==
            * Input: pawn is citizen, does not have an abundance of resources
            * Expected output: GATHER_RESOURCES
        */
        // TODO: implement

        /**
         * == Scenario 11 ==
            * Input: pawn is citizen, has an abundance of resources, nation leader has enough money
            * Expected output: SELL_RESOURCES
        */
        // TODO: implement

        /**
         * == Scenario 12 ==
            * Input: pawn is citizen, has an abundance of resources, nation leader does not have enough money
            * Expected output: GO_HOME
        */
        // TODO: implement
    }
}