Shader "Custom/CRT_Image"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ScanlinePeriod ("Scanline Period", Range(0, 1500.0)) = 480.0
        _ScanlineMinValue ("Scanline Min Value", Range(0, 1)) = 0.0
        _ScanlineSpeed ("Scanline Speed", Range(0, 400)) = 1.0
        _InterlacingBool ("Interlacing", Range(0, 1)) = 0.0
        _VignetteExponent ("Vignette Exponent", Range(0, 4)) = 1
        _VignetteFactor ("Vignette Factor", Range(0, 2)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float _ScanlinePeriod;
            float _ScanlineMinValue;
            float _ScanlineSpeed;
            float _InterlacingBool;
            float _VignetteExponent;
            float _VignetteFactor;

            v2f vert (appdata_full v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                return o;
            }

            // draw scanlines over the image
            float4 ApplyScanline(float4 color, float2 uv)
            {
                //make each second line black
                float scanlineFactor = (sin(uv.y * _ScanlinePeriod*3.14159265359) + 1.0) * 0.5;

                // Invert scanline factor based on the interlacing bool
                if(_InterlacingBool > 0.5f)
                {
                    scanlineFactor = 1 - scanlineFactor;
                }

                // Clamp the scanline value to [0, 1] using lerp
                scanlineFactor = lerp(_ScanlineMinValue, 1.0f, step(0.5f, scanlineFactor));

                // Apply scanline factor to the color
                color.rgb *= scanlineFactor;

                return color;
            }

            // apply a vignette effect to the image
            float4 ApplyVignette(float4 color, float2 uv)
            {
                float2 position = uv * 2.0 - 1.0;
                float dist = length(position);
                dist = pow(dist, _VignetteExponent) * _VignetteFactor;
                color.rgb *= 1.0 - dist;
                return color;
            }

            float4 frag (v2f i) : SV_Target
            {
                float4 color = tex2D(_MainTex, i.uv);
                color = ApplyScanline(color, i.uv);
                color = ApplyVignette(color, i.uv);
                return color;
            }
            ENDHLSL
        }
    }
    FallBack "Diffuse"
}
