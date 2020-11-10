/*
This message class is generated automatically with 'SimpleMessageGenerator' of ROS#
*/

using Newtonsoft.Json;
using RosSharp.RosBridgeClient.Messages.Geometry;
using RosSharp.RosBridgeClient.Messages.Navigation;
using RosSharp.RosBridgeClient.Messages.Sensor;
using RosSharp.RosBridgeClient.Messages.Standard;
using RosSharp.RosBridgeClient.Messages.Actionlib;
using Rover;
using Rover.Util.IOStream;

namespace RosSharp.RosBridgeClient.Messages
{
    /// <summary>
    /// TYPE CODE: 0x03
    /// </summary>
    public class WheelSpeed : Message, ISerializable
    {
        public static readonly byte TYPE_CODE = 0x03;
        [JsonIgnore] public const string RosMessageName = "DriveControl/WheelSpeed";

        public float[] Wheel_Speed;

        public WheelSpeed()
        {
            Wheel_Speed = new float[2];
        }

        public void Serialize(ByteArrayOutputStream ostream)
        {
            ostream.Write(TYPE_CODE);
            ostream.WriteArray(Wheel_Speed);
        }

        public void Deserialize(ByteArrayInputStream istream)
        {
            Wheel_Speed = istream.ReadArray<float>();
        }
    }
}