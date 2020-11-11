/*
This message class is generated automatically with 'SimpleMessageGenerator' of ROS#
*/

using System.Runtime.InteropServices;
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
    [StructLayout(LayoutKind.Explicit, Size = 6 * sizeof(float))]
    public unsafe struct ProcessedControllerInput : IMessage, IBlittable<ProcessedControllerInput>
    {
        public byte TypeCode => 0x02;

        [FieldOffset(0)]
        public fixed float ControllerInput[6];
    }
}
