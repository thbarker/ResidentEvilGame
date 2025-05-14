Shader"Custom/PulseMonitorShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint Color", Color) = (1,1,1,1)   // <- Add this line
        _Speed ("Pulse Speed", Float) = 1.0
        _Width ("Pulse Width", Float) = 0.02
        _Sharpness ("Right Edge Sharpness", Float) = 0.005
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" "CanvasUI"="true" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

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
            float4 _MainTex_ST;
            float4 _Color; // <- Declare tint color
            float _Speed;
            float _Width;
            float _Sharpness;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float xLine = frac(_Time.y * _Speed);
                float dx = i.uv.x - xLine;

                dx = dx - floor(dx + 0.5); // wrap to [-0.5, 0.5]

                float alphaPulse = 0.0;

                if (dx < 0.0)
                {
                    alphaPulse = 1.0 - smoothstep(0.0, _Width, -dx);
                }
                else
                {
                    alphaPulse = 1.0 - smoothstep(0.0, _Sharpness, dx);
                }

                fixed4 col = tex2D(_MainTex, i.uv) * _Color; // <- Multiply by tint color
                col.a *= saturate(alphaPulse);
                return col;
            }
            ENDCG
        }
    }
}
