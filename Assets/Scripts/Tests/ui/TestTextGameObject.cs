using UnityEngine;
using UnityEngine.UI;

using beyondnations;

namespace beyondnationstests {

    public static class TestTextGameObject {

        public static void runTests() {
            testInstantiation();
            testUpdateText();
        }
        
        private static void testInstantiation() {
            // prepare
            string text = "test";
            int fontSize = 10;
            int x = 0;
            int y = 0;

            // execute
            TextGameObject textGameObject = new TextGameObject(text, fontSize, x, y);

            // verify
            UnityEngine.Debug.Assert(textGameObject.getText() == text);
            UnityEngine.Debug.Assert(textGameObject.getCanvasObject().GetComponentInChildren<Text>().text == text);

            // cleanup
            GameObject.Destroy(textGameObject.getCanvasObject());
        }

        private static void testUpdateText() {
            // prepare
            string text = "test";
            int fontSize = 10;
            int x = 0;
            int y = 0;
            TextGameObject textGameObject = new TextGameObject(text, fontSize, x, y);

            // execute
            string newText = "new text";
            textGameObject.updateText(newText);

            // verify
            UnityEngine.Debug.Assert(textGameObject.getText() == newText);
            UnityEngine.Debug.Assert(textGameObject.getCanvasObject().GetComponentInChildren<Text>().text == newText);

            // cleanup
            GameObject.Destroy(textGameObject.getCanvasObject());
        }
    }
}