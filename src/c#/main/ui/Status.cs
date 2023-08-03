using UnityEngine;

namespace beyondnations {

    public class Status {
        private TickCounter tickCounter;
        private int ticksToExpire;
        private TextGameObject statusText;
        private int tickLastSet = 0;
        private int duplicateMessageCount = 0;

        public Status(TickCounter tickCounter, int ticksToExpire) {
            this.tickCounter = tickCounter;
            this.ticksToExpire = ticksToExpire;
            createStatusText();
        }

        public void update(string status) {
            if (getStatus().Contains(status)) {
                duplicateMessageCount += 1;
                status += " (x" + (duplicateMessageCount + 1) + ")";
            }
            else {
                duplicateMessageCount = 0;
            }
            statusText.updateText(status);
            tickLastSet = tickCounter.getTotalTicks();
        }

        public void clearStatusIfExpired() {
            if (tickCounter.getTotalTicks() - tickLastSet > ticksToExpire) {
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

        public TextGameObject getTextGameObject() {
            return statusText;
        }

        private void createStatusText() {
            int x = 0;
            int y = -(Screen.height / 4);
            int fontSize = 20;
            statusText = new TextGameObject("Game started.", fontSize, x, y);
        }
    }
}