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
    }

    public float getCurrentAngle()
    {
        return this.angle;
    }

    public float[] getCurrentDistances()
    {
        return this.m_Distances;
    }

    async Task RayCast(int incr)
    {
        Vector3 dir;
        //print("incr "+ incr +" layer "+layer+"\n");
        int indx = incr;
        m_Azimuts[incr] = incr * m_AzimutIncrAngle;
        dir = transform.rotation * Quaternion.Euler(-angle, m_Azimuts[incr], 0) * Vector3.forward;

        bool outOfRange = false;
        float distance = 0.0f;
        Vector3 rayOrigin = transform.position;
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

    async void FixedUpdate()
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
            angle += (MaxAngle - MinAngle) / sweepTime * Time.deltaTime;
        }
        else
        {
            angle -= (MaxAngle - MinAngle) / sweepTime * Time.deltaTime;
        }

        Task[] tasks = new Task[NumberOfIncrements];
        for (int incr = 0; incr < NumberOfIncrements; incr++)
        {
            tasks[incr] = RayCast(incr);
        }

        await Task.WhenAll(tasks);

        //if (counter == 100) {
        //    Debug.Log(m_Distances[1]);
        //    counter = 0;
        //}
    }
}