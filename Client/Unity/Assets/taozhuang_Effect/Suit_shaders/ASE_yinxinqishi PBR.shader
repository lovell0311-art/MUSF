// Upgrade NOTE: replaced 'defined FOG_COMBINED_WITH_WORLD_POS' with 'defined (FOG_COMBINED_WITH_WORLD_POS)'

// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ASE/yinxinqishi_PBR"
{
	Properties
	{
		[Enum(Off,0,On,1)]_Stage_1("Stage_1", Float) = 0
		[Enum(Off,0,On,1)]_Stage_2("Stage_2", Float) = 0
		[Enum(OFF,0,On,1)]_Stage_3("Stage_3", Float) = 0
		_Stage_3_SweepSpeed("Stage_3_SweepSpeed", Float) = 1
		_Stage_3_JianGe("Stage_3_JianGe", Float) = 5
		_Metallic("Metallic", Range( 0 , 1)) = 0
		_Smothness("Smothness", Range( 0 , 1)) = 1
		[Enum(UnityEngine.Rendering.CullMode)]_CullMode("CullMode", Float) = 0
		_Color2("Color2", Color) = (0.1249153,0.00711997,0.3018868,1)
		_Color1("Color1", Color) = (0.4245283,0.2346271,0.09011215,1)
		_MainTexIntensity("MainTexIntensity", Float) = 1
		_MainTexSaturation("MainTexSaturation", Range( 0 , 1)) = 1
		_MainTex("MainTex", 2D) = "white" {}
		_MainMask("MainMask", 2D) = "white" {}
		[HDR]_NoiseTexColor("NoiseTexColor", Color) = (1,1,1,1)
		_NoiseTex("NoiseTex", 2D) = "white" {}
		_Noise_U_Speed("Noise_U_Speed", Float) = 0
		_Noise_V_Speed("Noise_V_Speed", Float) = 0
		_NoiseTex2("NoiseTex2", 2D) = "white" {}
		[HDR]_NoiseTexColor2("NoiseTexColor2", Color) = (1,1,1,1)
		_Noise2_U_Speed("Noise2_U_Speed", Float) = 0
		_Noise2_V_Speed("Noise2_V_Speed", Float) = 0
		[HDR]_AddtiveLightColor("AddtiveLightColor", Color) = (1,1,1,1)
		_AddtiveLightTex("AddtiveLightTex", 2D) = "black" {}
		_TwistIntensity("TwistIntensity", Range( 0 , 0.5)) = 0.2
		_TwistTex("TwistTex", 2D) = "white" {}
		_TwistSpeed("TwistSpeed", Vector) = (0,0,0,0)
		_NoiseTwist("NoiseTwist", Float) = 0.3
		[HideInInspector] _texcoord2( "", 2D ) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull [_CullMode]
		Blend SrcAlpha OneMinusSrcAlpha
		
		#LINE 138

		
	// ------------------------------------------------------------
	// Surface shader code generated out of a CGPROGRAM block:
	

	// ---- forward rendering base pass:
	Pass {
		Name "FORWARD"
		//Tags { "LightMode" = "ForwardBase" }

CGPROGRAM
// compile directives
#pragma vertex vert_surf
#pragma fragment frag_surf
#pragma target 3.0
#pragma multi_compile_instancing
#pragma multi_compile_fog
#pragma multi_compile_fwdbase
#include "HLSLSupport.cginc"
#define UNITY_INSTANCED_LOD_FADE
#define UNITY_INSTANCED_SH
#define UNITY_INSTANCED_LIGHTMAPSTS
#include "UnityShaderVariables.cginc"
#include "UnityShaderUtilities.cginc"
// -------- variant for: <when no other keywords are defined>
#if !defined(INSTANCING_ON)
// Surface shader code generated based on:
// writes to per-pixel normal: no
// writes to emission: YES
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: YES
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 2 texcoords actually used
//   float2 _texcoord
//   float2 _texcoord2
#include "UnityCG.cginc"
#include "AutoLight.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 47

		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		//#pragma target 3.0
		struct Input
		{
			float2 uv_texcoord;
			float2 uv2_texcoord2;
		};

		uniform float _CullMode;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform float4 _Color1;
		uniform float4 _Color2;
		uniform float _Stage_2;
		uniform float _MainTexSaturation;
		uniform float _MainTexIntensity;
		uniform float _Stage_1;
		uniform float4 _NoiseTexColor;
		uniform sampler2D _NoiseTex;
		uniform float _Noise_U_Speed;
		uniform float _Noise_V_Speed;
		uniform float4 _NoiseTex_ST;
		uniform sampler2D _TwistTex;
		uniform float2 _TwistSpeed;
		uniform float4 _TwistTex_ST;
		uniform float _TwistIntensity;
		uniform sampler2D _MainMask;
		uniform float4 _MainMask_ST;
		uniform sampler2D _AddtiveLightTex;
		uniform float4 _AddtiveLightTex_ST;
		uniform float _NoiseTwist;
		uniform float _Stage_3_SweepSpeed;
		uniform float _Stage_3_JianGe;
		uniform float4 _AddtiveLightColor;
		uniform float _Stage_3;
		uniform float4 _NoiseTexColor2;
		uniform sampler2D _NoiseTex2;
		uniform float _Noise2_U_Speed;
		uniform float _Noise2_V_Speed;
		uniform float4 _NoiseTex2_ST;
		uniform float _Metallic;
		uniform float _Smothness;


		float MyCustomExpression27( float OffsetY, float jiange )
		{
			for(int i = 0 ;i<9999;i++)
			{
			if(OffsetY>jiange)
			{
				OffsetY -= jiange+1;
			}
			}
			return OffsetY;
		}


		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float4 tex2DNode1 = tex2D( _MainTex, uv_MainTex );
			float4 lerpResult78 = lerp( _Color1 , _Color2 , (0.0 + (sin( ( _Time.y * 2.0 ) ) - -1.0) * (1.0 - 0.0) / (1.0 - -1.0)));
			float4 lerpResult89 = lerp( tex2DNode1 , lerpResult78 , _Stage_2);
			o.Albedo = lerpResult89.rgb;
			float4 lerpResult68 = lerp( tex2DNode1 , ( tex2DNode1 * tex2DNode1 * tex2DNode1 ) , _MainTexSaturation);
			float2 appendResult12 = (float2(_Noise_U_Speed , _Noise_V_Speed));
			float2 uv2_NoiseTex = i.uv2_texcoord2 * _NoiseTex_ST.xy + _NoiseTex_ST.zw;
			float2 panner9 = ( 1.0 * _Time.y * appendResult12 + uv2_NoiseTex);
			float2 uv2_TwistTex = i.uv2_texcoord2 * _TwistTex_ST.xy + _TwistTex_ST.zw;
			float2 panner34 = ( 1.0 * _Time.y * _TwistSpeed + uv2_TwistTex);
			float4 temp_output_25_0 = ( tex2D( _TwistTex, panner34 ) * _TwistIntensity );
			float4 tex2DNode5 = tex2D( _NoiseTex, ( float4( panner9, 0.0 , 0.0 ) + temp_output_25_0 ).rg );
			float2 uv_MainMask = i.uv_texcoord * _MainMask_ST.xy + _MainMask_ST.zw;
			float4 tex2DNode73 = tex2D( _MainMask, uv_MainMask );
			float2 uv2_AddtiveLightTex = i.uv2_texcoord2 * _AddtiveLightTex_ST.xy + _AddtiveLightTex_ST.zw;
			float mulTime29 = _Time.y * _Stage_3_SweepSpeed;
			float OffsetY27 = mulTime29;
			float jiange27 = _Stage_3_JianGe;
			float localMyCustomExpression27 = MyCustomExpression27( OffsetY27 , jiange27 );
			float4 tex2DNode14 = tex2D( _AddtiveLightTex, ( float4( uv2_AddtiveLightTex, 0.0 , 0.0 ) + ( _NoiseTwist * temp_output_25_0 ) + localMyCustomExpression27 ).rg );
			float2 appendResult56 = (float2(_Noise2_U_Speed , _Noise2_V_Speed));
			float2 uv2_NoiseTex2 = i.uv2_texcoord2 * _NoiseTex2_ST.xy + _NoiseTex2_ST.zw;
			float2 panner57 = ( 1.0 * _Time.y * appendResult56 + uv2_NoiseTex2);
			o.Emission = saturate( ( ( lerpResult68 * _MainTexIntensity ) + ( _Stage_1 * ( _NoiseTexColor * tex2DNode5 * tex2DNode5.a ) * tex2DNode73.r ) + ( tex2DNode14 * tex2DNode14.a * _AddtiveLightColor * _Stage_3 * tex2DNode73.r ) + ( _Stage_2 * ( _NoiseTexColor2 * ( tex2D( _NoiseTex2, ( float4( panner57, 0.0 , 0.0 ) + ( temp_output_25_0 * _NoiseTwist ) ).rg ) + 0.0 ) ) * tex2DNode73.r ) ) ).rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Smothness;
			o.Alpha = tex2DNode1.a;
		}

		#line 137 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
		//#pragma surface surf Standard keepalpha fullforwardshadows 

		

// vertex-to-fragment interpolation data
// no lightmaps:
#ifndef LIGHTMAP_ON
// half-precision fragment shader registers:
#ifdef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
#define FOG_COMBINED_WITH_WORLD_POS
struct v2f_surf {
  UNITY_POSITION(pos);
  float4 pack0 : TEXCOORD0; // _texcoord _texcoord2
  float3 worldNormal : TEXCOORD1;
  float4 worldPos : TEXCOORD2;
  #if UNITY_SHOULD_SAMPLE_SH
  half3 sh : TEXCOORD3; // SH
  #endif
  UNITY_LIGHTING_COORDS(4,5)
  #if SHADER_TARGET >= 30
  float4 lmap : TEXCOORD6;
  #endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
// high-precision fragment shader registers:
#ifndef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
struct v2f_surf {
  UNITY_POSITION(pos);
  float4 pack0 : TEXCOORD0; // _texcoord _texcoord2
  float3 worldNormal : TEXCOORD1;
  float3 worldPos : TEXCOORD2;
  #if UNITY_SHOULD_SAMPLE_SH
  half3 sh : TEXCOORD3; // SH
  #endif
  UNITY_FOG_COORDS(4)
  UNITY_SHADOW_COORDS(5)
  #if SHADER_TARGET >= 30
  float4 lmap : TEXCOORD6;
  #endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
#endif
// with lightmaps:
#ifdef LIGHTMAP_ON
// half-precision fragment shader registers:
#ifdef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
#define FOG_COMBINED_WITH_WORLD_POS
struct v2f_surf {
  UNITY_POSITION(pos);
  float4 pack0 : TEXCOORD0; // _texcoord _texcoord2
  float3 worldNormal : TEXCOORD1;
  float4 worldPos : TEXCOORD2;
  float4 lmap : TEXCOORD3;
  UNITY_LIGHTING_COORDS(4,5)
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
// high-precision fragment shader registers:
#ifndef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
struct v2f_surf {
  UNITY_POSITION(pos);
  float4 pack0 : TEXCOORD0; // _texcoord _texcoord2
  float3 worldNormal : TEXCOORD1;
  float3 worldPos : TEXCOORD2;
  float4 lmap : TEXCOORD3;
  UNITY_FOG_COORDS(4)
  UNITY_SHADOW_COORDS(5)
  #ifdef DIRLIGHTMAP_COMBINED
  float3 tSpace0 : TEXCOORD6;
  float3 tSpace1 : TEXCOORD7;
  float3 tSpace2 : TEXCOORD8;
  #endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
#endif
float4 _texcoord_ST;
float4 _texcoord2_ST;

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  o.pos = UnityObjectToClipPos(v.vertex);
  o.pack0.xy = TRANSFORM_TEX(v.texcoord, _texcoord);
  o.pack0.zw = TRANSFORM_TEX(v.texcoord1, _texcoord2);
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  #if defined(LIGHTMAP_ON) && defined(DIRLIGHTMAP_COMBINED)
  fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
  fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
  fixed3 worldBinormal = cross(worldNormal, worldTangent) * tangentSign;
  #endif
  #if defined(LIGHTMAP_ON) && defined(DIRLIGHTMAP_COMBINED) && !defined(UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS)
  o.tSpace0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
  o.tSpace1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
  o.tSpace2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);
  #endif
  o.worldPos.xyz = worldPos;
  o.worldNormal = worldNormal;
  #ifdef DYNAMICLIGHTMAP_ON
  o.lmap.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
  #endif
  #ifdef LIGHTMAP_ON
  o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
  #endif

  // SH/ambient and vertex lights
  #ifndef LIGHTMAP_ON
    #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
      o.sh = 0;
      // Approximated illumination from non-important point lights
      #ifdef VERTEXLIGHT_ON
        o.sh += Shade4PointLights (
          unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
          unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
          unity_4LightAtten0, worldPos, worldNormal);
      #endif
      o.sh = ShadeSHPerVertex (worldNormal, o.sh);
    #endif
  #endif // !LIGHTMAP_ON

  UNITY_TRANSFER_LIGHTING(o,v.texcoord1.xy); // pass shadow and, possibly, light cookie coordinates to pixel shader
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_TRANSFER_FOG_COMBINED_WITH_TSPACE(o,o.pos); // pass fog coordinates to pixel shader
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_TRANSFER_FOG_COMBINED_WITH_WORLD_POS(o,o.pos); // pass fog coordinates to pixel shader
  #else
    UNITY_TRANSFER_FOG(o,o.pos); // pass fog coordinates to pixel shader
  #endif
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.uv_texcoord.x = 1.0;
  surfIN.uv2_texcoord2.x = 1.0;
  surfIN.uv_texcoord = IN.pack0.xy;
  surfIN.uv2_texcoord2 = IN.pack0.zw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  float3 worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos));
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutputStandard o = (SurfaceOutputStandard)0;
  #else
  SurfaceOutputStandard o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Alpha = 0.0;
  o.Occlusion = 1.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);
  o.Normal = IN.worldNormal;
  normalWorldVertex = IN.worldNormal;

  // call surface function
  surf (surfIN, o);

  // compute lighting & shadowing factor
  UNITY_LIGHT_ATTENUATION(atten, IN, worldPos)
  fixed4 c = 0;

  // Setup lighting environment
  UnityGI gi;
  UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
  gi.indirect.diffuse = 0;
  gi.indirect.specular = 0;
  gi.light.color = _LightColor0.rgb;
  gi.light.dir = lightDir;
  // Call GI (lightmaps/SH/reflections) lighting function
  UnityGIInput giInput;
  UNITY_INITIALIZE_OUTPUT(UnityGIInput, giInput);
  giInput.light = gi.light;
  giInput.worldPos = worldPos;
  giInput.worldViewDir = worldViewDir;
  giInput.atten = atten;
  #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
    giInput.lightmapUV = IN.lmap;
  #else
    giInput.lightmapUV = 0.0;
  #endif
  #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
    giInput.ambient = IN.sh;
  #else
    giInput.ambient.rgb = 0.0;
  #endif
  giInput.probeHDR[0] = unity_SpecCube0_HDR;
  giInput.probeHDR[1] = unity_SpecCube1_HDR;
  #if defined(UNITY_SPECCUBE_BLENDING) || defined(UNITY_SPECCUBE_BOX_PROJECTION)
    giInput.boxMin[0] = unity_SpecCube0_BoxMin; // .w holds lerp value for blending
  #endif
  #ifdef UNITY_SPECCUBE_BOX_PROJECTION
    giInput.boxMax[0] = unity_SpecCube0_BoxMax;
    giInput.probePosition[0] = unity_SpecCube0_ProbePosition;
    giInput.boxMax[1] = unity_SpecCube1_BoxMax;
    giInput.boxMin[1] = unity_SpecCube1_BoxMin;
    giInput.probePosition[1] = unity_SpecCube1_ProbePosition;
  #endif
  LightingStandard_GI(o, giInput, gi);

  // realtime lighting: call lighting function
  c += LightingStandard (o, worldViewDir, gi);
  c.rgb += o.Emission;
  UNITY_APPLY_FOG(_unity_fogCoord, c); // apply fog
  return c;
}


#endif

// -------- variant for: INSTANCING_ON 
#if defined(INSTANCING_ON)
// Surface shader code generated based on:
// writes to per-pixel normal: no
// writes to emission: YES
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: YES
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 2 texcoords actually used
//   float2 _texcoord
//   float2 _texcoord2
#include "UnityCG.cginc"
#include "AutoLight.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 47

		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		//#pragma target 3.0
		struct Input
		{
			float2 uv_texcoord;
			float2 uv2_texcoord2;
		};

		uniform float _CullMode;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform float4 _Color1;
		uniform float4 _Color2;
		uniform float _Stage_2;
		uniform float _MainTexSaturation;
		uniform float _MainTexIntensity;
		uniform float _Stage_1;
		uniform float4 _NoiseTexColor;
		uniform sampler2D _NoiseTex;
		uniform float _Noise_U_Speed;
		uniform float _Noise_V_Speed;
		uniform float4 _NoiseTex_ST;
		uniform sampler2D _TwistTex;
		uniform float2 _TwistSpeed;
		uniform float4 _TwistTex_ST;
		uniform float _TwistIntensity;
		uniform sampler2D _MainMask;
		uniform float4 _MainMask_ST;
		uniform sampler2D _AddtiveLightTex;
		uniform float4 _AddtiveLightTex_ST;
		uniform float _NoiseTwist;
		uniform float _Stage_3_SweepSpeed;
		uniform float _Stage_3_JianGe;
		uniform float4 _AddtiveLightColor;
		uniform float _Stage_3;
		uniform float4 _NoiseTexColor2;
		uniform sampler2D _NoiseTex2;
		uniform float _Noise2_U_Speed;
		uniform float _Noise2_V_Speed;
		uniform float4 _NoiseTex2_ST;
		uniform float _Metallic;
		uniform float _Smothness;


		float MyCustomExpression27( float OffsetY, float jiange )
		{
			for(int i = 0 ;i<9999;i++)
			{
			if(OffsetY>jiange)
			{
				OffsetY -= jiange+1;
			}
			}
			return OffsetY;
		}


		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float4 tex2DNode1 = tex2D( _MainTex, uv_MainTex );
			float4 lerpResult78 = lerp( _Color1 , _Color2 , (0.0 + (sin( ( _Time.y * 2.0 ) ) - -1.0) * (1.0 - 0.0) / (1.0 - -1.0)));
			float4 lerpResult89 = lerp( tex2DNode1 , lerpResult78 , _Stage_2);
			o.Albedo = lerpResult89.rgb;
			float4 lerpResult68 = lerp( tex2DNode1 , ( tex2DNode1 * tex2DNode1 * tex2DNode1 ) , _MainTexSaturation);
			float2 appendResult12 = (float2(_Noise_U_Speed , _Noise_V_Speed));
			float2 uv2_NoiseTex = i.uv2_texcoord2 * _NoiseTex_ST.xy + _NoiseTex_ST.zw;
			float2 panner9 = ( 1.0 * _Time.y * appendResult12 + uv2_NoiseTex);
			float2 uv2_TwistTex = i.uv2_texcoord2 * _TwistTex_ST.xy + _TwistTex_ST.zw;
			float2 panner34 = ( 1.0 * _Time.y * _TwistSpeed + uv2_TwistTex);
			float4 temp_output_25_0 = ( tex2D( _TwistTex, panner34 ) * _TwistIntensity );
			float4 tex2DNode5 = tex2D( _NoiseTex, ( float4( panner9, 0.0 , 0.0 ) + temp_output_25_0 ).rg );
			float2 uv_MainMask = i.uv_texcoord * _MainMask_ST.xy + _MainMask_ST.zw;
			float4 tex2DNode73 = tex2D( _MainMask, uv_MainMask );
			float2 uv2_AddtiveLightTex = i.uv2_texcoord2 * _AddtiveLightTex_ST.xy + _AddtiveLightTex_ST.zw;
			float mulTime29 = _Time.y * _Stage_3_SweepSpeed;
			float OffsetY27 = mulTime29;
			float jiange27 = _Stage_3_JianGe;
			float localMyCustomExpression27 = MyCustomExpression27( OffsetY27 , jiange27 );
			float4 tex2DNode14 = tex2D( _AddtiveLightTex, ( float4( uv2_AddtiveLightTex, 0.0 , 0.0 ) + ( _NoiseTwist * temp_output_25_0 ) + localMyCustomExpression27 ).rg );
			float2 appendResult56 = (float2(_Noise2_U_Speed , _Noise2_V_Speed));
			float2 uv2_NoiseTex2 = i.uv2_texcoord2 * _NoiseTex2_ST.xy + _NoiseTex2_ST.zw;
			float2 panner57 = ( 1.0 * _Time.y * appendResult56 + uv2_NoiseTex2);
			o.Emission = saturate( ( ( lerpResult68 * _MainTexIntensity ) + ( _Stage_1 * ( _NoiseTexColor * tex2DNode5 * tex2DNode5.a ) * tex2DNode73.r ) + ( tex2DNode14 * tex2DNode14.a * _AddtiveLightColor * _Stage_3 * tex2DNode73.r ) + ( _Stage_2 * ( _NoiseTexColor2 * ( tex2D( _NoiseTex2, ( float4( panner57, 0.0 , 0.0 ) + ( temp_output_25_0 * _NoiseTwist ) ).rg ) + 0.0 ) ) * tex2DNode73.r ) ) ).rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Smothness;
			o.Alpha = tex2DNode1.a;
		}

		#line 137 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
		//#pragma surface surf Standard keepalpha fullforwardshadows 

		

