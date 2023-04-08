namespace osg {

    public class Inventory {
        private int numWood = 0;

        public void addWood(int amount) {
            numWood += amount;
        }

        public int getNumWood() {
            return numWood;
        }

        public void removeWood(int amount) {
            numWood -= amount;
        }
    }
}