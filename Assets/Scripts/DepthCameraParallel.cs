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

    private ConcurrentQueue<(Vector3, Matrix4x4, Quaternion)> m_PosMatRotQueue = new ConcurrentQueue<(Vector3, Matrix4x4, Quaternion)>();
    private ConcurrentQueue<(NativeArray<RaycastCommand>, Matrix4x4)> m_RaycastCommandWorldToLocalQueue =
        new ConcurrentQueue<(NativeArray<RaycastCommand>, Matrix4x4)>();
    private ConcurrentQueue<(JobHandle, NativeArray<RaycastHit>, Matrix4x4, NativeArray<RaycastCommand>, IntPtr)> m_RaycastJobResultsWorldToLocalQueue = 
        new ConcurrentQueue<(JobHandle, NativeArray<RaycastHit>, Matrix4x4, NativeArray<RaycastCommand>, IntPtr)>();
    private ConcurrentQueue<(NativeArray<RaycastHit>, Matrix4x4, IntPtr)> m_RaycastHitWorldToLocalQueue = new ConcurrentQueue<(NativeArray<RaycastHit>, Matrix4x4, IntPtr)>();

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
        (Vector3, Matrix4x4, Quaternion) posWorldToLocalRot;
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

        m_RaycastCommandWorldToLocalQueue.Enqueue((commands, posWorldToLocalRot.Item2));

        Debug.Log($"Finished raycast command preparation {DateTime.Now}");
    }

    private void DoParallelRaycast()
    {
        bool hasNext = m_RaycastCommandWorldToLocalQueue.TryDequeue(out (NativeArray<RaycastCommand>, Matrix4x4) cmdWorldToLocal);
        if (!hasNext)
        {
            return;
        }

        NativeArray<RaycastHit> results = m_RaycastHitPool.ObtainArray();
        IntPtr resultsPtr = (IntPtr) results.GetUnsafePtr();
        JobHandle raycastJob = RaycastCommand.ScheduleBatch(cmdWorldToLocal.Item1, results, 100);
        m_RaycastJobResultsWorldToLocalQueue.Enqueue((raycastJob, results, cmdWorldToLocal.Item2, cmdWorldToLocal.Item1, resultsPtr));

        Debug.Log($"Finished raycast scheduling {DateTime.Now}");
    }

    private void ObtainParallelRaycastResult()
    {
        bool hasNext = m_RaycastJobResultsWorldToLocalQueue.TryPeek(out (JobHandle, NativeArray<RaycastHit>, Matrix4x4, NativeArray<RaycastCommand>, IntPtr) handleHitsWorldToLocal);
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
        m_RaycastHitWorldToLocalQueue.Enqueue((handleHitsWorldToLocal.Item2, handleHitsWorldToLocal.Item3, handleHitsWorldToLocal.Item5));

        Debug.Log($"Finished obtaining results {DateTime.Now}");
    }

    private void PostProcessRaycastResult()
    {
        bool hasNext = m_RaycastHitWorldToLocalQueue.TryDequeue(out (NativeArray<RaycastHit>, Matrix4x4, IntPtr) hitsWorldToLocal);
        if (!hasNext)
        {
            return;
        }
        Matrix4x4 worldToLocalMat = hitsWorldToLocal.Item2;
        RaycastHit* hits = (RaycastHit*) hitsWorldToLocal.Item3;
        int nonZeroCount = 0;
        for (int i = 0; i < LENGTH; i++)
        {
            Vector3 worldHit = hits[i].point;
            if (worldHit != Vector3.zero)
            {
                nonZeroCount++;
            }
            Vector3 localPoint = worldToLocalMat.MultiplyPoint3x4(worldHit) * 30.0f;
            localPoint.x *= -1;
        }
        Debug.Log($"Finished post processing {DateTime.Now} with {nonZeroCount} non zero point(s)");

        m_RaycastHitPool.ReturnArray(hitsWorldToLocal.Item1);
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


    private RayCastPixel* m_CachedDirections;

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
        }


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

    private void Update()
    {
        m_PosMatRotQueue.Enqueue((transform.position, transform.worldToLocalMatrix, transform.rotation * CAM_ROT));
        DoParallelRaycast();
        ObtainParallelRaycastResult();
        m_RaycastCommandPool.EnsureExtraCapacity();
        m_RaycastHitPool.EnsureExtraCapacity();
    }

    private static readonly Quaternion CAM_ROT = Quaternion.Euler(0.0f, -90.0f, 270.0f);

    private static readonly TimeSpan MAX_MUTEX_WAIT_TIME =
        new TimeSpan(0, 0, 0, 0, (int)(DEPTH_CAM_DELTA_TIME * 0.9f * 1000.0f));
    
    private void OnDestroy()
    {
        lock (this)
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
}