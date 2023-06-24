using UnityEngine;
using UnityEngine.UI;

using osg;

namespace osgtests {

    public static class TestCanvasFactory {

        public static void runTests() {
            testCreateCanvasObject();
        }
        
        private static void testCreateCanvasObject() {
            // execute
            GameObject canvasObject = new CanvasFactory().createCanvasObject("test", 12, 0, 0);

            // verify canvas render mode
            Canvas canvas = canvasObject.GetComponent<Canvas>();
            Debug.Assert(canvas != null);
            Debug.Assert(canvas.renderMode == RenderMode.ScreenSpaceOverlay);

            // verify text component
            GameObject textObject = canvasObject.transform.Find("Text").gameObject;
            Debug.Assert(textObject != null);
            Text textComponent = textObject.GetComponent<Text>();
            Debug.Assert(textComponent != null);
            Debug.Assert(textComponent.text == "test");
            Debug.Assert(textComponent.font != null);
            Debug.Assert(textComponent.fontSize == 12);
            Debug.Assert(textComponent.color == Color.black);
            Debug.Assert(textComponent.rectTransform.sizeDelta == new Vector2(250, 100));
            Debug.Assert(textComponent.rectTransform.anchoredPosition == new Vector2(0, 0));
        }
    }
}