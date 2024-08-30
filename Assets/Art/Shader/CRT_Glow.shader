Shader "Custom/CRT_Glow"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        
        _Strength ("Glow Strength", Range(0, 1)) = 0.5
        _Radius ("Glow Radius", Range(0, 10)) = 5.0
        _Threshold ("Glow Threshold", Range(0,3)) = 1.0
        _f ("Glow Factor", Range(0, 1)) = 0.25
        _e ("Glow Exponent", Range(0, 15)) = 5
        _a ("Glow Additive", Range(-1, 1)) = 0.0
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
            float _Strength;
            float _Radius;
            float _Threshold;
            float _f;
            float _e;
            float _a;
            

            v2f vert (appdata_full v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                return o;
            }

            

            float4 ApplyGlow(float4 color, float2 uv)
            {
                if(_Radius == 0) return color;
                
                // pixelSize = 1/1080 of the screen width
                float2 pixelSize = 1/1080.0;

                float4 averageColor = float4(0.0, 0.0, 0.0, 0.0);
                int sampleCount = 0;
                float distance = 0;
                float weight = 0;

                //Iterate over a diamond shape around the pixel
                for(int y = -_Radius; y <= _Radius; y++)
                {
                    //calc amount of iterations for this row
                    int xMax = _Radius - abs(y);

                    for (int x = -xMax; x <= xMax; x++)
                    {
                        //sample the pixel
                        float2 offset = float2(x, y) * pixelSize;
                        float4 sample = tex2D(_MainTex, uv + offset);

                        //calculate the distance to the center; the closer the pixel, the more weight it has
                        distance = length(float2(x, y)) + 1;
                        weight = pow((distance- _a) * _f, -_e) ;

                        //clamp the weight to [0, 1]
                        weight = saturate(weight);

                        //add the sample to the average color
                        averageColor += sample * weight * _Strength;
                        sampleCount++;
                    }
                }
                
                //Calculate the average color
                averageColor /= sampleCount;
                

                float isGlowColor = step(_Threshold, color.r + color.g + color.b);
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
