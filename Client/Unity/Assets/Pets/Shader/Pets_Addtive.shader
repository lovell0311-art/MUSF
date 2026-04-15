
Shader "ASE/Pets_Addtive"
{
	Properties
	{
		_MainTex("MainTex", 2D) = "white" {}
		[HDR]_MainColor("MainColor", Color) = (1,1,1,1)
		_MainTexSpeed("MainTexSpeed", Vector) = (0,0,0,0)
		_Mask("Mask", 2D) = "white" {}
		_MaskSpeed("MaskSpeed", Vector) = (0,0,0,0)
		_TwistTex("TwistTex", 2D) = "white" {}
		_TwistSpeed("TwistSpeed", Vector) = (0,0,0,0)
		_TwistIntensity("TwistIntensity", Range( 0 , 0.2)) = 0

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
		Cull Off
		ColorMask RGBA
		ZWrite Off
		ZTest LEqual
		Offset 0 , 0
		
		
		
		Pass
		{
			Name "Unlit"
			//Tags { "LightMode"="ForwardBase" }
			HLSLPROGRAM

			

			#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
			//only defining to not throw compilation error over Unity 5.5
			#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
			#endif
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			//#include "UnityCG.cginc"
			//#include "UnityShaderVariables.cginc"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#define ASE_NEEDS_FRAG_COLOR


			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 worldPos : TEXCOORD0;
				#endif
				float4 ase_color : COLOR;
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
                float4 _MainColor;          // 主颜色
                float4 _MainTexSpeed;       // 主纹理滚动速度
                float4 _MainTex_ST;         // 主纹理 UV 变换
                float4 _MaskSpeed;          // Mask 滚动速度
                float4 _Mask_ST;            // Mask UV 变换
                float4 _TwistSpeed;         // Twist 纹理滚动速度
                float4 _TwistTex_ST;        // Twist UV 变换
                float _TwistIntensity;      // Twist 强度
            CBUFFER_END

			TEXTURE2D(_MainTex);        // 主纹理
            SAMPLER(sampler_MainTex);  // 主纹理采样器
            TEXTURE2D(_Mask);          // Mask 纹理
            SAMPLER(sampler_Mask);     // Mask 纹理采样器
            TEXTURE2D(_TwistTex);      // Twist 纹理
            SAMPLER(sampler_TwistTex); // Twist 纹理采样器

			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				o.ase_color = v.color;
				o.ase_texcoord1.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord1.zw = 0;
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
				o.vertex = TransformObjectToHClip(v.vertex);

				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				#endif
				return o;
			}
			
			half4 frag (v2f i ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
				half4 finalColor;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 WorldPosition = i.worldPos;
				#endif
				float2 uv_MainTex = i.ase_texcoord1.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				float2 panner7 = ( 1.0 * _Time.y * _MainTexSpeed + uv_MainTex);
				float2 uv_TwistTex = i.ase_texcoord1.xy * _TwistTex_ST.xy + _TwistTex_ST.zw;
				float2 panner24 = ( 1.0 * _Time.y * _TwistSpeed + uv_TwistTex);
				//float2 temp_cast_0 = (tex2D( _TwistTex, panner24 ).r).xx;
				float2 temp_cast_0 = (SAMPLE_TEXTURE2D(_TwistTex, sampler_TwistTex, panner24).r).xx;
				float2 lerpResult21 = lerp( panner7 , temp_cast_0 , _TwistIntensity);
				float4 temp_output_4_0 = ( i.ase_color * _MainColor * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, lerpResult21) );
				float2 uv_Mask = i.ase_texcoord1.xy * _Mask_ST.xy + _Mask_ST.zw;
				float2 panner16 = ( 1.0 * _Time.y * _MaskSpeed + uv_Mask);
				
				
				finalColor = ( temp_output_4_0 * ( temp_output_4_0.a * SAMPLE_TEXTURE2D(_Mask, sampler_Mask, panner16).r ) );
				return finalColor;
			}
			ENDHLSL
		}
	}
	CustomEditor "ASEMaterialInspector"
}




