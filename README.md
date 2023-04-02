# ![Open Source Game](https://raw.githubusercontent.com/Preponderous-Software/osg-project/master/.github/media/banner.svg)
The Open Source Game Project is a collaborative effort between Preponderous Software and the Fairfield Programming Association. It aims to create an open-source game that can serve as a reference for aspiring developers.

## Game Concept
Embark on a journey of exploration and conquest in a vast procedurally generated world featuring multiple nations, each with its own unique culture and history. In this game, players can build their own nation from scratch or join an existing one.

The game's main objective is to claim territory, develop your nation, and ensure a prosperous life for your citizens. Players will have the opportunity to forge alliances and engage in battles with their enemies as they navigate the complex relationships between nations and citizens.

With sandbox-style gameplay that promotes emergent experiences, every playthrough will be a new adventure. Whether players prefer a multiplayer environment or a single-player experience with a single-player nation, this game offers something for everyone.

## Controls
The following controls are available in the game:
---
| Key | Action |
| --- | --- |
| `W` | Move forward |
| `A` | Turn left |
| `S` | Move backward |
| `D` | Turn right |
| `Space` | Jump |
| `Left Shift` | Sprint |
| `N` | Create a new nation |

## Game Systems
There are a number of systems that will be implemented in the game. These systems will be used to create a rich and engaging gameplay experience. Details can be found in the [Systems Document](./docs/SYSTEMS.md).

## Tech Stack
Our game will be built using the following technologies:
- [C#](https://docs.microsoft.com/en-us/dotnet/csharp/): This object-oriented programming language will be used to write the game's code.
- [Unity](https://unity.com/): A popular game engine that provides a framework for designing and developing games.
- [Blender](https://www.blender.org/): A powerful 3D modeling software that will be used to create the game's visual assets.
- [JSON](https://www.json.org/json-en.html): A lightweight data format that will be used to store and exchange game data, providing efficient and reliable data persistence.
- [Git](https://git-scm.com/): A version control system that allows for collaborative development and efficient management of codebase changes.
- [GitHub](https://github.com/): A web-based Git repository hosting service that enables version control and collaboration for developers.
- [Visual Studio Code](https://code.visualstudio.com/): A code editor that supports a wide range of programming languages and offers features such as debugging, syntax highlighting, and extensions.

## Inspiration
Our inspiration for this project comes from the Medieval Factions Minecraft plugin. This plugin enables players to create their own nation, claim territory, and form relations with other nations. The plugin is limited because it is designed for a game that is not created for this kind of gameplay. The objective of our project is to develop a game specifically designed for this kind of gameplay.

## Getting Started
To get started with the project, follow these steps:

1. Open Unity Hub.
1. Click on the New Project button in the top right.
1. Select the 3D template.
1. Name your project and choose where to store it.
1. Select the Unity Editor version (2020.3.21f1 is recommended).
1. Click the Create Project button.
1. Open a terminal and navigate to the Assets directory of the project.
1. Clone this repository using the following command:
    > git clone https://github.com/Preponderous-Software/osg-project
1. In Unity, navigate to the osg-project/src directory, which should now be in the Assets folder.
1. Double click on the main scene to open it.

## Troubleshooting
If you encounter a Missing Editor Version error, it's because you tried to open the repository folder directly as a Unity project. It's recommended to clone the repository inside the Assets folder of an existing empty project.

## Roles
Preponderous Software leads the development of this project and focuses on core elements such as player control, gameplay, story, UI, and more. The Fairfield Programming Association is responsible for community management, deadlines, publishing, press, advertisement, and general assistance with development.
