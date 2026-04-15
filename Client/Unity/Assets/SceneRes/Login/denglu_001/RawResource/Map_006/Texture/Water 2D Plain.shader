// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Water/Water 2D Plain"
{
	Properties 
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color ("Color", Color) = (1, 1, 1, 0.5)
		[Normal] _Refraction ("Refraction", 2D) = "bump" {}
		_Intensity ("Refraction Intensity", Float) = 0.02
		_Current ("Current Speed", Float) = -0.25
		_AlphaTex("Alpha Texture(A)", 2D) = "white"{}
	}
	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			//"PreviewType" = "Plane"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		//GrabPass { }
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			//#include "Water2D.cginc"
			uniform float4 _Color;
			uniform float4 _MainTex_ST;
			uniform sampler2D _MainTex;

			uniform float _Intensity;
			uniform sampler2D _Refraction;
			uniform float4 _Refraction_ST;
			uniform float _Current;
			uniform sampler2D _AlphaTex;
			struct appdata_t 
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};
			
			struct v2f 
			{
				float4 vertex : SV_POSITION;
				float2 texcoord : TEXCOORD0;
			};
			
			v2f vert (appdata_t IN)
			{
				v2f OUT;
				OUT.texcoord = IN.texcoord;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				return OUT;
			}
			
			fixed4 frag(v2f IN) : SV_Target
			{
				fixed alpha = tex2D(_AlphaTex, IN.texcoord).a;				
				IN.texcoord.x += _Current * _Time.y;
				fixed4 texColor = tex2D(_MainTex, TRANSFORM_TEX(IN.texcoord, _MainTex));
				fixed4 c = texColor * _Color;
				c.rgb *= c.a;
				c.rgb *= alpha;
				c.a *= alpha;
				return c;
			}
			ENDCG
		}
	}
}
