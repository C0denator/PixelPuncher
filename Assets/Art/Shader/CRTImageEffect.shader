Shader "Custom/CRTShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _CurvatureCenter ("Curvature Center", Range(0, 1)) = 0.5
        _CurvatureEdge ("Curvature Edge", Range(0, 1)) = 0.5
        _VignetteIntensity ("Vignette Intensity", Range(0, 1)) = 0.5
        _VignetteSmoothness ("Vignette Smoothness", Range(0, 1)) = 0.5
        _ScanlineIntensity ("Scanline Intensity", Range(0, 1)) = 0.5
        _ScanlineSpeed ("Scanline Speed", Range(0, 10)) = 1.0
        _ScanlineTime ("_ScanlineTime", Float) = 3.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _CurvatureCenter;
            float _CurvatureEdge;
            float _Curvature;
            float _VignetteIntensity;
            float _VignetteSmoothness;
            float _ScanlineIntensity;
            float _ScanlineSpeed;
            float _ScanlineTime;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;

                // Apply CRT curvature with separate parameters for center and edges
                uv = uv * 2.0 - 1.0; // Transform UV coordinates from [0, 1] to [-1, 1]
                float dist = length(uv);
                float theta = atan2(uv.y, uv.x);
                
                // Mix between center curvature and edge curvature
                float curvature = lerp(_CurvatureCenter, _CurvatureEdge, dist);
                dist = pow(dist, 1.0 + curvature * 0.5); // Radial distortion
                
                uv = dist * float2(cos(theta), sin(theta));
                uv = (uv + 1.0) * 0.5; // Transform UV coordinates back to [0, 1]

                // Prevent sampling outside texture coordinates
                if (uv.x < 0.0 || uv.x > 1.0 || uv.y < 0.0 || uv.y > 1.0)
                {
                    return float4(0, 0, 0, 1);
                }

                float4 color = tex2D(_MainTex, uv);

                // Apply vignette effect
                float2 position = i.uv - 0.5;
                float vignetteDist = length(position);
                float vignette = smoothstep(0.5 - _VignetteSmoothness, 0.5 + _VignetteSmoothness, 1.0 - vignetteDist * _VignetteIntensity);
                color.rgb *= vignette;

                // Apply scanline effect
                float scanline = sin((i.uv.y + _ScanlineTime * _ScanlineSpeed) * 100.0) * 0.5 + 0.5;
                color.rgb *= lerp(1.0, scanline, _ScanlineIntensity);

                return color;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}