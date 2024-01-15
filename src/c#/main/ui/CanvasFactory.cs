using UnityEngine;
using UnityEngine.UI;

namespace beyondnations {

    public class CanvasFactory {
        private static int canvasCount = 0;
        
        public GameObject createCanvasObject(string text, int fontSize, int x, int y) {
            GameObject canvasObject = new GameObject("Canvas-" + canvasCount++);
            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            GameObject textObject = new GameObject("Text");
            textObject.transform.SetParent(canvasObject.transform);
            Text textComponent = textObject.AddComponent<Text>();
            textComponent.text = text;
            textComponent.font = Resources.GetBuiltinResource(typeof(Font), "LegacyRuntime.ttf") as Font;
            textComponent.fontSize = fontSize;
            textComponent.color = Color.black;
            textComponent.alignment = TextAnchor.MiddleCenter;
            textComponent.rectTransform.sizeDelta = new Vector2(250, 100);
            textComponent.rectTransform.anchoredPosition = new Vector2(x, y);

            return canvasObject;
        }
    }
}