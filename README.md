<p align="center">
    <img src="Resources~/Readme/cav-logo.png" alt="Cavrnus Logo window"/>
</p>

# <p style="text-align: center;">Cavrnus Spatial Connector: Unity Game Project</p>

<h4 align="center">
  <a href="https://www.cavrnus.com/">
    <img src="https://img.shields.io/badge/Cavrnus%20Website-label?style=flat&color=white" alt="cavrnus" style="height: 20px;">
  </a>
  <a href="https://www.youtube.com/@cavrnus">
    <img src="https://img.shields.io/badge/Cavrnus%20YouTube-label?style=flat&logo=YouTube&logoColor=red&labelColor=white&color=white" alt="youtube" style="height:20px;">
  </a>
  <a href="https://twitter.com/cavrnus">
    <img src="https://img.shields.io/badge/Cavrnus_Twitter-label?style=flat&logo=x&logoColor=black&labelColor=white&color=white" alt="twitter" style="height: 20px;">
  </a>
  <a href="https://discord.gg/AzgenDT7Ez">
    <img src="https://img.shields.io/badge/Cavrnus_Support-label?style=flat&logo=discord&labelColor=white&color=white" alt="discord" style="height: 20px;">
  </a>
</h4>

### Getting Started
Welcome to the Cavrnus Unity Game Project! This Unity project leverages the [Cavrnus Spatial Connector](https://cavrnus.atlassian.net/wiki/spaces/CSM/overview) to provide essential tools for building multiplayer games with persistence.

Follow these steps to get started:

1. **Install Unity**  
   Download and install **Unity version 2022.3.50f1 (URP)** from the [Unity Hub](https://unity.com/download).

2. **Clone or Download the Project**  
   Clone the repository from your source control system or download the project files as a ZIP.

3. **Open the Project in Unity**  
   - Launch Unity Hub.
   - Click the **Add** button and select the downloaded project folder.
   - Open the project with Unity 2022.3.50f1.

4. **Check URP Settings**  
   - Ensure the project is set up with the **Universal Render Pipeline (URP)**.
   - Verify the URP settings under **Edit > Project Settings > Graphics**.

5. **Select CavrnusMultiplayerGameSample Scene**
    - Open scene located in Assets/CavrnusMultiplayerGameSample

6. **Setup Cavrnus Spatial Connector**
    - The default scene already includes a Cavrnus Spatial Connector, but server settings and member login info must be setup. 
    - For in-depth help with setting up the Cavrnus Spatial Connector prefab, see [Setup Your Scene](https://cavrnus.atlassian.net/wiki/spaces/CSM/pages/827916295/Setup+Your+Scene) documentation.

7. **Run the Project** 
   - Press **Play** in the Unity Editor to run the scene.

## Feature Guide

### **Avatar Switching** 
Enable users to switch between avatars during gameplay, with updates reflected for all connected players using player metadata

<img src="Resources~/Readme/avatar-selection.png" alt="Avatar Selection Image"/>

### **Avatar Emotes** 
Synchronize avatar emotes across all participants in real time, allowing users to express themselves with gestures, animations, or predefined emote actions.

<img src="Resources~/Readme/emotes.png" alt="Emotes Image"/>

### **Environment Properties** 
Customize and synchronize environmental settings like lighting, skyboxes, and interactive objects to create a consistent shared experience.

<img src="Resources~/Readme/environment.png" alt="Environment Properties Image"/>

### **Level Switching** 
Seamlessly switch between levels or scenes while maintaining user copresence and synchronization.

<img src="Resources~/Readme/level.png" alt="Level Swiching Image"/>

## Full API Reference
The Cavrnus Spatial Connector includes an [API reference](https://cavrnus.atlassian.net/wiki/spaces/CSM/pages/824934449/API+Reference+Unity). Build more custom features to support your project!

## Support and Feedback
Do you need help? Have you found a bug? Reach out through the [Cavrnus Discord](https://discord.gg/AzgenDT7Ez).