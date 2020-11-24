using System.Runtime.InteropServices;
using RosSharp.RosBridgeClient.MessageTypes;
using RosSharp;
using UnityEngine;
using RVector3 = RosSharp.RosBridgeClient.MessageTypes.Geometry.Vector3;
using UVector3 = UnityEngine.Vector3;
using RQuaternion = RosSharp.RosBridgeClient.MessageTypes.Geometry.Quaternion;
using UQuaternion = UnityEngine.Quaternion;


namespace roverstd
{
    public static class CoordinateSystems
    {
        public static UVector3 RosVectorToUnityVector(this RVector3 vec)
        {
            return new UVector3((float) vec.x, (float) vec.y, (float) vec.z);
        }

        public static UVector3 TransformToUnityCoordinates(this RVector3 vec)
        {
            return vec.RosVectorToUnityVector().Ros2Unity();
        }

        public static RVector3 UnityVectorToRosVector(this UVector3 vec)
        {
            return new RVector3(vec.x, vec.y, vec.z);
        }

        public static RVector3 TransformToRosCoordinates(this UVector3 vec)
        {
            return vec.Unity2Ros().UnityVectorToRosVector();
        }

        public static UQuaternion RosQuaternionToUnityQuaternion(this RQuaternion quaternion)
        {
            return new UQuaternion((float)quaternion.x, (float)quaternion.y, (float)quaternion.z, (float)quaternion.w);
        }

        public static UQuaternion TransformToUnityCoordinates(this RQuaternion quaternion)
        {
            return quaternion.RosQuaternionToUnityQuaternion().Ros2Unity();
        }

        public static RQuaternion UnityQuaternionToRosQuaternion(this UQuaternion quaternion)
        {
            return new RQuaternion(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
        }

        public static RQuaternion TransformToRosCoordinates(this UQuaternion quaternion)
        {
            return quaternion.Unity2Ros().UnityQuaternionToRosQuaternion();
        }

    }
}