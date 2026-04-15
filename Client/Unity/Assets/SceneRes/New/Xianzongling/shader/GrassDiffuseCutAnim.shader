
Shader "Valkyrie/Toon/Scene/Other-GrassDiffuseCutAnim"
{
	Properties
	{
		_MainTex("Main Tex", 2D) = "white" {}
		_AlphaCutoff("Alpha Cut Off", Float) = 0.9
		_Color("Main Color", Color) = (1,1,1,1)
		_LightPower("Power of Light", Float) = 1.0
		_Dist("Dist", float) = 0.04
		_Speed("Speed", float) = 0.2
		_SpeedOffset("SpeedOffset", float) = 0.2
		_Random("Random", float) = 1
	}

	SubShader
	{
		Tags { "IgnoreProjector" = "True" "RenderType" = "Opaque" }
		Pass
		{
			ZWrite		On
			Cull		Off
			Lighting 	Off

			CGPROGRAM
			#pragma vertex			vert
			#pragma fragment		frag
			#pragma multi_compile_fwdbase

			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"

			sampler2D		_MainTex;
			half4			_MainTex_ST;
			half			_AlphaCutoff;

			half4			_Color;
			half			_LightPower;
			half			_Dist;
			half			_Speed;
			half			_SpeedOffset;
			half			_Random;
			struct v2f
			{
				float4	pos		: POSITION;
				float2	uv		: TEXCOORD0;
			};

			v2f vert(appdata_full v)
			{
				float4 vertex = mul(v.vertex, unity_ObjectToWorld);
				float a = vertex.x * vertex.z;
				float fa = fmod(a, 2);
				v.vertex.xyz += float3(sin(a), 0, cos(a)) *_Dist * _Random * sin(_Time.w * (_Speed + _SpeedOffset*(1 - fa)) + a);

				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}


			float4 frag(v2f i) : COLOR
			{
				half4 fCol = tex2D(_MainTex, float2(i.uv.x, i.uv.y));

				half4 c;
				c.rgb = fCol.rgb * _Color * _LightPower;
				c.a = fCol.a;

				if (c.a < _AlphaCutoff)
				{
					discard;
				}

				UNITY_OPAQUE_ALPHA(c.a);
				return c;
			}

			ENDCG
		}
	}
	Fallback "Transparent/VertexLit"
}
