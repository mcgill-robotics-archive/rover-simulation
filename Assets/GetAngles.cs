using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetAngles : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    public float getPitch()
    {
        Vector3 right = transform.right;
        right.y = 0;
        right *= Mathf.Sign(transform.up.y);
        Vector3 fwd = Vector3.Cross(right, Vector3.up).normalized;
        return Vector3.SignedAngle(fwd, transform.forward, Vector3.right); // maybe replace Vector3.right with transform.right

    }

    public float getRoll()
    {
        Vector3 forward = transform.forward;
        forward.y = 0;
        forward *= Mathf.Sign(transform.up.y);
        Vector3 right = Vector3.Cross(Vector3.up, forward).normalized;
        return Vector3.SignedAngle(right, transform.right, Vector3.forward); // maybe replace Vector3.forward with transform.forward
        
    }

    public float getYaw()
    {
        Vector3 right = transform.right;
        right.y = 0;
        Vector3 forward = Vector3.Cross(right, Vector3.up).normalized;
        return Vector3.SignedAngle(forward, Vector3.forward, Vector3.up);   // maybe replace Vector3.up with trasnform.up
    }

    public float getX()
    {
        return transform.position.x;
    }

    public float getY()
    {
        return transform.position.z;
    }

    public float getZ()
    {
        return transform.position.y;
    }

}
