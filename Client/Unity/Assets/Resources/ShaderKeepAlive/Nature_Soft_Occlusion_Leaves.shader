Shader "Nature/Soft Occlusion Leaves"
{
    Properties
    {
        _Color("Main Color", Color) = (1,1,1,1)
        _MainTex("Base (RGB) Alpha (A)", 2D) = "white" {}
        _Cutoff("Alpha cutoff", Range(0, 1)) = 0.33
    }

    SubShader
    {
        Tags { "Queue" = "AlphaTest" "IgnoreProjector" = "True" "RenderType" = "TransparentCutout" }
        LOD 200
        Cull Off

        CGPROGRAM
        #pragma surface surf Lambert alphatest:_Cutoff addshadow vertex:vert

        sampler2D _MainTex;
        fixed4 _Color;
        half _Cutoff;

        struct Input
        {
            float2 uv_MainTex;
            fixed4 color : COLOR;
        };

        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.color = v.color;
        }

        void surf(Input IN, inout SurfaceOutput o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            fixed3 vertexTint = max(IN.color.rgb, fixed3(0.35, 0.35, 0.35));
            o.Albedo = c.rgb * vertexTint;
            o.Alpha = saturate(c.a * max(IN.color.a, 0.35));
        }
        ENDCG
    }

    FallBack "Legacy Shaders/Transparent/Cutout/VertexLit"
}
