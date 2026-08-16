using System;
using System.Numerics;
using Xunit;

using beyondnations;

namespace beyondnationstests {

    /**
    * The snapshot is what the instanced renderer draws from, so what it puts in
    * its lists, and how much it allocates doing so, are both worth pinning down.
    */
    public class TestWorldSnapshot {
        private readonly RandomSource random = new RandomSource(20260816);

        private EntityRepository newRepository() {
            return new EntityRepository(random);
        }

        [Fact]
        public void testCaptureWithNoEnvironmentYieldsEntitiesAndNoGround() {
            // prepare
            EntityRepository entityRepository = newRepository();
            entityRepository.addEntity(new Rock(new Vector3(1, 0, 2)));
            WorldSnapshot snapshot = new WorldSnapshot();

            // run
            snapshot.capture(entityRepository, null);

            // verify
            Assert.Single(snapshot.getEntityItems());
            Assert.Empty(snapshot.getGroundItems());
            Assert.Equal(PrimitiveKind.Cube, snapshot.getEntityItems()[0].kind);
            Assert.Equal(new Vector3(1, 0, 2), snapshot.getEntityItems()[0].position);
        }

        [Fact]
        public void testCaptureEmitsOneItemPerAppearancePart() {
            // prepare
            EntityRepository entityRepository = newRepository();
            entityRepository.addEntity(new AppleTree(new Vector3(0, 0, 0), 5, random));
            WorldSnapshot snapshot = new WorldSnapshot();

            // run
            snapshot.capture(entityRepository, null);

            // verify: a tree is a trunk and a block of leaves
            Assert.Equal(2, snapshot.getEntityItems().Count);
            Assert.Equal(PrimitiveKind.Cylinder, snapshot.getEntityItems()[0].kind);
            Assert.Equal(PrimitiveKind.Cube, snapshot.getEntityItems()[1].kind);
        }

        [Fact]
        public void testCaptureSkipsEntitiesMarkedForDeletion() {
            // prepare
            EntityRepository entityRepository = newRepository();
            Rock rock = new Rock(new Vector3(0, 0, 0));
            entityRepository.addEntity(rock);
            WorldSnapshot snapshot = new WorldSnapshot();

            // run
            rock.markForDeletion();
            snapshot.capture(entityRepository, null);

            // verify
            Assert.Empty(snapshot.getEntityItems());
        }

        [Fact]
        public void testCaptureIncludesEveryGroundTile() {
            // prepare
            EntityRepository entityRepository = newRepository();
            // an environment starts with one chunk of its own, so this is two
            beyondnations.Environment environment = new beyondnations.Environment(3, 2, entityRepository, random);
            environment.addChunk(new Chunk(1, 0, 3, 2, random));
            WorldSnapshot snapshot = new WorldSnapshot();

            // run
            snapshot.capture(entityRepository, environment);

            // verify
            Assert.Equal(18, snapshot.getGroundItems().Count);
            Assert.Equal(PrimitiveKind.Cube, snapshot.getGroundItems()[0].kind);
            Assert.Equal(new Vector3(2, 1, 2), snapshot.getGroundItems()[0].scale);
        }

        [Fact]
        public void testRepeatedCaptureDoesNotAccumulateOrAllocate() {
            // prepare
            EntityRepository entityRepository = newRepository();
            beyondnations.Environment environment = new beyondnations.Environment(3, 2, entityRepository, random);
            entityRepository.addEntity(new AppleTree(new Vector3(0, 0, 0), 5, random));
            WorldSnapshot snapshot = new WorldSnapshot();

            // run: the first captures grow the lists, after which the buffers
            // are reused and nothing more should be allocated
            for (int i = 0; i < 5; i++) {
                snapshot.capture(entityRepository, environment);
            }
            long before = GC.GetAllocatedBytesForCurrentThread();
            for (int i = 0; i < 20; i++) {
                snapshot.capture(entityRepository, environment);
            }
            long allocated = GC.GetAllocatedBytesForCurrentThread() - before;

            // verify
            Assert.Equal(9, snapshot.getGroundItems().Count);
            Assert.Equal(2, snapshot.getEntityItems().Count);
            Assert.Equal(11, snapshot.getTotalItemCount());
            Assert.Equal(0, allocated);
        }
    }
}
