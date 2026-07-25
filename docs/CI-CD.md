# Continuous Integration (CI) Documentation

This document describes the CI/CD pipeline for the Beyond Nations Unity project.

## Overview

The Beyond Nations project uses GitHub Actions for continuous integration to ensure code quality, project stability, and build consistency. The CI pipeline automatically runs on all pull requests and pushes to the main, master, or develop branches.

## CI Workflow

The CI pipeline consists of three main jobs:

### 1. Meta File Consistency Check
**Purpose**: Ensures all Unity assets have corresponding `.meta` files and no orphaned `.meta` files exist.

**What it checks**:
- Every file and directory in `Assets/` has a `.meta` file
- No `.meta` files exist without their corresponding asset

**Note**: Unity does not create `.meta` files for `ProjectSettings/` files, so they are not checked.

**Why it matters**: Missing or orphaned `.meta` files can cause:
- GUID conflicts between developers
- Asset reference breakage
- Merge conflicts that are difficult to resolve

### 2. Unity Tests
**Purpose**: Validates that the project's test infrastructure is properly configured.

**Current state**:
- The project uses a **custom test framework** based on `Debug.Assert` (see `Assets/Scripts/Tests/Tests.cs`)
- These custom tests are **not compatible** with Unity Test Framework (NUnit-based)
- The CI currently runs `game-ci/unity-test-runner` which expects Unity Test Framework tests
- If no Unity Test Framework tests are found, the runner will report 0 tests and pass

**What it checks**:
- Unity Test Framework infrastructure is functional
- No compilation errors prevent test execution
- Unity license activation is working

**Test location**: Custom tests are located in `Assets/Scripts/Tests/`

**Note**: To have the CI actually run the existing custom tests, they would need to be either:
1. Migrated to Unity Test Framework with proper `[Test]` attributes and NUnit assertions, or
2. Executed via a custom script that calls `Tests.runTests()` in batch mode

### 3. Build Validation
**Purpose**: Verifies the project can be built successfully for target platforms.

**Target platforms**:
- StandaloneLinux64 (default)

**What it checks**:
- Project compiles without errors
- No missing script references
- All required assets are included in the build
- Build completes successfully without errors

## Unity Version

The project uses **Unity 2022.3.7f1** (LTS).

All CI jobs use this exact version to ensure consistency. If you need to update the Unity version:

1. Update the version in `ProjectSettings/ProjectVersion.txt`
2. Update the `unityVersion` parameter in `.github/workflows/unity-ci.yml`
3. Test locally before committing

## Required Secrets

The CI workflow requires Unity license credentials to be configured as GitHub repository secrets:

- `UNITY_LICENSE`: Your Unity license file content (for Personal license)
- `UNITY_EMAIL`: Unity account email (for Professional/Plus license activation)
- `UNITY_PASSWORD`: Unity account password (for Professional/Plus license activation)

### Setting Up Unity License

#### For Personal License (Free):

1. Install Unity Hub and Unity Editor 2022.3.7f1 locally
2. Activate your personal license in Unity
3. Find your license file:
   - **Windows**: `C:\ProgramData\Unity\Unity_lic.ulf`
   - **macOS**: `/Library/Application Support/Unity/Unity_lic.ulf`
   - **Linux**: `~/.local/share/unity3d/Unity/Unity_lic.ulf`
4. Copy the entire contents of the license file
5. In your GitHub repository, go to Settings → Secrets and variables → Actions
6. Create a new secret named `UNITY_LICENSE` and paste the license content

#### For Professional/Plus License:

1. In your GitHub repository, go to Settings → Secrets and variables → Actions
2. Create secrets:
   - `UNITY_EMAIL`: Your Unity ID email
   - `UNITY_PASSWORD`: Your Unity ID password

**Note**: The first run will take longer as Unity needs to be installed and activated.

## Running Checks Locally

You can run equivalent checks on your local machine before pushing code.

### Check for Missing .meta Files

```bash
# Check Assets folder for missing .meta files
# Uses null-terminated strings to handle filenames with spaces/special characters
find Assets -type f -o -type d | grep -v "\.meta$" | while IFS= read -r file; do
  if [[ "$file" != "Assets" ]] && [ ! -f "$file.meta" ]; then
    echo "Missing .meta: $file"
  fi
done

# Check for orphaned .meta files
find Assets -name "*.meta" -type f | while IFS= read -r metafile; do
  original="${metafile%.meta}"
  if [ ! -e "$original" ]; then
    echo "Orphaned .meta: $metafile"
  fi
done
```

**Note**: For files with special characters in names, the CI uses a more robust approach with null-terminated strings (`tr '\n' '\0'` and `read -d ''`). The above simplified version works for most cases but may have issues with unusual filenames.

### Run Unity Tests Locally

**Note**: The project currently uses a custom test framework, not Unity Test Framework.

#### Running Custom Tests

The existing tests in `Assets/Scripts/Tests/` use `Debug.Assert` and need to be manually triggered:

1. Open the project in Unity Editor 2022.3.7f1
2. In the Unity Console, call: `beyondnationstests.Tests.runTests()`
3. Check the Console for any assertion failures

#### Running Unity Test Framework Tests (if migrated)

If/when tests are migrated to Unity Test Framework:

1. Open the project in Unity Editor 2022.3.7f1
2. Go to **Window → General → Test Runner**
3. Select **EditMode** tab
4. Click **Run All** to run all tests
5. Ensure all tests pass (green checkmarks)

