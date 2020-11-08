using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class TerrainBehavior : MonoBehaviour
{
    static readonly IRandom<float> s_Random = new NativeRandom(100);

    void Start()
    {
        for (int i = 0; i < 1000; i++)
        {
            GameObject obj = Instantiate(GameObject.Find("RockTemplate"));
            obj.transform.localScale = new Vector3(200.0f * (s_Random.Next() + 0.1f), 200.0f * (s_Random.Next() + 0.1f), 200.0f * (s_Random.Next() + 0.1f));
            obj.transform.parent = transform;
            Vector3 rockSize = obj.GetComponent<MeshRenderer>().bounds.size;
            float boundingBoxVolume = rockSize.x * rockSize.y * rockSize.z;
            Assert.IsTrue(boundingBoxVolume > 0.0f);
            obj.GetComponent<Rigidbody>().mass = boundingBoxVolume;
            obj.name = $"Rock{i}";
        }
    }

}
