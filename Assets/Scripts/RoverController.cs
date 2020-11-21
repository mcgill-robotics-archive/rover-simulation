using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using RosSharp.RosBridgeClient;
using RosSharp.RosBridgeClient.Messages;
using roverstd;
using UnityEngine;

public class RoverController : MonoBehaviour
{
    private RosSocket m_Socket;
    private GameObject[] m_Wheels;
    private pair<pair<float, float>, DateTime> m_InputWheelSpeeds;

    unsafe void Start()
    {
        m_Wheels = new GameObject[4];
        m_Wheels[0] = GameObject.Find("WheelFL");
        m_Wheels[1] = GameObject.Find("WheelRL");
        m_Wheels[2] = GameObject.Find("WheelFR");
        m_Wheels[3] = GameObject.Find("WheelRR");
        m_Socket = RosConnection.RosSocket;

        RosConnection.SubscribeUnmanaged<WheelSpeed>("test_topic_wheels", msg =>
        {
            float* arr = msg->WheelSpeeds;
            Debug.Log($"Received wheel speeds {arr[0]} and {arr[1]}");
        });
        //m_Socket.SubscribeUnmanaged<WheelSpeed>("/WheelSpeed", speed =>
        //{
        //    m_InputWheelSpeeds.first.first = speed.Wheel_Speed[0];
        //    m_InputWheelSpeeds.first.second = speed.Wheel_Speed[1];
        //    m_InputWheelSpeeds.second = DateTime.Now;
        //});
    }

    void Update()
    {
        pair<pair<float, float>, DateTime> speeds = m_InputWheelSpeeds;
        if ((DateTime.Now - speeds.second).Milliseconds > 500)
        {
            // value expired
            m_InputWheelSpeeds = default;
        }

        m_Wheels[0].GetComponent<Motor>().TargetAngularSpeedAbsolute = speeds.first.first;
        m_Wheels[1].GetComponent<Motor>().TargetAngularSpeedAbsolute = speeds.first.first;
        m_Wheels[2].GetComponent<Motor>().TargetAngularSpeedAbsolute = speeds.first.second;
        m_Wheels[3].GetComponent<Motor>().TargetAngularSpeedAbsolute = speeds.first.second;
    }
}
