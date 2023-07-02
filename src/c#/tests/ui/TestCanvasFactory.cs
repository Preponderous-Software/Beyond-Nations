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
            UnityEngine.Debug.Assert(canvas != null);
            UnityEngine.Debug.Assert(canvas.renderMode == RenderMode.ScreenSpaceOverlay);

            // verify text component
            GameObject textObject = canvasObject.transform.Find("Text").gameObject;
            UnityEngine.Debug.Assert(textObject != null);
            Text textComponent = textObject.GetComponent<Text>();
            UnityEngine.Debug.Assert(textComponent != null);
            UnityEngine.Debug.Assert(textComponent.text == "test");
            UnityEngine.Debug.Assert(textComponent.font != null);
            UnityEngine.Debug.Assert(textComponent.fontSize == 12);
            UnityEngine.Debug.Assert(textComponent.color == Color.black);
            UnityEngine.Debug.Assert(textComponent.rectTransform.sizeDelta == new Vector2(250, 100));
            UnityEngine.Debug.Assert(textComponent.rectTransform.anchoredPosition == new Vector2(0, 0));

            // cleanup
            GameObject.Destroy(canvasObject);
        }
    }
}