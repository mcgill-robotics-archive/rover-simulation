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
    public class ProcessedControllerInput : Message
    {
        [JsonIgnore] public const string RosMessageName = "ArmControl/ProcessedControllerInput";

        public float[] ControllerInput;

        public ProcessedControllerInput()
        {
            ControllerInput = new float[6];
        }
    }
}
