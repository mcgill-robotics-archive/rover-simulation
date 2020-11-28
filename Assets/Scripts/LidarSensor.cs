using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class LidarSensor : MonoBehaviour
{
    public float maxHorizontalRange = 360f;
    private bool sweepDirection = true; // true -> up, false -> down
    public int sweepTime = 3;
    public float angle = -45;
    public float MaxAngle = 45;
    public float MinAngle = -45;
    public int NumberOfLayers = 16;
    public int NumberOfIncrements = 360;
    public float MaxRange = 100f;
    public int counter = 0;
    private static readonly float LIDAR_DELTA_TIME = 0.05f;
    float m_VertIncrement;
    float m_AzimutIncrAngle;

    public bool DrawRaycast = false;

    // TODO : consider using an array list for flexible size
    [HideInInspector] public float[] m_Distances;
    [HideInInspector] public float[] m_Azimuts;


    // Use this for initialization
    void Start()
    {
        m_Distances = new float[NumberOfIncrements];
        m_Azimuts = new float[NumberOfIncrements];
        m_VertIncrement = (float) (MaxAngle - MinAngle) / (float) (NumberOfLayers - 1);
        m_AzimutIncrAngle = (float) (maxHorizontalRange / NumberOfIncrements);

        InvokeRepeating("DoLidarScan", 2.0f, LIDAR_DELTA_TIME);
    }

    public float getCurrentAngle()
    {
        return this.angle;
    }

    public float[] getCurrentDistances()
    {
        return this.m_Distances;
    }

    void RayCast(int incr)
    {
        Vector3 rayOrigin = transform.position;
        Vector3 dir = transform.rotation * Quaternion.Euler(-angle, m_Azimuts[incr], 0) * Vector3.forward;
        //print("incr "+ incr +" layer "+layer+"\n");
        int indx = incr;
        m_Azimuts[incr] = incr * m_AzimutIncrAngle;

        bool outOfRange = false;
        float distance = 0.0f;
        while (!outOfRange)
        {
            RaycastHit hit;
            if (Physics.Raycast(rayOrigin, dir, out hit, MaxRange * 2))
            {
                distance += hit.distance;
                if (distance > MaxRange)
                {
                    distance = MaxRange;
                    outOfRange = true;
                }
                else
                {
                    if (DrawRaycast)
                    {
                        Debug.DrawRay(rayOrigin, dir * hit.distance,
                            Color.Lerp(Color.red, Color.green, hit.distance / MaxRange));
                    }

                    if (hit.collider.gameObject.CompareTag("Mirror"))
                    {
                        if (Vector3.Angle(hit.normal, -dir) < 1.0f)
                        {
                            // bounce back
                            outOfRange = true;
                        }
                        else
                        {
                            // continue ray cast
                            Vector3 proj = Vector3.Project(-dir, hit.normal);
                            rayOrigin = hit.point;
                            dir = proj - ((-dir) - proj);
                        }
                    }
                    else
                    {
                        outOfRange = true;
                    }
                }
            }
            else
            {
                distance = MaxRange;
                outOfRange = true;
            }
        }

        m_Distances[incr] = distance;
    }

    private void DoLidarScan()
    {
        counter += 1;

        if (angle > MaxAngle)
        {
            sweepDirection = false;
        }

        if (angle < MinAngle)
        {
            sweepDirection = true;
        }

        if (sweepDirection)
        {
            angle += (MaxAngle - MinAngle) / sweepTime * LIDAR_DELTA_TIME;
        }
        else
        {
            angle -= (MaxAngle - MinAngle) / sweepTime * LIDAR_DELTA_TIME;
        }

        for (int incr = 0; incr < NumberOfIncrements; incr++)
        {
            RayCast(incr);
        }

        //if (counter == 100) {
        //    Debug.Log(m_Distances[1]);
        //    counter = 0;
        //}
    }
}