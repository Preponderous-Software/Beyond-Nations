using System.Collections.Generic;
using System.Numerics;

namespace beyondnations {

    /**
    * One primitive to draw, flattened out of the simulation. Positions are
    * already in world space, so the host does no transform-tree walking.
    */
    public struct RenderItem {
        public PrimitiveKind kind;
        public Vector3 position;
        public Vector3 scale;
        public Rgba color;
    }

    /**
    * A world-space label to draw above something.
    */
    public struct LabelItem {
        public Vector3 position;
        public string text;
    }

    /**
    * Everything the host needs in order to draw a frame, and nothing else.
    *
    * This is the query API #213 calls for. The host asks the simulation what is
    * there; the simulation never reaches into the host to create or destroy a
    * graphics resource, because it holds none.
    *
    * capture() refills the same buffers rather than allocating new ones, since
    * #218 requires that no per-object allocation happens in the render loop.
    * Callers should hold one WorldSnapshot and re-capture into it each frame.
    */
    public class WorldSnapshot {
        private readonly List<RenderItem> entityItems = new List<RenderItem>();
        private readonly List<RenderItem> groundItems = new List<RenderItem>();
        private readonly List<LabelItem> labels = new List<LabelItem>();

        /**
        * Pawns, settlements, trees, rocks, saplings, chickens and item drops.
        */
        public IReadOnlyList<RenderItem> getEntityItems() {
            return entityItems;
        }

        /**
        * Ground tiles, which are locations rather than entities.
        */
        public IReadOnlyList<RenderItem> getGroundItems() {
            return groundItems;
        }

        public IReadOnlyList<LabelItem> getLabels() {
            return labels;
        }

        public int getTotalItemCount() {
            return entityItems.Count + groundItems.Count;
        }

        /**
        * Rebuilds this snapshot from the current state of the world.
        *
        * Entities marked for deletion are omitted, so marking one stops it being
        * drawn on the very next frame without anything having to be destroyed.
        * The environment may be null, which yields entities with no ground.
        */
        public void capture(EntityRepository entityRepository, Environment environment) {
            entityItems.Clear();
            groundItems.Clear();
            labels.Clear();

            // Indexed rather than foreach throughout: getEntities() would copy
            // the whole entity list, and enumerating an IReadOnlyList or a
            // rectangular array through its interface allocates an enumerator.
            // All three are allocations proportional to the number of things in
            // the world, once a frame, which is what #218 rules out.
            int entityCount = entityRepository.getEntityCount();
            for (int e = 0; e < entityCount; e++) {
                Entity entity = entityRepository.getEntityAt(e);
                if (entity.isMarkedForDeletion() || !entity.isVisible()) {
                    continue;
                }

                Vector3 origin = entity.getPosition();
                Appearance appearance = entity.getAppearance();

                IReadOnlyList<AppearancePart> parts = appearance.getParts();
                for (int p = 0; p < parts.Count; p++) {
                    AppearancePart part = parts[p];
                    RenderItem item;
                    item.kind = part.getKind();
                    item.position = origin + part.getOffset();
                    item.scale = part.getScale();
                    item.color = part.getColor();
                    entityItems.Add(item);
                }

                if (appearance.hasLabel()) {
                    LabelItem label;
                    label.position = origin;
                    label.text = appearance.getLabel();
                    labels.Add(label);
                }
            }

            if (environment == null) {
                return;
            }

            List<Chunk> chunks = environment.getChunks();
            for (int c = 0; c < chunks.Count; c++) {
                Location[,] locations = chunks[c].getLocations();
                int width = locations.GetLength(0);
                int depth = locations.GetLength(1);
                for (int x = 0; x < width; x++) {
                    for (int z = 0; z < depth; z++) {
                        Location location = locations[x, z];
                        if (location == null) {
                            continue;
                        }
                        RenderItem item;
                        item.kind = PrimitiveKind.Cube;
                        item.position = location.getPosition();
                        item.scale = location.getScaleVector();
                        item.color = location.getColor();
                        groundItems.Add(item);
                    }
                }
            }
        }
    }
}
