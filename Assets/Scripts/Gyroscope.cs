using UnityEngine;

public class Gyroscope : MonoBehaviour
{
    public Quaternion Rotation;

    void Update()
    {
        Rotation = transform.rotation;
    }
}