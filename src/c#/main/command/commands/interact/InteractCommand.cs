using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace osg {

    public class InteractCommand {
        private Environment environment;
        private NationRepository nationRepository;
        private EventProducer eventProducer;

        public InteractCommand(Environment environment, NationRepository nationRepository, EventProducer eventProducer) {
            this.environment = environment;
            this.nationRepository = nationRepository;
            this.eventProducer = eventProducer;
        }

        public void execute(Player player) {
            Status status = player.getStatus();

            TreeEntity tree = environment.getNearestTree(player.getGameObject().transform.position);
            RockEntity rock = environment.getNearestRock(player.getGameObject().transform.position);
            Pawn pawn = (Pawn) environment.getNearestEntityOfType(player.getGameObject().transform.position, EntityType.PAWN);

            // if within range of tree or rock, gather resources
            if (tree != null && Vector3.Distance(player.getGameObject().transform.position, tree.getGameObject().transform.position) < 5) {
                tree.markForDeletion();
                player.getInventory().transferContentsOfInventory(tree.getInventory());
                status.update("Gathered wood from tree.");
            }
            else if (rock != null && Vector3.Distance(player.getGameObject().transform.position, rock.getGameObject().transform.position) < 5) {
                rock.markForDeletion();
                player.getInventory().transferContentsOfInventory(rock.getInventory());
                status.update("Gathered stone from rock.");
            }
            else if (pawn != null && Vector3.Distance(player.getGameObject().transform.position, pawn.getGameObject().transform.position) < 5) {
                Nation pawnsNation = nationRepository.getNation(pawn.getNationId());
                EntityId randomMemberId = pawnsNation.getRandomMemberId();

                // make pawn say random phrase to player
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
                if (player.getNationId() != null) {
                    Nation playersNation = nationRepository.getNation(player.getNationId());
                    if (playersNation.getLeaderId() == pawn.getId()) {
                        // sell items to pawn
                        int numWood = player.getInventory().getNumItems(ItemType.WOOD);
                        int numStone = player.getInventory().getNumItems(ItemType.STONE);
                        int numApples = player.getInventory().getNumItems(ItemType.APPLE);

                        int cost = (numWood * 5) + (numStone * 10) + (numApples * 1);

                        if (cost == 0) {
                            status.update("You don't have anything to sell.");
                            return;
                        }

                        if (pawn.getInventory().getNumItems(ItemType.GOLD_COIN) < cost) {
                            status.update(pawn.getName() + " doesn't have enough gold coins to buy your items.");
                            return;
                        }

                        player.getInventory().removeItem(ItemType.WOOD, numWood);
                        player.getInventory().removeItem(ItemType.STONE, numStone);
                        player.getInventory().removeItem(ItemType.APPLE, numApples);
                        pawn.getInventory().addItem(ItemType.WOOD, numWood);
                        pawn.getInventory().addItem(ItemType.STONE, numStone);
                        pawn.getInventory().addItem(ItemType.APPLE, numApples);
                        player.getInventory().addItem(ItemType.GOLD_COIN, cost);
                        status.update("Sold " + numWood + " wood, " + numStone + " stone, and " + numApples + " apples to " + pawn.getName() + " for " + cost + " gold coins.");
                        return;
                    }
                }
                string phrase = phrases[Random.Range(0, phrases.Count)];
                status.update(pawn.getName() + ": \"" + phrase + "\"");
            }
            else {
                status.update("No entities within range to interact with.");
            }
        }
    }
}