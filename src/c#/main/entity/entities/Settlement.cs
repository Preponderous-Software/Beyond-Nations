using UnityEngine;
using System.Collections.Generic;

namespace osg {

    public class Settlement : Entity {
        private Color color;
        private GameObject nameTag;
        private NationId nationId;
        private string nationName;
        private List<EntityId> currentlyPresentEntities = new List<EntityId>();
        private Market market;
            
        public Settlement(Vector3 position, NationId nationId, Color color, string nationName) : base(EntityType.SETTLEMENT) {
            this.color = color;
            this.nationId = nationId;
            createGameObject(position);
            this.nationName = nationName;
            initializeNameTag();
            market = new Market(4);
            market.createStall();
        }

        public override void createGameObject(Vector3 position) {
            GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            gameObject.transform.localScale = new Vector3(10, 5, 10);
            gameObject.GetComponent<Renderer>().material.color = this.color;
            gameObject.transform.position = position;
            gameObject.name = "Settlement";
            UnityEngine.Object.Destroy(gameObject.GetComponent<CapsuleCollider>());
            setGameObject(gameObject);
        }

        public override void destroyGameObject() {
            UnityEngine.Object.Destroy(getGameObject());
        }

        public List<EntityId> getCurrentlyPresentEntities() {
            return this.currentlyPresentEntities;
        }

        public void addCurrentlyPresentEntity(EntityId entityId) {
            this.currentlyPresentEntities.Add(entityId);
            updateNameTagWithCurrentlyPresentEntities();
        }

        public void removeCurrentlyPresentEntity(EntityId entityId) {
            this.currentlyPresentEntities.Remove(entityId);
            updateNameTagWithCurrentlyPresentEntities();
        }

        public int getCurrentlyPresentEntitiesCount() {
            return this.currentlyPresentEntities.Count;
        }

        public Color getColor() {
            return this.color;
        }

        public string getNameTagText() {
            return nameTag.GetComponent<TextMesh>().text;
        }

        public Market getMarket() {
            return this.market;
        }

        private void initializeNameTag() {
            nameTag = new GameObject();
            nameTag.transform.parent = getGameObject().transform;
            nameTag.transform.localPosition = new Vector3(0, (float)0.75, 0);
            TextMesh textMesh = nameTag.AddComponent<TextMesh>();
            textMesh.text = "Settlement of " + this.nationName;
            textMesh.fontSize = 64;
            textMesh.color = Color.black;
            textMesh.alignment = TextAlignment.Center;
            textMesh.anchor = TextAnchor.MiddleCenter;
            textMesh.characterSize = 0.1f;
            textMesh.GetComponent<Renderer>().material.color = Color.black;
        }

        private void setNameTag(string name) {
            nameTag.GetComponent<TextMesh>().text = name;
        }

        private void updateNameTagWithCurrentlyPresentEntities() {
            string name = "Settlement of " + this.nationName + "\n\n";
            name += "(" + getCurrentlyPresentEntitiesCount() + ")";
            setNameTag(name);
        }
    }
}