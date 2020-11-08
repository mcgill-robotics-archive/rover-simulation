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
    public class WheelSpeed : Message
    {
        [JsonIgnore] public const string RosMessageName = "DriveControl/WheelSpeed";

        public float[] Wheel_Speed;

        public WheelSpeed()
        {
            Wheel_Speed = new float[2];
        }
    }
}