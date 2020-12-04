using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using RosSharp.RosBridgeClient.MessageTypes;
using RosSharp;
using RosSharp.RosBridgeClient.MessageTypes.Geometry;
using UnityEngine;
using RVector3 = RosSharp.RosBridgeClient.MessageTypes.Geometry.Vector3;
using UVector3 = UnityEngine.Vector3;
using RQuaternion = RosSharp.RosBridgeClient.MessageTypes.Geometry.Quaternion;
using UQuaternion = UnityEngine.Quaternion;


namespace roverstd
{
    public static class CoordinateSystems
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UVector3 RosVectorToUnityVector(this RVector3 vec)
        {
            return new UVector3((float) vec.x, (float) vec.y, (float) vec.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UVector3 TransformToUnityCoordinates(this RVector3 vec)
        {
            return vec.RosVectorToUnityVector().Ros2Unity();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RVector3 UnityVectorToRosVector(this UVector3 vec)
        {
            return new RVector3(vec.x, vec.y, vec.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RVector3 TransformToRosCoordinates(this UVector3 vec)
        {
            return vec.Unity2Ros().UnityVectorToRosVector();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point32 TransformToRosCoordinatesPoint32(this UVector3 vec)
        {
            UVector3 rosVec = vec.Unity2Ros();
            return new Point32(rosVec.x, rosVec.y, rosVec.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UQuaternion RosQuaternionToUnityQuaternion(this RQuaternion quaternion)
        {
            return new UQuaternion((float)quaternion.x, (float)quaternion.y, (float)quaternion.z, (float)quaternion.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UQuaternion TransformToUnityCoordinates(this RQuaternion quaternion)
        {
            return quaternion.RosQuaternionToUnityQuaternion().Ros2Unity();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RQuaternion UnityQuaternionToRosQuaternion(this UQuaternion quaternion)
        {
            return new RQuaternion(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RQuaternion TransformToRosCoordinates(this UQuaternion quaternion)
        {
            return quaternion.Unity2Ros().UnityQuaternionToRosQuaternion();
        }

    }
}