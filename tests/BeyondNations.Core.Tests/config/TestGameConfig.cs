
using Xunit;

using beyondnations;

namespace beyondnationstests {

    public class TestGameConfig {


        [Fact]
        public void testInstantiation() {
            // run
            GameConfig config = new GameConfig();
            
            // check
            Assert.True(config.getChunkSize() > 0);
            Assert.True(config.getLocationScale() > 0);
            Assert.True(config.getStatusExpirationTicks() > 0);
            Assert.True(config.getPlayerWalkSpeed() > 0);
            Assert.True(config.getPlayerRunSpeed() > config.getPlayerWalkSpeed());
            Assert.True(config.getTicksBetweenBehaviorCalculations() > 0);
            Assert.True(config.getTicksBetweenBehaviorExecutions() > 0);
            Assert.True(config.getMinDistanceBetweenSettlements() > 0);
            Assert.True(config.getSettlementJoinRange() > 0);
        }
    }    
}