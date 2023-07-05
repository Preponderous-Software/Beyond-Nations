using UnityEngine;

using osg;

namespace osgtests {

    public static class TestPawnBehaviorCalculator{

        public static void runTests() {
            testComputeBehaviorTypeNoNationShouldWander();
            testComputeBehaviorTypeNationLeaderNoSettlementsShouldCreateSettlement();
        }
        /**
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
            GameConfig gameConfig = new GameConfig();

            PawnBehaviorCalculator calculator = new PawnBehaviorCalculator(environment, entityRepository, nationRepository, gameConfig);

            // run
            BehaviorType behaviorType = calculator.computeBehaviorType(pawn);

            // check
            UnityEngine.Debug.Assert(behaviorType == BehaviorType.WANDER);

            // cleanup
            environment.destroyGameObject();
            pawn.destroyGameObject();
        }

        /**
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
            GameConfig gameConfig = new GameConfig();

            PawnBehaviorCalculator calculator = new PawnBehaviorCalculator(environment, entityRepository, nationRepository, gameConfig);

            // run
            BehaviorType behaviorType = calculator.computeBehaviorType(nationLeader);

            // check
            UnityEngine.Debug.Assert(behaviorType == BehaviorType.CONSTRUCT_SETTLEMENT);

            // cleanup
            environment.destroyGameObject();
            pawn.destroyGameObject();
            nationLeader.destroyGameObject();
        }
    }
}