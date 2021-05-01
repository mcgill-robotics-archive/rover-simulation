using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RosSharp.RosBridgeClient.MessageTypes.Std;
using System.Linq;
using System;

public class WheelEncoder : MonoBehaviour
{
    private Motor[] m_Motors;
    private bool m_Published = false;
    private const float WHEEL_ENCODER_DELTA_TIME = 0.1f;

    void Start()
    {
        InvokeRepeating(nameof(UpdateWheelEncoder), 1.0f, WHEEL_ENCODER_DELTA_TIME);
    }

    void UpdateWheelEncoder()
    {
        if (!m_Published)
        {
            try
            {
                RosConnection.RosSocket.Advertise<Float32MultiArray>("/wheel_pos");
                m_Published = true;
            }
            catch (Exception)
            {
                //Debug.Log("Nullptr");
            }
            return;
        }

        if (m_Motors == null)
        {
            m_Motors = GameObject.Find("Rover").GetComponent<RoverController>().Motors;
            return;
        }

        Float32MultiArray data = new Float32MultiArray();
        data.data = (from motor in m_Motors select motor.CurrentAngularPosition).ToArray();

        RosConnection.RosSocket.Publish("/wheel_pos", data);

        // Debug.Log("Published wheel positions");
    }

}