// vertex-to-fragment interpolation data
// no lightmaps:
#ifndef LIGHTMAP_ON
// half-precision fragment shader registers:
#ifdef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
#define FOG_COMBINED_WITH_WORLD_POS
struct v2f_surf {
  UNITY_POSITION(pos);
  float4 pack0 : TEXCOORD0; // _texcoord _texcoord2
  float3 worldNormal : TEXCOORD1;
  float4 worldPos : TEXCOORD2;
  #if UNITY_SHOULD_SAMPLE_SH
  half3 sh : TEXCOORD3; // SH
  #endif
  UNITY_LIGHTING_COORDS(4,5)
  #if SHADER_TARGET >= 30
  float4 lmap : TEXCOORD6;
  #endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
// high-precision fragment shader registers:
#ifndef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
struct v2f_surf {
  UNITY_POSITION(pos);
  float4 pack0 : TEXCOORD0; // _texcoord _texcoord2
  float3 worldNormal : TEXCOORD1;
  float3 worldPos : TEXCOORD2;
  #if UNITY_SHOULD_SAMPLE_SH
  half3 sh : TEXCOORD3; // SH
  #endif
  UNITY_FOG_COORDS(4)
  UNITY_SHADOW_COORDS(5)
  #if SHADER_TARGET >= 30
  float4 lmap : TEXCOORD6;
  #endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
#endif
// with lightmaps:
#ifdef LIGHTMAP_ON
// half-precision fragment shader registers:
#ifdef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
#define FOG_COMBINED_WITH_WORLD_POS
struct v2f_surf {
  UNITY_POSITION(pos);
  float4 pack0 : TEXCOORD0; // _texcoord _texcoord2
  float3 worldNormal : TEXCOORD1;
  float4 worldPos : TEXCOORD2;
  float4 lmap : TEXCOORD3;
  UNITY_LIGHTING_COORDS(4,5)
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
// high-precision fragment shader registers:
#ifndef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
struct v2f_surf {
  UNITY_POSITION(pos);
  float4 pack0 : TEXCOORD0; // _texcoord _texcoord2
  float3 worldNormal : TEXCOORD1;
  float3 worldPos : TEXCOORD2;
  float4 lmap : TEXCOORD3;
  UNITY_FOG_COORDS(4)
  UNITY_SHADOW_COORDS(5)
  #ifdef DIRLIGHTMAP_COMBINED
  float3 tSpace0 : TEXCOORD6;
  float3 tSpace1 : TEXCOORD7;
  float3 tSpace2 : TEXCOORD8;
  #endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
#endif
float4 _texcoord_ST;
float4 _texcoord2_ST;

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  o.pos = UnityObjectToClipPos(v.vertex);
  o.pack0.xy = TRANSFORM_TEX(v.texcoord, _texcoord);
  o.pack0.zw = TRANSFORM_TEX(v.texcoord1, _texcoord2);
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  #if defined(LIGHTMAP_ON) && defined(DIRLIGHTMAP_COMBINED)
  fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
  fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
  fixed3 worldBinormal = cross(worldNormal, worldTangent) * tangentSign;
  #endif
  #if defined(LIGHTMAP_ON) && defined(DIRLIGHTMAP_COMBINED) && !defined(UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS)
  o.tSpace0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
  o.tSpace1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
  o.tSpace2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);
  #endif
  o.worldPos.xyz = worldPos;
  o.worldNormal = worldNormal;
  #ifdef DYNAMICLIGHTMAP_ON
  o.lmap.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
  #endif
  #ifdef LIGHTMAP_ON
  o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
  #endif

