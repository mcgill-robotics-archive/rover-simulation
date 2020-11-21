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
using roverstd;

namespace RosSharp.RosBridgeClient.Messages
{
    /// <summary>
    /// TYPE CODE: 0x03
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 2 * sizeof(float))]
    public unsafe struct WheelSpeed : IMessage, IBlittable<WheelSpeed>
    {
        public byte TypeCode => 0x03;

        public bool IsManaged => false;

        [FieldOffset(0)]
        public fixed float WheelSpeeds[2];
    }
}