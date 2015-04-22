##XBuilder Project (Working)

The XBuilder Project is my senior project. I am in charge of working on the Unity3D side. A summary of the project is that we are attempting to create a game where users can build various toys with lego-like blocks. Because this is all virtual, our rockets blocks are not merely red plastic pieces with paint on them: they are functional rockets. In this way we are attempting to create a sandbox enviroment where users are free to play around and build automata.

Various build have been implemented as of now and some are still planned.

Current tasks that are at a functional state tasks:

* Yumascript: A simple script that users will use to program the behaviour of their automata *(Interpreter.cs)*

* Block Generation: Turns a matrix of integers into an XBuild in the scene *(BlockObjectScript.cs)*

* Basic Physics Implementations: Different Rocket blocks - depending on their position relative to the center of the xbuild *(yet to implement center of gravity calculator)* will result in both **force** and **torque** 
    **Physics/BlockPhysics.cs*
    **RootScript.cs*

* Rocket Block: The rocket blocks apply force 

* Missile Block: Fires a projectile

* Radar: Is not a block, maintains a list of objects within a certain radius
