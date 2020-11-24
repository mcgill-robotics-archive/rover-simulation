using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using roverstd;
using RosSharp.RosBridgeClient.MessageTypes.Geometry;
using UnityEngine;
using Pose = RosSharp.RosBridgeClient.MessageTypes.Geometry.Pose;
using Quaternion = RosSharp.RosBridgeClient.MessageTypes.Geometry.Quaternion;
using Transform = RosSharp.RosBridgeClient.MessageTypes.Geometry.Transform;
using Vector3 = RosSharp.RosBridgeClient.MessageTypes.Geometry.Vector3;


public class InertialMeasurementUnit : MonoBehaviour
{
    private UnityEngine.Vector3 m_StartingPos;
    public bool OverrideIMU;
    public UnityEngine.Vector3 OutputPosition;
    public UnityEngine.Quaternion OutputRotation;

    public Pose PoseMessage
    {
        get
        {
            if (OverrideIMU)
            {
                Vector3 position = OutputPosition.UnityVectorToRosVector();
                Quaternion rotation = OutputRotation.UnityQuaternionToRosQuaternion();
                return new Pose(new Point(position.x, position.y, position.z), rotation);
            }
            else
            {
                Vector3 position = RosRelativePos;
                OutputPosition = position.RosVectorToUnityVector();
                Quaternion rotation = transform.rotation.TransformToRosCoordinates();
                OutputRotation = rotation.RosQuaternionToUnityQuaternion();
                return new Pose(new Point(position.x, position.y, position.z), rotation);
            }
        }
    }

    public Vector3 RosRelativePos => UnityRelativePos.TransformToRosCoordinates();

    public UnityEngine.Vector3 UnityRelativePos => (transform.position - m_StartingPos);

    public Quaternion RosRotation => transform.rotation.TransformToRosCoordinates();

    public UnityEngine.Quaternion UnityRotation => transform.rotation;

    private void Start()
    {
        m_StartingPos = transform.position;
    }
}