  // SH/ambient and vertex lights
  #ifndef LIGHTMAP_ON
    #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
      o.sh = 0;
      // Approximated illumination from non-important point lights
      #ifdef VERTEXLIGHT_ON
        o.sh += Shade4PointLights (
          unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
          unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
          unity_4LightAtten0, worldPos, worldNormal);
      #endif
      o.sh = ShadeSHPerVertex (worldNormal, o.sh);
    #endif
  #endif // !LIGHTMAP_ON

  UNITY_TRANSFER_LIGHTING(o,v.texcoord1.xy); // pass shadow and, possibly, light cookie coordinates to pixel shader
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_TRANSFER_FOG_COMBINED_WITH_TSPACE(o,o.pos); // pass fog coordinates to pixel shader
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_TRANSFER_FOG_COMBINED_WITH_WORLD_POS(o,o.pos); // pass fog coordinates to pixel shader
  #else
    UNITY_TRANSFER_FOG(o,o.pos); // pass fog coordinates to pixel shader
  #endif
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.uv_texcoord.x = 1.0;
  surfIN.uv2_texcoord2.x = 1.0;
  surfIN.uv_texcoord = IN.pack0.xy;
  surfIN.uv2_texcoord2 = IN.pack0.zw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  float3 worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos));
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutputStandard o = (SurfaceOutputStandard)0;
  #else
  SurfaceOutputStandard o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Alpha = 0.0;
  o.Occlusion = 1.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);
  o.Normal = IN.worldNormal;
  normalWorldVertex = IN.worldNormal;

  // call surface function
  surf (surfIN, o);

  // compute lighting & shadowing factor
  UNITY_LIGHT_ATTENUATION(atten, IN, worldPos)
  fixed4 c = 0;

  // Setup lighting environment
  UnityGI gi;
  UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
  gi.indirect.diffuse = 0;
  gi.indirect.specular = 0;
  gi.light.color = _LightColor0.rgb;
  gi.light.dir = lightDir;
  // Call GI (lightmaps/SH/reflections) lighting function
  UnityGIInput giInput;
  UNITY_INITIALIZE_OUTPUT(UnityGIInput, giInput);
  giInput.light = gi.light;
  giInput.worldPos = worldPos;
  giInput.worldViewDir = worldViewDir;
  giInput.atten = atten;
  #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
    giInput.lightmapUV = IN.lmap;
  #else
    giInput.lightmapUV = 0.0;
  #endif
  #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
    giInput.ambient = IN.sh;
  #else
    giInput.ambient.rgb = 0.0;
  #endif
  giInput.probeHDR[0] = unity_SpecCube0_HDR;
  giInput.probeHDR[1] = unity_SpecCube1_HDR;
  #if defined(UNITY_SPECCUBE_BLENDING) || defined(UNITY_SPECCUBE_BOX_PROJECTION)
    giInput.boxMin[0] = unity_SpecCube0_BoxMin; // .w holds lerp value for blending
  #endif
  #ifdef UNITY_SPECCUBE_BOX_PROJECTION
    giInput.boxMax[0] = unity_SpecCube0_BoxMax;
    giInput.probePosition[0] = unity_SpecCube0_ProbePosition;
    giInput.boxMax[1] = unity_SpecCube1_BoxMax;
    giInput.boxMin[1] = unity_SpecCube1_BoxMin;
    giInput.probePosition[1] = unity_SpecCube1_ProbePosition;
  #endif
  LightingStandard_GI(o, giInput, gi);

  // realtime lighting: call lighting function
  c += LightingStandard (o, worldViewDir, gi);
  c.rgb += o.Emission;
  UNITY_APPLY_FOG(_unity_fogCoord, c); // apply fog
  return c;
}


