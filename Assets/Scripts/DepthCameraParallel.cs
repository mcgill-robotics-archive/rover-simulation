using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using RosSharp;
using RosSharp.RosBridgeClient.MessageTypes.Geometry;
using RosSharp.RosBridgeClient.MessageTypes.Sensor;
using RosSharp.RosBridgeClient.MessageTypes.Std;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using static roverstd.Native;
using roverstd;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor.ShaderGraph.Internal;
using Pose = RosSharp.RosBridgeClient.MessageTypes.Geometry.Pose;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public unsafe class DepthCameraParallel : MonoBehaviour
{
    private const int POOL_ARRAY_COUNT = 128;
    
    private ArrayPool<RaycastCommand> m_RaycastCommandPool = new ArrayPool<RaycastCommand>(POOL_ARRAY_COUNT, LENGTH);

    private BlockingCollection<(Vector3, Matrix4x4, Quaternion)> m_PosMatRotQueue = new BlockingCollection<(Vector3, Matrix4x4, Quaternion)>();

    private Thread m_RaycastDirectionCalculationThread;
    private Thread m_RaycastCommandPreparationThread;
    private Thread m_RaycastDataPostprocessingThread;

    private void CalculateRaycastDirection()
    {
        // TODO use transform.worldToLocalMatrix
        (Vector3 pos, Matrix4x4 worldToLocalMatrix, Quaternion rot) = m_PosMatRotQueue.Take();

        NativeArray<RaycastCommand> commands = m_RaycastCommandPool.ObtainArray();
        RaycastCommand* ptr = (RaycastCommand*) commands.GetUnsafePtr();
        
        
    }

    public static readonly int PIXEL_COUNT_WIDTH = 200;
    public static readonly int PIXEL_COUNT_HEIGHT = 150;
    public static readonly float IMAGE_WIDTH = 2.0f;
    public static readonly float IMAGE_HEIGHT = 1.5f;
    public static readonly float IMAGE_DISTANCE = 1.0f;
    public static readonly float PIXEL_WIDTH = (float)IMAGE_WIDTH / PIXEL_COUNT_HEIGHT;
    public static readonly float PIXEL_HEIGHT = (float)IMAGE_HEIGHT / PIXEL_COUNT_HEIGHT;
    public static readonly float MAX_RANGE = 20.0f;
    public bool IsManagingExternRes { get; set; }

    [StructLayout(LayoutKind.Explicit, Size = 3 * sizeof(float))]
    private struct RayCastPixel
    {
        [FieldOffset(0 * sizeof(float))] public Vector3 Direction;
        [FieldOffset(3 * sizeof(float))] public Vector2 Coordinates;
    }

    ~DepthCameraParallel()
    {
        OnDestroy();
    }

    private double* m_ImageBuffer;

    private RayCastPixel* m_CachedDirections;
    private pair<cpointer<Vector3>, Mutex> m_BufferA;
    private pair<cpointer<Vector3>, Mutex> m_BufferB;
    private pair<cpointer<Vector3>, Mutex> m_ActiveBuffer;
    public RenderTexture DepthImageTexture;
    public Texture2D DepthTexture2D;

    private bool m_UsingBufferA;

    private static readonly int LENGTH = PIXEL_COUNT_WIDTH * PIXEL_COUNT_HEIGHT;

    private static readonly float DEPTH_CAM_DELTA_TIME = 0.2f;

    private void Start()
    {
        //RosConnection.RosSocket.Advertise<PointCloud>("/depth_camera_point_cloud");
        RosConnection.RosSocket.Advertise<UInt8MultiArray>("/depth_camera_point_cloud_bytes");

        lock (this)
        {
            IsManagingExternRes = true;
            m_CachedDirections = (RayCastPixel*)calloc(LENGTH, sizeof(RayCastPixel));
            m_BufferA.first = (Vector3*)calloc(LENGTH, sizeof(Vector3));
            m_BufferB.first = (Vector3*)calloc(LENGTH, sizeof(Vector3));
            m_BufferA.second = new Mutex();
            m_BufferB.second = new Mutex();
            m_ActiveBuffer = m_BufferA;

            m_ImageBuffer = (double*)calloc(LENGTH, sizeof(double));
        }

        m_UsingBufferA = true;

        Vector3 op0 = new Vector3(-IMAGE_WIDTH / 2.0f, IMAGE_HEIGHT / 2.0f);
        Vector3 co = new Vector3(0.0f, 0.0f, IMAGE_DISTANCE);
        // for [2, 1]: 
        for (int x = 0; x < PIXEL_COUNT_WIDTH; x++)
        {
            for (int y = 0; y < PIXEL_COUNT_HEIGHT; y++)
            {
                int index = y * PIXEL_COUNT_WIDTH + x;
                RayCastPixel* pixel = m_CachedDirections + index;
                pixel->Coordinates = new Vector2(x, y);
                Vector3 p0p = new Vector3(x * PIXEL_WIDTH, -y * PIXEL_HEIGHT, 0.0f);
                Vector3 op = op0 + p0p;
                Vector3 cp = co + op;
                pixel->Direction = cp.normalized;
            }
        }

        InvokeRepeating(nameof(GenerateImage), 1.0f, DEPTH_CAM_DELTA_TIME);
    }

    private static readonly Quaternion CAM_ROT = Quaternion.Euler(0.0f, -90.0f, 270.0f);

    private static readonly TimeSpan MAX_MUTEX_WAIT_TIME =
        new TimeSpan(0, 0, 0, 0, (int)(DEPTH_CAM_DELTA_TIME * 0.9f * 1000.0f));

    private void GenerateImage()
    {
        Mutex activeMutex = m_ActiveBuffer.second;
        if (!activeMutex.WaitOne(MAX_MUTEX_WAIT_TIME)) return;
        int inc = 0;
        Vector3 pos = transform.position;
        Quaternion rot = transform.rotation * CAM_ROT;
        try
        {
            Vector3* ptr = m_ActiveBuffer.first.value;
            for (int i = 0; i < LENGTH; i++)
            {
                RayCastPixel pixel = m_CachedDirections[i];
                RaycastHit hit;
                Vector3 dir = rot * pixel.Direction;
                if (Physics.Raycast(pos, dir, out hit, MAX_RANGE))
                {
                    Debug.DrawRay(pos, dir * hit.distance, Color.green, DEPTH_CAM_DELTA_TIME);

                    ptr[inc] = transform.InverseTransformPoint(hit.point);
                    //double ratio = (double) hit.distance / (double) MAX_RANGE;
                    //Debug.Assert(ratio >= 0.0 && ratio <= 1.0);
                    //m_ImageBuffer[(int)pixel.Coordinates.x * PIXEL_COUNT_HEIGHT + (int)pixel.Coordinates.y] = ratio;
                }
                //else
                //{
                //    m_ImageBuffer[(int)pixel.Coordinates.x * PIXEL_COUNT_HEIGHT + (int)pixel.Coordinates.y] = 1.0d;
                //}

                //Debug.DrawLine(pos, pos + (rot * CAM_ROT) * pixel.Direction * pixel.Distance, Color.green);
                inc++;
            }

            //for (int i = 0; i < PIXEL_COUNT_WIDTH; i++)
            //{
            //    for (int j = 0; j < PIXEL_COUNT_HEIGHT; j++)
            //    {
            //        int index = i * PIXEL_COUNT_HEIGHT + j;
            //        m_ImageBuffer[index] = (double) index;
            //    }
            //}

            //using (var cam = new UnmanagedMemoryStream((byte*) m_ImageBuffer, LENGTH * sizeof(double)))
            //{
            //    using (var outFile = new FileStream($"C:\\Users\\njche\\Desktop\\Images\\IMAGE{DateTime.Now.ToBinary()}.floats", FileMode.Create))
            //    {
            //        cam.CopyTo(outFile);
            //    }
            //}
        }
        finally
        {
            //free(m_DepthImage);

            // creating the 4x4 affine transformation matrix
            //Vector4 ti = -transform.up;
            //Vector4 tj = transform.forward;
            //Vector4 tk = -transform.right;
            //Vector4 pos4 = pos;
            //pos4.w = 1.0f;
            //pos4 = new Vector4();
            //Matrix4x4 affineTransformation = new Matrix4x4(ti, tj, tk, pos4);
            // run post processing on a separate thread
            Vector3 original = new Vector3(0.0f, 0.0f, 1.0f);
            float multiplier = original.magnitude / transform.InverseTransformVector(original).magnitude;
            Debug.Log($"Transform Multiplier: {multiplier}");
            Thread t = new Thread(() =>
            {
                RunVertexPostProcessing(m_ActiveBuffer, inc, multiplier);
            });
            t.Start();
            //RenderTexture.active = DepthImageTexture;
            //for (int i = 0; i < DepthImageTexture.width; i++)
            //{
            //    for (int j = 0; j < DepthImageTexture.height; j++)
            //    {
            //        DepthTexture2D.SetPixel(i, j, Color.blue);
            //    }
            //}
            //DepthTexture2D.Apply();

            //RenderTexture.active = null;
            m_ActiveBuffer = m_UsingBufferA ? m_BufferB : m_BufferA;
            m_UsingBufferA = !m_UsingBufferA;
            activeMutex.ReleaseMutex();
            //RunVertexPostProcessing(affineTransformation, m_ActiveBuffer, inc);
            //t.Join();
        }
    }

    private static readonly ChannelFloat32[] ChannelFloats = new ChannelFloat32[1];

    private static void RunVertexPostProcessing(pair<cpointer<Vector3>, Mutex> dataToProcess, int count, float multiplier)
    {
        if (!dataToProcess.second.WaitOne(MAX_MUTEX_WAIT_TIME)) return;
        //PointCloud2 cloud = new PointCloud2();
        UInt8MultiArray arrData = new UInt8MultiArray();
        try
        {
            Vector3* ptr = dataToProcess.first;
            for (int i = 0; i < count; i++)
            {
                //Vector4 ptVec4 = ptr[i];
                //ptVec4.w = 1.0f;
                //Vector3 result = transformation * ptVec4;

                Vector3 result = ptr[i] * multiplier;
                result.x *= -1;
                ptr[i] = result;
            }
            arrData.data = new byte[count * sizeof(Vector3)];
            fixed (byte* dataStart = arrData.data)
            {
                memcpy(dataStart, ptr, sizeof(Vector3) * count);
            }
        }
        finally
        {
            dataToProcess.second.ReleaseMutex();
            //cloud.width = (uint)count;
            //cloud.height = 1;
            //cloud.is_dense = true;
            //cloud.point_step = (uint)sizeof(Vector3);
            //cloud.row_step = (uint)(count * sizeof(Vector3));
            //cloud.fields = new PointField[count];
            //PointField fieldStructure = new PointField("pos", 0, PointField.FLOAT32, 3);
            //for (int i = 0; i < cloud.fields.Length; i++)
            //{
            //    cloud.fields[i] = fieldStructure;
            //}
            //RosConnection.RosSocket.Publish("/depth_camera_point_cloud", cloud);

            RosConnection.RosSocket.Publish("/depth_camera_point_cloud_bytes", arrData);
        }
    }

    private void OnDestroy()
    {
        lock (this)
        {
            if (IsManagingExternRes)
            {
                free(m_CachedDirections);
                free(m_BufferA.first);
                free(m_BufferB.first);
                m_BufferA.second.Dispose();
                m_BufferB.second.Dispose();

                free(m_ImageBuffer);
                IsManagingExternRes = false;
            }
        }
    }
}