using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DayNightCycle : MonoBehaviour
{
    public bool DoDayNightCycle;
    public float CycleSpeed;

    void Update()
    {
        if (DoDayNightCycle)
        {
            transform.Rotate(Vector3.right, CycleSpeed * Time.deltaTime, Space.Self);
        }
    }
}
