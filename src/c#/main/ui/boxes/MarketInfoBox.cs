using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace beyondnations {
    public class MarketInfoBox : InfoBox {
        private Market market;
        private EntityRepository entityRepository;
        private int numDataPoints = 5;

        public MarketInfoBox(int padding, int width, int height, int x, int y, string title, Market market, EntityRepository entityRepository) : base(padding, width, height, x, y, title) {
            this.market = market;
            this.entityRepository = entityRepository;
        }

        public override void draw() {
            int initialY = y;
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

            // list of names of merchants
            List<string> merchantNames = new List<string>();
            foreach (Stall stall in market.getStalls()) {
                if (stall.getOwnerId() == null) {
                    continue;
                }
                Entity owner = entityRepository.getEntity(stall.getOwnerId());
                if (owner is Player) {
                    Player player = (Player) owner;
                    merchantNames.Add("You");
                }
                else if (owner is Pawn) {
                    Pawn pawn = (Pawn) owner;
                    merchantNames.Add(pawn.getName());
                }
                else {
                    merchantNames.Add(owner.getType().ToString());
                }
            }

            // draw list of names of merchants
            if (merchantNames.Count > 0) {
                GUI.Label(new Rect(x, y, width, height), "Merchants:");
                numDataPoints++;
                y += height;
                foreach (string name in merchantNames) {
                    GUI.Label(new Rect(x, y, width, height), "- " + name);
                    y += height;
                }
                numDataPoints += merchantNames.Count;
            }

            // draw box with padding
            GUI.Box(new Rect(x - 10, initialY - 10, width + 20, (height * (numDataPoints + 2))), title);
        }
    } 
}