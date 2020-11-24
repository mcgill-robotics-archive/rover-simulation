using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class Motor : MonoBehaviour
{
    public float MaxAngularSpeed = 20.0f;
    public float MinAngularSpeed = -20.0f;

    /// <summary>
    /// DO NOT MODIFY, FOR EDITOR USE ONLY
    /// </summary>
    public bool InvertRotationDirection;

    /// <summary>
    /// DO NOT MODIFY, FOR EDITOR USE ONLY
    /// </summary>
    public float TargetAngularSpeed;

    /// <summary>
    /// DO NOT MODIFY, FOR EDITOR USE ONLY
    /// </summary>
    public bool TargetAngularSpeedOverride;

    public float TargetAngularSpeedAbsolute
    {
        get => TargetAngularSpeed;
        set
        {
            if (!TargetAngularSpeedOverride)
            {
                TargetAngularSpeed = Mathf.Clamp(value, MinAngularSpeed, MaxAngularSpeed);
            }
        }
    }

    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = float.MaxValue;

        InvokeRepeating("CheckWheelSpeed", 5.0f, 1.0f);
    }

    void CheckWheelSpeed()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        float currentAngularSpeed = Vector3.Project(rb.angularVelocity, transform.forward).magnitude;
        if (Mathf.Abs(currentAngularSpeed - Mathf.Abs(TargetAngularSpeed)) > 1.0f)
        {
            // Debug.LogWarning($"Wheel speed above / below from target: {currentAngularSpeed}");
        }
    }

    void Update()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        float currentAngularSpeed = Vector3.Project(rb.angularVelocity, transform.forward).magnitude;

        if (TargetAngularSpeed > 0.05f) // forward
        {
            if (TargetAngularSpeed < 0.05f)
            {
                ApplyBrakes();
            }
            if (currentAngularSpeed < TargetAngularSpeed)
            {
                rb.AddRelativeTorque(
                    new Vector3(0.0f, 0.0f, InvertRotationDirection ? -20.0f : 20.0f),
                    ForceMode.Force
                );
            }
        }
        else if (Mathf.Abs(TargetAngularSpeed) < 0.05f) // near zero target speed
        {
            ApplyBrakes();
        }
        else // backward
        {
            if (TargetAngularSpeed > 0.05f)
            {
                ApplyBrakes();
            }

            if (currentAngularSpeed < -TargetAngularSpeed)
            {
                rb.AddRelativeTorque(
                    new Vector3(0.0f, 0.0f, InvertRotationDirection ? 20.0f : -20.0f),
                    ForceMode.Force
                );
            }
        }
    }

    public void ApplyBrakes()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.angularVelocity = new Vector3(rb.angularVelocity.x, rb.angularVelocity.y, 0.0f);
    }
}