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

    public float CurrentAngularSpeed { get; private set; }
    public float CurrentAngularPosition { get; private set; }

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
    }

    void FixedUpdate()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        float currentAngularSpeed = Vector3.Project(rb.angularVelocity, transform.forward).magnitude;

        float angPos = transform.localRotation.eulerAngles.z;
        if (InvertRotationDirection)
        {
            angPos = 360.0f - angPos;
        }

        CurrentAngularPosition = angPos * Mathf.Deg2Rad;

        //while (CurrentAngularPosition >= 2 * Mathf.PI)
        //{
        //    CurrentAngularPosition -= 2 * Mathf.PI;
        //}

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