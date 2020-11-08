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
    /// TYPE CODE: 0x00
    /// </summary>
    public class ArmMotorCommand : Message, ISerializable
    {
        public static readonly byte TYPE_CODE = 0x00;
        [JsonIgnore] public const string RosMessageName = "ArmControl/ArmMotorCommand";

        public int[] MotorVel;

        public ArmMotorCommand()
        {
            MotorVel = new int[6];
        }

        public void Serialize(ByteArrayOutputStream ostream)
        {
            ostream.Write(TYPE_CODE);
            ostream.Write(MotorVel.Length);
            foreach (int i in MotorVel)
            {
                ostream.Write(i);
            }
        }

        public void Deserialize(ByteArrayInputStream istream)
        {
            int length = istream.ReadInt();
            MotorVel = new int[length];
            for (int i = 0; i < length; i++)
            {
                MotorVel[i] = istream.ReadInt();
            }
        }
    }
}