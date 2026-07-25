using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace beyondnations {

    /**
    * The main menu screen of the game.
    */
    public class MainMenuScreen {

        public ScreenType OnGUI() {
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

            // draw "Beyond Nations" in top center in big font
            GUI.Label(new Rect(titleX, titleY, titleWidth, titleHeight), "Beyond Nations", new GUIStyle() {
                fontSize = Screen.height / 15,
                alignment = TextAnchor.MiddleCenter
            });

            // draw start button at center
            if (GUI.Button(new Rect(buttonX, buttonY, buttonWidth, buttonHeight), "Start")) {
                return ScreenType.WORLD;
            }

            // draw config button at center
            if (GUI.Button(new Rect(buttonX, buttonY + buttonHeight * 2, buttonWidth, buttonHeight), "Config")) {
                return ScreenType.CONFIG;
            }

            // draw exit at bottom center
            if (GUI.Button(new Rect(buttonX, buttonY + buttonHeight * 4, buttonWidth, buttonHeight), "Exit")) {
                Application.Quit();

                // stop
                #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
                #endif
            }
            return ScreenType.MAIN_MENU;
        }
    }
}