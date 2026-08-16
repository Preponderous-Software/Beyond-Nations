using System.Collections.Generic;

namespace beyondnations {

    public class EntityRepository {
        private RandomSource random;
        // TODO: transition to using a dictionary of dictionaries with entity type as the key for the outer dictionary
        private Dictionary<EntityId, Entity> entities;
        private List<EntityId> entityIds;

        public EntityRepository(RandomSource random) {
            this.random = random;
            entities = new Dictionary<EntityId, Entity>();
            entityIds = new List<EntityId>();
        }

        public List<Entity> getEntities() {
            // Iterates entityIds rather than entities.Values: dictionary order
            // follows EntityId's Guid hash and therefore varies between runs,
            // which makes world generation irreproducible. Insertion order does not.
            List<Entity> toReturn = new List<Entity>();
            foreach (EntityId id in entityIds) {
                toReturn.Add(entities[id]);
            }
            return toReturn;
        }

        public List<Entity> getEntitiesOfType(EntityType type) {
            List<Entity> entitiesOfType = new List<Entity>();
            foreach (EntityId id in entityIds) {
                Entity entity = entities[id];
                if (entity.getType() == type) {
                    entitiesOfType.Add(entity);
                }
            }
            return entitiesOfType;
        }

        public Entity getEntity(EntityId id) {
            try {
                return entities[id];
            } catch (KeyNotFoundException) {
                return null;
            }
        }

        public void addEntity(Entity entity) {
            entities.Add(entity.getId(), entity);
            entityIds.Add(entity.getId());
        }

        public void removeEntity(Entity entity) {
            entities.Remove(entity.getId());
            entityIds.Remove(entity.getId());
        }

        public int getNumEntities() {
            return entities.Count;
        }

        public int getNumEntitiesOfType(EntityType type) {
            int numEntitiesOfType = 0;
            foreach (Entity entity in entities.Values) {
                if (entity.getType() == type) {
                    numEntitiesOfType++;
                }
            }
            return numEntitiesOfType;
        }
        
        public Entity getRandomEntity() {
            if (entities.Count == 0) {
                return null;
            }
            int randomIndex = random.range(0, entities.Count);
            return entities[entityIds[randomIndex]];
        }
    }
}