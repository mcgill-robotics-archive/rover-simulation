/*
This message class is generated automatically with 'SimpleMessageGenerator' of ROS#
*/

using System.Runtime.CompilerServices;
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
    /// TYPE CODE: 0x00
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 6 * sizeof(sbyte))]
    public unsafe struct ArmMotorCommand : IMessage, IBlittable<ArmMotorCommand>
    {
        [FieldOffset(0)]
        public fixed sbyte MotorVel[6];

        public byte TypeCode => 0x00;

        public bool IsManaged => false;
    }
}