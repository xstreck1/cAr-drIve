# cAr-drIve
cAr-drIve - reinforcement learning for simulated self-driving car

This is a project that allows to train an AI able to drive alongside any track built from pre-defined pieces.

![Self-tought AI on a race track](https://miro.medium.com/max/800/1*IkRIQavCAPI96PH3XE36xQ.gif)

This is the code for an accompanying [Medium article](http://medium.com/p/60b0e7a10d9e), for details on the content, please check the article.

## Requirements
This project needs the ML-Agents environment version 0.15.0 to be set up. At best I'd suggest to follow the [official installation guide](https://github.com/Unity-Technologies/ml-agents/blob/master/docs/Installation.md). However the following steps should suffice:

1. Get Python 3.6 64-bit (the 64 bit seems necessary, at least on Windows)
2. Get Unity 2019.2+
3. Download the ML-Agents 0.15.0+ at https://github.com/Unity-Technologies/ml-agents/releases
4. Install dependencies: `pip3 install "mlagents<=0.15.0"`
5. Clone this repository
6. Open project in Unity
7. Import the ML-Agents package that you have downloaded.

## Scenes
* **CarTrain** This scene is used for training a model.  
* **CarTest** This scene contains a different track for verification of the trained model.  
* **CarPlay** This scene contains a set of 3 Agents and a car controlled by the player to mimic a game scenario.  
