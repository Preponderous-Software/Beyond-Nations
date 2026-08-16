using System.Diagnostics;

using System.Numerics;
using Xunit;

using beyondnations;

namespace beyondnationstests {

    public class TestMarket {
        private readonly RandomSource random = new RandomSource(20260816);



        [Fact]
        public void testInstantiation() {
            Market market = new Market(10, random);
            Assert.NotNull(market);
            Assert.Equal(0, market.getNumStalls());
            Assert.Equal(10, market.getMaxNumStalls());
        }

        [Fact]
        public void testCreateStall() {
            // prepare
            Market market = new Market(10, random);

            // execute
            bool result = market.createStall();

            // verify
            Assert.True(result);
            Assert.Equal(1, market.getNumStalls());
            Assert.Equal(1, market.getNumStallsForSale());
        }

        [Fact]
        public void testGetStallForSale() {
            // prepare
            Market market = new Market(10, random);
            market.createStall();

            // execute
            Stall stall = market.getStallForSale();

            // verify
            Assert.NotNull(stall);
            Assert.Null(stall.getOwnerId());
        }

        [Fact]
        public void testGetStall() {
            // prepare
            Market market = new Market(10, random);
            Pawn pawn = new Pawn(new Vector3(0, 0, 0), "test", random);
            market.createStall();
            market.getStallForSale().setOwnerId(pawn.getId());

            // execute
            Stall stall = market.getStall(pawn.getId());

            // verify
            Assert.NotNull(stall);

            // cleanup
        }
    }
}