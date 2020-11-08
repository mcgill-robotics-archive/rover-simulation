using System.Collections;
using System.Collections.Generic;
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
        m_AzimutIncrAngle = (float) ( maxHorizontalRange/ NumberOfIncrements);
    }

    public float getCurrentAngle(){
        return this.angle;
    }

    public float[] getCurrentDistances() {
        return this.m_Distances;
    }
    void FixedUpdate()
    {
        counter += 1;
        Vector3 fwd = new Vector3(0, 0, 1);
        Vector3 dir;
        RaycastHit hit;
        int indx = 0;

        if (angle > MaxAngle) {
            sweepDirection = false;
        }

        if (angle < MinAngle) {
            sweepDirection = true;
        }
        if (sweepDirection) {angle += (MaxAngle - MinAngle)/sweepTime * Time.deltaTime;}
        else {angle -= (MaxAngle - MinAngle)/sweepTime * Time.deltaTime;}
        
        for (int incr = 0; incr < NumberOfIncrements; incr++)
        {
            
            //print("incr "+ incr +" layer "+layer+"\n");
            indx = incr;
            m_Azimuts[incr] = incr * m_AzimutIncrAngle;
            dir = transform.rotation * Quaternion.Euler(-angle, m_Azimuts[incr], 0) * fwd;

            if (Physics.Raycast(transform.position, dir, out hit, MaxRange))
            {
                m_Distances[indx] = (float) hit.distance;
                if (DrawRaycast)
                {
                    Debug.DrawRay(transform.position, dir * hit.distance,
                        Color.Lerp(Color.red, Color.green, hit.distance / MaxRange));
                }
            }
            else
            {
                m_Distances[indx] = 100.0f;
            }
            
        }

        if (counter == 100) {
            Debug.Log(m_Distances[1]);
            counter = 0;
        }
    }
}