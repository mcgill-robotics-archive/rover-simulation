Shader "DepthTest"
{
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }

        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            #include "UnityCG.cginc"

            // vertex shader inputs
            struct appdata
            {
                float4 vertex : POSITION; // vertex position
                float2 uv : TEXCOORD0; // texture coordinate
            };

    // vertex shader outputs ("vertex to fragment")
    struct v2f
    {
        float2 uv : TEXCOORD0; // texture coordinate
        float4 vertex : SV_POSITION; // clip space position
        float4 scrPos : TEXCOORD1;
    };

    // vertex shader
    v2f vert(appdata v)
    {
        v2f o;
        o.vertex = UnityObjectToClipPos(v.vertex);
        o.uv = v.uv;
        return o;
    }

    sampler2D _CameraDepthTexture;

    // pixel shader; returns low precision ("fixed4" type)
    // color ("SV_Target" semantic)
    float4 frag(v2f input) : SV_Target
    {

        //float4 colCameraDepth = tex2D(_CameraDepthTexture, float2(i.uv.x , i.uv.y));
        //float r = colCameraDepth.r;
        //r = 1 - Linear01Depth(r);
        //colCameraDepth = fixed4(r,0,0,1);

        //return float4(r, r, r, 1);
        float4 val = tex2D(_CameraDepthTexture, float2(input.uv.x * 10, input.uv.y * 10));
        //float outVal = 0;
        //for (int i = 0; i < 10; i++)
        //{
        //    for (int j = 0; j < 10; j++)
        //    {
        //        float4 depthTex = tex2D(_CameraDepthTexture, float2(i, j));
        //        if (depthTex.r != val)
        //        {
        //            outVal = 1;
        //        }
        //    }
        //}
        return val;
    }
    ENDCG
}
    }
}