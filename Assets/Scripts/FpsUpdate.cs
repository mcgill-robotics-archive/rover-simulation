using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FpsUpdate : MonoBehaviour
{
    private GameObject m_FpsText;

    void Start()
    {
        m_FpsText = GameObject.Find("FpsDisplay");
        InvokeRepeating("UpdateFps", 0.5f, 0.5f);
    }

    void UpdateFps()
    {
        float ts = Time.deltaTime;
        m_FpsText.GetComponent<Text>().text = $"{(int)(1.0f / ts)} FPS";
    }
}