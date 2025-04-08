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
            float3 worldPos;
            float3 viewDir; // View direction in world space
        };

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;

            // Alpha clip
            clip(c.a - _Cutoff);

            // Get view direction in world space
            float3 viewDir = normalize(IN.viewDir);

            // Create a diagonal direction in view space (top-left to bottom-right sweep)
            float3 diagonalDir = normalize(float3(1, 1, 0));

            // Project the world position onto the view-aligned diagonal direction
            float sweepValue = dot(IN.worldPos, diagonalDir);

            // Move the glint line along time
            float movingLine = frac(sweepValue + _Time.y * _GlintSpeed);
            float glintDist = abs(movingLine - 0.5);
            float glint = smoothstep(0.0, _GlintWidth, _GlintWidth - glintDist);

            // Toggle glint on/off every other interval
            // float glintToggle = fmod(floor(_Time.y * _GlintSpeed), 2);
            // glint *= step(1.0, glintToggle + 0.01);

            // Blend in glint color
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
