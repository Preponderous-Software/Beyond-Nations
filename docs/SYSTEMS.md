# Systems
The game will consist of the following systems:
- Event
- World Generation
- Inventory
- Nation
- Resource Collection
- Crafting
- Territory
- Building
- Citizen

**Note:** This document is a work in progress. It will be updated as the project progresses and more systems are planned/implemented.

## Event
The event system will be used to manage events that occur in the game. Events will be used to trigger actions and update the state of the game. Events will be triggered by the player, the world, or other systems. Events will be able to trigger other events. Events will be able to be subscribed to by other systems. Events will be able to be queued and executed at a later time. We will use Kafka under the hood for this system.

## World Generation
The world generation system will be used to create a procedurally generated world. The world will be populated with biomes, terrain, and other features.

## Inventory
The inventory system will be used to manage items and resources. Players will be able to collect items and resources from the world and use them to craft new items.

## Nation
The nation system will be used to create and manage nations. Each nation will have its own unique culture and history. Players will be able to create their own nation or join an existing one. Nations will be able to claim territory, form alliances, and engage in battles with other nations.

## Resource Collection
Players will be able to collect resources from the world. These resources will be used to craft items and build structures.

## Crafting
The crafting system will be used to create items from resources. Players will be able to craft items using the resources they collect from the world.

## Territory
The territory system will be used to create and manage territories. Territories will be used to claim land and build structures.

## Building
The building system will be used to create and manage buildings. Buildings will be used to provide shelter and storage for citizens.

## Citizen
The citizen system will be used to create and manage citizens. Citizens will be used to populate nations and territories. Citizens will have their own unique personalities and will be able to interact with each other, the player, and the world.