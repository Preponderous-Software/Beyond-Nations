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
            int titleY = centerY - titleHeight;

            // draw title in top center in large font
            GUI.Label(new Rect(titleX, titleY, titleWidth, titleHeight), "Beyond Nations", new GUIStyle() {
                fontSize = Screen.height / 10,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            });
        }
    }
}