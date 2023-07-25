using UnityEngine;
using UnityEngine.UI;

namespace osg {
    public class MarketInfoBox : InfoBox {
        private Market market;
        private int numDataPoints = 5;

        public MarketInfoBox(int padding, int width, int height, int x, int y, string title, Market market) : base(padding, width, height, x, y, title) {
            this.market = market;
        }

        public override void draw() {
            // draw box with padding
            GUI.Box(new Rect(x - 10, y - 10, width + 20, (height * (numDataPoints + 2))), title);
            y += 10;

            // draw num stalls
            GUI.Label(new Rect(x, y, width, height), "Stalls: " + market.getNumStalls() + "/" + market.getMaxNumStalls());
            y += height;
            
            // draw num stalls for sale
            GUI.Label(new Rect(x, y, width, height), "Stalls for Sale: " + market.getNumStallsForSale());
            y += height;

            // draw num coins
            GUI.Label(new Rect(x, y, width, height), "Total Coins: " + market.getTotalCoins());
            y += height;

            // draw num items bought
            GUI.Label(new Rect(x, y, width, height), "Items Bought: " + market.getTotalNumItemsBought());
            y += height;

            // draw num items sold
            GUI.Label(new Rect(x, y, width, height), "Items Sold: " + market.getTotalNumItemsSold());
            y += height;
        }
    } 
}