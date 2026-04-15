Shader "Nature/Soft Occlusion Bark Rendertex"
{
    Properties
    {
        _Color("Main Color", Color) = (1,1,1,1)
        _MainTex("Base (RGB) Alpha (A)", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "Queue" = "Geometry" "RenderType" = "Opaque" "IgnoreProjector" = "True" }
        LOD 200
        Cull Back

        CGPROGRAM
        #pragma surface surf Lambert vertex:vert addshadow

        sampler2D _MainTex;
        fixed4 _Color;

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
            o.Alpha = 1;
        }
        ENDCG
    }

    FallBack "Legacy Shaders/VertexLit"
}
