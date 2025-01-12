Shader "Custom/CRT_Chrom"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ChromaticAberrationFactor ("Chromatic Aberration Factor", Range(0, 0.01)) = 0.005
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

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            float _ChromaticAberrationFactor;
            float _ChromaticAberrationExponent;
            float _ChromaticAberrationStrength;

            float4 ApplyChromaticAberration(float4 color, float2 uv)
            {
                //calculate the distance from the center of the screen
                float2 position = uv * 2.0 - 1.0;
                float dist = length(position);
                float strength = pow(dist* _ChromaticAberrationFactor, _ChromaticAberrationExponent) ;

                //move uv coordinates of each channel by a different amount
                float2 uvRed = uv + position * strength;  //red channel to the outside   
                float2 uvGreen = uv;                      //green channel stays in the center
                float2 uvBlue = uv - position * strength; //blue channel to the inside

                //sample the texture at the new uv coordinates
                float4 colorRed = tex2D(_MainTex, uvRed);
                float4 colorGreen = tex2D(_MainTex, uvGreen);
                float4 colorBlue = tex2D(_MainTex, uvBlue);

                float4 newColor = float4(0.0, 0.0, 0.0, 1.0);

                //combine the three channels
                newColor.r = colorRed.r;
                newColor.g = colorGreen.g;
                newColor.b = colorBlue.b;

                //apply the strength factor and return the new color
                return lerp(color, newColor, _ChromaticAberrationStrength);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 color = tex2D(_MainTex, i.uv);
                
                return ApplyChromaticAberration(color, i.uv);
            }
            ENDHLSL
        }
    }
    FallBack "Diffuse"
}
