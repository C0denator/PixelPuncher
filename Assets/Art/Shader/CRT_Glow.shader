Shader "Custom/CRT_Glow"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        
        _GlowStrength ("Glow Strength", Range(0, 1)) = 0.5
        _GlowRadius ("Glow Radius", Range(0, 10)) = 5.0
        _GlowThreshold ("Glow Threshold", Range(0,3)) = 1.0
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
            float _GlowStrength;
            float _GlowRadius;
            float _GlowThreshold;
            

            v2f vert (appdata_full v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                return o;
            }

            

            float4 ApplyGlow(float4 color, float2 uv)
            {
                if(_GlowRadius == 0) return color;
                
                // Sample neighboring pixels
                float2 texelSize = 1.0 / _ScreenParams.xy;

                float4 averageColor = float4(0.0, 0.0, 0.0, 0.0);
                int sampleCount = 0;
                float distance = 0.0;
                float weight = 0.0;

                //Iterate over a diamond shape around the pixel
                for(int y = -_GlowRadius; y <= _GlowRadius; y++)
                {
                    //calc amount of iterations for this row
                    int xMax = _GlowRadius - abs(y);

                    for (int x = -xMax; x <= xMax; x++)
                    {
                        //sample the pixel
                        float2 offset = float2(x, y) * texelSize;
                        float4 sample = tex2D(_MainTex, uv + offset);

                        //calculate the distance to the center; the closer the pixel, the more weight it has
                        distance = length(float2(x, y));
                        weight = 1.0 - distance / _GlowRadius;

                        //add the sample to the average color
                        averageColor += sample * weight * _GlowStrength;
                        sampleCount++;
                    }
                }
                
                //Calculate the average color
                averageColor /= sampleCount;
                

                float isGlowColor = step(_GlowThreshold, color.r + color.g + color.b);
                return lerp(averageColor, color, isGlowColor);
                
            }
            
            float4 frag (v2f i) : SV_Target
            {
                float4 color = tex2D(_MainTex, i.uv);
                
                color = ApplyGlow(color, i.uv);
                return color;
            }
            ENDHLSL
        }
    }
    FallBack "Diffuse"
}
