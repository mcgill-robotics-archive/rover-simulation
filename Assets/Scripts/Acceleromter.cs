using System;
using UnityEngine;

public class Acceleromter : MonoBehaviour
{
    private bool m_FirstRun;
    private Vector3 m_LastPosition;
    private Quaternion m_LastRotation;

    public Vector3 LinearAcceleration;
    public Quaternion AngularAcceleration;

    private void Awake()
    {
        m_FirstRun = true;
    }

    private void Update()
    {
        if (m_FirstRun)
        {
            m_LastPosition = transform.position;
            m_LastRotation = transform.rotation;
            m_FirstRun = false;
            return;
        }

        Vector3 currentPosition = transform.position;
        Quaternion currentRotation = transform.rotation;

        LinearAcceleration = (currentPosition - m_LastPosition) / Time.deltaTime;
        AngularAcceleration = currentRotation * Quaternion.Inverse(m_LastRotation);

        m_LastPosition = currentPosition;
        m_LastRotation = currentRotation;
    }
}
