namespace beyondnations {

    /**
    * Which screen the game is showing.
    *
    * This lived in BeyondNations.cs, the single MonoBehaviour in the project,
    * alongside a Unity Update() that switched on it. That class is dropped
    * rather than adapted, as #216 requires, and the state machine becomes host
    * state here.
    *
    * The transitions are the ones the Unity version had: any key leaves the
    * title screen, Escape moves between the main menu, the world and the pause
    * screen, and the config screen is reached from the main menu.
    */
    public class ScreenState {
        private ScreenType current = ScreenType.TITLE;
        private ScreenType previous = ScreenType.TITLE;

        public ScreenType getCurrent() {
            return current;
        }

        public ScreenType getPrevious() {
            return previous;
        }

        public bool isWorldActive() {
            return current == ScreenType.WORLD;
        }

        /**
        * The world keeps ticking behind the pause screen only if the host says
        * so; by default the simulation advances only while the world is shown.
        */
        public bool shouldAdvanceSimulation() {
            return current == ScreenType.WORLD;
        }

        public void goTo(ScreenType screen) {
            if (screen == current) {
                return;
            }
            previous = current;
            current = screen;
            Log.info("screen: " + previous + " -> " + current);
        }

        /**
        * Any key press leaves the title screen, as before.
        */
        public void anyKeyPressed() {
            if (current == ScreenType.TITLE) {
                goTo(ScreenType.MAIN_MENU);
            }
        }

        /**
        * Escape, which means something different on each screen. Returns false
        * when the press should quit the game, which is what Escape did on the
        * main menu.
        */
        public bool escapePressed() {
            if (current == ScreenType.WORLD) {
                goTo(ScreenType.PAUSE);
                return true;
            }
            if (current == ScreenType.PAUSE) {
                goTo(ScreenType.WORLD);
                return true;
            }
            if (current == ScreenType.CONFIG) {
                goTo(ScreenType.MAIN_MENU);
                return true;
            }
            if (current == ScreenType.MAIN_MENU) {
                return false;
            }
            return true;
        }
    }
}
