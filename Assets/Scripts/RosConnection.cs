using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RosSharp.RosBridgeClient;
using RosSharp.RosBridgeClient.Messages;
using RosSharp.RosBridgeClient.Messages.Standard;
using UnityEditor;
using Int32 = RosSharp.RosBridgeClient.Messages.Standard.Int32;
using Random = System.Random;
using Time = RosSharp.RosBridgeClient.Messages.Standard.Time;

public class RosConnection : MonoBehaviour
{
    private RosSocket m_Socket;
    private RosConnector m_RosConnector;
    public LidarSensor lidarSensor;

    public static RosSocket RosSocket
    {
        get
        {
            RosConnection connection = GameObject.Find("RosConnection").GetComponent<RosConnection>();
            while (connection.m_Socket == null)
            {
            }

            return connection.m_Socket;
        }
    }


    void Awake()
    {
        m_RosConnector?.Awake();
    }

    void OnApplicationQuit()
    {
        m_RosConnector.OnApplicationQuit();
    }

    void Start()
    {
        string urlPath = Application.persistentDataPath + "/simunity.txt";
        Debug.Log($"Reading websocket URL from {urlPath}");
        if (!File.Exists(urlPath))
        {
            using (StreamWriter writer = File.CreateText(urlPath))
            {
                writer.Write("ws://localhost:9090");
            }
        }

        string url = File.ReadAllText(urlPath);

        m_RosConnector = new RosConnector
        {
            Protocol = RosConnector.Protocols.WebSocketNET,
            RosBridgeServerUrl = url
        };
        m_RosConnector.Awake();
        while (m_RosConnector.RosSocket == null)
        {
        }

        m_Socket = m_RosConnector.RosSocket;

        m_Socket.Subscribe<ArmMotorCommand>("/arm_control_data", msg =>
        {
            for (int i = 0; i < 6; i++)
            {
                Debug.Log($"Velocity output of motor #{i} is {msg.MotorVel[i]}");
            }
        });

        lidarSensor = GameObject.Find("Lidar").GetComponent<LidarSensor>();

        m_Socket.Advertise<LidarData>("/LidarData");
        //m_Socket.Advertise<ProcessedControllerInput>("/processed_arm_controller_input");
        //m_Socket.Advertise<WheelSpeed>("/WheelSpeed");
        //m_Socket.Advertise<WheelSpeed>("/TestTopic");
        //m_Socket.Subscribe<WheelSpeed>("/WheelSpeed", speed =>
        //{
        //    Debug.Log(speed.Wheel_Speed[0]);
        //    Debug.Log(speed.Wheel_Speed[1]);
        //});
        //m_Socket.Subscribe<Int32>("/TestTopic", num =>
        //{
        //    //Debug.Log(num.Wheel_Speed);
        //});
    }

    private static readonly Random s_RandomInstance = new Random();

    void Update()
    {
        //float[] randomInput = new float[6];
        //for (int i = 0; i < 6; i++)
        //{
        //    randomInput[i] = (float) (s_RandomInstance.NextDouble() * 2 - 1);
        //}

        //m_Socket.Publish("/processed_arm_controller_input", new ProcessedControllerInput
        //{
        //    ControllerInput = randomInput
        //});

        //m_Socket.Publish("/WheelSpeed", new WheelSpeed
        //{
        //    Wheel_Speed = new[] { 5.0f, 5.0f }
        //});

        m_Socket.Publish("/LidarData", new LidarData
        {
            distances = lidarSensor.getCurrentDistances(),
            angle = lidarSensor.getCurrentAngle()
        });


        //m_Socket.Publish("/TestTopic", new Int32());
    }
}