Alternatively, run via command line:

```bash
# For macOS/Linux
/Applications/Unity/Hub/Editor/2022.3.7f1/Unity.app/Contents/MacOS/Unity \
  -batchmode \
  -nographics \
  -projectPath . \
  -runTests \
  -testPlatform EditMode \
  -testResults ./test-results.xml \
  -logFile -

# For Windows
"C:\Program Files\Unity\Hub\Editor\2022.3.7f1\Editor\Unity.exe" ^
  -batchmode ^
  -nographics ^
  -projectPath . ^
  -runTests ^
  -testPlatform EditMode ^
  -testResults ./test-results.xml ^
  -logFile -
```

### Build Project Locally

1. Open the project in Unity Editor 2022.3.7f1
2. Go to **File → Build Settings**
3. Select **Linux** (or your target platform)
4. Click **Build** to ensure the project builds without errors

Alternatively, build via command line:

```bash
# For macOS/Linux
/Applications/Unity/Hub/Editor/2022.3.7f1/Unity.app/Contents/MacOS/Unity \
  -batchmode \
  -nographics \
  -projectPath . \
  -buildLinux64Player ./build/StandaloneLinux64/BeyondNations \
  -quit \
  -logFile -

# For Windows
"C:\Program Files\Unity\Hub\Editor\2022.3.7f1\Editor\Unity.exe" ^
  -batchmode ^
  -nographics ^
  -projectPath . ^
  -buildLinux64Player ./build/StandaloneLinux64/BeyondNations ^
  -quit ^
  -logFile -
```

## CI Workflow Triggers

The CI pipeline runs automatically on:

- **Pull Requests**: All pull requests targeting `main`, `master`, or `develop` branch
- **Push Events**: Direct pushes to `main`, `master`, or `develop` branch

The workflow uses concurrency control to automatically cancel previous runs when new commits are pushed to the same branch.

## Viewing CI Results

### On Pull Requests

1. Navigate to your pull request on GitHub
2. Scroll to the "Checks" section at the bottom
3. Click on "Unity CI" to see detailed results
4. Each job (meta files, tests, build) will show as passed or failed
5. Click on individual jobs to see detailed logs

### On the Actions Tab

1. Go to the "Actions" tab in the GitHub repository
2. Click on "Unity CI" in the left sidebar
3. View all workflow runs and their status
4. Click on any run to see detailed job results

### Artifacts

Test results and builds are uploaded as artifacts:

- **Test Results**: Available even if tests fail
- **Build Artifacts**: Successfully built project binaries

To download artifacts:
1. Navigate to the workflow run
2. Scroll to the "Artifacts" section
3. Click on the artifact name to download

## Troubleshooting

### CI Fails with "Missing .meta file"

**Cause**: A file or folder was added to the project without its corresponding `.meta` file.

**Solution**:
1. Open the project in Unity Editor
2. Unity will automatically generate missing `.meta` files
3. Commit the new `.meta` files
4. Push your changes

### CI Fails with "Orphaned .meta file"

**Cause**: A file was deleted but its `.meta` file was not deleted.

**Solution**:
1. Identify the orphaned `.meta` file from the CI logs
2. Delete the orphaned `.meta` file
3. Commit and push

### CI Fails with "Unity Activation Failed"

**Cause**: Unity license secrets are not configured or are invalid.

**Solution**:
1. Verify that `UNITY_LICENSE` (or `UNITY_EMAIL`/`UNITY_PASSWORD`) secrets are set correctly
2. Ensure the license is valid and not expired
3. For Personal licenses, regenerate the license file if needed

### CI Fails on Tests

**Cause**: Tests are failing due to code changes.

**Solution**:
1. Run tests locally using the Test Runner (Window → General → Test Runner)
2. Fix failing tests
3. Ensure all tests pass locally before pushing

### CI Fails on Build

**Cause**: Project has compilation errors or missing references.

**Solution**:
1. Open the project in Unity Editor
2. Check the Console for any errors (Window → General → Console)
3. Fix all errors
4. Verify the project builds locally (File → Build Settings → Build)
5. Commit and push fixes

## Best Practices

1. **Run tests locally** before pushing code
2. **Always commit .meta files** when adding new assets
3. **Don't modify .meta files manually** unless absolutely necessary
4. **Wait for CI to pass** before merging pull requests
5. **Keep Unity version consistent** across all developers
6. **Review CI logs** when checks fail to understand the issue

## Extending the CI Pipeline

### Adding New Target Platforms

To add additional build targets (e.g., Windows, macOS):

1. Edit `.github/workflows/unity-ci.yml`
2. Add new platform to the matrix:
   ```yaml
   matrix:
     targetPlatform:
       - StandaloneLinux64
       - StandaloneWindows64  # Add Windows
       - StandaloneOSX        # Add macOS
   ```

### Adding Code Quality Checks

Consider adding:
- **Code linting**: C# code style checks
- **Asset validation**: Check texture sizes, audio formats
- **Scene validation**: Ensure required scenes are configured

### Performance Testing

Add performance benchmarks to ensure the game maintains target frame rates.

## Further Reading

- [GameCI Documentation](https://game.ci/docs) - Official documentation for Unity CI/CD
- [Unity Manual - Command Line Arguments](https://docs.unity3d.com/Manual/CommandLineArguments.html)
- [GitHub Actions Documentation](https://docs.github.com/en/actions)