#endif


ENDCG

}

	

	// ---- meta information extraction pass:
	Pass {
		Name "Meta"
		Tags { "LightMode" = "Meta" }
		Cull Off

CGPROGRAM
// compile directives
#pragma vertex vert_surf
#pragma fragment frag_surf
#pragma target 3.0
#pragma multi_compile_instancing
#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
#pragma shader_feature EDITOR_VISUALIZATION

#include "HLSLSupport.cginc"
#define UNITY_INSTANCED_LOD_FADE
#define UNITY_INSTANCED_SH
#define UNITY_INSTANCED_LIGHTMAPSTS
#include "UnityShaderVariables.cginc"
#include "UnityShaderUtilities.cginc"
// -------- variant for: <when no other keywords are defined>
#if !defined(INSTANCING_ON)
// Surface shader code generated based on:
// writes to per-pixel normal: no
// writes to emission: YES
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: YES
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 2 texcoords actually used
//   float2 _texcoord
//   float2 _texcoord2
#include "UnityCG.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 47

		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		//#pragma target 3.0
		struct Input
		{
			float2 uv_texcoord;
			float2 uv2_texcoord2;
		};

		uniform float _CullMode;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform float4 _Color1;
		uniform float4 _Color2;
		uniform float _Stage_2;
		uniform float _MainTexSaturation;
		uniform float _MainTexIntensity;
		uniform float _Stage_1;
		uniform float4 _NoiseTexColor;
		uniform sampler2D _NoiseTex;
		uniform float _Noise_U_Speed;
		uniform float _Noise_V_Speed;
		uniform float4 _NoiseTex_ST;
		uniform sampler2D _TwistTex;
		uniform float2 _TwistSpeed;
		uniform float4 _TwistTex_ST;
		uniform float _TwistIntensity;
		uniform sampler2D _MainMask;
		uniform float4 _MainMask_ST;
		uniform sampler2D _AddtiveLightTex;
		uniform float4 _AddtiveLightTex_ST;
		uniform float _NoiseTwist;
		uniform float _Stage_3_SweepSpeed;
		uniform float _Stage_3_JianGe;
		uniform float4 _AddtiveLightColor;
		uniform float _Stage_3;
		uniform float4 _NoiseTexColor2;
		uniform sampler2D _NoiseTex2;
		uniform float _Noise2_U_Speed;
		uniform float _Noise2_V_Speed;
		uniform float4 _NoiseTex2_ST;
		uniform float _Metallic;
		uniform float _Smothness;


		float MyCustomExpression27( float OffsetY, float jiange )
		{
			for(int i = 0 ;i<9999;i++)
			{
			if(OffsetY>jiange)
			{
				OffsetY -= jiange+1;
			}
			}
			return OffsetY;
		}


		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float4 tex2DNode1 = tex2D( _MainTex, uv_MainTex );
			float4 lerpResult78 = lerp( _Color1 , _Color2 , (0.0 + (sin( ( _Time.y * 2.0 ) ) - -1.0) * (1.0 - 0.0) / (1.0 - -1.0)));
			float4 lerpResult89 = lerp( tex2DNode1 , lerpResult78 , _Stage_2);
			o.Albedo = lerpResult89.rgb;
			float4 lerpResult68 = lerp( tex2DNode1 , ( tex2DNode1 * tex2DNode1 * tex2DNode1 ) , _MainTexSaturation);
			float2 appendResult12 = (float2(_Noise_U_Speed , _Noise_V_Speed));
			float2 uv2_NoiseTex = i.uv2_texcoord2 * _NoiseTex_ST.xy + _NoiseTex_ST.zw;
			float2 panner9 = ( 1.0 * _Time.y * appendResult12 + uv2_NoiseTex);
			float2 uv2_TwistTex = i.uv2_texcoord2 * _TwistTex_ST.xy + _TwistTex_ST.zw;
			float2 panner34 = ( 1.0 * _Time.y * _TwistSpeed + uv2_TwistTex);
			float4 temp_output_25_0 = ( tex2D( _TwistTex, panner34 ) * _TwistIntensity );
			float4 tex2DNode5 = tex2D( _NoiseTex, ( float4( panner9, 0.0 , 0.0 ) + temp_output_25_0 ).rg );
			float2 uv_MainMask = i.uv_texcoord * _MainMask_ST.xy + _MainMask_ST.zw;
			float4 tex2DNode73 = tex2D( _MainMask, uv_MainMask );
			float2 uv2_AddtiveLightTex = i.uv2_texcoord2 * _AddtiveLightTex_ST.xy + _AddtiveLightTex_ST.zw;
			float mulTime29 = _Time.y * _Stage_3_SweepSpeed;
			float OffsetY27 = mulTime29;
			float jiange27 = _Stage_3_JianGe;
			float localMyCustomExpression27 = MyCustomExpression27( OffsetY27 , jiange27 );
			float4 tex2DNode14 = tex2D( _AddtiveLightTex, ( float4( uv2_AddtiveLightTex, 0.0 , 0.0 ) + ( _NoiseTwist * temp_output_25_0 ) + localMyCustomExpression27 ).rg );
			float2 appendResult56 = (float2(_Noise2_U_Speed , _Noise2_V_Speed));
			float2 uv2_NoiseTex2 = i.uv2_texcoord2 * _NoiseTex2_ST.xy + _NoiseTex2_ST.zw;
			float2 panner57 = ( 1.0 * _Time.y * appendResult56 + uv2_NoiseTex2);
			o.Emission = saturate( ( ( lerpResult68 * _MainTexIntensity ) + ( _Stage_1 * ( _NoiseTexColor * tex2DNode5 * tex2DNode5.a ) * tex2DNode73.r ) + ( tex2DNode14 * tex2DNode14.a * _AddtiveLightColor * _Stage_3 * tex2DNode73.r ) + ( _Stage_2 * ( _NoiseTexColor2 * ( tex2D( _NoiseTex2, ( float4( panner57, 0.0 , 0.0 ) + ( temp_output_25_0 * _NoiseTwist ) ).rg ) + 0.0 ) ) * tex2DNode73.r ) ) ).rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Smothness;
			o.Alpha = tex2DNode1.a;
		}

		#line 137 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
		//#pragma surface surf Standard keepalpha fullforwardshadows 

		
