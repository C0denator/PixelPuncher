Shader "Custom/CRT_Curvature"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BorderThickness ("Border Thickness", Range(0, 0.02)) = 0.01
        _CurvatureCenter ("Curvature Center", Range(0, 1)) = 0.5
        _CurvatureEdge ("Curvature Edge", Range(0, 1)) = 0.5
        _VignetteExponent ("Vignette Exponent", Range(0, 4)) = 1
        _VignetteFactor ("Vignette Factor", Range(0, 2)) = 1
    }    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            HLSLPROGRAM
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
            float _BorderThickness;
            float _CurvatureCenter;
            float _CurvatureEdge;
            float _VignetteExponent;
            float _VignetteFactor;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            // manipulate the UV coordinates to apply a curvature effect
            float2 ApplyCurvature(float2 uv)
            {
                uv = uv * 2.0 - 1.0; // Transform UV coordinates from [0, 1] to [-1, 1]
                float dist = length(uv); // Calculate the distance from the center
                float theta = atan2(uv.y, uv.x);// Calculate the angle from the center
                float curvature = lerp(_CurvatureCenter, _CurvatureEdge, dist); //Lerp the curvature based on the distance from the center
                dist = pow(dist, 1.0 + curvature * 0.5); // Increase the distance from the center by a power function
                uv = dist * float2(cos(theta), sin(theta)); // Apply the curvature effect by changing the UV coordinates in direction of the angle
                uv = (uv + 1.0) * 0.5; // Transform UV coordinates back to [0, 1] for texture sampling
                return uv;
            }

            // apply a vignette effect to the image
            float4 ApplyVignette(float4 color, float2 uv)
            {
                // calculate the distance from the center of the screen
                float2 position = uv * 2.0 - 1.0;
                float dist = length(position);
                
                //increase the distance from the center by a power function
                dist = pow(dist, _VignetteExponent) * _VignetteFactor;
                
                //apply the vignette effect
                color.rgb *= 1.0 - dist;
                
                return color;
            }

            float4 DrawBorder(float4 color, float2 uv)
            {
                //get screen ratio
                float screenRatio = _ScreenParams.x / _ScreenParams.y;

                //get curved UV
                float2 curvedUV = ApplyCurvature(uv);

                //look if the pixel is in the border
                float borderX = step(curvedUV.x, _BorderThickness) + step(1.0 - _BorderThickness, curvedUV.x);
                float borderY = step(curvedUV.y, _BorderThickness * screenRatio) + step(1.0 - _BorderThickness * screenRatio, curvedUV.y);

                //combine both arguments
                float isBorder = step(0.5, borderX + borderY);

                //return white if the pixel is in the border
                return lerp(color, float4(1.0, 1.0, 1.0, 1.0), isBorder);
                
            }

            float4 frag(v2f i) : SV_Target
            {
                float2 uv = ApplyCurvature(i.uv);

                float4 color = tex2D(_MainTex, uv);

                color = DrawBorder(color, i.uv);

                color = ApplyVignette(color, i.uv);

                //  discard pixels outside the texture
                float mask = step(0.0, uv.x) * step(uv.x, 1.0) * step(0.0, uv.y) * step(uv.y, 1.0);
                color.rgb *= mask; // color to black if outside the texture
                
                return color;
            }
            ENDHLSL
        }
    }
    FallBack "Diffuse"
}