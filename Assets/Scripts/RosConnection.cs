using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using RosSharp.RosBridgeClient;
using RosSharp.RosBridgeClient.Messages;
using RosSharp.RosBridgeClient.Messages.Standard;
using RosSharp.RosBridgeClient.MessageTypes.Std;
using Rover;
using UnityEditor;
using Int32 = RosSharp.RosBridgeClient.Messages.Standard.Int32;
using Random = System.Random;
using Time = RosSharp.RosBridgeClient.Messages.Standard.Time;
using RosString = RosSharp.RosBridgeClient.Messages.Standard.String;
using static Assets.Scripts.Util.Native;
using Object = UnityEngine.Object;

public class RosConnection : MonoBehaviour
{
    private RosSocket m_Socket;
    private RosConnector m_RosConnector;
    public LidarSensor lidarSensor;
    private IDictionary<string, SubscriberCallbackTypeErased> m_TopicToCallback;

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

    public static RosConnection Instance => GameObject.Find("RosConnection").GetComponent<RosConnection>();


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
        m_TopicToCallback = new Dictionary<string, SubscriberCallbackTypeErased>();
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

        m_Socket.Advertise<UInt8MultiArray>("/unity_to_ros_topic");

        m_Socket.Subscribe<UInt8MultiArray>("/ros_to_unity_topic", MessageReceived);

        //m_Socket.Subscribe<ArmMotorCommand>("/arm_control_data", msg =>
        //{
        //    for (int i = 0; i < 6; i++)
        //    {
        //        Debug.Log($"Velocity output of motor #{i} is {msg.MotorVel[i]}");
        //    }
        //});

        //lidarSensor = GameObject.Find("Lidar").GetComponent<LidarSensor>();

        //m_Socket.Advertise<LidarData>("/LidarData");
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

    private unsafe void MessageReceived(UInt8MultiArray arr)
    {
        fixed (byte* start = arr.data)
        {
            Debug.Assert((ulong) start % 8 == 0); // assert aligned
            byte* head = start;
            int topicStringLength = *(int*) head;
            head += 4;
            byte* topicCString = stackalloc byte[topicStringLength + 1];
            memcpy(topicCString, head, topicStringLength + 1);
            head += topicStringLength + 1;
            string topic = Marshal.PtrToStringAnsi((IntPtr) topicCString);
            Debug.Assert(topic != null);
            if (!m_TopicToCallback.ContainsKey(topic)) return;
            // bump the head to the next 8-byte aligned position
            head = (byte*) RoundUp((ulong) head, 8);
            m_TopicToCallback[topic](head);
        }
    }

    private static readonly Random s_RandomInstance = new Random();

    unsafe void Update()
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

        //m_Socket.Publish("/LidarData", new LidarData
        //{
        //    distances = lidarSensor.getCurrentDistances(),
        //    angle = lidarSensor.getCurrentAngle()
        //});


        //m_Socket.Publish("/TestTopic", new Int32());
        WheelSpeed speed;

        speed.WheelSpeeds[0] = 1;
        speed.WheelSpeeds[1] = 2;
        Publish("test_topic", speed);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static unsafe void memset(void* ptr, int value, int num)
    {
        byte* dest = (byte*) ptr;
        for (int i = 0; i < num; i++)
        {
            dest[i] = 0;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int RoundUp(int num, int multiple)
    {
        if (num % multiple == 0)
        {
            return num;
        }

        return (num / multiple + 1) * multiple;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ulong RoundUp(ulong num, ulong multiple)
    {
        if (num % multiple == 0)
        {
            return num;
        }

        return (num / multiple + 1) * multiple;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void Publish<T>(string topic, T data) where T : unmanaged, IMessage
    {
        Publish(topic, &data);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static unsafe R CopyReinterpretCast<R, T>(T t) where T : unmanaged where R : unmanaged
    {
        R* result = stackalloc R[1];
        memset(result, 0, sizeof(R)); // ECMA-334 23.9 "The content of the newly allocated memory is undefined."
        memcpy(result, &t, Math.Min(sizeof(T), sizeof(R)));
        return *result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void Publish<T>(string topic, T* data) where T : unmanaged, IMessage
    {
        RosSocket socket = RosSocket;
        UInt8MultiArray arr = new UInt8MultiArray();
        // 16 bytes for the type code, then followed by the topic (8 bytes for the length, then the null terminated string), then the data
        byte[] arrData = new byte[sizeof(ulong) + sizeof(int) + topic.Length + 1 + sizeof(T)];
        fixed (byte* destFixed = arrData)
        {
            Debug.Assert((ulong) destFixed % 8 == 0); // assert that the data is 8-byte aligned
            *destFixed = data->TypeCode;
            byte* dest = destFixed;
            dest += 8;
            int topicLength = topic.Length;
            memcpy(dest, (byte*) &topicLength, sizeof(int));

            dest += sizeof(int);

            fixed (byte* stringStart = Encoding.ASCII.GetBytes(topic))
            {
                memcpy(dest, stringStart, topicLength);
            }

            dest += topicLength;
            *dest = 0;
            dest++;

            memcpy(dest, (byte*) data, sizeof(T));
        }

        arr.data = arrData;

        socket.Publish("/unity_to_ros_topic", arr);
    }

    public unsafe delegate void SubscriberCallback<T>(T* msg) where T : unmanaged;

    private unsafe delegate void SubscriberCallbackTypeErased(void* msg);

    public static unsafe void Subscribe<T>(string topic, SubscriberCallback<T> callback) where T : unmanaged, IMessage
    {
        RosConnection connection = Instance;

        void Func(void* msg)
        {
            callback((T*) msg);
        }

        connection.m_TopicToCallback[topic] = Func;
    }

    public static void RemoveSubscription(string topic)
    {
        RosConnection connection = Instance;

        connection.m_TopicToCallback.Remove(topic);
    }
}