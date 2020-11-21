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
    public float vertical_angle;
    public float yaw;
    public float pitch;
    public float roll;
    public float x;
    public float y;
    public float z;
    

public LidarData()
{
    distances = new float[0];
    vertical_angle = new float();
    yaw = new float();
    pitch = new float();
    roll = new float();
    x = new float();
    y = new float();
    z = new float();
}
}
}