#include "UnityMetaPass.cginc"

// vertex-to-fragment interpolation data
struct v2f_surf {
  UNITY_POSITION(pos);
  float4 pack0 : TEXCOORD0; // _texcoord _texcoord2
  float3 worldPos : TEXCOORD1;
#ifdef EDITOR_VISUALIZATION
  float2 vizUV : TEXCOORD2;
  float4 lightCoord : TEXCOORD3;
#endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
float4 _texcoord_ST;
float4 _texcoord2_ST;

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST);
#ifdef EDITOR_VISUALIZATION
  o.vizUV = 0;
  o.lightCoord = 0;
  if (unity_VisualizationMode == EDITORVIZ_TEXTURE)
    o.vizUV = UnityMetaVizUV(unity_EditorViz_UVIndex, v.texcoord.xy, v.texcoord1.xy, v.texcoord2.xy, unity_EditorViz_Texture_ST);
  else if (unity_VisualizationMode == EDITORVIZ_SHOWLIGHTMASK)
  {
    o.vizUV = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
    o.lightCoord = mul(unity_EditorViz_WorldToLight, mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1)));
  }
#endif
  o.pack0.xy = TRANSFORM_TEX(v.texcoord, _texcoord);
  o.pack0.zw = TRANSFORM_TEX(v.texcoord1, _texcoord2);
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  o.worldPos.xyz = worldPos;
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.uv_texcoord.x = 1.0;
  surfIN.uv2_texcoord2.x = 1.0;
  surfIN.uv_texcoord = IN.pack0.xy;
  surfIN.uv2_texcoord2 = IN.pack0.zw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutputStandard o = (SurfaceOutputStandard)0;
  #else
  SurfaceOutputStandard o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Alpha = 0.0;
  o.Occlusion = 1.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);

  // call surface function
  surf (surfIN, o);
  UnityMetaInput metaIN;
  UNITY_INITIALIZE_OUTPUT(UnityMetaInput, metaIN);
  metaIN.Albedo = o.Albedo;
  metaIN.Emission = o.Emission;
