# Contributing Guide

## Getting Started
To get started with the project, follow these steps:

1. Clone this repository:
   ```bash
   git clone https://github.com/Preponderous-Software/beyond-nations
   cd beyond-nations
   ```

2. Open Unity Hub.

3. Click "Add" (or "Open") and select the cloned `beyond-nations` directory.

4. Select Unity Editor version **2022.3.7f1** (or compatible version) when prompted.
   - If you don't have this version installed, Unity Hub will prompt you to install it.

5. Once the project opens in Unity, the main scene should load automatically.
   - If it doesn't, navigate to `Assets/Scenes/` in the Project window and double-click `main.unity`.

6. Press the Play button in the Unity Editor to run the game.

### Troubleshooting
- **Missing Editor Version**: If you see this error, install Unity 2022.3.7f1 (or a compatible 2022.3.x version) through Unity Hub.
- **Package Resolution Errors**: Unity will automatically download required packages when you first open the project. This may take a few minutes.
- **Scene Not Loading**: Manually open the scene from `Assets/Scenes/main.unity` in the Project window.

## Branching
To keep the codebase organized, we use a branching model that is similar to Git Flow. The main branch is the default branch, and it contains the latest stable version of the codebase. The develop branch contains the latest version of the codebase, and it is the branch that should be used for development.

## Pull Requests
To contribute to the project, you must create a pull request. A pull request is a request to merge your changes into the develop branch. To create a pull request, follow these steps:
1. Create a new branch from the develop branch.
1. Make your changes.
1. Commit your changes.
1. Push your changes to the remote repository.
1. Create a pull request on GitHub.
1. Wait for your pull request to be reviewed, approved & merged. Comments may be made on your pull request, and you may be asked to make changes.

### Running CI Checks Locally
Before creating a pull request, it's recommended to run the CI checks locally to ensure your changes pass all automated tests. See the [CI/CD Documentation](./CI-CD.md) for detailed instructions on:
- Running Unity tests locally
- Building the project to verify it compiles
- Checking for missing or orphaned .meta files

All pull requests must pass the automated CI checks before they can be merged.

## Issues
If you encounter a bug or have a feature request, you can create an issue. An issue is a way to report a bug or request a feature. To create an issue, follow these steps:
1. Create a new issue on GitHub.
1. Describe the bug or feature request.
1. Wait for the issue to be reviewed and resolved.