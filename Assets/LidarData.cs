/*
This message class is generated automatically with 'SimpleMessageGenerator' of ROS#
*/ 

using Newtonsoft.Json;
using RosSharp.RosBridgeClient.Messages.Geometry;
using RosSharp.RosBridgeClient.Messages.Navigation;
using RosSharp.RosBridgeClient.Messages.Sensor;
using RosSharp.RosBridgeClient.Messages.Standard;
using RosSharp.RosBridgeClient.Messages.Actionlib;

namespace RosSharp.RosBridgeClient.Messages
{
public class LidarData : Message
{
[JsonIgnore] public const string RosMessageName = "lidar/LidarData";

public float[] distances;
public float angle;

public LidarData()
{
distances = new float[0];
angle = new float();
}
}
}

