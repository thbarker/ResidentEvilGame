Shader "Custom/ItemShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Cutoff ("Alpha Cutoff", Range(0,1)) = 0.4

        // Glint settings
        _GlintColor ("Glint Color", Color) = (1,1,1,1)
        _GlintWidth ("Glint Width", Range(0.01, 1)) = 0.1
        _GlintStrength ("Glint Strength", Range(0, 1)) = 0.5
        _GlintSpeed ("Glint Speed", Range(0, 10)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="TransparentCutout" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows alpha:clip
        #pragma target 3.0

        sampler2D _MainTex;
        half _Glossiness;
        half _Metallic;
        half _Cutoff;
        fixed4 _Color;

        fixed4 _GlintColor;
        float _GlintWidth;
        float _GlintStrength;
        float _GlintSpeed;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos; // Needed for glint world-space effect
        };

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;

            // Alpha clip
            clip(c.a - _Cutoff);

            // Glint mask calculation
            float glint = abs(frac((IN.worldPos.x + IN.worldPos.y + _Time.y * _GlintSpeed)) - 0.5); // Diagonal line
            glint = smoothstep(0.0, _GlintWidth, _GlintWidth - glint); // Soft edge

            // Add glint color
            float3 glintedColor = lerp(c.rgb, _GlintColor.rgb, glint * _GlintStrength);

            o.Albedo = glintedColor;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Transparent/Cutout/VertexLit"
}
