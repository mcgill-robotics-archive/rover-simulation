using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using RosSharp.RosBridgeClient;
using RosSharp.RosBridgeClient.Messages;
using Rover.Util.Containers;
using UnityEngine;

public class RoverController : MonoBehaviour
{
    private RosSocket m_Socket;
    private GameObject[] m_Wheels;
    private Pair<Pair<float, float>, DateTime> m_InputWheelSpeeds;

    void Start()
    {
        m_Wheels = new GameObject[4];
        m_Wheels[0] = GameObject.Find("WheelFL");
        m_Wheels[1] = GameObject.Find("WheelRL");
        m_Wheels[2] = GameObject.Find("WheelFR");
        m_Wheels[3] = GameObject.Find("WheelRR");
        m_Socket = RosConnection.RosSocket;

        m_Socket.Subscribe<WheelSpeed>("/WheelSpeed", speed =>
        {
            m_InputWheelSpeeds.First.First = speed.Wheel_Speed[0];
            m_InputWheelSpeeds.First.Second = speed.Wheel_Speed[1];
            m_InputWheelSpeeds.Second = DateTime.Now;
        });
    }

    void Update()
    {
        Pair<Pair<float, float>, DateTime> speeds = m_InputWheelSpeeds;
        if ((DateTime.Now - speeds.Second).Milliseconds > 500)
        {
            // value expired
            m_InputWheelSpeeds = default;
        }

        m_Wheels[0].GetComponent<Motor>().TargetAngularSpeedAbsolute = speeds.First.First;
        m_Wheels[1].GetComponent<Motor>().TargetAngularSpeedAbsolute = speeds.First.First;
        m_Wheels[2].GetComponent<Motor>().TargetAngularSpeedAbsolute = speeds.First.Second;
        m_Wheels[3].GetComponent<Motor>().TargetAngularSpeedAbsolute = speeds.First.Second;
    }
}
