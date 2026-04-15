// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
//Shader "ASE/Water001"
//{
//	Properties
//	{
//		_WaterColor("WaterColor", Color) = (1,1,1,1)
//		_MainTex("MainTex", 2D) = "white" {}
//		_WaterSpeed("WaterSpeed", Vector) = (0,0,0,0)
//		_NoiseTex("NoiseTex", 2D) = "white" {}
//		_NoiseSpeed("NoiseSpeed", Vector) = (0,-0.2,0,0)
//		_NoiseScale("NoiseScale", Range( 0 , 1)) = 0.5
//		_AlphaTex("AlphaTex", 2D) = "white" {}

//	}
	
//	SubShader
//	{
		
		
//		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
//	LOD 100

//		CGINCLUDE
//		#pragma target 3.0
//		ENDCG
//		Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha
//		AlphaToMask Off
//		Cull Back
//		ColorMask RGBA
//		ZWrite On
//		ZTest LEqual
//		Offset 0 , 0
		
		
		
//		Pass
//		{
//			Name "Unlit"
//			//Tags { "LightMode"="ForwardBase" }
//			CGPROGRAM

//			#define ASE_ABSOLUTE_VERTEX_POS 1


//			#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
//			//only defining to not throw compilation error over Unity 5.5
//			#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
//			#endif
//			#pragma vertex vert
//			#pragma fragment frag
//			#pragma multi_compile_instancing
//			#include "UnityCG.cginc"
//			#include "UnityShaderVariables.cginc"


//			struct appdata
//			{
//				float4 vertex : POSITION;
//				float4 color : COLOR;
//				float4 ase_texcoord : TEXCOORD0;
//				UNITY_VERTEX_INPUT_INSTANCE_ID
//			};
			
//			struct v2f
//			{
//				float4 vertex : SV_POSITION;
//				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
//				float3 worldPos : TEXCOORD0;
//				#endif
//				float4 ase_texcoord1 : TEXCOORD1;
//				UNITY_VERTEX_INPUT_INSTANCE_ID
//				UNITY_VERTEX_OUTPUT_STEREO
//			};

//			uniform float4 _WaterColor;
//			uniform sampler2D _MainTex;
//			uniform sampler2D _NoiseTex;
//			uniform float2 _NoiseSpeed;
//			uniform float4 _NoiseTex_ST;
//			uniform float _NoiseScale;
//			uniform float2 _WaterSpeed;
//			uniform float4 _MainTex_ST;
//			uniform sampler2D _AlphaTex;

			
//			v2f vert ( appdata v )
//			{
//				v2f o;
//				UNITY_SETUP_INSTANCE_ID(v);
//				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
//				UNITY_TRANSFER_INSTANCE_ID(v, o);

//				o.ase_texcoord1.xy = v.ase_texcoord.xy;
				
//				//setting value to unused interpolator channels and avoid initialization warnings
//				o.ase_texcoord1.zw = 0;
//				float3 vertexValue = float3(0, 0, 0);
//				#if ASE_ABSOLUTE_VERTEX_POS
//				vertexValue = v.vertex.xyz;
//				#endif
//				vertexValue = vertexValue;
//				#if ASE_ABSOLUTE_VERTEX_POS
//				v.vertex.xyz = vertexValue;
//				#else
//				v.vertex.xyz += vertexValue;
//				#endif
//				o.vertex = UnityObjectToClipPos(v.vertex);

//				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
//				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
//				#endif
//				return o;
//			}
			
