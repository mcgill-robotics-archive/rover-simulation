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
using UnityEngine.EventSystems;
using Pose = RosSharp.RosBridgeClient.MessageTypes.Geometry.Pose;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;
using Unity.Jobs;

public unsafe class DepthCameraParallel : MonoBehaviour
{
    private const int POOL_ARRAY_COUNT = 128;

    public static DepthCameraParallel Instance { get; private set; }
    
    private ArrayPool<RaycastCommand> m_RaycastCommandPool;
    private ArrayPool<RaycastHit> m_RaycastHitPool;

    private ConcurrentQueue<(Vector3, Matrix4x4, Quaternion, ulong)> m_PosMatRotQueue = new ConcurrentQueue<(Vector3, Matrix4x4, Quaternion, ulong)>();
    private ConcurrentQueue<(NativeArray<RaycastCommand>, Matrix4x4, ulong)> m_RaycastCommandWorldToLocalQueue =
        new ConcurrentQueue<(NativeArray<RaycastCommand>, Matrix4x4, ulong)>();
    private ConcurrentQueue<(JobHandle, NativeArray<RaycastHit>, Matrix4x4, NativeArray<RaycastCommand>, IntPtr, ulong)> m_RaycastJobResultsWorldToLocalQueue = 
        new ConcurrentQueue<(JobHandle, NativeArray<RaycastHit>, Matrix4x4, NativeArray<RaycastCommand>, IntPtr, ulong)>();
    private ConcurrentQueue<(NativeArray<RaycastHit>, Matrix4x4, IntPtr, ulong)> m_RaycastHitWorldToLocalQueue = new ConcurrentQueue<(NativeArray<RaycastHit>, Matrix4x4, IntPtr, ulong)>();
    private ConcurrentQueue<(Vector3[], ulong)> m_PointCloudQueue = new ConcurrentQueue<(Vector3[], ulong)>();

    private Thread m_RaycastCommandPreparationThread;
    private Thread m_RaycastDataPostprocessingThread;

    private void Awake()
    {
        Instance = this;
        m_RaycastCommandPool = new ArrayPool<RaycastCommand>(POOL_ARRAY_COUNT, LENGTH);
        m_RaycastHitPool = new ArrayPool<RaycastHit>(POOL_ARRAY_COUNT, LENGTH);
    }

