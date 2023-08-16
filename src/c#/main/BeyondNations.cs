using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace beyondnations {

    /**
    * The main class and entry point of the game.
    */
    public class BeyondNations : MonoBehaviour {
        private TitleScreen titleScreen;
        private WorldScreen worldScreen;
        private PauseScreen pauseScreen;
        private MainMenuScreen mainMenuScreen;
        private ConfigScreen configScreen;
    
        private GameConfig gameConfig;

        private ScreenType currentScreen = ScreenType.TITLE;
        
        private string version = "0.3.0-alpha";

        public bool runTests;
        public bool debugMode;

        public void Start() {
            if (runTests) {
                Debug.Log("Running tests...");
                beyondnationstests.Tests.runTests();
                Debug.Log("Tests complete.");
            }
            else {
                Debug.Log("Not running tests. Set `runTests` to true to run tests.");
            }
            titleScreen = new TitleScreen();
            pauseScreen = new PauseScreen();
            mainMenuScreen = new MainMenuScreen();
            configScreen = new ConfigScreen();
            gameConfig = new GameConfig();
        }

        public void Update() {
            if (currentScreen == ScreenType.TITLE) {
                if (Input.anyKey) {
                    currentScreen = ScreenType.MAIN_MENU;
                }
                return;
            }
            else if (currentScreen == ScreenType.WORLD) {
                if (Input.GetKeyDown(KeyCode.Escape)) {
                    currentScreen = ScreenType.PAUSE;
                    return;
                }
                if (worldScreen == null) {
                    initializeWorldScreen();
                }
                worldScreen.Update();
            }
            else if (currentScreen == ScreenType.PAUSE) {
                if (Input.GetKeyDown(KeyCode.Escape)) {
                    currentScreen = ScreenType.WORLD;
                    return;
                }
            }
            else if (currentScreen == ScreenType.MAIN_MENU) {
                if (Input.GetKeyDown(KeyCode.Escape)) {
                    // exit
                    Application.Quit();

                    // stop
                    #if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
                    #endif
                    return;
                }
            }
            else if (currentScreen == ScreenType.CONFIG) {
                if (Input.GetKeyDown(KeyCode.Escape)) {
                    currentScreen = ScreenType.MAIN_MENU;
                    return;
                }
            }
            else {
                throw new Exception("Unknown screen type: " + currentScreen);
            }

            captureScreenshotIfKeyPressed();
        }

        public void FixedUpdate() {
            if (currentScreen == ScreenType.TITLE) {
                return;
            }
            else if (currentScreen == ScreenType.WORLD) {
                worldScreen.FixedUpdate();
            }
            else if (currentScreen == ScreenType.PAUSE) {
                return;
            }
            else if (currentScreen == ScreenType.MAIN_MENU) {
                return;
            }
            else if (currentScreen == ScreenType.CONFIG) {
                return;
            }
            else {
                throw new Exception("Unknown screen type: " + currentScreen);
            }
        }

        public void OnGUI() {
            if (currentScreen == ScreenType.TITLE) {
                titleScreen.OnGUI();
            }
            else if (currentScreen == ScreenType.WORLD) {
                if (worldScreen == null) {
                    initializeWorldScreen();
                }
                worldScreen.OnGUI();
            }
            else if (currentScreen == ScreenType.PAUSE) {
                pauseScreen.OnGUI();
            }
            else if (currentScreen == ScreenType.MAIN_MENU) {
                currentScreen = mainMenuScreen.OnGUI();
            }
            else if (currentScreen == ScreenType.CONFIG) {
                currentScreen = configScreen.OnGUI(gameConfig);
            }
            else {
                throw new Exception("Unknown screen type: " + currentScreen);
            }

            // put version number in bottom left corner
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.white;
            style.fontSize = 12;
            GUI.Label(new Rect(10, Screen.height - 20, 100, 20), version, style);
        }

        private void initializeWorldScreen() {
            worldScreen = new WorldScreen(gameConfig, debugMode);
        }

        private void captureScreenshotIfKeyPressed() {
            if (Input.GetKeyDown(KeyBindings.takeScreenshot)) {
                // generate filename
                string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssffff");
                string filename = "screenshot_" + timestamp + ".png";
                string path = "C:\\BeyondNations\\Screenshots\\" + filename;

                // create directory if it doesn't exist
                System.IO.Directory.CreateDirectory(gameConfig.getBeyondNationsDirectoryPath() + "\\Screenshots\\");

                // take screenshot
                ScreenCapture.CaptureScreenshot(path);
                Debug.Log("Screenshot saved to " + path);
            }
        }
    }
}