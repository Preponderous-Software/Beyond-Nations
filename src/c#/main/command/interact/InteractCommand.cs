using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace osg {

    public class InteractCommand {
        private Environment environment;
        private NationRepository nationRepository;
        private EventProducer eventProducer;
        private EntityRepository entityRepository;

        public InteractCommand(Environment environment, NationRepository nationRepository, EventProducer eventProducer, EntityRepository entityRepository) {
            this.environment = environment;
            this.nationRepository = nationRepository;
            this.eventProducer = eventProducer;
            this.entityRepository = entityRepository;
        }

        public void execute(Player player) {
            AppleTree tree = environment.getNearestTree(player.getGameObject().transform.position);
            Rock rock = environment.getNearestRock(player.getGameObject().transform.position);
            Pawn pawn = (Pawn) environment.getNearestEntityOfType(player.getGameObject().transform.position, EntityType.PAWN);
            Settlement settlement = (Settlement) environment.getNearestEntityOfType(player.getGameObject().transform.position, EntityType.SETTLEMENT);

            if (settlement != null && Vector3.Distance(player.getGameObject().transform.position, settlement.getGameObject().transform.position) < 5) {
                List<string> possibleStrings = new List<string>();
                
                int numCurrentlyPresentEntities = settlement.getCurrentlyPresentEntitiesCount();
                if (numCurrentlyPresentEntities == 0) {
                    possibleStrings.Add("This settlement is empty.");
                }
                else {
                    possibleStrings.Add("This settlement is occupied by " + numCurrentlyPresentEntities + " pawns at the moment.");
                }

                int numStalls = settlement.getMarket().getNumStalls();
                if (numStalls == 0) {
                    possibleStrings.Add("The market is barren. No stalls have been built.");
                }
                else {
                    int numStallsForSale = settlement.getMarket().getNumStallsForSale();
                    if (numStallsForSale == 0) {
                        possibleStrings.Add("The market is full. All " + numStalls + " stalls have been built and are owned by someone.");
                    }
                    else {
                        possibleStrings.Add("The market has " + numStallsForSale + "/" + numStalls + " stalls for sale.");
                    }
                }

                int numCoins = settlement.getInventory().getNumItems(ItemType.COIN);
                if (numCoins == 0) {
                    possibleStrings.Add("This settlement is destitute. It has no coins.");
                }
                else {
                    possibleStrings.Add("The wealth of this settlement amounts to " + numCoins + " coins.");
                }

                int numItemsBought = settlement.getMarket().getTotalNumItemsBought();
                if (numItemsBought == 0) {
                    possibleStrings.Add("No items have been bought at this settlement's market.");
                }
                else {
                    possibleStrings.Add(numItemsBought + " items have been bought at this settlement's market.");
                }

                int numItemsSold = settlement.getMarket().getTotalNumItemsSold();
                if (numItemsSold == 0) {
                    possibleStrings.Add("No items have been sold at this settlement's market.");
                }
                else {
                    possibleStrings.Add(numItemsSold + " items have been sold at this settlement's market.");
                }

                string statusUpdate = possibleStrings[UnityEngine.Random.Range(0, possibleStrings.Count)];
                player.getStatus().update(statusUpdate);
            }
            else if (pawn != null && Vector3.Distance(player.getGameObject().transform.position, pawn.getGameObject().transform.position) < 5) {
                Nation pawnsNation = nationRepository.getNation(pawn.getNationId());

                List <string> phrases = generatePhrases(pawnsNation, pawn, player);
                string phrase = phrases[UnityEngine.Random.Range(0, phrases.Count)];
                player.getStatus().update(pawn.getName() + ": \"" + phrase + "\"");
            }
            else if (tree != null && Vector3.Distance(player.getGameObject().transform.position, tree.getGameObject().transform.position) < 5) {
                tree.markForDeletion();
                player.getInventory().transferContentsOfInventory(tree.getInventory());
                player.getStatus().update("Gathered wood from tree.");
            }
            else if (rock != null && Vector3.Distance(player.getGameObject().transform.position, rock.getGameObject().transform.position) < 5) {
                rock.markForDeletion();
                player.getInventory().transferContentsOfInventory(rock.getInventory());
                player.getStatus().update("Gathered stone from rock.");
            }
            else {
                player.getStatus().update("No entities within range to interact with.");
            }
        }

        private List<string> generatePhrases(Nation pawnsNation, Pawn pawn, Player player) {
            List <string> phrases = new List<string>() {
                "Hello!",
                "How are you?",
                "Glory to " + pawnsNation.getName() + "!",
                "What's your name?",
                "I'm " + pawn.getName() + ".",
                "Have you heard of " + pawnsNation.getName() + "?",
                "Nice to meet you!",
                "I'm hungry.",
                "The weather is nice today."
            };
            if (pawnsNation.getNumberOfMembers() > 1) {
                phrases.Add("There are " + pawnsNation.getNumberOfMembers() + " members in " + pawnsNation.getName() + ".");

                if (pawnsNation.getLeaderId() == player.getId()) {
                    phrases.Add("Hey boss!");
                    phrases.Add("How's it going boss?");
                    phrases.Add("What's up boss?");
                    phrases.Add("I'm glad to be a member of " + pawnsNation.getName() + ".");
                    phrases.Add("Please don't kick me out of " + pawnsNation.getName() + ".");
                    phrases.Add("When are we going to build more houses?");
                }
            }

            return phrases;
        }
    }
}