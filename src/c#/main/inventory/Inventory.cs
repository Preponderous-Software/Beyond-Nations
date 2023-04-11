namespace osg {

    public class Inventory {
        private int numWood = 0;
        private int numStone = 0;

        public void addWood(int amount) {
            numWood += amount;
        }

        public int getNumWood() {
            return numWood;
        }

        public void setNumWood(int numWood) {
            this.numWood = numWood;
        }

        public void removeWood(int amount) {
            numWood -= amount;
        }

        public void addStone(int amount) {
            numStone += amount;
        }

        public int getNumStone() {
            return numStone;
        }

        public void setNumStone(int numStone) {
            this.numStone = numStone;
        }

        public void removeStone(int amount) {
            numStone -= amount;
        }
    }
}