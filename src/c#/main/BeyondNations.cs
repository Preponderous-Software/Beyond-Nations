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
    
        private GameConfig gameConfig;

        private ScreenType currentScreen = ScreenType.TITLE;

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
            gameConfig = new GameConfig();
        }

        public void Update() {
            if (currentScreen == ScreenType.TITLE) {
                if (Input.anyKey) {
                    currentScreen = ScreenType.WORLD;
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
            else {
                throw new Exception("Unknown screen type: " + currentScreen);
            }
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
            else {
                throw new Exception("Unknown screen type: " + currentScreen);
            }
        }

        private void initializeWorldScreen() {
            worldScreen = new WorldScreen(gameConfig, debugMode);
        }
    }
}