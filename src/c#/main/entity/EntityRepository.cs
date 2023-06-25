using UnityEngine;
using System.Collections.Generic;

namespace osg {

    public class EntityRepository {
        // TODO: transition to using a dictionary of dictionaries with entity type as the key for the outer dictionary
        private Dictionary<EntityId, Entity> entities;
        private List<EntityId> entityIds;

        public EntityRepository() {
            entities = new Dictionary<EntityId, Entity>();
            entityIds = new List<EntityId>();
        }

        public List<Entity> getEntities() {
            return new List<Entity>(entities.Values);
        }

        public List<Entity> getEntitiesOfType(EntityType type) {
            List<Entity> entitiesOfType = new List<Entity>();
            foreach (Entity entity in entities.Values) {
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

        public int getNumberOfEntitys() {
            return entities.Count;
        }

        public Entity getRandomEntity() {
            int randomIndex = Random.Range(0, entityIds.Count);
            EntityId randomEntityId = entityIds[randomIndex];
            return entities[randomEntityId];
        }
    }
}