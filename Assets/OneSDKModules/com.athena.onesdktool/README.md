# One SDK Tool

## Installation
1. In Unity Editor, go to `Window` > `Package Manager`
2. Click on the `+` button, select `Add package from Git URL...`
3. Use this URL: `https://gitlab.com/game_athena/mobile_game/onesdktool.git`
* If you want to lock to a specific version add `#<version>` at the end: `...onesdktool.git#1.0.0`
4. Click the `Add` button to add.
* **NOTE:** You may have to refresh/save project to apply. You may also have to retry.
If you want to upgrade One SDK Tool, do the same step as installation

## Usage
1. In Unity Editor, go to OneSDK > SDK Tool
2. Select the plugin you want to install.

## CAUTION
When removing plugins, shared files being used by multiple plugins will also be removed or left behind depending on the situation.

**Please check your diff to verify the correct files have been removed.**

Known special cases:
* `ExternalDependencyManager` will never be removed.
* `Firebase plugins` share too many files and will be removed almost entirely. If you uninstall 1 `Firebase plugin`, remember to refresh and reinstall the other remaining ones.