#ifdef EDITOR_VISUALIZATION
  metaIN.VizUV = IN.vizUV;
  metaIN.LightCoord = IN.lightCoord;
#endif
  return UnityMetaFragment(metaIN);
}


#endif

// -------- variant for: INSTANCING_ON 
#if defined(INSTANCING_ON)
// Surface shader code generated based on:
// writes to per-pixel normal: no
// writes to emission: YES
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: YES
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 2 texcoords actually used
//   float2 _texcoord
//   float2 _texcoord2
#include "UnityCG.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 47

		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		//#pragma target 3.0
		struct Input
		{
			float2 uv_texcoord;
			float2 uv2_texcoord2;
		};

		uniform float _CullMode;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform float4 _Color1;
		uniform float4 _Color2;
		uniform float _Stage_2;
		uniform float _MainTexSaturation;
		uniform float _MainTexIntensity;
		uniform float _Stage_1;
		uniform float4 _NoiseTexColor;
		uniform sampler2D _NoiseTex;
		uniform float _Noise_U_Speed;
		uniform float _Noise_V_Speed;
		uniform float4 _NoiseTex_ST;
		uniform sampler2D _TwistTex;
		uniform float2 _TwistSpeed;
		uniform float4 _TwistTex_ST;
		uniform float _TwistIntensity;
		uniform sampler2D _MainMask;
		uniform float4 _MainMask_ST;
		uniform sampler2D _AddtiveLightTex;
		uniform float4 _AddtiveLightTex_ST;
		uniform float _NoiseTwist;
		uniform float _Stage_3_SweepSpeed;
		uniform float _Stage_3_JianGe;
		uniform float4 _AddtiveLightColor;
		uniform float _Stage_3;
		uniform float4 _NoiseTexColor2;
		uniform sampler2D _NoiseTex2;
		uniform float _Noise2_U_Speed;
		uniform float _Noise2_V_Speed;
		uniform float4 _NoiseTex2_ST;
		uniform float _Metallic;
		uniform float _Smothness;


		float MyCustomExpression27( float OffsetY, float jiange )
		{
			for(int i = 0 ;i<9999;i++)
			{
			if(OffsetY>jiange)
			{
				OffsetY -= jiange+1;
			}
			}
			return OffsetY;
		}


		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float4 tex2DNode1 = tex2D( _MainTex, uv_MainTex );
			float4 lerpResult78 = lerp( _Color1 , _Color2 , (0.0 + (sin( ( _Time.y * 2.0 ) ) - -1.0) * (1.0 - 0.0) / (1.0 - -1.0)));
			float4 lerpResult89 = lerp( tex2DNode1 , lerpResult78 , _Stage_2);
			o.Albedo = lerpResult89.rgb;
			float4 lerpResult68 = lerp( tex2DNode1 , ( tex2DNode1 * tex2DNode1 * tex2DNode1 ) , _MainTexSaturation);
			float2 appendResult12 = (float2(_Noise_U_Speed , _Noise_V_Speed));
			float2 uv2_NoiseTex = i.uv2_texcoord2 * _NoiseTex_ST.xy + _NoiseTex_ST.zw;
			float2 panner9 = ( 1.0 * _Time.y * appendResult12 + uv2_NoiseTex);
			float2 uv2_TwistTex = i.uv2_texcoord2 * _TwistTex_ST.xy + _TwistTex_ST.zw;
			float2 panner34 = ( 1.0 * _Time.y * _TwistSpeed + uv2_TwistTex);
			float4 temp_output_25_0 = ( tex2D( _TwistTex, panner34 ) * _TwistIntensity );
			float4 tex2DNode5 = tex2D( _NoiseTex, ( float4( panner9, 0.0 , 0.0 ) + temp_output_25_0 ).rg );
			float2 uv_MainMask = i.uv_texcoord * _MainMask_ST.xy + _MainMask_ST.zw;
			float4 tex2DNode73 = tex2D( _MainMask, uv_MainMask );
			float2 uv2_AddtiveLightTex = i.uv2_texcoord2 * _AddtiveLightTex_ST.xy + _AddtiveLightTex_ST.zw;
			float mulTime29 = _Time.y * _Stage_3_SweepSpeed;
			float OffsetY27 = mulTime29;
			float jiange27 = _Stage_3_JianGe;
			float localMyCustomExpression27 = MyCustomExpression27( OffsetY27 , jiange27 );
			float4 tex2DNode14 = tex2D( _AddtiveLightTex, ( float4( uv2_AddtiveLightTex, 0.0 , 0.0 ) + ( _NoiseTwist * temp_output_25_0 ) + localMyCustomExpression27 ).rg );
			float2 appendResult56 = (float2(_Noise2_U_Speed , _Noise2_V_Speed));
			float2 uv2_NoiseTex2 = i.uv2_texcoord2 * _NoiseTex2_ST.xy + _NoiseTex2_ST.zw;
			float2 panner57 = ( 1.0 * _Time.y * appendResult56 + uv2_NoiseTex2);
			o.Emission = saturate( ( ( lerpResult68 * _MainTexIntensity ) + ( _Stage_1 * ( _NoiseTexColor * tex2DNode5 * tex2DNode5.a ) * tex2DNode73.r ) + ( tex2DNode14 * tex2DNode14.a * _AddtiveLightColor * _Stage_3 * tex2DNode73.r ) + ( _Stage_2 * ( _NoiseTexColor2 * ( tex2D( _NoiseTex2, ( float4( panner57, 0.0 , 0.0 ) + ( temp_output_25_0 * _NoiseTwist ) ).rg ) + 0.0 ) ) * tex2DNode73.r ) ) ).rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Smothness;
			o.Alpha = tex2DNode1.a;
		}

		#line 137 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
		//#pragma surface surf Standard keepalpha fullforwardshadows 

		
