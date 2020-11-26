using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RockBehavior : MonoBehaviour
{
    static readonly IRandom<float> s_Random = new NativeRandom(100);

    private void Start()
    {
        // randomize position
        GameObject terrain = GameObject.Find("Terrain");
        Vector3 size = terrain.GetComponent<MeshRenderer>().bounds.size;
        Vector2 posRock;
        posRock.x = terrain.transform.position.x + size.x / 2.0f * (s_Random.Next() * 2.0f - 1.0f) * 0.8f;
        posRock.y = terrain.transform.position.z + size.z / 2.0f * (s_Random.Next() * 2.0f - 1.0f) * 0.8f;

        transform.position = new Vector3(posRock.x, 25.0f, posRock.y);

        Destroy(this, 10.0f);
    }

    private void Update()
    {
        if (transform.position.y < -5.0f)
            Destroy(gameObject);
    }
}