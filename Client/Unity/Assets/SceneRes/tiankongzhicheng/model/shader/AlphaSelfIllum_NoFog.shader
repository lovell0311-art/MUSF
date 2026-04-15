// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//Shader "AlphaSelfIllum_NoFog" {
//Properties {
//    _Color ("Main Color", Color) = (1,1,1,1)
//    _MainTex ("Texture", 2D) = "white" { }
//}
//SubShader {
//	Tags { "Queue" = "Transparent" }
//    Pass {
		
//		//Tags { "LightMode" = "Vertex" }
//        Fog { Mode Off }
		
//		Cull Back
//        Blend SrcAlpha OneMinusSrcAlpha 
        
		
//		CGPROGRAM
//		#pragma vertex vert
//		#pragma fragment frag
		
//		#include "UnityCG.cginc"
		
//		float4 _Color;
//		sampler2D _MainTex;
			
//		struct v2f {
//		    float4  pos : SV_POSITION;
//		    float2  uv : TEXCOORD0;
//		};
		
//		float4 _MainTex_ST;
		
//		v2f vert (appdata_base v)
//		{
//		    v2f o;
//		    o.pos = UnityObjectToClipPos (v.vertex);
//		    o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
//		    return o;
//		}
		
//		half4 frag (v2f i) : COLOR
//		{
//		    half4 texcol = tex2D (_MainTex, i.uv);
//		    return texcol * _Color;
//		}
//		ENDCG
//    }
//}
//Fallback off
//} 


Shader "AlphaSelfIllum_NoFog" 
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType"="Transparent" "LightMode"="UniversalForward" }
        Pass
        {
            Tags { "LightMode" = "UniversalForward" }
            
            Cull Back
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;          // 主颜色
                float4 _MainTex_ST;     // UV 的缩放和平移参数
            CBUFFER_END

            //float4 _Color;
            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
            //float4 _MainTex_ST;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION; // 裁剪空间位置
                float2 uv : TEXCOORD0;           // UV 坐标
            };

            Varyings vert(Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz); // 转换到裁剪空间
                o.uv = v.uv * _MainTex_ST.xy + _MainTex_ST.zw;
                return o;
            }

            half4 frag(Varyings i) : SV_Target
            {
                // 从纹理采样颜色
                half4 texcol = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                return texcol * _Color;
            }

            ENDHLSL
        }
    }
    Fallback Off
}