#include "UnityMetaPass.cginc"

// vertex-to-fragment interpolation data
struct v2f_surf {
  UNITY_POSITION(pos);
  float4 pack0 : TEXCOORD0; // _texcoord _texcoord2
  float3 worldPos : TEXCOORD1;
#ifdef EDITOR_VISUALIZATION
  float2 vizUV : TEXCOORD2;
  float4 lightCoord : TEXCOORD3;
#endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
float4 _texcoord_ST;
float4 _texcoord2_ST;

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST);
#ifdef EDITOR_VISUALIZATION
  o.vizUV = 0;
  o.lightCoord = 0;
  if (unity_VisualizationMode == EDITORVIZ_TEXTURE)
    o.vizUV = UnityMetaVizUV(unity_EditorViz_UVIndex, v.texcoord.xy, v.texcoord1.xy, v.texcoord2.xy, unity_EditorViz_Texture_ST);
  else if (unity_VisualizationMode == EDITORVIZ_SHOWLIGHTMASK)
  {
    o.vizUV = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
    o.lightCoord = mul(unity_EditorViz_WorldToLight, mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1)));
  }
#endif
  o.pack0.xy = TRANSFORM_TEX(v.texcoord, _texcoord);
  o.pack0.zw = TRANSFORM_TEX(v.texcoord1, _texcoord2);
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  o.worldPos.xyz = worldPos;
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.uv_texcoord.x = 1.0;
  surfIN.uv2_texcoord2.x = 1.0;
  surfIN.uv_texcoord = IN.pack0.xy;
  surfIN.uv2_texcoord2 = IN.pack0.zw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutputStandard o = (SurfaceOutputStandard)0;
  #else
  SurfaceOutputStandard o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Alpha = 0.0;
  o.Occlusion = 1.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);

  // call surface function
  surf (surfIN, o);
  UnityMetaInput metaIN;
  UNITY_INITIALIZE_OUTPUT(UnityMetaInput, metaIN);
  metaIN.Albedo = o.Albedo;
  metaIN.Emission = o.Emission;
