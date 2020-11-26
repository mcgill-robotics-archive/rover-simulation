using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

[ExecuteAlways]
public class DepthCamera : MonoBehaviour
{
    private Camera m_DepthCamera;
    private RenderTexture m_RenderTexture;
    public Material ShaderMaterial;
    public RenderTexture DepthTexture;

    void Start()
    {
        //m_RenderTexture = new RenderTexture(400, 300, 1);
        //m_DepthTexture = new RenderTexture(400, 300, 1);
        m_DepthCamera = GetComponent<Camera>();
        m_DepthCamera.depthTextureMode = DepthTextureMode.Depth;
        m_RenderTexture = m_DepthCamera.targetTexture;
    }

    void Update()
    {
        Graphics.Blit(m_RenderTexture, DepthTexture, ShaderMaterial);
        //Graphics.Blit(m_RenderTexture, null, ShaderMaterial);
    }
}
