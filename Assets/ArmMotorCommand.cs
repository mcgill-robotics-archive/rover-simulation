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
    public class ArmMotorCommand : Message, ISerializable
    {
        [JsonIgnore] public const string RosMessageName = "ArmControl/ArmMotorCommand";

        public int[] MotorVel;

        public ArmMotorCommand()
        {
            MotorVel = new int[6];
        }

        public void Serialize(ByteArrayOutputStream ostream)
        {
            throw new System.NotImplementedException();
        }

        public void Deserialize(ByteArrayInputStream istream)
        {
            throw new System.NotImplementedException();
        }
    }
}