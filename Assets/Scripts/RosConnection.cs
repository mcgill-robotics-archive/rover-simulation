﻿using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;
using RosSharp.RosBridgeClient;
using RosSharp.RosBridgeClient.Messages;
using RosSharp.RosBridgeClient.Messages.Standard;
using RosSharp.RosBridgeClient.MessageTypes.Std;
using roverstd;
using UnityEditor;
using UnityEngine.Windows.Speech;
using Int32 = RosSharp.RosBridgeClient.Messages.Standard.Int32;
using Random = System.Random;
using Time = RosSharp.RosBridgeClient.Messages.Standard.Time;
using RosString = RosSharp.RosBridgeClient.Messages.Standard.String;
using static roverstd.Native;
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


    unsafe void Awake()
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
    }

    void OnApplicationQuit()
    {
        m_RosConnector.OnApplicationQuit();
    }

    unsafe void Start()
    {
        //SubscribeUnmanaged<ArmMotorCommand>("test_topic", msg => Debug.Log(msg->MotorVel[0]));

        //m_Socket.SubscribeUnmanaged<ArmMotorCommand>("/arm_control_data", msg =>
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
        //m_Socket.SubscribeUnmanaged<WheelSpeed>("/WheelSpeed", speed =>
        //{
        //    Debug.Log(speed.Wheel_Speed[0]);
        //    Debug.Log(speed.Wheel_Speed[1]);
        //});
        //m_Socket.SubscribeUnmanaged<Int32>("/TestTopic", num =>
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
            byte typecode = *start;
            byte isManagedType = start[1];
            head += 8;
            int topicStringLength = *(int*) head;
            head += sizeof(int);
            byte* topicCString = stackalloc byte[topicStringLength + 1];
            memcpy(topicCString, head, topicStringLength + 1);
            head += topicStringLength + 1;
            string topic = Marshal.PtrToStringAnsi((IntPtr) topicCString);
            Debug.Assert(topic != null);
            if (!m_TopicToCallback.ContainsKey(topic)) return;
            // bump the head to the next 8-byte aligned position
            head = (byte*) RoundUp((ulong) head, 8);
            if (isManagedType == 1)
            {
                ByteArrayInputStream bais = new ByteArrayInputStream(head, arr.data.Length - (head - start));
                object o = new object();
                // TODO
                m_TopicToCallback[topic](o);
            }
            else
            {
                Debug.Log((from b in arr.data select b.ToString()).Aggregate("", (s, s1) => s + " " + s1));
                Debug.LogWarning($"Received msg from {topic}");
                m_TopicToCallback[topic](new void_pointer(head));
            }
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

        //m_Socket.PublishUnmanaged("/processed_arm_controller_input", new ProcessedControllerInput
        //{
        //    ControllerInput = randomInput
        //});

        //m_Socket.PublishUnmanaged("/WheelSpeed", new WheelSpeed
        //{
        //    Wheel_Speed = new[] { 5.0f, 5.0f }
        //});

        //m_Socket.PublishUnmanaged("/LidarData", new LidarData
        //{
        //    distances = lidarSensor.getCurrentDistances(),
        //    angle = lidarSensor.getCurrentAngle()
        //});


        //m_Socket.PublishUnmanaged("/TestTopic", new Int32());

        //WheelSpeed speed;

        //speed.WheelSpeeds[0] = 5;
        //speed.WheelSpeeds[1] = 5;
        //PublishUnmanaged("test_topic_wheels", speed);
        //ArmMotorCommand cmd;
        //memset(cmd.MotorVel, 12, sizeof(ArmMotorCommand));
        //cmd.MotorVel[0] = 10;
        //cmd.MotorVel[1] = 11;
        //cmd.MotorVel[2] = 12;
        //cmd.MotorVel[3] = 13;
        //cmd.MotorVel[4] = 14;
        //cmd.MotorVel[5] = 15;
        //PublishUnmanaged("some_topic", cmd);
        //WheelSpeed wheelSpeed;
        //wheelSpeed.WheelSpeeds[0] = 5.0f;
        //wheelSpeed.WheelSpeeds[1] = 5.0f;
        //PublishUnmanaged("wheel_speed", &wheelSpeed);
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
    public static unsafe void PublishUnmanaged<T>(string topic, T data) where T : unmanaged, IMessage
    {
        PublishUnmanaged(topic, &data);
    }

    public static unsafe void PublishManaged<T>(string topic, T data) where T : ISerializable, IMessage
    {
        ByteArrayOutputStream baos = new ByteArrayOutputStream();
        data.Serialize(baos);
        byte[] byteData = baos.ToArray();
        byte[] messageBytes = new byte[SizeOf(byteData) + 8 + sizeof(int) + topic.Length + 1];
        byte* topicBytes = stackalloc byte[topic.Length + 1];
        memset(topicBytes, 0, topic.Length + 1);
        fixed (char* topicStart = topic)
        {
            Encoding.ASCII.GetBytes(topicStart, topic.Length, topicBytes, topic.Length + 1);
        }

        fixed (byte* messageStart = messageBytes)
        {
            byte* head = messageStart;
            head += 8;
            *(int*) head = topic.Length;
            head += sizeof(int);
            memcpy(head, topicBytes, topic.Length + 1);
        }

        Array.Copy(byteData, 0, messageBytes, 8 + sizeof(int) + topic.Length + 1, byteData.Length);

        // byte 0 to 7 are reserved for metadata
        messageBytes[0] = data.TypeCode;
        messageBytes[1] = 1;
        UInt8MultiArray msg = new UInt8MultiArray
        {
            data = messageBytes
        };
        RosSocket socket = RosSocket;
        socket.Publish("/unity_to_ros_topic", msg);
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
    public static unsafe void PublishUnmanaged<T>(string topic, T* data) where T : unmanaged, IMessage
    {
        RosSocket socket = RosSocket;
        UInt8MultiArray arr = new UInt8MultiArray();
        // 16 bytes for the type code, then followed by the topic (8 bytes for the length, then the null terminated string), then the data
        byte[] arrData = new byte[sizeof(ulong) + sizeof(int) + topic.Length + 1 + sizeof(T) + 8];
        fixed (byte* destFixed = arrData)
        {
            Debug.Assert((ulong) destFixed % 8 == 0); // assert that the data is 8-byte aligned
            destFixed[0] = data->TypeCode;
            destFixed[1] = 0;
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
            dest = (byte*)RoundUp((ulong) dest, 8UL);
            memcpy(dest, (byte*) data, sizeof(T));
        }

        arr.data = arrData;
        //Debug.Log((from b in arrData select b.ToString()).Aggregate("", (s, s1) => s + " " + s1));
        socket.Publish("/unity_to_ros_topic", arr);
    }

    public unsafe delegate void SubscriberCallbackUnmanaged<T>([NotNull] T* msg) where T : unmanaged;

    public delegate void SubscriberCallbackManaged<in T>([NotNull] T msg) where T : ISerializable;

    public delegate void SubscriberCallbackTypeErased([NotNull] object msg);

    [StructLayout(LayoutKind.Explicit, Size = 1)]
    private struct SubscribeSignal : IMessage, IBlittable<SubscribeSignal>
    {
        public byte TypeCode => 0xFF;
        public bool IsManaged => false;

        [FieldOffset(0)]
        public byte TypeToSubscribe;
    }

    public static unsafe void SubscribeUnmanaged<T>(string topic, SubscriberCallbackUnmanaged<T> callbackUnmanaged) where T : unmanaged, IMessage
    {
        RosConnection connection = Instance;

        void Func(object msg)
        {
            void_pointer ptr = (void_pointer) msg;
            callbackUnmanaged((T*) ptr);
        }

        connection.m_TopicToCallback[topic] = Func;

        SubscribeSignal signal;
        signal.TypeToSubscribe = (new T()).TypeCode;
        PublishUnmanaged(topic, &signal);
    }

    public static void SubscribeManaged<T>(string topic, SubscriberCallbackManaged<T> callback) where T : ISerializable
    {
        RosConnection connection = Instance;

        void Func(object msg)
        {
            callback((T) msg);
        }

        connection.m_TopicToCallback[topic] = Func;
    }

    public static void RemoveSubscription(string topic)
    {
        RosConnection connection = Instance;

        connection.m_TopicToCallback.Remove(topic);
    }
}