using System;
using System.Collections;
using System.Collections.Generic;
using RosSharp.RosBridgeClient.MessageTypes.Std;
using UnityEngine;

public class DistanceSensor : MonoBehaviour
{
    public const float MAX_DISTANCE = 2.0f;
    public const float DIST_DELTA_TIME = 0.01f;

    public float Distance;
    public bool OverrideDistanceSensorData;

    private void Start()
    {
        RosConnection.RosSocket.Advertise<Float32>("/distance");
        if (!OverrideDistanceSensorData)
            UpdateDistanceSensorData();

        InvokeRepeating(nameof(PublishDistanceSensorData), 1.0f, DIST_DELTA_TIME);
    }

    private void UpdateDistanceSensorData()
        => Distance = Physics.Raycast(transform.position, -transform.right, out RaycastHit hit, MAX_DISTANCE)
            ? hit.distance
            : MAX_DISTANCE;

    private void PublishDistanceSensorData()
    {
        if (!OverrideDistanceSensorData)
            UpdateDistanceSensorData();

        Float32 dist = new Float32(Distance);
        RosConnection.RosSocket.Publish("/distance", dist);
    }
}