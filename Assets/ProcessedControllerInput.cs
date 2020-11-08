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
    /// TYPE_CODE 0x02
    /// </summary>
    public class ProcessedControllerInput : Message, ISerializable
    {
        public static readonly byte TYPE_CODE = 0x02;
        [JsonIgnore] public const string RosMessageName = "ArmControl/ProcessedControllerInput";

        public float[] ControllerInput;

        public ProcessedControllerInput()
        {
            ControllerInput = new float[6];
        }

        public void Serialize(ByteArrayOutputStream ostream)
        {
            ostream.Write(TYPE_CODE);
            ostream.Write(ControllerInput.Length);
            foreach (float input in ControllerInput)
            {
                ostream.Write(input);
            }
        }

        public void Deserialize(ByteArrayInputStream istream)
        {
            int lenght = istream.ReadInt();
            ControllerInput = new float[lenght];
            for (int i = 0; i < lenght; i++)
            {
                ControllerInput[i] = istream.ReadFloat();
            }
        }
    }
}
