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
    /// TYPE CODE: 0x01
    /// </summary>
    public class LidarData : Message, ISerializable
    {
        public static readonly byte TYPE_CODE = 0x01;
        [JsonIgnore] public const string RosMessageName = "lidar/LidarData";

        public float[] distances;
        public float angle;

        public LidarData()
        {
            distances = new float[0];
            angle = new float();
        }

        public void Serialize(ByteArrayOutputStream ostream)
        {
            ostream.Write(TYPE_CODE);
            ostream.WriteArray(distances);
            ostream.Write(angle);
        }

        public void Deserialize(ByteArrayInputStream istream)
        {
            distances = istream.ReadArray<float>();
            angle = istream.ReadFloat();
        }
    }
}