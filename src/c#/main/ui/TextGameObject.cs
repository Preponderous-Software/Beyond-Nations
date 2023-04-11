using UnityEngine;
using UnityEngine.UI;

namespace osg
{
    class TextGameObject
    {
        private string text;
        private int fontSize;
        private int x;
        private int y;
        private GameObject canvasObject;

        public TextGameObject(string text, int fontSize, int x, int y)
        {
            this.text = text;
            this.fontSize = fontSize;
            this.x = x;
            this.y = y;
            CanvasFactory canvasFactory = new CanvasFactory();
            this.canvasObject = canvasFactory.createCanvasObject(text, fontSize, x, y);
        }

        public void updateText(string text)
        {
            this.text = text;
            Text textComponent = canvasObject.GetComponentInChildren<Text>();
            textComponent.text = text;
        }

        public string getText()
        {
            return text;
        }
    }
}