    private void PrepareRaycastCommands()
    {
        (Vector3, Matrix4x4, Quaternion, ulong) posWorldToLocalRot;
        bool hasNext = m_PosMatRotQueue.TryDequeue(out posWorldToLocalRot);
        if (!hasNext)
        {
            return;
        }

        NativeArray<RaycastCommand> commands = m_RaycastCommandPool.ObtainArray();
        
        for (int i = 0; i < LENGTH; i++)
        {
            commands[i] = new RaycastCommand(posWorldToLocalRot.Item1, posWorldToLocalRot.Item3 * m_CachedDirections[i].Direction, MAX_RANGE);
        }

        m_RaycastCommandWorldToLocalQueue.Enqueue((commands, posWorldToLocalRot.Item2, posWorldToLocalRot.Item4));

        Debug.Log($"#{posWorldToLocalRot.Item4} Finished raycast command preparation {DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff")}");
    }

    private void DoParallelRaycast()
    {
        bool hasNext = m_RaycastCommandWorldToLocalQueue.TryDequeue(out (NativeArray<RaycastCommand>, Matrix4x4, ulong) cmdWorldToLocal);
        if (!hasNext)
        {
            return;
        }

        NativeArray<RaycastHit> results = m_RaycastHitPool.ObtainArray();
        IntPtr resultsPtr = (IntPtr) results.GetUnsafePtr();
        JobHandle raycastJob = RaycastCommand.ScheduleBatch(cmdWorldToLocal.Item1, results, 100);
        m_RaycastJobResultsWorldToLocalQueue.Enqueue((raycastJob, results, cmdWorldToLocal.Item2, cmdWorldToLocal.Item1, resultsPtr, cmdWorldToLocal.Item3));

        Debug.Log($"#{cmdWorldToLocal.Item3} Finished raycast scheduling {DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff")}");
    }

    private void ObtainParallelRaycastResult()
    {
        bool hasNext = m_RaycastJobResultsWorldToLocalQueue.TryPeek(out (JobHandle, NativeArray<RaycastHit>, Matrix4x4, NativeArray<RaycastCommand>, IntPtr, ulong) handleHitsWorldToLocal);
        if (!hasNext)
        {
            return;
        }

        JobHandle handle = handleHitsWorldToLocal.Item1;
        if (!handle.IsCompleted)
        {
            return;
        }

        m_RaycastJobResultsWorldToLocalQueue.TryDequeue(out _);
        handle.Complete();

        m_RaycastCommandPool.ReturnArray(handleHitsWorldToLocal.Item4);
        m_RaycastHitWorldToLocalQueue.Enqueue((handleHitsWorldToLocal.Item2, handleHitsWorldToLocal.Item3, handleHitsWorldToLocal.Item5, handleHitsWorldToLocal.Item6));

        Debug.Log($"#{handleHitsWorldToLocal.Item6} Finished obtaining results {DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff")}");
    }

    private void PostProcessRaycastResult()
    {
        bool hasNext = m_RaycastHitWorldToLocalQueue.TryDequeue(out (NativeArray<RaycastHit>, Matrix4x4, IntPtr, ulong) hitsWorldToLocal);
        if (!hasNext)
        {
            return;
        }
        Matrix4x4 worldToLocalMat = hitsWorldToLocal.Item2;
        RaycastHit* hits = (RaycastHit*) hitsWorldToLocal.Item3;
        List<Vector3> localPoints = new List<Vector3>(); 
        for (int i = 0; i < LENGTH; i++)
        {
            Vector3 worldHit = hits[i].point;
            if (worldHit == Vector3.zero)
            {
                continue;
            }

            Vector3 localPoint = worldToLocalMat.MultiplyPoint3x4(worldHit) * 30.0f;
            localPoint.x *= -1;
            localPoints.Add(localPoint);
        }

        m_PointCloudQueue.Enqueue((localPoints.ToArray(), hitsWorldToLocal.Item4));
        Debug.Log($"#{hitsWorldToLocal.Item4} Finished post processing {DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff")}");

        m_RaycastHitPool.ReturnArray(hitsWorldToLocal.Item1);
    }

    private void SendArrayToRos()
    {
        bool hasNext = m_PointCloudQueue.TryDequeue(out (Vector3[], ulong) hitsWorldToLocal);
        if (!hasNext)
        {
            return;
        }

        UInt8MultiArray arr = new UInt8MultiArray();
        arr.data = new byte[hitsWorldToLocal.Item1.Length * sizeof(Vector3)];

        fixed (Vector3* vec3ArrStart = hitsWorldToLocal.Item1)
        {
            fixed (byte* arrStart = arr.data)
            {
                memcpy(arrStart, vec3ArrStart, arr.data.Length);
            }
        }

        RosConnection.RosSocket.Publish("/depth_camera_point_cloud_bytes", arr);
        Debug.Log($"#{hitsWorldToLocal.Item2} Finished sending array to ROS {DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff")} with {hitsWorldToLocal.Item1.Length} points");
    }

    public static readonly int PIXEL_COUNT_WIDTH = 100;
    public static readonly int PIXEL_COUNT_HEIGHT = 100;
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


    private RayCastPixel* m_CachedDirections;

    private static readonly int LENGTH = PIXEL_COUNT_WIDTH * PIXEL_COUNT_HEIGHT;

    private static readonly float DEPTH_CAM_DELTA_TIME = 0.2f;

    private void Start()
    {
        //RosConnection.RosSocket.Advertise<PointCloud>("/depth_camera_point_cloud");
        RosConnection.RosSocket.Advertise<UInt8MultiArray>("/depth_camera_point_cloud_bytes");

        IsManagingExternRes = true;
        m_CachedDirections = (RayCastPixel*)calloc(LENGTH, sizeof(RayCastPixel));


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

        //InvokeRepeating(nameof(GenerateImage), 1.0f, DEPTH_CAM_DELTA_TIME);

        PreparationJob prepJob;
        prepJob.Schedule();
        PostProcessJob postProcessJob;
        postProcessJob.Schedule();

        new Thread(() =>
        {
            for (;;)
            {
                SendArrayToRos();
            }
        }).Start();

        InvokeRepeating(nameof(CustomUpdate), 1.0f, DEPTH_CAM_DELTA_TIME);
    }

    private struct PreparationJob : IJob
    {
        public void Execute()
        {
            DepthCameraParallel cam = DepthCameraParallel.Instance;
            for (;;)
            {
                cam.PrepareRaycastCommands();
            }
        }
    }

    private struct ScheduleRaycastJob : IJob
    {
        public void Execute()
        {
            DepthCameraParallel cam = Instance;
            for (;;)
            {
                cam.DoParallelRaycast();
            }
        }
    }

    private struct GetRaycastResultJob : IJob
    {
        public void Execute()
        {
            DepthCameraParallel cam = Instance;
            for (;;)
            {
                cam.ObtainParallelRaycastResult();
            }
        }
    }

    private struct PostProcessJob : IJob
    {
        public void Execute()
        {
            DepthCameraParallel cam = DepthCameraParallel.Instance;
            for (;;)
            {
                cam.PostProcessRaycastResult();
            }
        }
    }

    private ulong m_PointCloudFrameCounter = 0;

    private void CustomUpdate()
    {
        if (m_PosMatRotQueue.Count < 100)
        {
            m_PosMatRotQueue.Enqueue((transform.position, transform.worldToLocalMatrix, transform.rotation * CAM_ROT, m_PointCloudFrameCounter));
        }

        DoParallelRaycast();
        ObtainParallelRaycastResult();
        m_RaycastCommandPool.EnsureExtraCapacity();
        m_RaycastHitPool.EnsureExtraCapacity();
        m_PointCloudFrameCounter++;
    }

    private static readonly Quaternion CAM_ROT = Quaternion.Euler(0.0f, -90.0f, 270.0f);

    private static readonly TimeSpan MAX_MUTEX_WAIT_TIME =
        new TimeSpan(0, 0, 0, 0, (int)(DEPTH_CAM_DELTA_TIME * 0.9f * 1000.0f));
    
    private void OnDestroy()
    {
        if (IsManagingExternRes)
        {
            free(m_CachedDirections);

            m_RaycastCommandPool.Dispose();
            m_RaycastHitPool.Dispose();

            IsManagingExternRes = false;
        }
    }
}