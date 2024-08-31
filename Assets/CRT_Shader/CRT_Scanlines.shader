Shader "Custom/CRT_Image"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Rows ("Horizontal Scanlines", Range(0, 2000.0)) = 480.0
        _Cols ("Vertical Scanlines", Range(0, 1500.0)) = 960.0
        _RowStrength ("Horizontal Strength", Range(0, 1)) = 0.0
        _ColStrength ("Vertical Strength", Range(0, 1)) = 0.0
        _Interlace ("Interlacing", Range(0, 1)) = 0.0
        _InterlaceFrequency ("Interlacing Frequency", Range(0, 120)) = 50.0
        _Offset ("Scanline Offset", Range(0, 4)) = 1.0
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
            float _Rows;
            float _Cols;
            float _RowStrength;
            float _ColStrength;
            float _Interlace;
            float _Offset;
            

            v2f vert (appdata_full v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                return o;
            }

            // draw scanlines over the image
            float4 ApplyScanlines(float2 uv)
            {
                // Calculate the horizontal and vertical scanline factors based on the UV coordinates
                // 0 == Scanline, 1 == No Scanline
                float horizontalFactor = (sin(uv.y * _Rows*3.14159265359) + 1.0) * 0.5;
                float verticalFactor = (sin(uv.x * _Cols*3.14159265359) + 1.0) * 0.5;

                // if horizontal factor == 1, move UV to the right by 1/1920 of the screen width
                float texelSize = 1/1920.0;
                float4 color = tex2D(_MainTex, uv + step(0.5, horizontalFactor) * float2(texelSize*_Offset, 0.0f));

                // Apply interlacing effect by inverting the horizontal factor when the interlace factor is greater than 0.5
                horizontalFactor = lerp(horizontalFactor, 1.0f - horizontalFactor, step(0.5f, _Interlace));
                
                // Clamp the scanline value to [1-Strength, 1] range
                horizontalFactor = lerp(1.0f - _RowStrength, 1.0f, step(0.5f, horizontalFactor));
                verticalFactor = lerp(1.0f - _ColStrength, 1.0f, step(0.5f, verticalFactor));

                // Apply scanline factor to the color
                color.rgb *= horizontalFactor * verticalFactor;

                return color;
            }

            float4 frag (v2f i) : SV_Target
            {
                return ApplyScanlines(i.uv);
            }
            ENDHLSL
        }
    }
    FallBack "Diffuse"
}