//			fixed4 frag (v2f i ) : SV_Target
//			{
//				UNITY_SETUP_INSTANCE_ID(i);
//				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
//				fixed4 finalColor;
//				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
//				float3 WorldPosition = i.worldPos;
//				#endif
//				float2 uv_NoiseTex = i.ase_texcoord1.xy * _NoiseTex_ST.xy + _NoiseTex_ST.zw;
//				float2 panner25 = ( 1.0 * _Time.y * _NoiseSpeed + uv_NoiseTex);
//				float2 uv_MainTex = i.ase_texcoord1.xy * _MainTex_ST.xy + _MainTex_ST.zw;
//				float2 panner16 = ( 1.0 * _Time.y * _WaterSpeed + uv_MainTex);
//				float4 temp_output_3_0 = ( _WaterColor * tex2D( _MainTex, ( ( tex2D( _NoiseTex, panner25 ).r * _NoiseScale ) + panner16 ) ) );
//				float2 texCoord43 = i.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
//				float4 appendResult33 = (float4(temp_output_3_0.rgb , ( (temp_output_3_0).a * tex2D( _AlphaTex, texCoord43 ).r )));
				
				
//				finalColor = appendResult33;
//				return finalColor;
//			}
//			ENDCG
//		}
//	}
//	CustomEditor "ASEMaterialInspector"
	
	
//}



Shader "ASE/Water001"
{
    Properties
    {
        _WaterColor("WaterColor", Color) = (1,1,1,1)
        _MainTex("MainTex", 2D) = "white" {}
        _WaterSpeed("WaterSpeed", Vector) = (0,0,0,0)
        _NoiseTex("NoiseTex", 2D) = "white" {}
        _NoiseSpeed("NoiseSpeed", Vector) = (0,-0.2,0,0)
        _NoiseScale("NoiseScale", Range(0, 1)) = 0.5
        _AlphaTex("AlphaTex", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" "LightMode" = "UniversalForward" }
        LOD 100

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

            // 使用 CBUFFER 声明材质属性，兼容 URP Batcher
            CBUFFER_START(UnityPerMaterial)
                float4 _WaterColor;          // 水的颜色
                float4 _WaterSpeed;          // 水的速度
                float4 _NoiseSpeed;          // 噪声速度
                float _NoiseScale;           // 噪声缩放
                float4 _MainTex_ST;          // 主纹理 UV 变换
                float4 _NoiseTex_ST;         // 噪声纹理 UV 变换
            CBUFFER_END

            TEXTURE2D(_MainTex);        // 主纹理
            SAMPLER(sampler_MainTex);  // 主纹理采样器
            TEXTURE2D(_NoiseTex);      // 噪声纹理
            SAMPLER(sampler_NoiseTex); // 噪声纹理采样器
            TEXTURE2D(_AlphaTex);      // Alpha 纹理
            SAMPLER(sampler_AlphaTex); // Alpha 纹理采样器

            struct Attributes
            {
                float4 positionOS : POSITION; // 顶点位置
                float2 uv : TEXCOORD0;        // UV 坐标
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION; // 裁剪空间位置
                float2 uv : TEXCOORD0;           // UV 坐标
            };

            // 顶点着色器
            Varyings vert(Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz); // 将物体空间位置转换为裁剪空间
                o.uv = v.uv;
                return o;
            }

            // 片段着色器
            half4 frag(Varyings i) : SV_Target
            {
                // 噪声纹理 UV 计算
                float2 uv_NoiseTex = i.uv * _NoiseTex_ST.xy + _NoiseTex_ST.zw;
                float2 noisePanner = _NoiseSpeed.xy * _Time.y + uv_NoiseTex;

                // 主纹理 UV 计算
                float2 uv_MainTex = i.uv * _MainTex_ST.xy + _MainTex_ST.zw;
                float2 mainTexPanner = _WaterSpeed.xy * _Time.y + uv_MainTex;

                // 从噪声纹理和主纹理采样
                float noiseValue = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, noisePanner).r;
                float4 mainTexColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, noiseValue * _NoiseScale + mainTexPanner);

                // 采样 Alpha 纹理
                float alphaValue = SAMPLE_TEXTURE2D(_AlphaTex, sampler_AlphaTex, i.uv).r;

                // 合成最终颜色
                float4 finalColor = float4(mainTexColor.rgb, mainTexColor.a * alphaValue) * _WaterColor;

                return finalColor;
            }
            ENDHLSL
        }
    }

    CustomEditor "ASEMaterialInspector"
}

