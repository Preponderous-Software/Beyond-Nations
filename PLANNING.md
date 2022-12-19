# Planning Document
## General Goals
- Provide a reasonably-sized 3D virtual world that provokes curiosity and a sense of exploration.
- Allow the player to visit a number of locations containing a number of NPCs.
- Allow the player to perform location-specific actions and make performing actions cause time to pass.
- Allow multiple players to cooperatively experience the game.
- Allow users in the community to easily mod the game.
- Ensure that the game is cross-platform (Windows/UNIX-based)

## Setting
The game will take place in a medieval island town with nearby landmarks like forests/caves.

### Locations
- [ ] Player Home
- [ ] Town Square
- [ ] Docks
- [ ] Coastline
- [ ] General Store
- [ ] Credit Union / Bank
- [ ] Tavern
- [ ] Forest
- [ ] Cave

### Actions
- Player Home
  - sleep
  - go to town square
- Town Square
  - talk to townsfolk
  - go to player home
  - go to the docks
  - go to the general store
  - go to the credit union / bank
  - go to the tavern
  - go to the forest
- Docks
  - talk to townsfolk
  - go to town square
  - go to coastline
- Coastline
  - talk to fishermen
  - go to docks
  - go to forest
- General Store
  - talk to shopkeeper
  - go to town square
- Credit Union / Bank
  - talk to teller
  - go to town square
- Tavern
  - talk to patrons
  - gamble
  - go to town square
- Forest
  - cut lumber
  - go to town square
  - go to coastline
- Cave
  - go to forest
 
## Tech Stack
- C# (object code)
- Unity (graphics)
- SQL or JSON (data persistence)

## Elements Ordered By Least To Most Complex
1. Menus
1. Movement
1. Sounds
1. Music
1. Combat
1. Quests
1. Data Persistence
1. Multiplayer Mode

## Inspiration
The inspiration for this project is [FishE](https://github.com/Stephenson-Software/FishE), a simple text-adventure created in 2016.
