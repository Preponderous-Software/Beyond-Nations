using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace beyondnations {

    /**
    * The config screen of the game.
    */
    public class ConfigScreen {

        public ScreenType OnGUI(GameConfig gameConfig) {
            int width = Screen.width;
            int height = Screen.height;

            int centerX = width / 2;
            int centerY = height / 2;

            int titleWidth = 200;
            int titleHeight = 100;
            int titleX = centerX - titleWidth / 2;
            int titleY = centerY/2 - titleHeight / 2;

            int buttonWidth = 100;
            int buttonHeight = 20;
            int buttonX = centerX - buttonWidth / 2;
            int buttonY = centerY + buttonHeight / 2;

            // draw "Config" title
            GUI.Label(new Rect(titleX, titleY, titleWidth, titleHeight), "Config", new GUIStyle() {
                fontSize = Screen.height / 15,
                alignment = TextAnchor.MiddleCenter
            });
            
            // chunk size (increment by 1, allow 7-17)
            GUI.Label(new Rect(buttonX, buttonY + buttonHeight * 1, buttonWidth, buttonHeight), "Chunk Size");
            if (GUI.Button(new Rect(buttonX + buttonWidth, buttonY + buttonHeight * 1, buttonWidth, buttonHeight), gameConfig.getChunkSize().ToString())) {
                gameConfig.setChunkSize(gameConfig.getChunkSize() + 1);

                if (gameConfig.getChunkSize() > 17) {
                    gameConfig.setChunkSize(7);
                }
            }

            // location scale (increment by 1, allow 7-17)
            GUI.Label(new Rect(buttonX, buttonY + buttonHeight * 2, buttonWidth, buttonHeight), "Location Scale");
            if (GUI.Button(new Rect(buttonX + buttonWidth, buttonY + buttonHeight * 2, buttonWidth, buttonHeight), gameConfig.getLocationScale().ToString())) {
                gameConfig.setLocationScale(gameConfig.getLocationScale() + 1);

                if (gameConfig.getLocationScale() > 17) {
                    gameConfig.setLocationScale(7);
                }
            }

            // respawn pawns (true or false)
            GUI.Label(new Rect(buttonX, buttonY + buttonHeight * 3, buttonWidth, buttonHeight), "Respawn Pawns");
            if (GUI.Button(new Rect(buttonX + buttonWidth, buttonY + buttonHeight * 3, buttonWidth, buttonHeight), gameConfig.getRespawnPawns().ToString())) {
                gameConfig.setRespawnPawns(!gameConfig.getRespawnPawns());
            }

            // keep inventory on death (true or false)
            GUI.Label(new Rect(buttonX, buttonY + buttonHeight * 4, buttonWidth, buttonHeight), "Keep Inventory");
            if (GUI.Button(new Rect(buttonX + buttonWidth, buttonY + buttonHeight * 4, buttonWidth, buttonHeight), gameConfig.getKeepInventoryOnDeath().ToString())) {
                gameConfig.setKeepInventoryOnDeath(!gameConfig.getKeepInventoryOnDeath());
            }

            // lag prevention (true or false)
            GUI.Label(new Rect(buttonX, buttonY + buttonHeight * 5, buttonWidth, buttonHeight), "Lag Prevention");
            if (GUI.Button(new Rect(buttonX + buttonWidth, buttonY + buttonHeight * 5, buttonWidth, buttonHeight), gameConfig.getLagPreventionEnabled().ToString())) {
                gameConfig.setLagPreventionEnabled(!gameConfig.getLagPreventionEnabled());
            }

            // draw back at bottom center (1 buttonheight from bottom)
            if (GUI.Button(new Rect(buttonX, Screen.height - buttonHeight * 2, buttonWidth, buttonHeight), "Back")) {
                return ScreenType.MAIN_MENU;
            }

            return ScreenType.CONFIG;
        }
    }
}