#ifdef EDITOR_VISUALIZATION
  metaIN.VizUV = IN.vizUV;
  metaIN.LightCoord = IN.lightCoord;
#endif
  return UnityMetaFragment(metaIN);
}


#endif


ENDCG

}

	// ---- end of surface shader generated code

#LINE 142

		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
#include "HLSLSupport.cginc"
#define UNITY_INSTANCED_LOD_FADE
#define UNITY_INSTANCED_SH
#define UNITY_INSTANCED_LIGHTMAPSTS
#include "UnityShaderVariables.cginc"
#include "UnityShaderUtilities.cginc"
#line 47

		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float2 uv_texcoord;
			float2 uv2_texcoord2;
		};

		uniform float _CullMode;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform float4 _Color1;
		uniform float4 _Color2;
		uniform float _Stage_2;
		uniform float _MainTexSaturation;
		uniform float _MainTexIntensity;
		uniform float _Stage_1;
		uniform float4 _NoiseTexColor;
		uniform sampler2D _NoiseTex;
		uniform float _Noise_U_Speed;
		uniform float _Noise_V_Speed;
		uniform float4 _NoiseTex_ST;
		uniform sampler2D _TwistTex;
		uniform float2 _TwistSpeed;
		uniform float4 _TwistTex_ST;
		uniform float _TwistIntensity;
		uniform sampler2D _MainMask;
		uniform float4 _MainMask_ST;
		uniform sampler2D _AddtiveLightTex;
		uniform float4 _AddtiveLightTex_ST;
		uniform float _NoiseTwist;
		uniform float _Stage_3_SweepSpeed;
		uniform float _Stage_3_JianGe;
		uniform float4 _AddtiveLightColor;
		uniform float _Stage_3;
		uniform float4 _NoiseTexColor2;
		uniform sampler2D _NoiseTex2;
		uniform float _Noise2_U_Speed;
		uniform float _Noise2_V_Speed;
		uniform float4 _NoiseTex2_ST;
		uniform float _Metallic;
		uniform float _Smothness;


		float MyCustomExpression27( float OffsetY, float jiange )
		{
			for(int i = 0 ;i<9999;i++)
			{
			if(OffsetY>jiange)
			{
				OffsetY -= jiange+1;
			}
			}
			return OffsetY;
		}


		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float4 tex2DNode1 = tex2D( _MainTex, uv_MainTex );
			float4 lerpResult78 = lerp( _Color1 , _Color2 , (0.0 + (sin( ( _Time.y * 2.0 ) ) - -1.0) * (1.0 - 0.0) / (1.0 - -1.0)));
			float4 lerpResult89 = lerp( tex2DNode1 , lerpResult78 , _Stage_2);
			o.Albedo = lerpResult89.rgb;
			float4 lerpResult68 = lerp( tex2DNode1 , ( tex2DNode1 * tex2DNode1 * tex2DNode1 ) , _MainTexSaturation);
			float2 appendResult12 = (float2(_Noise_U_Speed , _Noise_V_Speed));
			float2 uv2_NoiseTex = i.uv2_texcoord2 * _NoiseTex_ST.xy + _NoiseTex_ST.zw;
			float2 panner9 = ( 1.0 * _Time.y * appendResult12 + uv2_NoiseTex);
			float2 uv2_TwistTex = i.uv2_texcoord2 * _TwistTex_ST.xy + _TwistTex_ST.zw;
			float2 panner34 = ( 1.0 * _Time.y * _TwistSpeed + uv2_TwistTex);
			float4 temp_output_25_0 = ( tex2D( _TwistTex, panner34 ) * _TwistIntensity );
			float4 tex2DNode5 = tex2D( _NoiseTex, ( float4( panner9, 0.0 , 0.0 ) + temp_output_25_0 ).rg );
			float2 uv_MainMask = i.uv_texcoord * _MainMask_ST.xy + _MainMask_ST.zw;
			float4 tex2DNode73 = tex2D( _MainMask, uv_MainMask );
			float2 uv2_AddtiveLightTex = i.uv2_texcoord2 * _AddtiveLightTex_ST.xy + _AddtiveLightTex_ST.zw;
			float mulTime29 = _Time.y * _Stage_3_SweepSpeed;
			float OffsetY27 = mulTime29;
			float jiange27 = _Stage_3_JianGe;
			float localMyCustomExpression27 = MyCustomExpression27( OffsetY27 , jiange27 );
			float4 tex2DNode14 = tex2D( _AddtiveLightTex, ( float4( uv2_AddtiveLightTex, 0.0 , 0.0 ) + ( _NoiseTwist * temp_output_25_0 ) + localMyCustomExpression27 ).rg );
			float2 appendResult56 = (float2(_Noise2_U_Speed , _Noise2_V_Speed));
			float2 uv2_NoiseTex2 = i.uv2_texcoord2 * _NoiseTex2_ST.xy + _NoiseTex2_ST.zw;
			float2 panner57 = ( 1.0 * _Time.y * appendResult56 + uv2_NoiseTex2);
			o.Emission = saturate( ( ( lerpResult68 * _MainTexIntensity ) + ( _Stage_1 * ( _NoiseTexColor * tex2DNode5 * tex2DNode5.a ) * tex2DNode73.r ) + ( tex2DNode14 * tex2DNode14.a * _AddtiveLightColor * _Stage_3 * tex2DNode73.r ) + ( _Stage_2 * ( _NoiseTexColor2 * ( tex2D( _NoiseTex2, ( float4( panner57, 0.0 , 0.0 ) + ( temp_output_25_0 * _NoiseTwist ) ).rg ) + 0.0 ) ) * tex2DNode73.r ) ) ).rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Smothness;
			o.Alpha = tex2DNode1.a;
		}

		#line 146 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float4 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.customPack1.zw = customInputData.uv2_texcoord2;
				o.customPack1.zw = v.texcoord1;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				surfIN.uv2_texcoord2 = IN.customPack1.zw;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG

#LINE 212

		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
