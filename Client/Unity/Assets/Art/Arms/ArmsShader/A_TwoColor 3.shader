// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ASE/A_TwoColor 3"
{
	Properties
	{
		[IntRange]_EffectIntensity("EffectIntensity", Range( 1 , 15)) = 15
		_FlashSpeed("FlashSpeed", Float) = 1
		_MainTex("MainTex", 2D) = "white" {}
		_MainTexSpeed("MainTexSpeed", Vector) = (0,0,0,0)
		_AddtiveTex("AddtiveTex", 2D) = "white" {}
		[HDR]_AddtiveColor("AddtiveColor", Color) = (1,0.6428995,0,0)
		_AddtiveSpeed("AddtiveSpeed", Vector) = (0,0,0,0)
		_TwistTex("TwistTex", 2D) = "black" {}
		_TwistSpeed("TwistSpeed", Vector) = (0.1,0.1,0,0)
		_TwistScale("TwistScale", Range( 0 , 0.2)) = 0.05600879
		[Enum(UnityEngine.Rendering.CullMode)]_CullMode("CullMode", Float) = 0
		[HDR]_46Color1("4-6Color1", Color) = (3.890196,1.929412,0.7843137,0)
		[HDR]_46Color2("4-6Color2", Color) = (0.5176471,1.411765,3.482353,0)
		_79Color2("7-9Color2", Color) = (0.1071556,0.2724442,0.5283019,0)
		_79Color1("7-9Color1", Color) = (0.01779993,0.04205022,0.1509434,0)
		_1012Color1("10-12Color1", Color) = (0.1886792,0.09813809,0.03648984,0)
		_1012Color2("10-12Color2", Color) = (0.5471698,0.2325777,0.05420078,0)
		[HDR]_1315Color1("13-15Color1", Color) = (0.1871753,0.6239178,1.06066,0)
		[HDR]_1315Color2("13-15Color2", Color) = (1.5,0.9647059,0,0)

	}
	
	SubShader
	{
		
		
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
	LOD 100

		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend One One
		AlphaToMask Off
		Cull [_CullMode]
		ColorMask RGBA
		ZWrite On
		ZTest LEqual
		Offset 0 , 0
		
		
		
		Pass
		{
			Name "Unlit"
			//Tags { "LightMode"="ForwardBase" }


			CGPROGRAM

			

			#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
			//only defining to not throw compilation error over Unity 5.5
			#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
			#endif
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"
			#include "UnityShaderVariables.cginc"


			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 worldPos : TEXCOORD0;
				#endif
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			uniform float _CullMode;
			uniform float4 _46Color1;
			uniform float _EffectIntensity;
			uniform float4 _79Color1;
			uniform float4 _1012Color1;
			uniform float4 _1315Color1;
			uniform float4 _46Color2;
			uniform float4 _79Color2;
			uniform float4 _1012Color2;
			uniform float4 _1315Color2;
			uniform float _FlashSpeed;
			uniform sampler2D _MainTex;
			uniform float2 _MainTexSpeed;
			uniform float4 _MainTex_ST;
			uniform float _TwistScale;
			uniform sampler2D _TwistTex;
			uniform float2 _TwistSpeed;
			uniform sampler2D _AddtiveTex;
			uniform float2 _AddtiveSpeed;
			uniform float4 _AddtiveTex_ST;
			uniform float4 _AddtiveColor;

			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				o.ase_texcoord1.xy = v.ase_texcoord.xy;
				o.ase_texcoord1.zw = v.ase_texcoord1.xy;
				float3 vertexValue = float3(0, 0, 0);
				#if ASE_ABSOLUTE_VERTEX_POS
				vertexValue = v.vertex.xyz;
				#endif
				vertexValue = vertexValue;
				#if ASE_ABSOLUTE_VERTEX_POS
				v.vertex.xyz = vertexValue;
				#else
				v.vertex.xyz += vertexValue;
				#endif
				o.vertex = UnityObjectToClipPos(v.vertex);

				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				#endif
				return o;
			}
			
			fixed4 frag (v2f i ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
				fixed4 finalColor;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 WorldPosition = i.worldPos;
				#endif
				float temp_output_47_0 = (0.0 + (_EffectIntensity - 1.0) * (1.0 - 0.0) / (15.0 - 1.0));
				float temp_output_49_0 = step( 0.15 , temp_output_47_0 );
				float4 lerpResult50 = lerp( float4( 0,0,0,0 ) , _46Color1 , temp_output_49_0);
				float temp_output_51_0 = step( 0.4 , temp_output_47_0 );
				float4 lerpResult53 = lerp( lerpResult50 , _79Color1 , temp_output_51_0);
				float temp_output_56_0 = step( 0.6 , temp_output_47_0 );
				float4 lerpResult60 = lerp( lerpResult53 , _1012Color1 , temp_output_56_0);
				float temp_output_61_0 = step( 0.85 , temp_output_47_0 );
				float4 lerpResult65 = lerp( lerpResult60 , _1315Color1 , temp_output_61_0);
				float4 lerpResult48 = lerp( float4( 0,0,0,0 ) , _46Color2 , temp_output_49_0);
				float4 lerpResult52 = lerp( lerpResult48 , _79Color2 , temp_output_51_0);
				float4 lerpResult57 = lerp( lerpResult52 , _1012Color2 , temp_output_56_0);
				float4 lerpResult62 = lerp( lerpResult57 , _1315Color2 , temp_output_61_0);
				float4 lerpResult3_g5 = lerp( lerpResult65 , lerpResult62 , (0.0 + (sin( ( _Time.y * _FlashSpeed ) ) - -1.0) * (1.0 - 0.0) / (1.0 - -1.0)));
				float2 uv_MainTex = i.ase_texcoord1.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				float2 panner5 = ( 1.0 * _Time.y * _MainTexSpeed + uv_MainTex);
				float2 texCoord2_g8 = i.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float2 panner5_g8 = ( 1.0 * _Time.y * _TwistSpeed + texCoord2_g8);
				float4 temp_output_68_0 = ( _TwistScale * tex2D( _TwistTex, panner5_g8 ) );
				float2 uv2_AddtiveTex = i.ase_texcoord1.zw * _AddtiveTex_ST.xy + _AddtiveTex_ST.zw;
				float2 panner21 = ( 1.0 * _Time.y * _AddtiveSpeed + uv2_AddtiveTex);
				
				
				finalColor = saturate( ( ( ( lerpResult3_g5 * tex2D( _MainTex, ( float4( panner5, 0.0 , 0.0 ) + temp_output_68_0 ).rg ) ) + ( tex2D( _AddtiveTex, ( float4( panner21, 0.0 , 0.0 ) + temp_output_68_0 ).rg ) * _AddtiveColor * step( 0.95 , temp_output_47_0 ) ) ) * (0.3 + (_EffectIntensity - 1.0) * (1.5 - 0.3) / (15.0 - 1.0)) ) );
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
