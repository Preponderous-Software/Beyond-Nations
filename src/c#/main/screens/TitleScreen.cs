using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace beyondnations {

    /**
    * The title screen of the game.
    */
    public class TitleScreen {

        public void OnGUI() {
            int width = Screen.width;
            int height = Screen.height;

            int centerX = width / 2;
            int centerY = height / 2;

            int titleWidth = 200;
            int titleHeight = 100;
            int titleX = centerX - titleWidth / 2;
            int titleY = centerY/2 - titleHeight / 2;

            int buttonWidth = 100;
            int buttonHeight = 50;
            int buttonX = centerX - buttonWidth / 2;
            int buttonY = centerY + buttonHeight / 2;

            // draw title in top center in large font
            GUI.Label(new Rect(titleX, titleY, titleWidth, titleHeight), "Beyond Nations", new GUIStyle() {
                fontSize = Screen.height / 10,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            });

            // draw "press any key to start" in bottom center in small font
            GUI.Label(new Rect(titleX, buttonY, titleWidth, titleHeight), "(Press any key to start)", new GUIStyle() {
                fontSize = Screen.height / 30,
                alignment = TextAnchor.MiddleCenter
            });
        }
    }
}