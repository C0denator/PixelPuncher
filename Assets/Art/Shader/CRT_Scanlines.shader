Shader "Custom/CRT_Image"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _HorizontalScanlines ("Horizontal Scanlines", Range(0, 1500.0)) = 480.0
        _VerticalScanlines ("Vertical Scanlines", Range(0, 1500.0)) = 960.0
        _HorizontalStrength ("Horizontal Strength", Range(0, 1)) = 0.0
        _VerticalStrength ("Vertical Strength", Range(0, 1)) = 0.0
        _InterlacingBool ("Interlacing", Range(0, 1)) = 0.0
        _ScanlineOffset ("Scanline Offset", Range(0, 4)) = 1.0
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
            float _HorizontalScanlines;
            float _VerticalScanlines;
            float _HorizontalStrength;
            float _VerticalStrength;
            float _InterlacingBool;
            float _ScanlineOffset;
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
            float4 ApplyScanlines(float4 color, float2 uv)
            {
                float horizontalFactor = (sin(uv.y * _HorizontalScanlines*3.14159265359) + 1.0) * 0.5;
                float verticalFactor = (sin(uv.x * _VerticalScanlines*3.14159265359) + 1.0) * 0.5;

                // if horizontal factor == 1, move pixel to the right by 1/1920 of the screen width
                float texelSize = _ScreenParams.x / pow(1920.0, 2.0);
                color = tex2D(_MainTex, uv + step(0.5, horizontalFactor) * float2(texelSize*_ScanlineOffset, 0.0f));

                // Invert scanline factor based on the interlacing bool
                if(_InterlacingBool > 0.5f)
                {
                    horizontalFactor = 1 - horizontalFactor;
                }

                // Clamp the scanline value to [0, 1] using lerp
                horizontalFactor = lerp(1.0f - _HorizontalStrength, 1.0f, step(0.5f, horizontalFactor));
                verticalFactor = lerp(1.0f - _VerticalStrength, 1.0f, step(0.5f, verticalFactor));

                // Apply scanline factor to the color
                color.rgb *= horizontalFactor * verticalFactor;

                return color;
            }

            float4 frag (v2f i) : SV_Target
            {
                float4 color = tex2D(_MainTex, i.uv);
                
                color = ApplyScanlines(color, i.uv);
                
                return color;
            }
            ENDHLSL
        }
    }
    FallBack "Diffuse"
}
