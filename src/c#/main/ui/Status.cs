using UnityEngine;

namespace osg {

    public class Status {
        private TickCounter tickCounter;
        private int ticksToExpire;
        private TextGameObject statusText;
        private int tickLastSet = 0;

        public Status(TickCounter tickCounter, int ticksToExpire) {
            this.tickCounter = tickCounter;
            this.ticksToExpire = ticksToExpire;
            createStatusText();
        }

        public void update(string status) {
            statusText.updateText(status);
            tickLastSet = tickCounter.getTick();
        }

        public void clearStatusIfExpired() {
            if (tickCounter.getTick() - tickLastSet > ticksToExpire) {
                statusText.updateText("");
            }
        }

        public int getTicksToExpire() {
            return ticksToExpire;
        }

        public int getTickLastSet() {
            return tickLastSet;
        }

        public string getStatus() {
            return statusText.getText();
        }

        private void createStatusText() {
            int x = 0;
            int y = -(Screen.height / 4);
            int fontSize = 20;
            statusText = new TextGameObject("Game started.", fontSize, x, y);
        }
    }
}