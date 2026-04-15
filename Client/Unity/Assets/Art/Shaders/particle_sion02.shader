Shader "Particle_Sion02"
{
	Properties
	{
		[Header(BASE)] [Space]
		[HDR]_TintColor("Tint Color", Color) = (1.0,1.0,1.0,1.0)
		_MainTex ("Texture", 2D) = "white" {}
		[Toggle(_MAINSPEEDAUTOCUSTOMXY_ON)] _MainSpeedAutoCustomXY("MainSpeedAuto/Custom(XY)", Float) = 0.0
		_MainSpeedX  ("Main Speed X", float) = 0.0
		_MainSpeedY  ("Main Speed X", float) = 0.0

		[Header(DISTORTION)] [Space]
		[Toggle(_DISTORTION_ON)] _EnableDistortion("Enable Distortion", Float) = 0.0
		[Toggle(_DISTORTIONCUSTOMZ_ON)] _EnableDistortionCustomZ("DistortionInt/Cusmtom(Z)", Float) = 0.0
		_DistortionMap ("Distortion Tex", 2D) = "white" {}
		_DistortionSpeedU("DistortionSpeedU", Float) = 0.0
		_DistortionSpeedV("DistortionSpeedV", Float) = 0.0
		_DistortionPowerX  ("Distortion Power X", range (0.0,1.0)) = 1.0
		_DistortionPowerY  ("Distortion Power Y", range (0.0,1.0)) = 1.0
		_DistortionPower("Distort Power", Range( 0.0 , 1.0)) = 0.2

		[Header(DISSOLVE)] [Space]
		[Toggle(_DISSOLVE_ON)] _EnableDissolve("Enable Dissolve", Float) = 0.0
		[Toggle(_DISSOLVESLIDERCUSTOMW_ON)] _DissolveSliderCusmtomW("DissolveSlider/Cusmtom(W)", Float) = 0.0
		[Toggle] _DissolveWithAlpha("DissolveWithAlpha", Float) = 0.0
		_DissolveMap("DissolveMap", 2D) = "white"{}
		_DissolveSpeedU("DissolveSpeedU", Float) = 0.0
		_DissolveSpeedV("DissolveSpeedV", Float) = 0.0
		_DissolveGradiant("Dissolve Gradiant", Float) = 1.0
		_DissolveSmoothness("Dissolve Smoothness", Range(0.0, 0.99)) = 0.0
		_DissolveRange("DissolveRangge", Range(0.0 , 1.0)) = 0.0

		[Space]
		[Enum(UVMask,0.0,NOMask,1.0)]_DissolveMask("Dissolve Mask", Float) = 1.0
		[Enum(U,0,V,1)]_DissolveMaskUV("Dissolve Mask_U/V", Float) = 0.0
		[Toggle] _DissolveMaskUVFlip ("Flip Dissolve Mask_U/V",int) = 0.0
		_DissolveMaskNoisePower("Dissolve Mask_NoisePower", Range( 0.0 , 1.0)) = 0.2

		[Space]
		[Toggle(_DISSOLVE_EDGE_ON)] _EnableDissolveEdge ("Edge",int) = 0.0
		_DissolveEdgeWidth ("DissolveEdgeWidth", Range(0,1)) = 0.2 
		[HDR]_DissolveEdgeColor ("DissolveEdgeColor",color) = (1,1,1,1)

		[Header(MASK)] [Space]
		[Toggle(_ENABLE_MASK)] _EnableMask("Enable Mask", Float) = 0.0
		[Toggle(_MASK_ALPHA_CHANNEL)] _MaskAlphaChannel("Use Alpha Channel as Mask", Float) = 0
		_MaskTex("Mask Tex", 2D) = "white" {}
		_MaskTexSpeedU("MaskTexSpeedU", Float) = 0.0
		_MaskTexSpeedV("MaskTexSpeedV", Float) = 0.0

		[Header(SOFTPARTICLE)] [Space]
		[Toggle(_ENABLE_SOFTPARTICLE)] _EnableSoftParticle("Enable SoftParticle", Float) = 0.0
		_Distance("Distance", Range( 0.0, 1.0)) = 0.0

		[Header(RIM)] [Space]
		[Toggle(_ENABLE_RIM)]_EnableRim("EnableRim", float) = 0
		[HDR]_RimColor("Rim Color", Color) = (1,1,1,1)
		_RimPower("Rim power", Range(-5,10.0)) = 3.0
		_RimScale("Rim Scale", Range(0.0,10.0)) = 1.0
		_RimBias("RimBias", Range(0.0,1.0)) = 0.0
		[Toggle(_ENABLE_ONEMINUE_RIM)]_OneMinusRim("OneMinusRim", float) = 0
		[Toggle(_ENABLE_EMSSION_ADD)]_RimAadd("RimAdd", float) = 0

		[Header(StencilBuffer)] 
		[HideInInspector]_StencilComp ("Stencil Comparison", Float) = 8
		[HideInInspector]_Stencil ("Stencil ID", Float) = 0
		[HideInInspector]_StencilOp ("Stencil Operation", Float) = 0
		[HideInInspector]_StencilWriteMask ("Stencil Write Mask", Float) = 255
		[HideInInspector]_StencilReadMask ("Stencil Read Mask", Float) = 255


		[Header(RenderingMode)] [Space] 
		[MaterialEnum(LessEqual,4,Always,10)] _ZTest("ZtestMode",int) = 4
		[MaterialEnum(One,1, OneMinusSrcAlpha, 10)]_BlendMode("BlendMode", Int) = 10

		[Enum(UnityEngine.Rendering.CullMode)] _Cull("Cull Mode", Float) = 2 

	}
	SubShader
	{
		Tags { 
			"Queue"="Transparent" "RenderType"="Transparent"
		}
		ZWrite Off 
		Blend SrcAlpha [_BlendMode]
		ZTest[_ZTest]
		Cull[_Cull]

		Stencil
		{
			Ref [_Stencil]
			Comp [_StencilComp]
			Pass [_StencilOp]
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
		}

		

		Pass
		{
			Name "Unlit"
			//Tags { "LightMode"="ForwardBase" }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma shader_feature_local _MAINSPEEDAUTOCUSTOMXY_ON
			#pragma shader_feature_local _DISTORTION_ON
			#pragma shader_feature_local _DISTORTIONCUSTOMZ_ON
			#pragma shader_feature_local _DISSOLVE_ON
			#pragma shader_feature_local _DISSOLVESLIDERCUSTOMW_ON
			#pragma shader_feature_local _DISSOLVE_EDGE_ON
			#pragma shader_feature_local _ENABLE_MASK
			#pragma shader_feature_local _ENABLE_SOFTPARTICLE
			#pragma shader_feature_local _MASK_ALPHA_CHANNEL
			#pragma shader_feature_local _ENABLE_EMSSION_ADD
			#pragma shader_feature_local _ENABLE_ONEMINUE_RIM
			#pragma shader_feature_local _ENABLE_RIM



			#include "UnityCG.cginc"

			sampler2D _MainTex;        
			sampler2D _DistortionMap;  
			sampler2D _DissolveMap;	
			sampler2D _MaskTex;   
			UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );     

			half4 _TintColor;
			half4 _MainTex_ST;
			half _MainSpeedX;
			half _MainSpeedY;

			half4 _DistortionMap_ST;
			half _DistortionSpeedU;
			half _DistortionSpeedV;
			half _DistortionPowerX;
			half _DistortionPowerY;
			half _DistortionPower;

			half _DissolveWithAlpha;
			half _DissolveRange;
			half4 _DissolveMap_ST;
			half _DissolveSpeedU;
			half _DissolveSpeedV;
			half _DissolveSmoothness;
			half _DissolveGradiant;
			half _DissolveMask;
			half _DissolveMaskUV;
			half _DissolveMaskUVFlip;
			half _DissolveMaskNoisePower;
			half _DissolveEdgeWidth;
			half4 _DissolveEdgeColor;

			half4 _MaskTex_ST;
			half _MaskTexSpeedU;
			half _MaskTexSpeedV;
			float4 _ClipRect;

			float4 _RimColor;
			half _RimPower;
			half _RimScale;
			half _RimBias;

			half _Distance;

			struct appdata
			{
				float4 positionOS : POSITION;
				float2 texcoord   : TEXCOORD0;
				float4 uv2 : TEXCOORD1;
				half4 color      : COLOR;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 positionCS : SV_POSITION;
				float4 texcoord   : TEXCOORD0;
				half4 color      : COLOR;

				float4 texcoord1  : TEXCOORD1;

				float4 uv2 : TEXCOORD2;

				float3 positionWS : TEXCOORD3;

				float4 ScreenPos : TEXCOORD4;
				float3 normal : TEXCOORD5;

			};


			v2f vert(appdata v)
			{
				v2f o;
				UNITY_INITIALIZE_OUTPUT(v2f,o); 

				o.positionCS = UnityObjectToClipPos(v.positionOS);

				#if defined(_ENABLE_SOFTPARTICLE)
					o.ScreenPos = ComputeScreenPos(o.positionCS);
				#endif

				o.positionWS = mul(unity_ObjectToWorld, v.positionOS).xyz;
				o.normal = UnityObjectToWorldNormal(v.normal);

				#if defined(_MAINSPEEDAUTOCUSTOMXY_ON)
					o.texcoord.xy = TRANSFORM_TEX(v.texcoord, _MainTex) + v.uv2.xy;
				#else
					o.texcoord.xy = TRANSFORM_TEX(v.texcoord, _MainTex) + frac(float2(_MainSpeedX, _MainSpeedY) * _Time.y);
				#endif

				#if defined(_DISSOLVE_ON)
					o.texcoord.zw = TRANSFORM_TEX(v.texcoord, _DissolveMap) + frac(float2(_DissolveSpeedU, _DissolveSpeedV) * _Time.y); 
				#endif

				#if defined(_DISTORTION_ON)
					o.texcoord1.xy = TRANSFORM_TEX(v.texcoord, _DistortionMap) + frac(float2(_DistortionSpeedU, _DistortionSpeedV) * _Time.y);
				#endif

				#if defined(_ENABLE_MASK)
					o.texcoord1.zw = TRANSFORM_TEX(v.texcoord, _MaskTex) + frac(float2(_MaskTexSpeedU, _MaskTexSpeedV) * _Time.y);
				#endif

				o.uv2.xy = v.texcoord.xy;
				o.uv2.zw = v.uv2.zw;

				o.color = v.color;


				return o;
			}

			half4 frag(v2f i) : SV_Target
			{
				float2 mainUV = i.texcoord.xy;

				#if defined(_DISSOLVE_ON)
					float2 DissUV = i.texcoord.zw;
				#endif

				#if defined(_DISTORTION_ON)
					half4 DistortTex = tex2D(_DistortionMap, i.texcoord1.xy);
					half offset1 = (DistortTex.r - 0.5) * _DistortionPowerX;
					half offset2 = (DistortTex.g - 0.5) * _DistortionPowerY;
					float2 DistortUV = float2 (offset1,offset2);
					#if defined(_DISTORTIONCUSTOMZ_ON)
						float2 DistoreUVBias = DistortUV * i.uv2.z;
					#else
						float2 DistoreUVBias = DistortUV * _DistortionPower;
					#endif
					mainUV = i.texcoord.xy + DistoreUVBias;
					#if defined(_DISSOLVE_ON)
						DissUV = i.texcoord.zw + DistoreUVBias;
					#endif
				#endif

				half4 color = tex2D(_MainTex, mainUV) * lerp(i.color,half4(i.color.rgb,1.0),_DissolveWithAlpha) * _TintColor;

				#if defined(_DISSOLVE_ON)
					half Dissnoise = lerp(_DissolveMaskNoisePower,0.0,_DissolveMask);;
					float dissmode = ( 2.0 - _DissolveSmoothness ) * (1.0 + Dissnoise);

					#if defined(_DISSOLVESLIDERCUSTOMW_ON) 
						half dissolveThreshold = i.uv2.w * dissmode;
					#else
						half dissolveThreshold = lerp(_DissolveRange,1.0-i.color.a,_DissolveWithAlpha) * dissmode;
					#endif

					half Dissmask01 = 0.0;
					half Dissnoise01 = 1.0;

					half Dissmask02 = lerp(i.uv2.x , i.uv2.y , _DissolveMaskUV);
					Dissmask02 = lerp(Dissmask02, 1 - Dissmask02, _DissolveMaskUVFlip);
					half Dissnoise02 = _DissolveMaskNoisePower;

					half disstex = pow(tex2D(_DissolveMap, DissUV).r, _DissolveGradiant) * lerp(Dissnoise02,Dissnoise01,_DissolveMask) + lerp(Dissmask02,Dissmask01,_DissolveMask) + 1.0;

					#if defined(_DISSOLVE_EDGE_ON)
						half DissValue = disstex - ( 1.0 + _DissolveEdgeWidth ) * dissolveThreshold;
						half diss01 = saturate((DissValue  - _DissolveSmoothness) / (1.0 - _DissolveSmoothness));
						color.rgb = lerp( _DissolveEdgeColor.rgb * i.color , color.rgb , diss01);
						float DissValue02 = DissValue + _DissolveEdgeWidth;
					#else
						float DissValue02 = disstex - dissolveThreshold;
					#endif
					half Diss02 = saturate((DissValue02  - _DissolveSmoothness) / (1.0 - _DissolveSmoothness));

					color.a *= Diss02;
				#endif

				#if defined(_ENABLE_MASK)
					half4 maskMap = tex2D(_MaskTex, i.texcoord1.zw);
					#if defined(_MASK_ALPHA_CHANNEL)
						half mask = maskMap.a;
					#else
						half mask = maskMap.r;
					#endif
					color.a *= mask;
				#endif

				#if defined(_ENABLE_SOFTPARTICLE)
					float4 scpos = i.ScreenPos;
					float4 screenPosNorm = scpos / scpos.w;
					screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? screenPosNorm.z : screenPosNorm.z * 0.5 + 0.5;
					float screenDepth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, screenPosNorm.xy ));
					float distanceDepth = saturate(abs( ( screenDepth - LinearEyeDepth( screenPosNorm.z ) ) / ( _Distance ) ));


					color.a *= distanceDepth;
				#endif
				#ifdef _ENABLE_RIM
					float3 WorldPosition = i.positionWS;
					float3 WorldVieDir = normalize(UnityWorldSpaceViewDir(WorldPosition));
					float3 WorldNormal = i.normal;
					half rim = saturate(dot(WorldNormal, WorldVieDir));

					#ifdef _ENABLE_ONEMINUE_RIM
						rim = 1.0 - rim;
					#endif

					half finrim = (_RimBias + _RimScale * pow( 1.0 - rim, _RimPower));
					half4 finrimcol = finrim * _RimColor;

					#ifdef _ENABLE_EMSSION_ADD
						color.rgb += finrimcol;
					#else
						color.a *= finrim;
					#endif
				#endif

				return color;
			}
			ENDCG
		}
	}

}
