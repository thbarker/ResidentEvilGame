Shader "Custom/ItemShaderUnlit"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Cutoff ("Alpha Cutoff", Range(0,1)) = 0.5

        _GlintColor ("Glint Color", Color) = (1,1,1,1)
        _GlintWidth ("Glint Width", Range(0.01, 1)) = 0.1
        _GlintStrength ("Glint Strength", Range(0,1)) = 0.5
        _GlintSpeed ("Glint Speed", Range(0,10)) = 1
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _Cutoff;


            float4 _GlintColor;
            float _GlintWidth;
            float _GlintStrength;
            float _GlintSpeed;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 screenPos : TEXCOORD1;
                float3 worldNormal : TEXCOORD2;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.screenPos = o.vertex;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 screenUV = i.screenPos.xy / i.screenPos.w;
                screenUV = screenUV * 0.5 + 0.5;

                float4 texColor = tex2D(_MainTex, i.uv) * _Color;

                // Alpha clip
                clip(texColor.a - _Cutoff);

                // Directional light (Unity _WorldSpaceLightPos0 is a direction if w = 0)
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                float3 normal = normalize(i.worldNormal);

                // Lambert diffuse term
                float NdotL = saturate(dot(normal, lightDir));
                //texColor.rgb *= NdotL;

                float glintCoord = screenUV.x + screenUV.y;
                float glint = abs(frac(glintCoord + _Time.y * _GlintSpeed) - 0.5);
                glint = smoothstep(0.0, _GlintWidth, _GlintWidth - glint);

                float3 finalColor = lerp(texColor.rgb, _GlintColor.rgb, glint * _GlintStrength);

                return float4(finalColor, texColor.a);
            }
            ENDCG
        }
    }
}
