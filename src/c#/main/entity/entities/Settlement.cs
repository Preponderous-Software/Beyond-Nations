using UnityEngine;

namespace osg {

    public class Settlement : Entity {
        private Color color;
        private GameObject nameTag;
        private NationId nationId;
        private string nationName;
            
        public Settlement(Vector3 position, NationId nationId, Color color, string nationName) : base(EntityType.SETTLEMENT) {
            this.color = color;
            this.nationId = nationId;
            createGameObject(position);
            this.nationName = nationName;
            initializeNameTag();
        }

        public override void createGameObject(Vector3 position) {
            GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            gameObject.transform.localScale = new Vector3(10, 5, 10);
            gameObject.GetComponent<Renderer>().material.color = this.color;
            gameObject.transform.position = position;
            gameObject.name = "Settlement";
            setGameObject(gameObject);
        }

        public override void destroyGameObject() {
            UnityEngine.Object.Destroy(getGameObject());
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
    }
}