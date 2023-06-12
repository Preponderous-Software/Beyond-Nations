using UnityEngine;

using osg;

namespace osgtests {

    public static class TestPawnBehaviorCalculator{

        public static void runTests() {
            testComputeBehaviorTypePawnMarkedForDeletionShouldDoNothing();
            testComputeBehaviorTypePawnNeedsFoodNationLeaderHasApplesShouldPurchaseFood();
            testComputeBehaviorTypePawnNeedsFoodNationLeaderDoesNotHaveApplesShouldGatherResources();
            testComputeBehaviorTypeNoNationShouldWander();
        }

        // input: pawn marked for deletion
        // expected output: none
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

        // input: pawn needs food, nation leader has apples
        // expected output: purchase food
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

        // input: pawn needs food, nation leader does not have apples
        // expected output: gather resources
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

        // input: pawn has no nation
        // expected output: wander
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
    }    
}