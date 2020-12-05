# Unity Simulation

### Getting Started
1. Set up ROS Bridge using this tutorial: http://wiki.ros.org/rosbridge_suite/Tutorials/RunningRosbridge
2. Start ROS Bridge Server
3. Take note of the IP address and port that the server is listening to
4. Start the Unity Scene and take note of the directory in which the configuration file is stored (displayed in the debug console)
5. Stop the scene before any exception is thrown, and go to the directory mentioned in the previous point
6. Modify the file so that it contains the IP address and port that the server is listening to e.g. ```ws://localhost:9090```or```ws://192.168.1.12:9090```
7. Start the Scene again and the Unity should be able to 

### IMPORTANT
If you get a "Fatal Error in GC" while running the simulation, please report it to me (Hongjun), and let me know the following:
1. Your platform (Windows/Linux/macOS standalone / Universal Windows Platform)
2. Go to player settings. Are you using IL2CPP or Mono?
3. Whether or not Unity is properly connected to ROS when this error occured

### A few things to look out for
1. Unity Mono uses doubles internally for floating point calculations. Beware of that when you want to inspect the binary representation of floats
2. "Fatal Error in GC" when sending huge messages and/or having a large number of messages waiting to be sent
3. Unity's standard library is not thread safe
4. The physics system does not handle wheel constraints properly (partially solved)
5. If you attempt to build the simulation on windows with IL2CPP and MSVC and turn on optimizations, the compiler will erroneously turn a spinning null check into an infinite loop without the null check due to incorrect assumptions, which prevents the scene from ever loading. (partially fixed)
6. Do not use hot reload. It will very likely crash Unity. If not, it will still crash ROS#.
