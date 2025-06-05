# Exam-project

## Overview

This project is a 2D vertical platformer game developed in Unity. It showcases gameplay mechanics such as movement, power ups, and weapon use, and was created as part of an exam project.

## Features

- Smooth character movement
- Jumping mechanics
- Weapon usage
- Custom shaders and visual effects
- Developed with Unity using C#, ShaderLab, and HLSL

## Setup

To use the repository and access all its contents, it is **necessary** to install and set up Git LFS (Large File Storage).  
Follow the instructions here: [GitHub - Installing Git Large File Storage](https://docs.github.com/en/repositories/working-with-files/managing-large-files/installing-git-large-file-storage)

## Installation

1. Clone the repository:
    ```bash
    git clone https://github.com/lea10k/Exam-project.git
    ```
2. Open the project in [Unity Editor](https://unity.com/).
3. Select the MainMenu scene at Assets/Scenes.
4. Press the **Play** button in the Unity Editor to start the game.

## Requirements

- Unity (specify version, e.g., 2021.3 or later)
- Git LFS extention

## How to Execute the Built File on MacOS that was created on Windows

> **Note:** The built file for Mac cannot be compressed as a ZIP file. If you do, the game will no longer be executable. Therefor, a TAR file is the best solution. 

1. Decompress the TAR file.
2. Open a terminal and enter:
    ```bash
    cd built.app/Contents/MacOS
    ```
    You may need to adjust the command to navigate to the “MacOS” directory, depending on where you saved the TAR file.
3. Enter:
    ```bash
    chmod -R 755 game
    ```
4. Now you can double-click on the built application and the game should run. Force execute the file if your anti-virus program blocks the execution.

## How to Play
- **Move Left:** Press the **Left Arrow** key
- **Move Right:** Press the **Right Arrow** key
- **Jump:** Press the **Space Bar**
  > **Note:** The space bar should be pressed harder and longer to reach other objects properly. 
- **Use Weapon:** Press the **E** key
- **Reload Level:** Press **R**. A short on-screen hint reminds you of this during play.


## Project Structure

```
...
├── Assets
│   ├── Animated effects
│   ├── Art
│   ├── Buttons
│   ├── Chibi Monster Free (Unique Skill Animated Prefab with SFX)
│   ├── DefaultVolumeProfile.asset
│   ├── Editor
│   ├── Fonts
│   ├── MainMenuBG.png
│   ├── Materials
│   ├── Nine Pines Animation
│   ├── Prefabs
│   ├── Presets
│   ├── Resources
│   ├── Scenes
│   ├── Scripts
│   ├── Settings
│   ├── Settings.meta
│   ├── test_scene.unity
│   ├── TextMesh Pro
│   ├── UI
│   ├── UI pack
│   ├── UniversalRenderPipelineGlobalSettings.asset
│   ├── Warped Shooting Fx
...
```

## Credits

- Developed by [lea10k](https://github.com/lea10k)
- Developt by [EfusRyuga](https://github.com/EfusRyuga)
