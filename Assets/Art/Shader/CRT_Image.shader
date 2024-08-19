Shader "Custom/CRT_Image"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BorderThickness ("Border Thickness", Range(0, 0.02)) = 0.01
        _HorizontalScanlines ("Horizontal Scanlines", Range(0, 1500.0)) = 480.0
        _VerticalScanlines ("Vertical Scanlines", Range(0, 1500.0)) = 960.0
        _HorizontalMinValue ("Horizontal Min Value", Range(0, 1)) = 0.0
        _VerticalMinValue ("Vertical Min Value", Range(0, 1)) = 0.0
        _ScanlineSpeed ("Scanline Speed", Range(0, 400)) = 1.0
        _InterlacingBool ("Interlacing", Range(0, 1)) = 0.0
        _ChromaticAberrationFactor ("Chromatic Aberration Factor", Range(0, 0.1)) = 0.05
        _ChromaticAberrationExponent ("Chromatic Aberration Exponent", Range(0, 2)) = 1.0
        _ChromaticAberrationStrength ("Chromatic Aberration Strength", Range(0, 1)) = 0.5
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
            float _BorderThickness;
            float _HorizontalScanlines;
            float _VerticalScanlines;
            float _HorizontalMinValue;
            float _VerticalMinValue;
            float _ScanlineSpeed;
            float _InterlacingBool;
            float _VignetteExponent;
            float _VignetteFactor;
            float _ChromaticAberrationFactor;
            float _ChromaticAberrationExponent;
            float _ChromaticAberrationStrength;

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

                // Invert scanline factor based on the interlacing bool
                if(_InterlacingBool > 0.5f)
                {
                    horizontalFactor = 1 - horizontalFactor;
                }

                // Clamp the scanline value to [0, 1] using lerp
                horizontalFactor = lerp(_HorizontalMinValue, 1.0f, step(0.5f, horizontalFactor));
                verticalFactor = lerp(_VerticalMinValue, 1.0f, step(0.5f, verticalFactor));

                // Apply scanline factor to the color
                color.rgb *= horizontalFactor * verticalFactor;

                return color;
            }

            float4 ApplyChromaticAberration(float4 color, float2 uv)
            {
                float2 position = uv * 2.0 - 1.0;
                float dist = length(position);
                float factor = pow(dist, _ChromaticAberrationExponent) * _ChromaticAberrationFactor;

                //move positionb ack to Range [0, 1]
                //position = (position + 1.0) * 0.5;

                //move uv coordinates of each channel by a different amount
                float2 uvRed = uv + position * factor; 
                float2 uvGreen = uv;                           
                float2 uvBlue = uv - position * factor;

                //sample the texture at the new uv coordinates
                float4 colorRed = tex2D(_MainTex, uvRed);
                float4 colorGreen = tex2D(_MainTex, uvGreen);
                float4 colorBlue = tex2D(_MainTex, uvBlue);

                float4 newColor = float4(0.0, 0.0, 0.0, 1.0);

                //combine the three channels
                newColor.r = lerp(color.r, colorRed.r, _ChromaticAberrationStrength);
                newColor.g = lerp(color.g, colorGreen.g, _ChromaticAberrationStrength);
                newColor.b = lerp(color.b, colorBlue.b, _ChromaticAberrationStrength);

                return lerp(color, newColor, _ChromaticAberrationStrength);
            }

            float4 DrawBorder(float4 color, float2 uv)
            {
                //get screen ratio
                float screenRatio = _ScreenParams.x / _ScreenParams.y;

                //look if the pixel is in the border
                float borderX = step(uv.x, _BorderThickness) + step(1.0 - _BorderThickness, uv.x);
                float borderY = step(uv.y, _BorderThickness * screenRatio) + step(1.0 - _BorderThickness * screenRatio, uv.y);

                //combine both arguments
                float isBorder = step(0.5, borderX + borderY);

                //return white if the pixel is in the border
                return lerp(color, float4(1.0, 1.0, 1.0, 1.0), isBorder);
                
            }

            float4 frag (v2f i) : SV_Target
            {
                float4 color = tex2D(_MainTex, i.uv);
                color = ApplyScanlines(color, i.uv);
                color = ApplyChromaticAberration(color, i.uv);
                color = DrawBorder(color, i.uv);
                return color;
            }
            ENDHLSL
        }
    }
    FallBack "Diffuse"
}
