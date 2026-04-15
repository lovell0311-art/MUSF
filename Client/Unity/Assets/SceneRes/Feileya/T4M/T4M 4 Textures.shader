Shader "T4MShaders/ShaderModel2/Diffuse/T4M 4 Textures" {
Properties {
	_Splat0 ("Layer 1", 2D) = "white" {}
	_Splat1 ("Layer 2", 2D) = "white" {}
	_Splat2 ("Layer 3", 2D) = "white" {}
	_Splat3 ("Layer 4", 2D) = "white" {}
	_Control ("Control (RGBA)", 2D) = "white" {}
	_MainTex ("Never Used", 2D) = "white" {}
}
      
SubShader {
	Tags {
   //"SplatCount" = "4"
   "RenderType" = "Opaque"
	}

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
// vertex modifier: 'vert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 14 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
//#pragma surface surf Lambert vertex:vert finalcolor:FinalColor
// //#pragma exclude_renderers xbox360 ps3
// //#pragma multi_compile DOD_FOG_NONE DOD_FOG_LINEAR DOD_FOG_EXP DOD_FOG_EXP2
//#pragma target 3.0

// #include "CustomFog.cginc"
struct Input {
	float2 tc_Control : TEXCOORD0;
	float2 tc_Splat0 : TEXCOORD1;
	float2 tc_Splat1 : TEXCOORD2;
	float2 tc_Splat2 : TEXCOORD3;
	float2 tc_Splat3 : TEXCOORD4;
	// CUSTOM_FOG_COORDS(5)
	float3 worldPos : TEXCOORD6;
};
 
sampler2D _Control;
float4 _Control_ST;
sampler2D _Splat0,_Splat1,_Splat2,_Splat3;
float4 _Splat0_ST,_Splat1_ST,_Splat2_ST,_Splat3_ST;

void vert(inout appdata_full v, out Input data)
{
	UNITY_INITIALIZE_OUTPUT(Input, data);
	data.tc_Control.xy = TRANSFORM_TEX(v.texcoord, _Control);
	data.tc_Splat0 = TRANSFORM_TEX(v.texcoord, _Splat0);
	data.tc_Splat1 = TRANSFORM_TEX(v.texcoord, _Splat1);
	data.tc_Splat2 = TRANSFORM_TEX(v.texcoord, _Splat2);
	data.tc_Splat3 = TRANSFORM_TEX(v.texcoord, _Splat3);
	data.worldPos = mul(unity_ObjectToWorld, v.vertex);
	// CUSTOM_TRANSFER_FOG(data.fogCoord, v.vertex);

}

void FinalColor(Input IN, SurfaceOutput o, inout fixed4 color)
{
	// CUSTOM_APPLY_FOG(IN.fogCoord, IN.worldPos, color.rgb);
}
 
void surf (Input IN, inout SurfaceOutput o) {
	fixed4 splat_control = tex2D (_Control, IN.tc_Control).rgba;
		
	fixed3 lay1 = tex2D (_Splat0, IN.tc_Splat0);
	fixed3 lay2 = tex2D (_Splat1, IN.tc_Splat1);
	fixed3 lay3 = tex2D (_Splat2, IN.tc_Splat2);
	fixed3 lay4 = tex2D (_Splat3, IN.tc_Splat3);

	o.Alpha = 0.0;
	o.Albedo.rgb = (lay1 * splat_control.r + lay2 * splat_control.g + lay3 * splat_control.b + lay4 * splat_control.a);
}


// vertex-to-fragment interpolation data
// no lightmaps:
#ifndef LIGHTMAP_ON
// half-precision fragment shader registers:
#ifdef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc_Control tc_Splat0
  float4 custompack1 : TEXCOORD3; // tc_Splat1 tc_Splat2
  float2 custompack2 : TEXCOORD4; // tc_Splat3
  #if UNITY_SHOULD_SAMPLE_SH
  half3 sh : TEXCOORD5; // SH
  #endif
  UNITY_LIGHTING_COORDS(6,7)
  #if SHADER_TARGET >= 30
  float4 lmap : TEXCOORD8;
  #endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
// high-precision fragment shader registers:
#ifndef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc_Control tc_Splat0
  float4 custompack1 : TEXCOORD3; // tc_Splat1 tc_Splat2
  float2 custompack2 : TEXCOORD4; // tc_Splat3
  #if UNITY_SHOULD_SAMPLE_SH
  half3 sh : TEXCOORD5; // SH
  #endif
  UNITY_SHADOW_COORDS(6)
  #if SHADER_TARGET >= 30
  float4 lmap : TEXCOORD7;
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
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc_Control tc_Splat0
  float4 custompack1 : TEXCOORD3; // tc_Splat1 tc_Splat2
  float2 custompack2 : TEXCOORD4; // tc_Splat3
  float4 lmap : TEXCOORD5;
  UNITY_LIGHTING_COORDS(6,7)
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
// high-precision fragment shader registers:
#ifndef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc_Control tc_Splat0
  float4 custompack1 : TEXCOORD3; // tc_Splat1 tc_Splat2
  float2 custompack2 : TEXCOORD4; // tc_Splat3
  float4 lmap : TEXCOORD5;
  UNITY_SHADOW_COORDS(6)
  #ifdef DIRLIGHTMAP_COMBINED
  float3 tSpace0 : TEXCOORD7;
  float3 tSpace1 : TEXCOORD8;
  float3 tSpace2 : TEXCOORD9;
  #endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
#endif

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  vert (v, customInputData);
  o.custompack0.xy = customInputData.tc_Control;
  o.custompack0.zw = customInputData.tc_Splat0;
  o.custompack1.xy = customInputData.tc_Splat1;
  o.custompack1.zw = customInputData.tc_Splat2;
  o.custompack2.xy = customInputData.tc_Splat3;
  o.pos = UnityObjectToClipPos(v.vertex);
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
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  // prepare and unpack data
  Input surfIN;
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc_Control.x = 1.0;
  surfIN.tc_Splat0.x = 1.0;
  surfIN.tc_Splat1.x = 1.0;
  surfIN.tc_Splat2.x = 1.0;
  surfIN.tc_Splat3.x = 1.0;
  surfIN.worldPos.x = 1.0;
  surfIN.tc_Control = IN.custompack0.xy;
  surfIN.tc_Splat0 = IN.custompack0.zw;
  surfIN.tc_Splat1 = IN.custompack1.xy;
  surfIN.tc_Splat2 = IN.custompack1.zw;
  surfIN.tc_Splat3 = IN.custompack2.xy;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
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
  LightingLambert_GI(o, giInput, gi);

  // realtime lighting: call lighting function
  c += LightingLambert (o, gi);
  FinalColor (surfIN, o, c);
  UNITY_OPAQUE_ALPHA(c.a);
  return c;
}


#endif

// -------- variant for: INSTANCING_ON 
#if defined(INSTANCING_ON)
// Surface shader code generated based on:
// vertex modifier: 'vert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 14 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
//#pragma surface surf Lambert vertex:vert finalcolor:FinalColor
// //#pragma exclude_renderers xbox360 ps3
// //#pragma multi_compile DOD_FOG_NONE DOD_FOG_LINEAR DOD_FOG_EXP DOD_FOG_EXP2
//#pragma target 3.0

// #include "CustomFog.cginc"
struct Input {
	float2 tc_Control : TEXCOORD0;
	float2 tc_Splat0 : TEXCOORD1;
	float2 tc_Splat1 : TEXCOORD2;
	float2 tc_Splat2 : TEXCOORD3;
	float2 tc_Splat3 : TEXCOORD4;
	// CUSTOM_FOG_COORDS(5)
	float3 worldPos : TEXCOORD6;
};
 
sampler2D _Control;
float4 _Control_ST;
sampler2D _Splat0,_Splat1,_Splat2,_Splat3;
float4 _Splat0_ST,_Splat1_ST,_Splat2_ST,_Splat3_ST;

void vert(inout appdata_full v, out Input data)
{
	UNITY_INITIALIZE_OUTPUT(Input, data);
	data.tc_Control.xy = TRANSFORM_TEX(v.texcoord, _Control);
	data.tc_Splat0 = TRANSFORM_TEX(v.texcoord, _Splat0);
	data.tc_Splat1 = TRANSFORM_TEX(v.texcoord, _Splat1);
	data.tc_Splat2 = TRANSFORM_TEX(v.texcoord, _Splat2);
	data.tc_Splat3 = TRANSFORM_TEX(v.texcoord, _Splat3);
	data.worldPos = mul(unity_ObjectToWorld, v.vertex);
	// CUSTOM_TRANSFER_FOG(data.fogCoord, v.vertex);

}

void FinalColor(Input IN, SurfaceOutput o, inout fixed4 color)
{
	// CUSTOM_APPLY_FOG(IN.fogCoord, IN.worldPos, color.rgb);
}
 
void surf (Input IN, inout SurfaceOutput o) {
	fixed4 splat_control = tex2D (_Control, IN.tc_Control).rgba;
		
	fixed3 lay1 = tex2D (_Splat0, IN.tc_Splat0);
	fixed3 lay2 = tex2D (_Splat1, IN.tc_Splat1);
	fixed3 lay3 = tex2D (_Splat2, IN.tc_Splat2);
	fixed3 lay4 = tex2D (_Splat3, IN.tc_Splat3);

	o.Alpha = 0.0;
	o.Albedo.rgb = (lay1 * splat_control.r + lay2 * splat_control.g + lay3 * splat_control.b + lay4 * splat_control.a);
}


// vertex-to-fragment interpolation data
// no lightmaps:
#ifndef LIGHTMAP_ON
// half-precision fragment shader registers:
#ifdef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc_Control tc_Splat0
  float4 custompack1 : TEXCOORD3; // tc_Splat1 tc_Splat2
  float2 custompack2 : TEXCOORD4; // tc_Splat3
  #if UNITY_SHOULD_SAMPLE_SH
  half3 sh : TEXCOORD5; // SH
  #endif
  UNITY_LIGHTING_COORDS(6,7)
  #if SHADER_TARGET >= 30
  float4 lmap : TEXCOORD8;
  #endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
// high-precision fragment shader registers:
#ifndef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc_Control tc_Splat0
  float4 custompack1 : TEXCOORD3; // tc_Splat1 tc_Splat2
  float2 custompack2 : TEXCOORD4; // tc_Splat3
  #if UNITY_SHOULD_SAMPLE_SH
  half3 sh : TEXCOORD5; // SH
  #endif
  UNITY_SHADOW_COORDS(6)
  #if SHADER_TARGET >= 30
  float4 lmap : TEXCOORD7;
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
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc_Control tc_Splat0
  float4 custompack1 : TEXCOORD3; // tc_Splat1 tc_Splat2
  float2 custompack2 : TEXCOORD4; // tc_Splat3
  float4 lmap : TEXCOORD5;
  UNITY_LIGHTING_COORDS(6,7)
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
// high-precision fragment shader registers:
#ifndef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc_Control tc_Splat0
  float4 custompack1 : TEXCOORD3; // tc_Splat1 tc_Splat2
  float2 custompack2 : TEXCOORD4; // tc_Splat3
  float4 lmap : TEXCOORD5;
  UNITY_SHADOW_COORDS(6)
  #ifdef DIRLIGHTMAP_COMBINED
  float3 tSpace0 : TEXCOORD7;
  float3 tSpace1 : TEXCOORD8;
  float3 tSpace2 : TEXCOORD9;
  #endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
#endif

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  vert (v, customInputData);
  o.custompack0.xy = customInputData.tc_Control;
  o.custompack0.zw = customInputData.tc_Splat0;
  o.custompack1.xy = customInputData.tc_Splat1;
  o.custompack1.zw = customInputData.tc_Splat2;
  o.custompack2.xy = customInputData.tc_Splat3;
  o.pos = UnityObjectToClipPos(v.vertex);
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
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  // prepare and unpack data
  Input surfIN;
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc_Control.x = 1.0;
  surfIN.tc_Splat0.x = 1.0;
  surfIN.tc_Splat1.x = 1.0;
  surfIN.tc_Splat2.x = 1.0;
  surfIN.tc_Splat3.x = 1.0;
  surfIN.worldPos.x = 1.0;
  surfIN.tc_Control = IN.custompack0.xy;
  surfIN.tc_Splat0 = IN.custompack0.zw;
  surfIN.tc_Splat1 = IN.custompack1.xy;
  surfIN.tc_Splat2 = IN.custompack1.zw;
  surfIN.tc_Splat3 = IN.custompack2.xy;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
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
  LightingLambert_GI(o, giInput, gi);

  // realtime lighting: call lighting function
  c += LightingLambert (o, gi);
  FinalColor (surfIN, o, c);
  UNITY_OPAQUE_ALPHA(c.a);
  return c;
}


#endif


ENDCG

}

	// ---- forward rendering additive lights pass:
	Pass {
		Name "FORWARD"
		//Tags { "LightMode" = "ForwardAdd" }
		ZWrite Off Blend One One

CGPROGRAM
// compile directives
#pragma vertex vert_surf
#pragma fragment frag_surf
#pragma target 3.0
#pragma multi_compile_instancing
#pragma skip_variants INSTANCING_ON
#pragma multi_compile_fwdadd
#include "HLSLSupport.cginc"
#define UNITY_INSTANCED_LOD_FADE
#define UNITY_INSTANCED_SH
#define UNITY_INSTANCED_LIGHTMAPSTS
#include "UnityShaderVariables.cginc"
#include "UnityShaderUtilities.cginc"
// -------- variant for: <when no other keywords are defined>
#if !defined(INSTANCING_ON)
// Surface shader code generated based on:
// vertex modifier: 'vert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 14 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
//#pragma surface surf Lambert vertex:vert finalcolor:FinalColor
// //#pragma exclude_renderers xbox360 ps3
// //#pragma multi_compile DOD_FOG_NONE DOD_FOG_LINEAR DOD_FOG_EXP DOD_FOG_EXP2
//#pragma target 3.0

// #include "CustomFog.cginc"
struct Input {
	float2 tc_Control : TEXCOORD0;
	float2 tc_Splat0 : TEXCOORD1;
	float2 tc_Splat1 : TEXCOORD2;
	float2 tc_Splat2 : TEXCOORD3;
	float2 tc_Splat3 : TEXCOORD4;
	// CUSTOM_FOG_COORDS(5)
	float3 worldPos : TEXCOORD6;
};
 
sampler2D _Control;
float4 _Control_ST;
sampler2D _Splat0,_Splat1,_Splat2,_Splat3;
float4 _Splat0_ST,_Splat1_ST,_Splat2_ST,_Splat3_ST;

void vert(inout appdata_full v, out Input data)
{
	UNITY_INITIALIZE_OUTPUT(Input, data);
	data.tc_Control.xy = TRANSFORM_TEX(v.texcoord, _Control);
	data.tc_Splat0 = TRANSFORM_TEX(v.texcoord, _Splat0);
	data.tc_Splat1 = TRANSFORM_TEX(v.texcoord, _Splat1);
	data.tc_Splat2 = TRANSFORM_TEX(v.texcoord, _Splat2);
	data.tc_Splat3 = TRANSFORM_TEX(v.texcoord, _Splat3);
	data.worldPos = mul(unity_ObjectToWorld, v.vertex);
	// CUSTOM_TRANSFER_FOG(data.fogCoord, v.vertex);

}

void FinalColor(Input IN, SurfaceOutput o, inout fixed4 color)
{
	// CUSTOM_APPLY_FOG(IN.fogCoord, IN.worldPos, color.rgb);
}
 
void surf (Input IN, inout SurfaceOutput o) {
	fixed4 splat_control = tex2D (_Control, IN.tc_Control).rgba;
		
	fixed3 lay1 = tex2D (_Splat0, IN.tc_Splat0);
	fixed3 lay2 = tex2D (_Splat1, IN.tc_Splat1);
	fixed3 lay3 = tex2D (_Splat2, IN.tc_Splat2);
	fixed3 lay4 = tex2D (_Splat3, IN.tc_Splat3);

	o.Alpha = 0.0;
	o.Albedo.rgb = (lay1 * splat_control.r + lay2 * splat_control.g + lay3 * splat_control.b + lay4 * splat_control.a);
}


// vertex-to-fragment interpolation data
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc_Control tc_Splat0
  float4 custompack1 : TEXCOORD3; // tc_Splat1 tc_Splat2
  float2 custompack2 : TEXCOORD4; // tc_Splat3
  UNITY_LIGHTING_COORDS(5,6)
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  vert (v, customInputData);
  o.custompack0.xy = customInputData.tc_Control;
  o.custompack0.zw = customInputData.tc_Splat0;
  o.custompack1.xy = customInputData.tc_Splat1;
  o.custompack1.zw = customInputData.tc_Splat2;
  o.custompack2.xy = customInputData.tc_Splat3;
  o.pos = UnityObjectToClipPos(v.vertex);
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  o.worldPos.xyz = worldPos;
  o.worldNormal = worldNormal;

  UNITY_TRANSFER_LIGHTING(o,v.texcoord1.xy); // pass shadow and, possibly, light cookie coordinates to pixel shader
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  // prepare and unpack data
  Input surfIN;
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc_Control.x = 1.0;
  surfIN.tc_Splat0.x = 1.0;
  surfIN.tc_Splat1.x = 1.0;
  surfIN.tc_Splat2.x = 1.0;
  surfIN.tc_Splat3.x = 1.0;
  surfIN.worldPos.x = 1.0;
  surfIN.tc_Control = IN.custompack0.xy;
  surfIN.tc_Splat0 = IN.custompack0.zw;
  surfIN.tc_Splat1 = IN.custompack1.xy;
  surfIN.tc_Splat2 = IN.custompack1.zw;
  surfIN.tc_Splat3 = IN.custompack2.xy;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);
  o.Normal = IN.worldNormal;
  normalWorldVertex = IN.worldNormal;

  // call surface function
  surf (surfIN, o);
  UNITY_LIGHT_ATTENUATION(atten, IN, worldPos)
  fixed4 c = 0;

  // Setup lighting environment
  UnityGI gi;
  UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
  gi.indirect.diffuse = 0;
  gi.indirect.specular = 0;
  gi.light.color = _LightColor0.rgb;
  gi.light.dir = lightDir;
  gi.light.color *= atten;
  c += LightingLambert (o, gi);
  c.a = 0.0;
  FinalColor (surfIN, o, c);
  UNITY_OPAQUE_ALPHA(c.a);
  return c;
}


#endif


ENDCG

}

	// ---- deferred lighting base geometry pass:
//	Pass {
//		Name "PREPASS"
//		Tags { "LightMode" = "PrePassBase" }

//CGPROGRAM
//// compile directives
//#pragma vertex vert_surf
//#pragma fragment frag_surf
//#pragma target 3.0
//#pragma multi_compile_instancing
//#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2

//#include "HLSLSupport.cginc"
//#define UNITY_INSTANCED_LOD_FADE
//#define UNITY_INSTANCED_SH
//#define UNITY_INSTANCED_LIGHTMAPSTS
//#include "UnityShaderVariables.cginc"
//#include "UnityShaderUtilities.cginc"
//// -------- variant for: <when no other keywords are defined>
//#if !defined(INSTANCING_ON)
//// Surface shader code generated based on:
//// vertex modifier: 'vert'
//// writes to per-pixel normal: no
//// writes to emission: no
//// writes to occlusion: no
//// needs world space reflection vector: no
//// needs world space normal vector: no
//// needs screen space position: no
//// needs world space position: no
//// needs view direction: no
//// needs world space view direction: no
//// needs world space position for lighting: YES
//// needs world space view direction for lighting: no
//// needs world space view direction for lightmaps: no
//// needs vertex color: no
//// needs VFACE: no
//// needs SV_IsFrontFace: no
//// passes tangent-to-world matrix to pixel shader: no
//// reads from normal: YES
//// 0 texcoords actually used
//#include "UnityCG.cginc"
//#include "Lighting.cginc"

//#define INTERNAL_DATA
//#define WorldReflectionVector(data,normal) data.worldRefl
//#define WorldNormalVector(data,normal) normal

//// Original surface shader snippet:
//#line 14 ""
//#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
//#endif
///* UNITY: Original start of shader */
////#pragma surface surf Lambert vertex:vert finalcolor:FinalColor
//// //#pragma exclude_renderers xbox360 ps3
//// //#pragma multi_compile DOD_FOG_NONE DOD_FOG_LINEAR DOD_FOG_EXP DOD_FOG_EXP2
////#pragma target 3.0

//// #include "CustomFog.cginc"
//struct Input {
//	float2 tc_Control : TEXCOORD0;
//	float2 tc_Splat0 : TEXCOORD1;
//	float2 tc_Splat1 : TEXCOORD2;
//	float2 tc_Splat2 : TEXCOORD3;
//	float2 tc_Splat3 : TEXCOORD4;
//	// CUSTOM_FOG_COORDS(5)
//	float3 worldPos : TEXCOORD6;
//};
 
//sampler2D _Control;
//float4 _Control_ST;
//sampler2D _Splat0,_Splat1,_Splat2,_Splat3;
//float4 _Splat0_ST,_Splat1_ST,_Splat2_ST,_Splat3_ST;

//void vert(inout appdata_full v, out Input data)
//{
//	UNITY_INITIALIZE_OUTPUT(Input, data);
//	data.tc_Control.xy = TRANSFORM_TEX(v.texcoord, _Control);
//	data.tc_Splat0 = TRANSFORM_TEX(v.texcoord, _Splat0);
//	data.tc_Splat1 = TRANSFORM_TEX(v.texcoord, _Splat1);
//	data.tc_Splat2 = TRANSFORM_TEX(v.texcoord, _Splat2);
//	data.tc_Splat3 = TRANSFORM_TEX(v.texcoord, _Splat3);
//	data.worldPos = mul(unity_ObjectToWorld, v.vertex);
//	// CUSTOM_TRANSFER_FOG(data.fogCoord, v.vertex);

//}

//void FinalColor(Input IN, SurfaceOutput o, inout fixed4 color)
//{
//	// CUSTOM_APPLY_FOG(IN.fogCoord, IN.worldPos, color.rgb);
//}
 
//void surf (Input IN, inout SurfaceOutput o) {
//	fixed4 splat_control = tex2D (_Control, IN.tc_Control).rgba;
		
//	fixed3 lay1 = tex2D (_Splat0, IN.tc_Splat0);
//	fixed3 lay2 = tex2D (_Splat1, IN.tc_Splat1);
//	fixed3 lay3 = tex2D (_Splat2, IN.tc_Splat2);
//	fixed3 lay4 = tex2D (_Splat3, IN.tc_Splat3);

//	o.Alpha = 0.0;
//	o.Albedo.rgb = (lay1 * splat_control.r + lay2 * splat_control.g + lay3 * splat_control.b + lay4 * splat_control.a);
//}


//// vertex-to-fragment interpolation data
//struct v2f_surf {
//  UNITY_POSITION(pos);
//  float3 worldNormal : TEXCOORD0;
//  float3 worldPos : TEXCOORD1;
//  float4 custompack0 : TEXCOORD2; // tc_Control tc_Splat0
//  float4 custompack1 : TEXCOORD3; // tc_Splat1 tc_Splat2
//  float2 custompack2 : TEXCOORD4; // tc_Splat3
//  UNITY_VERTEX_INPUT_INSTANCE_ID
//  UNITY_VERTEX_OUTPUT_STEREO
//};

//// vertex shader
//v2f_surf vert_surf (appdata_full v) {
//  UNITY_SETUP_INSTANCE_ID(v);
//  v2f_surf o;
//  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
//  UNITY_TRANSFER_INSTANCE_ID(v,o);
//  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
//  Input customInputData;
//  vert (v, customInputData);
//  o.custompack0.xy = customInputData.tc_Control;
//  o.custompack0.zw = customInputData.tc_Splat0;
//  o.custompack1.xy = customInputData.tc_Splat1;
//  o.custompack1.zw = customInputData.tc_Splat2;
//  o.custompack2.xy = customInputData.tc_Splat3;
//  o.pos = UnityObjectToClipPos(v.vertex);
//  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
//  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
//  o.worldPos.xyz = worldPos;
//  o.worldNormal = worldNormal;
//  return o;
//}

//// fragment shader
//fixed4 frag_surf (v2f_surf IN) : SV_Target {
//  UNITY_SETUP_INSTANCE_ID(IN);
//  // prepare and unpack data
//  Input surfIN;
//  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
//  surfIN.tc_Control.x = 1.0;
//  surfIN.tc_Splat0.x = 1.0;
//  surfIN.tc_Splat1.x = 1.0;
//  surfIN.tc_Splat2.x = 1.0;
//  surfIN.tc_Splat3.x = 1.0;
//  surfIN.worldPos.x = 1.0;
//  surfIN.tc_Control = IN.custompack0.xy;
//  surfIN.tc_Splat0 = IN.custompack0.zw;
//  surfIN.tc_Splat1 = IN.custompack1.xy;
//  surfIN.tc_Splat2 = IN.custompack1.zw;
//  surfIN.tc_Splat3 = IN.custompack2.xy;
//  float3 worldPos = IN.worldPos.xyz;
//  #ifndef USING_DIRECTIONAL_LIGHT
//    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
//  #else
//    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
//  #endif
//  #ifdef UNITY_COMPILER_HLSL
//  SurfaceOutput o = (SurfaceOutput)0;
//  #else
//  SurfaceOutput o;
//  #endif
//  o.Albedo = 0.0;
//  o.Emission = 0.0;
//  o.Specular = 0.0;
//  o.Alpha = 0.0;
//  o.Gloss = 0.0;
//  fixed3 normalWorldVertex = fixed3(0,0,1);
//  o.Normal = IN.worldNormal;
//  normalWorldVertex = IN.worldNormal;

//  // call surface function
//  surf (surfIN, o);

//  // output normal and specular
//  fixed4 res;
//  res.rgb = o.Normal * 0.5 + 0.5;
//  res.a = o.Specular;
//  return res;
//}


//#endif

//// -------- variant for: INSTANCING_ON 
//#if defined(INSTANCING_ON)
//// Surface shader code generated based on:
//// vertex modifier: 'vert'
//// writes to per-pixel normal: no
//// writes to emission: no
//// writes to occlusion: no
//// needs world space reflection vector: no
//// needs world space normal vector: no
//// needs screen space position: no
//// needs world space position: no
//// needs view direction: no
//// needs world space view direction: no
//// needs world space position for lighting: YES
//// needs world space view direction for lighting: no
//// needs world space view direction for lightmaps: no
//// needs vertex color: no
//// needs VFACE: no
//// needs SV_IsFrontFace: no
//// passes tangent-to-world matrix to pixel shader: no
//// reads from normal: YES
//// 0 texcoords actually used
//#include "UnityCG.cginc"
//#include "Lighting.cginc"

//#define INTERNAL_DATA
//#define WorldReflectionVector(data,normal) data.worldRefl
//#define WorldNormalVector(data,normal) normal

//// Original surface shader snippet:
//#line 14 ""
//#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
//#endif
///* UNITY: Original start of shader */
////#pragma surface surf Lambert vertex:vert finalcolor:FinalColor
//// //#pragma exclude_renderers xbox360 ps3
//// //#pragma multi_compile DOD_FOG_NONE DOD_FOG_LINEAR DOD_FOG_EXP DOD_FOG_EXP2
////#pragma target 3.0

//// #include "CustomFog.cginc"
//struct Input {
//	float2 tc_Control : TEXCOORD0;
//	float2 tc_Splat0 : TEXCOORD1;
//	float2 tc_Splat1 : TEXCOORD2;
//	float2 tc_Splat2 : TEXCOORD3;
//	float2 tc_Splat3 : TEXCOORD4;
//	// CUSTOM_FOG_COORDS(5)
//	float3 worldPos : TEXCOORD6;
//};
 
//sampler2D _Control;
//float4 _Control_ST;
//sampler2D _Splat0,_Splat1,_Splat2,_Splat3;
//float4 _Splat0_ST,_Splat1_ST,_Splat2_ST,_Splat3_ST;

//void vert(inout appdata_full v, out Input data)
//{
//	UNITY_INITIALIZE_OUTPUT(Input, data);
//	data.tc_Control.xy = TRANSFORM_TEX(v.texcoord, _Control);
//	data.tc_Splat0 = TRANSFORM_TEX(v.texcoord, _Splat0);
//	data.tc_Splat1 = TRANSFORM_TEX(v.texcoord, _Splat1);
//	data.tc_Splat2 = TRANSFORM_TEX(v.texcoord, _Splat2);
//	data.tc_Splat3 = TRANSFORM_TEX(v.texcoord, _Splat3);
//	data.worldPos = mul(unity_ObjectToWorld, v.vertex);
//	// CUSTOM_TRANSFER_FOG(data.fogCoord, v.vertex);

//}

//void FinalColor(Input IN, SurfaceOutput o, inout fixed4 color)
//{
//	// CUSTOM_APPLY_FOG(IN.fogCoord, IN.worldPos, color.rgb);
//}
 
//void surf (Input IN, inout SurfaceOutput o) {
//	fixed4 splat_control = tex2D (_Control, IN.tc_Control).rgba;
		
//	fixed3 lay1 = tex2D (_Splat0, IN.tc_Splat0);
//	fixed3 lay2 = tex2D (_Splat1, IN.tc_Splat1);
//	fixed3 lay3 = tex2D (_Splat2, IN.tc_Splat2);
//	fixed3 lay4 = tex2D (_Splat3, IN.tc_Splat3);

//	o.Alpha = 0.0;
//	o.Albedo.rgb = (lay1 * splat_control.r + lay2 * splat_control.g + lay3 * splat_control.b + lay4 * splat_control.a);
//}


//// vertex-to-fragment interpolation data
//struct v2f_surf {
//  UNITY_POSITION(pos);
//  float3 worldNormal : TEXCOORD0;
//  float3 worldPos : TEXCOORD1;
//  float4 custompack0 : TEXCOORD2; // tc_Control tc_Splat0
//  float4 custompack1 : TEXCOORD3; // tc_Splat1 tc_Splat2
//  float2 custompack2 : TEXCOORD4; // tc_Splat3
//  UNITY_VERTEX_INPUT_INSTANCE_ID
//  UNITY_VERTEX_OUTPUT_STEREO
//};

//// vertex shader
//v2f_surf vert_surf (appdata_full v) {
//  UNITY_SETUP_INSTANCE_ID(v);
//  v2f_surf o;
//  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
//  UNITY_TRANSFER_INSTANCE_ID(v,o);
//  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
//  Input customInputData;
//  vert (v, customInputData);
//  o.custompack0.xy = customInputData.tc_Control;
//  o.custompack0.zw = customInputData.tc_Splat0;
//  o.custompack1.xy = customInputData.tc_Splat1;
//  o.custompack1.zw = customInputData.tc_Splat2;
//  o.custompack2.xy = customInputData.tc_Splat3;
//  o.pos = UnityObjectToClipPos(v.vertex);
//  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
//  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
//  o.worldPos.xyz = worldPos;
//  o.worldNormal = worldNormal;
//  return o;
//}

//// fragment shader
//fixed4 frag_surf (v2f_surf IN) : SV_Target {
//  UNITY_SETUP_INSTANCE_ID(IN);
//  // prepare and unpack data
//  Input surfIN;
//  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
//  surfIN.tc_Control.x = 1.0;
//  surfIN.tc_Splat0.x = 1.0;
//  surfIN.tc_Splat1.x = 1.0;
//  surfIN.tc_Splat2.x = 1.0;
//  surfIN.tc_Splat3.x = 1.0;
//  surfIN.worldPos.x = 1.0;
//  surfIN.tc_Control = IN.custompack0.xy;
//  surfIN.tc_Splat0 = IN.custompack0.zw;
//  surfIN.tc_Splat1 = IN.custompack1.xy;
//  surfIN.tc_Splat2 = IN.custompack1.zw;
//  surfIN.tc_Splat3 = IN.custompack2.xy;
//  float3 worldPos = IN.worldPos.xyz;
//  #ifndef USING_DIRECTIONAL_LIGHT
//    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
//  #else
//    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
//  #endif
//  #ifdef UNITY_COMPILER_HLSL
//  SurfaceOutput o = (SurfaceOutput)0;
//  #else
//  SurfaceOutput o;
//  #endif
//  o.Albedo = 0.0;
//  o.Emission = 0.0;
//  o.Specular = 0.0;
//  o.Alpha = 0.0;
//  o.Gloss = 0.0;
//  fixed3 normalWorldVertex = fixed3(0,0,1);
//  o.Normal = IN.worldNormal;
//  normalWorldVertex = IN.worldNormal;

//  // call surface function
//  surf (surfIN, o);

//  // output normal and specular
//  fixed4 res;
//  res.rgb = o.Normal * 0.5 + 0.5;
//  res.a = o.Specular;
//  return res;
//}


//#endif


//ENDCG

//}

//	// ---- deferred lighting final pass:
//	Pass {
//		Name "PREPASS"
//		Tags { "LightMode" = "PrePassFinal" }
//		ZWrite Off

//CGPROGRAM
//// compile directives
//#pragma vertex vert_surf
//#pragma fragment frag_surf
//#pragma target 3.0
//#pragma multi_compile_instancing
//#pragma multi_compile_prepassfinal
//#include "HLSLSupport.cginc"
//#define UNITY_INSTANCED_LOD_FADE
//#define UNITY_INSTANCED_SH
//#define UNITY_INSTANCED_LIGHTMAPSTS
//#include "UnityShaderVariables.cginc"
//#include "UnityShaderUtilities.cginc"
//// -------- variant for: <when no other keywords are defined>
//#if !defined(INSTANCING_ON)
//// Surface shader code generated based on:
//// vertex modifier: 'vert'
//// writes to per-pixel normal: no
//// writes to emission: no
//// writes to occlusion: no
//// needs world space reflection vector: no
//// needs world space normal vector: no
//// needs screen space position: no
//// needs world space position: no
//// needs view direction: no
//// needs world space view direction: no
//// needs world space position for lighting: YES
//// needs world space view direction for lighting: no
//// needs world space view direction for lightmaps: no
//// needs vertex color: no
//// needs VFACE: no
//// needs SV_IsFrontFace: no
//// passes tangent-to-world matrix to pixel shader: no
//// reads from normal: no
//// 0 texcoords actually used
//#include "UnityCG.cginc"
//#include "Lighting.cginc"

//#define INTERNAL_DATA
//#define WorldReflectionVector(data,normal) data.worldRefl
//#define WorldNormalVector(data,normal) normal

//// Original surface shader snippet:
//#line 14 ""
//#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
//#endif
///* UNITY: Original start of shader */
////#pragma surface surf Lambert vertex:vert finalcolor:FinalColor
//// //#pragma exclude_renderers xbox360 ps3
//// //#pragma multi_compile DOD_FOG_NONE DOD_FOG_LINEAR DOD_FOG_EXP DOD_FOG_EXP2
////#pragma target 3.0

//// #include "CustomFog.cginc"
//struct Input {
//	float2 tc_Control : TEXCOORD0;
//	float2 tc_Splat0 : TEXCOORD1;
//	float2 tc_Splat1 : TEXCOORD2;
//	float2 tc_Splat2 : TEXCOORD3;
//	float2 tc_Splat3 : TEXCOORD4;
//	// CUSTOM_FOG_COORDS(5)
//	float3 worldPos : TEXCOORD6;
//};
 
//sampler2D _Control;
//float4 _Control_ST;
//sampler2D _Splat0,_Splat1,_Splat2,_Splat3;
//float4 _Splat0_ST,_Splat1_ST,_Splat2_ST,_Splat3_ST;

//void vert(inout appdata_full v, out Input data)
//{
//	UNITY_INITIALIZE_OUTPUT(Input, data);
//	data.tc_Control.xy = TRANSFORM_TEX(v.texcoord, _Control);
//	data.tc_Splat0 = TRANSFORM_TEX(v.texcoord, _Splat0);
//	data.tc_Splat1 = TRANSFORM_TEX(v.texcoord, _Splat1);
//	data.tc_Splat2 = TRANSFORM_TEX(v.texcoord, _Splat2);
//	data.tc_Splat3 = TRANSFORM_TEX(v.texcoord, _Splat3);
//	data.worldPos = mul(unity_ObjectToWorld, v.vertex);
//	// CUSTOM_TRANSFER_FOG(data.fogCoord, v.vertex);

//}

//void FinalColor(Input IN, SurfaceOutput o, inout fixed4 color)
//{
//	// CUSTOM_APPLY_FOG(IN.fogCoord, IN.worldPos, color.rgb);
//}
 
//void surf (Input IN, inout SurfaceOutput o) {
//	fixed4 splat_control = tex2D (_Control, IN.tc_Control).rgba;
		
//	fixed3 lay1 = tex2D (_Splat0, IN.tc_Splat0);
//	fixed3 lay2 = tex2D (_Splat1, IN.tc_Splat1);
//	fixed3 lay3 = tex2D (_Splat2, IN.tc_Splat2);
//	fixed3 lay4 = tex2D (_Splat3, IN.tc_Splat3);

//	o.Alpha = 0.0;
//	o.Albedo.rgb = (lay1 * splat_control.r + lay2 * splat_control.g + lay3 * splat_control.b + lay4 * splat_control.a);
//}


//// vertex-to-fragment interpolation data
//struct v2f_surf {
//  UNITY_POSITION(pos);
//  float3 worldPos : TEXCOORD0;
//  float4 custompack0 : TEXCOORD1; // tc_Control tc_Splat0
//  float4 custompack1 : TEXCOORD2; // tc_Splat1 tc_Splat2
//  float2 custompack2 : TEXCOORD3; // tc_Splat3
//  float4 screen : TEXCOORD4;
//  float4 lmap : TEXCOORD5;
//#ifndef LIGHTMAP_ON
//  float3 vlight : TEXCOORD6;
//#else
//#ifdef DIRLIGHTMAP_OFF
//  float4 lmapFadePos : TEXCOORD6;
//#endif
//#endif
//  #if defined(LIGHTMAP_ON) && defined(DIRLIGHTMAP_COMBINED)
//  float3 tSpace0 : TEXCOORD7;
//  float3 tSpace1 : TEXCOORD8;
//  float3 tSpace2 : TEXCOORD9;
//  #endif
//  UNITY_VERTEX_INPUT_INSTANCE_ID
//  UNITY_VERTEX_OUTPUT_STEREO
//};

//// vertex shader
//v2f_surf vert_surf (appdata_full v) {
//  UNITY_SETUP_INSTANCE_ID(v);
//  v2f_surf o;
//  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
//  UNITY_TRANSFER_INSTANCE_ID(v,o);
//  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
//  Input customInputData;
//  vert (v, customInputData);
//  o.custompack0.xy = customInputData.tc_Control;
//  o.custompack0.zw = customInputData.tc_Splat0;
//  o.custompack1.xy = customInputData.tc_Splat1;
//  o.custompack1.zw = customInputData.tc_Splat2;
//  o.custompack2.xy = customInputData.tc_Splat3;
//  o.pos = UnityObjectToClipPos(v.vertex);
//  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
//  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
//  #if defined(LIGHTMAP_ON) && defined(DIRLIGHTMAP_COMBINED)
//  fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
//  fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
//  fixed3 worldBinormal = cross(worldNormal, worldTangent) * tangentSign;
//  #endif
//  #if defined(LIGHTMAP_ON) && defined(DIRLIGHTMAP_COMBINED) && !defined(UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS)
//  o.tSpace0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
//  o.tSpace1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
//  o.tSpace2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);
//  #endif
//  o.worldPos.xyz = worldPos;
//  o.screen = ComputeScreenPos (o.pos);
//#ifdef DYNAMICLIGHTMAP_ON
//  o.lmap.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
//#else
//  o.lmap.zw = 0;
//#endif
//#ifdef LIGHTMAP_ON
//  o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
//  #ifdef DIRLIGHTMAP_OFF
//    o.lmapFadePos.xyz = (mul(unity_ObjectToWorld, v.vertex).xyz - unity_ShadowFadeCenterAndType.xyz) * unity_ShadowFadeCenterAndType.w;
//    o.lmapFadePos.w = (-UnityObjectToViewPos(v.vertex).z) * (1.0 - unity_ShadowFadeCenterAndType.w);
//  #endif
//#else
//  o.lmap.xy = 0;
//  float3 worldN = UnityObjectToWorldNormal(v.normal);
//  o.vlight = ShadeSH9 (float4(worldN,1.0));
//#endif
//  return o;
//}
//sampler2D _LightBuffer;
//sampler2D _CameraNormalsTexture;
//#ifdef LIGHTMAP_ON
//float4 unity_LightmapFade;
//#endif
//fixed4 unity_Ambient;

//// fragment shader
//fixed4 frag_surf (v2f_surf IN) : SV_Target {
//  UNITY_SETUP_INSTANCE_ID(IN);
//  // prepare and unpack data
//  Input surfIN;
//  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
//  surfIN.tc_Control.x = 1.0;
//  surfIN.tc_Splat0.x = 1.0;
//  surfIN.tc_Splat1.x = 1.0;
//  surfIN.tc_Splat2.x = 1.0;
//  surfIN.tc_Splat3.x = 1.0;
//  surfIN.worldPos.x = 1.0;
//  surfIN.tc_Control = IN.custompack0.xy;
//  surfIN.tc_Splat0 = IN.custompack0.zw;
//  surfIN.tc_Splat1 = IN.custompack1.xy;
//  surfIN.tc_Splat2 = IN.custompack1.zw;
//  surfIN.tc_Splat3 = IN.custompack2.xy;
//  float3 worldPos = IN.worldPos.xyz;
//  #ifndef USING_DIRECTIONAL_LIGHT
//    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
//  #else
//    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
//  #endif
//  #ifdef UNITY_COMPILER_HLSL
//  SurfaceOutput o = (SurfaceOutput)0;
//  #else
//  SurfaceOutput o;
//  #endif
//  o.Albedo = 0.0;
//  o.Emission = 0.0;
//  o.Specular = 0.0;
//  o.Alpha = 0.0;
//  o.Gloss = 0.0;
//  fixed3 normalWorldVertex = fixed3(0,0,1);

//  // call surface function
//  surf (surfIN, o);
//  half4 light = tex2Dproj (_LightBuffer, UNITY_PROJ_COORD(IN.screen));
//#if defined (SHADER_API_MOBILE)
//  light = max(light, half4(0.001, 0.001, 0.001, 0.001));
//#endif
//#ifndef UNITY_HDR_ON
//  light = -log2(light);
//#endif
//  #ifdef LIGHTMAP_ON
//    #ifdef DIRLIGHTMAP_OFF
//      // single lightmap
//      fixed4 lmtex = UNITY_SAMPLE_TEX2D(unity_Lightmap, IN.lmap.xy);
//      fixed3 lm = DecodeLightmap (lmtex);
//      light.rgb += lm;
//    #elif DIRLIGHTMAP_COMBINED
//      half4 nspec = tex2Dproj (_CameraNormalsTexture, UNITY_PROJ_COORD(IN.screen));
//      half3 normal = nspec.rgb * 2 - 1;
//      o.Normal = normalize(normal);
//      // directional lightmaps
//      fixed4 lmtex = UNITY_SAMPLE_TEX2D(unity_Lightmap, IN.lmap.xy);
//      half4 lm = half4(DecodeLightmap(lmtex), 0);
//      light += lm;
//    #endif // DIRLIGHTMAP_OFF
//  #else
//    light.rgb += IN.vlight;
//  #endif // LIGHTMAP_ON

//  #ifdef DYNAMICLIGHTMAP_ON
//  fixed4 dynlmtex = UNITY_SAMPLE_TEX2D(unity_DynamicLightmap, IN.lmap.zw);
//  light.rgb += DecodeRealtimeLightmap (dynlmtex);
//  #endif

//  fixed4 c = LightingLambert_PrePass (o, light);
//  FinalColor (surfIN, o, c);
//  UNITY_OPAQUE_ALPHA(c.a);
//  return c;
//}


//#endif

//// -------- variant for: INSTANCING_ON 
//#if defined(INSTANCING_ON)
//// Surface shader code generated based on:
//// vertex modifier: 'vert'
//// writes to per-pixel normal: no
//// writes to emission: no
//// writes to occlusion: no
//// needs world space reflection vector: no
//// needs world space normal vector: no
//// needs screen space position: no
//// needs world space position: no
//// needs view direction: no
//// needs world space view direction: no
//// needs world space position for lighting: YES
//// needs world space view direction for lighting: no
//// needs world space view direction for lightmaps: no
//// needs vertex color: no
//// needs VFACE: no
//// needs SV_IsFrontFace: no
//// passes tangent-to-world matrix to pixel shader: no
//// reads from normal: no
//// 0 texcoords actually used
//#include "UnityCG.cginc"
//#include "Lighting.cginc"

//#define INTERNAL_DATA
//#define WorldReflectionVector(data,normal) data.worldRefl
//#define WorldNormalVector(data,normal) normal

//// Original surface shader snippet:
//#line 14 ""
//#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
//#endif
///* UNITY: Original start of shader */
////#pragma surface surf Lambert vertex:vert finalcolor:FinalColor
//// //#pragma exclude_renderers xbox360 ps3
//// //#pragma multi_compile DOD_FOG_NONE DOD_FOG_LINEAR DOD_FOG_EXP DOD_FOG_EXP2
////#pragma target 3.0

//// #include "CustomFog.cginc"
//struct Input {
//	float2 tc_Control : TEXCOORD0;
//	float2 tc_Splat0 : TEXCOORD1;
//	float2 tc_Splat1 : TEXCOORD2;
//	float2 tc_Splat2 : TEXCOORD3;
//	float2 tc_Splat3 : TEXCOORD4;
//	// CUSTOM_FOG_COORDS(5)
//	float3 worldPos : TEXCOORD6;
//};
 
//sampler2D _Control;
//float4 _Control_ST;
//sampler2D _Splat0,_Splat1,_Splat2,_Splat3;
//float4 _Splat0_ST,_Splat1_ST,_Splat2_ST,_Splat3_ST;

//void vert(inout appdata_full v, out Input data)
//{
//	UNITY_INITIALIZE_OUTPUT(Input, data);
//	data.tc_Control.xy = TRANSFORM_TEX(v.texcoord, _Control);
//	data.tc_Splat0 = TRANSFORM_TEX(v.texcoord, _Splat0);
//	data.tc_Splat1 = TRANSFORM_TEX(v.texcoord, _Splat1);
//	data.tc_Splat2 = TRANSFORM_TEX(v.texcoord, _Splat2);
//	data.tc_Splat3 = TRANSFORM_TEX(v.texcoord, _Splat3);
//	data.worldPos = mul(unity_ObjectToWorld, v.vertex);
//	// CUSTOM_TRANSFER_FOG(data.fogCoord, v.vertex);

//}

//void FinalColor(Input IN, SurfaceOutput o, inout fixed4 color)
//{
//	// CUSTOM_APPLY_FOG(IN.fogCoord, IN.worldPos, color.rgb);
//}
 
//void surf (Input IN, inout SurfaceOutput o) {
//	fixed4 splat_control = tex2D (_Control, IN.tc_Control).rgba;
		
//	fixed3 lay1 = tex2D (_Splat0, IN.tc_Splat0);
//	fixed3 lay2 = tex2D (_Splat1, IN.tc_Splat1);
//	fixed3 lay3 = tex2D (_Splat2, IN.tc_Splat2);
//	fixed3 lay4 = tex2D (_Splat3, IN.tc_Splat3);

//	o.Alpha = 0.0;
//	o.Albedo.rgb = (lay1 * splat_control.r + lay2 * splat_control.g + lay3 * splat_control.b + lay4 * splat_control.a);
//}


//// vertex-to-fragment interpolation data
//struct v2f_surf {
//  UNITY_POSITION(pos);
//  float3 worldPos : TEXCOORD0;
//  float4 custompack0 : TEXCOORD1; // tc_Control tc_Splat0
//  float4 custompack1 : TEXCOORD2; // tc_Splat1 tc_Splat2
//  float2 custompack2 : TEXCOORD3; // tc_Splat3
//  float4 screen : TEXCOORD4;
//  float4 lmap : TEXCOORD5;
//#ifndef LIGHTMAP_ON
//  float3 vlight : TEXCOORD6;
//#else
//#ifdef DIRLIGHTMAP_OFF
//  float4 lmapFadePos : TEXCOORD6;
//#endif
//#endif
//  #if defined(LIGHTMAP_ON) && defined(DIRLIGHTMAP_COMBINED)
//  float3 tSpace0 : TEXCOORD7;
//  float3 tSpace1 : TEXCOORD8;
//  float3 tSpace2 : TEXCOORD9;
//  #endif
//  UNITY_VERTEX_INPUT_INSTANCE_ID
//  UNITY_VERTEX_OUTPUT_STEREO
//};

//// vertex shader
//v2f_surf vert_surf (appdata_full v) {
//  UNITY_SETUP_INSTANCE_ID(v);
//  v2f_surf o;
//  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
//  UNITY_TRANSFER_INSTANCE_ID(v,o);
//  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
//  Input customInputData;
//  vert (v, customInputData);
//  o.custompack0.xy = customInputData.tc_Control;
//  o.custompack0.zw = customInputData.tc_Splat0;
//  o.custompack1.xy = customInputData.tc_Splat1;
//  o.custompack1.zw = customInputData.tc_Splat2;
//  o.custompack2.xy = customInputData.tc_Splat3;
//  o.pos = UnityObjectToClipPos(v.vertex);
//  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
//  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
//  #if defined(LIGHTMAP_ON) && defined(DIRLIGHTMAP_COMBINED)
//  fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
//  fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
//  fixed3 worldBinormal = cross(worldNormal, worldTangent) * tangentSign;
//  #endif
//  #if defined(LIGHTMAP_ON) && defined(DIRLIGHTMAP_COMBINED) && !defined(UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS)
//  o.tSpace0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
//  o.tSpace1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
//  o.tSpace2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);
//  #endif
//  o.worldPos.xyz = worldPos;
//  o.screen = ComputeScreenPos (o.pos);
//#ifdef DYNAMICLIGHTMAP_ON
//  o.lmap.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
//#else
//  o.lmap.zw = 0;
//#endif
//#ifdef LIGHTMAP_ON
//  o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
//  #ifdef DIRLIGHTMAP_OFF
//    o.lmapFadePos.xyz = (mul(unity_ObjectToWorld, v.vertex).xyz - unity_ShadowFadeCenterAndType.xyz) * unity_ShadowFadeCenterAndType.w;
//    o.lmapFadePos.w = (-UnityObjectToViewPos(v.vertex).z) * (1.0 - unity_ShadowFadeCenterAndType.w);
//  #endif
//#else
//  o.lmap.xy = 0;
//  float3 worldN = UnityObjectToWorldNormal(v.normal);
//  o.vlight = ShadeSH9 (float4(worldN,1.0));
//#endif
//  return o;
//}
//sampler2D _LightBuffer;
//sampler2D _CameraNormalsTexture;
//#ifdef LIGHTMAP_ON
//float4 unity_LightmapFade;
//#endif
//fixed4 unity_Ambient;

//// fragment shader
//fixed4 frag_surf (v2f_surf IN) : SV_Target {
//  UNITY_SETUP_INSTANCE_ID(IN);
//  // prepare and unpack data
//  Input surfIN;
//  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
//  surfIN.tc_Control.x = 1.0;
//  surfIN.tc_Splat0.x = 1.0;
//  surfIN.tc_Splat1.x = 1.0;
//  surfIN.tc_Splat2.x = 1.0;
//  surfIN.tc_Splat3.x = 1.0;
//  surfIN.worldPos.x = 1.0;
//  surfIN.tc_Control = IN.custompack0.xy;
//  surfIN.tc_Splat0 = IN.custompack0.zw;
//  surfIN.tc_Splat1 = IN.custompack1.xy;
//  surfIN.tc_Splat2 = IN.custompack1.zw;
//  surfIN.tc_Splat3 = IN.custompack2.xy;
//  float3 worldPos = IN.worldPos.xyz;
//  #ifndef USING_DIRECTIONAL_LIGHT
//    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
//  #else
//    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
//  #endif
//  #ifdef UNITY_COMPILER_HLSL
//  SurfaceOutput o = (SurfaceOutput)0;
//  #else
//  SurfaceOutput o;
//  #endif
//  o.Albedo = 0.0;
//  o.Emission = 0.0;
//  o.Specular = 0.0;
//  o.Alpha = 0.0;
//  o.Gloss = 0.0;
//  fixed3 normalWorldVertex = fixed3(0,0,1);

//  // call surface function
//  surf (surfIN, o);
//  half4 light = tex2Dproj (_LightBuffer, UNITY_PROJ_COORD(IN.screen));
//#if defined (SHADER_API_MOBILE)
//  light = max(light, half4(0.001, 0.001, 0.001, 0.001));
//#endif
//#ifndef UNITY_HDR_ON
//  light = -log2(light);
//#endif
//  #ifdef LIGHTMAP_ON
//    #ifdef DIRLIGHTMAP_OFF
//      // single lightmap
//      fixed4 lmtex = UNITY_SAMPLE_TEX2D(unity_Lightmap, IN.lmap.xy);
//      fixed3 lm = DecodeLightmap (lmtex);
//      light.rgb += lm;
//    #elif DIRLIGHTMAP_COMBINED
//      half4 nspec = tex2Dproj (_CameraNormalsTexture, UNITY_PROJ_COORD(IN.screen));
//      half3 normal = nspec.rgb * 2 - 1;
//      o.Normal = normalize(normal);
//      // directional lightmaps
//      fixed4 lmtex = UNITY_SAMPLE_TEX2D(unity_Lightmap, IN.lmap.xy);
//      half4 lm = half4(DecodeLightmap(lmtex), 0);
//      light += lm;
//    #endif // DIRLIGHTMAP_OFF
//  #else
//    light.rgb += IN.vlight;
//  #endif // LIGHTMAP_ON

//  #ifdef DYNAMICLIGHTMAP_ON
//  fixed4 dynlmtex = UNITY_SAMPLE_TEX2D(unity_DynamicLightmap, IN.lmap.zw);
//  light.rgb += DecodeRealtimeLightmap (dynlmtex);
//  #endif

//  fixed4 c = LightingLambert_PrePass (o, light);
//  FinalColor (surfIN, o, c);
//  UNITY_OPAQUE_ALPHA(c.a);
//  return c;
//}


//#endif


//ENDCG

//}

//	// ---- deferred shading pass:
//	Pass {
//		Name "DEFERRED"
//		Tags { "LightMode" = "Deferred" }

//CGPROGRAM
//// compile directives
//#pragma vertex vert_surf
//#pragma fragment frag_surf
//#pragma target 3.0
//#pragma multi_compile_instancing
//#pragma exclude_renderers nomrt
//#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
//#pragma multi_compile_prepassfinal
//#include "HLSLSupport.cginc"
//#define UNITY_INSTANCED_LOD_FADE
//#define UNITY_INSTANCED_SH
//#define UNITY_INSTANCED_LIGHTMAPSTS
//#include "UnityShaderVariables.cginc"
//#include "UnityShaderUtilities.cginc"
//// -------- variant for: <when no other keywords are defined>
//#if !defined(INSTANCING_ON)
//// Surface shader code generated based on:
//// vertex modifier: 'vert'
//// writes to per-pixel normal: no
//// writes to emission: no
//// writes to occlusion: no
//// needs world space reflection vector: no
//// needs world space normal vector: no
//// needs screen space position: no
//// needs world space position: no
//// needs view direction: no
//// needs world space view direction: no
//// needs world space position for lighting: YES
//// needs world space view direction for lighting: no
//// needs world space view direction for lightmaps: no
//// needs vertex color: no
//// needs VFACE: no
//// needs SV_IsFrontFace: no
//// passes tangent-to-world matrix to pixel shader: no
//// reads from normal: YES
//// 0 texcoords actually used
//#include "UnityCG.cginc"
//#include "Lighting.cginc"

//#define INTERNAL_DATA
//#define WorldReflectionVector(data,normal) data.worldRefl
//#define WorldNormalVector(data,normal) normal

//// Original surface shader snippet:
//#line 14 ""
//#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
//#endif
///* UNITY: Original start of shader */
////#pragma surface surf Lambert vertex:vert finalcolor:FinalColor
//// //#pragma exclude_renderers xbox360 ps3
//// //#pragma multi_compile DOD_FOG_NONE DOD_FOG_LINEAR DOD_FOG_EXP DOD_FOG_EXP2
////#pragma target 3.0

//// #include "CustomFog.cginc"
//struct Input {
//	float2 tc_Control : TEXCOORD0;
//	float2 tc_Splat0 : TEXCOORD1;
//	float2 tc_Splat1 : TEXCOORD2;
//	float2 tc_Splat2 : TEXCOORD3;
//	float2 tc_Splat3 : TEXCOORD4;
//	// CUSTOM_FOG_COORDS(5)
//	float3 worldPos : TEXCOORD6;
//};
 
//sampler2D _Control;
//float4 _Control_ST;
//sampler2D _Splat0,_Splat1,_Splat2,_Splat3;
//float4 _Splat0_ST,_Splat1_ST,_Splat2_ST,_Splat3_ST;

//void vert(inout appdata_full v, out Input data)
//{
//	UNITY_INITIALIZE_OUTPUT(Input, data);
//	data.tc_Control.xy = TRANSFORM_TEX(v.texcoord, _Control);
//	data.tc_Splat0 = TRANSFORM_TEX(v.texcoord, _Splat0);
//	data.tc_Splat1 = TRANSFORM_TEX(v.texcoord, _Splat1);
//	data.tc_Splat2 = TRANSFORM_TEX(v.texcoord, _Splat2);
//	data.tc_Splat3 = TRANSFORM_TEX(v.texcoord, _Splat3);
//	data.worldPos = mul(unity_ObjectToWorld, v.vertex);
//	// CUSTOM_TRANSFER_FOG(data.fogCoord, v.vertex);

//}

//void FinalColor(Input IN, SurfaceOutput o, inout fixed4 color)
//{
//	// CUSTOM_APPLY_FOG(IN.fogCoord, IN.worldPos, color.rgb);
//}
 
//void surf (Input IN, inout SurfaceOutput o) {
//	fixed4 splat_control = tex2D (_Control, IN.tc_Control).rgba;
		
//	fixed3 lay1 = tex2D (_Splat0, IN.tc_Splat0);
//	fixed3 lay2 = tex2D (_Splat1, IN.tc_Splat1);
//	fixed3 lay3 = tex2D (_Splat2, IN.tc_Splat2);
//	fixed3 lay4 = tex2D (_Splat3, IN.tc_Splat3);

//	o.Alpha = 0.0;
//	o.Albedo.rgb = (lay1 * splat_control.r + lay2 * splat_control.g + lay3 * splat_control.b + lay4 * splat_control.a);
//}


//// vertex-to-fragment interpolation data
//struct v2f_surf {
//  UNITY_POSITION(pos);
//  float3 worldNormal : TEXCOORD0;
//  float3 worldPos : TEXCOORD1;
//  float4 custompack0 : TEXCOORD2; // tc_Control tc_Splat0
//  float4 custompack1 : TEXCOORD3; // tc_Splat1 tc_Splat2
//  float2 custompack2 : TEXCOORD4; // tc_Splat3
//  float4 lmap : TEXCOORD5;
//#ifndef LIGHTMAP_ON
//  #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
//    half3 sh : TEXCOORD6; // SH
//  #endif
//#else
//  #ifdef DIRLIGHTMAP_OFF
//    float4 lmapFadePos : TEXCOORD6;
//  #endif
//#endif
//  UNITY_VERTEX_INPUT_INSTANCE_ID
//  UNITY_VERTEX_OUTPUT_STEREO
//};

//// vertex shader
//v2f_surf vert_surf (appdata_full v) {
//  UNITY_SETUP_INSTANCE_ID(v);
//  v2f_surf o;
//  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
//  UNITY_TRANSFER_INSTANCE_ID(v,o);
//  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
//  Input customInputData;
//  vert (v, customInputData);
//  o.custompack0.xy = customInputData.tc_Control;
//  o.custompack0.zw = customInputData.tc_Splat0;
//  o.custompack1.xy = customInputData.tc_Splat1;
//  o.custompack1.zw = customInputData.tc_Splat2;
//  o.custompack2.xy = customInputData.tc_Splat3;
//  o.pos = UnityObjectToClipPos(v.vertex);
//  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
//  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
//  o.worldPos.xyz = worldPos;
//  o.worldNormal = worldNormal;
//#ifdef DYNAMICLIGHTMAP_ON
//  o.lmap.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
//#else
//  o.lmap.zw = 0;
//#endif
//#ifdef LIGHTMAP_ON
//  o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
//  #ifdef DIRLIGHTMAP_OFF
//    o.lmapFadePos.xyz = (mul(unity_ObjectToWorld, v.vertex).xyz - unity_ShadowFadeCenterAndType.xyz) * unity_ShadowFadeCenterAndType.w;
//    o.lmapFadePos.w = (-UnityObjectToViewPos(v.vertex).z) * (1.0 - unity_ShadowFadeCenterAndType.w);
//  #endif
//#else
//  o.lmap.xy = 0;
//    #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
//      o.sh = 0;
//      o.sh = ShadeSHPerVertex (worldNormal, o.sh);
//    #endif
//#endif
//  return o;
//}
//#ifdef LIGHTMAP_ON
//float4 unity_LightmapFade;
//#endif
//fixed4 unity_Ambient;

//// fragment shader
//void frag_surf (v2f_surf IN,
//    out half4 outGBuffer0 : SV_Target0,
//    out half4 outGBuffer1 : SV_Target1,
//    out half4 outGBuffer2 : SV_Target2,
//    out half4 outEmission : SV_Target3
//#if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
//    , out half4 outShadowMask : SV_Target4
//#endif
//) {
//  UNITY_SETUP_INSTANCE_ID(IN);
//  // prepare and unpack data
//  Input surfIN;
//  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
//  surfIN.tc_Control.x = 1.0;
//  surfIN.tc_Splat0.x = 1.0;
//  surfIN.tc_Splat1.x = 1.0;
//  surfIN.tc_Splat2.x = 1.0;
//  surfIN.tc_Splat3.x = 1.0;
//  surfIN.worldPos.x = 1.0;
//  surfIN.tc_Control = IN.custompack0.xy;
//  surfIN.tc_Splat0 = IN.custompack0.zw;
//  surfIN.tc_Splat1 = IN.custompack1.xy;
//  surfIN.tc_Splat2 = IN.custompack1.zw;
//  surfIN.tc_Splat3 = IN.custompack2.xy;
//  float3 worldPos = IN.worldPos.xyz;
//  #ifndef USING_DIRECTIONAL_LIGHT
//    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
//  #else
//    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
//  #endif
//  #ifdef UNITY_COMPILER_HLSL
//  SurfaceOutput o = (SurfaceOutput)0;
//  #else
//  SurfaceOutput o;
//  #endif
//  o.Albedo = 0.0;
//  o.Emission = 0.0;
//  o.Specular = 0.0;
//  o.Alpha = 0.0;
//  o.Gloss = 0.0;
//  fixed3 normalWorldVertex = fixed3(0,0,1);
//  o.Normal = IN.worldNormal;
//  normalWorldVertex = IN.worldNormal;

//  // call surface function
//  surf (surfIN, o);
//fixed3 originalNormal = o.Normal;
//  half atten = 1;

//  // Setup lighting environment
//  UnityGI gi;
//  UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
//  gi.indirect.diffuse = 0;
//  gi.indirect.specular = 0;
//  gi.light.color = 0;
//  gi.light.dir = half3(0,1,0);
//  // Call GI (lightmaps/SH/reflections) lighting function
//  UnityGIInput giInput;
//  UNITY_INITIALIZE_OUTPUT(UnityGIInput, giInput);
//  giInput.light = gi.light;
//  giInput.worldPos = worldPos;
//  giInput.atten = atten;
//  #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
//    giInput.lightmapUV = IN.lmap;
//  #else
//    giInput.lightmapUV = 0.0;
//  #endif
//  #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
//    giInput.ambient = IN.sh;
//  #else
//    giInput.ambient.rgb = 0.0;
//  #endif
//  giInput.probeHDR[0] = unity_SpecCube0_HDR;
//  giInput.probeHDR[1] = unity_SpecCube1_HDR;
//  #if defined(UNITY_SPECCUBE_BLENDING) || defined(UNITY_SPECCUBE_BOX_PROJECTION)
//    giInput.boxMin[0] = unity_SpecCube0_BoxMin; // .w holds lerp value for blending
//  #endif
//  #ifdef UNITY_SPECCUBE_BOX_PROJECTION
//    giInput.boxMax[0] = unity_SpecCube0_BoxMax;
//    giInput.probePosition[0] = unity_SpecCube0_ProbePosition;
//    giInput.boxMax[1] = unity_SpecCube1_BoxMax;
//    giInput.boxMin[1] = unity_SpecCube1_BoxMin;
//    giInput.probePosition[1] = unity_SpecCube1_ProbePosition;
//  #endif
//  LightingLambert_GI(o, giInput, gi);

//  // call lighting function to output g-buffer
//  outEmission = LightingLambert_Deferred (o, gi, outGBuffer0, outGBuffer1, outGBuffer2);
//  #if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
//    outShadowMask = UnityGetRawBakedOcclusions (IN.lmap.xy, worldPos);
//  #endif
//  #ifndef UNITY_HDR_ON
//  outEmission.rgb = exp2(-outEmission.rgb);
//  #endif
//}


//#endif

//// -------- variant for: INSTANCING_ON 
//#if defined(INSTANCING_ON)
//// Surface shader code generated based on:
//// vertex modifier: 'vert'
//// writes to per-pixel normal: no
//// writes to emission: no
//// writes to occlusion: no
//// needs world space reflection vector: no
//// needs world space normal vector: no
//// needs screen space position: no
//// needs world space position: no
//// needs view direction: no
//// needs world space view direction: no
//// needs world space position for lighting: YES
//// needs world space view direction for lighting: no
//// needs world space view direction for lightmaps: no
//// needs vertex color: no
//// needs VFACE: no
//// needs SV_IsFrontFace: no
//// passes tangent-to-world matrix to pixel shader: no
//// reads from normal: YES
//// 0 texcoords actually used
//#include "UnityCG.cginc"
//#include "Lighting.cginc"

//#define INTERNAL_DATA
//#define WorldReflectionVector(data,normal) data.worldRefl
//#define WorldNormalVector(data,normal) normal

//// Original surface shader snippet:
//#line 14 ""
//#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
//#endif
///* UNITY: Original start of shader */
////#pragma surface surf Lambert vertex:vert finalcolor:FinalColor
//// //#pragma exclude_renderers xbox360 ps3
//// //#pragma multi_compile DOD_FOG_NONE DOD_FOG_LINEAR DOD_FOG_EXP DOD_FOG_EXP2
////#pragma target 3.0

//// #include "CustomFog.cginc"
//struct Input {
//	float2 tc_Control : TEXCOORD0;
//	float2 tc_Splat0 : TEXCOORD1;
//	float2 tc_Splat1 : TEXCOORD2;
//	float2 tc_Splat2 : TEXCOORD3;
//	float2 tc_Splat3 : TEXCOORD4;
//	// CUSTOM_FOG_COORDS(5)
//	float3 worldPos : TEXCOORD6;
//};
 
//sampler2D _Control;
//float4 _Control_ST;
//sampler2D _Splat0,_Splat1,_Splat2,_Splat3;
//float4 _Splat0_ST,_Splat1_ST,_Splat2_ST,_Splat3_ST;

//void vert(inout appdata_full v, out Input data)
//{
//	UNITY_INITIALIZE_OUTPUT(Input, data);
//	data.tc_Control.xy = TRANSFORM_TEX(v.texcoord, _Control);
//	data.tc_Splat0 = TRANSFORM_TEX(v.texcoord, _Splat0);
//	data.tc_Splat1 = TRANSFORM_TEX(v.texcoord, _Splat1);
//	data.tc_Splat2 = TRANSFORM_TEX(v.texcoord, _Splat2);
//	data.tc_Splat3 = TRANSFORM_TEX(v.texcoord, _Splat3);
//	data.worldPos = mul(unity_ObjectToWorld, v.vertex);
//	// CUSTOM_TRANSFER_FOG(data.fogCoord, v.vertex);

//}

//void FinalColor(Input IN, SurfaceOutput o, inout fixed4 color)
//{
//	// CUSTOM_APPLY_FOG(IN.fogCoord, IN.worldPos, color.rgb);
//}
 
//void surf (Input IN, inout SurfaceOutput o) {
//	fixed4 splat_control = tex2D (_Control, IN.tc_Control).rgba;
		
//	fixed3 lay1 = tex2D (_Splat0, IN.tc_Splat0);
//	fixed3 lay2 = tex2D (_Splat1, IN.tc_Splat1);
//	fixed3 lay3 = tex2D (_Splat2, IN.tc_Splat2);
//	fixed3 lay4 = tex2D (_Splat3, IN.tc_Splat3);

//	o.Alpha = 0.0;
//	o.Albedo.rgb = (lay1 * splat_control.r + lay2 * splat_control.g + lay3 * splat_control.b + lay4 * splat_control.a);
//}


//// vertex-to-fragment interpolation data
//struct v2f_surf {
//  UNITY_POSITION(pos);
//  float3 worldNormal : TEXCOORD0;
//  float3 worldPos : TEXCOORD1;
//  float4 custompack0 : TEXCOORD2; // tc_Control tc_Splat0
//  float4 custompack1 : TEXCOORD3; // tc_Splat1 tc_Splat2
//  float2 custompack2 : TEXCOORD4; // tc_Splat3
//  float4 lmap : TEXCOORD5;
//#ifndef LIGHTMAP_ON
//  #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
//    half3 sh : TEXCOORD6; // SH
//  #endif
//#else
//  #ifdef DIRLIGHTMAP_OFF
//    float4 lmapFadePos : TEXCOORD6;
//  #endif
//#endif
//  UNITY_VERTEX_INPUT_INSTANCE_ID
//  UNITY_VERTEX_OUTPUT_STEREO
//};

//// vertex shader
//v2f_surf vert_surf (appdata_full v) {
//  UNITY_SETUP_INSTANCE_ID(v);
//  v2f_surf o;
//  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
//  UNITY_TRANSFER_INSTANCE_ID(v,o);
//  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
//  Input customInputData;
//  vert (v, customInputData);
//  o.custompack0.xy = customInputData.tc_Control;
//  o.custompack0.zw = customInputData.tc_Splat0;
//  o.custompack1.xy = customInputData.tc_Splat1;
//  o.custompack1.zw = customInputData.tc_Splat2;
//  o.custompack2.xy = customInputData.tc_Splat3;
//  o.pos = UnityObjectToClipPos(v.vertex);
//  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
//  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
//  o.worldPos.xyz = worldPos;
//  o.worldNormal = worldNormal;
//#ifdef DYNAMICLIGHTMAP_ON
//  o.lmap.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
//#else
//  o.lmap.zw = 0;
//#endif
//#ifdef LIGHTMAP_ON
//  o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
//  #ifdef DIRLIGHTMAP_OFF
//    o.lmapFadePos.xyz = (mul(unity_ObjectToWorld, v.vertex).xyz - unity_ShadowFadeCenterAndType.xyz) * unity_ShadowFadeCenterAndType.w;
//    o.lmapFadePos.w = (-UnityObjectToViewPos(v.vertex).z) * (1.0 - unity_ShadowFadeCenterAndType.w);
//  #endif
//#else
//  o.lmap.xy = 0;
//    #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
//      o.sh = 0;
//      o.sh = ShadeSHPerVertex (worldNormal, o.sh);
//    #endif
//#endif
//  return o;
//}
//#ifdef LIGHTMAP_ON
//float4 unity_LightmapFade;
//#endif
//fixed4 unity_Ambient;

//// fragment shader
//void frag_surf (v2f_surf IN,
//    out half4 outGBuffer0 : SV_Target0,
//    out half4 outGBuffer1 : SV_Target1,
//    out half4 outGBuffer2 : SV_Target2,
//    out half4 outEmission : SV_Target3
//#if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
//    , out half4 outShadowMask : SV_Target4
//#endif
//) {
//  UNITY_SETUP_INSTANCE_ID(IN);
//  // prepare and unpack data
//  Input surfIN;
//  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
//  surfIN.tc_Control.x = 1.0;
//  surfIN.tc_Splat0.x = 1.0;
//  surfIN.tc_Splat1.x = 1.0;
//  surfIN.tc_Splat2.x = 1.0;
//  surfIN.tc_Splat3.x = 1.0;
//  surfIN.worldPos.x = 1.0;
//  surfIN.tc_Control = IN.custompack0.xy;
//  surfIN.tc_Splat0 = IN.custompack0.zw;
//  surfIN.tc_Splat1 = IN.custompack1.xy;
//  surfIN.tc_Splat2 = IN.custompack1.zw;
//  surfIN.tc_Splat3 = IN.custompack2.xy;
//  float3 worldPos = IN.worldPos.xyz;
//  #ifndef USING_DIRECTIONAL_LIGHT
//    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
//  #else
//    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
//  #endif
//  #ifdef UNITY_COMPILER_HLSL
//  SurfaceOutput o = (SurfaceOutput)0;
//  #else
//  SurfaceOutput o;
//  #endif
//  o.Albedo = 0.0;
//  o.Emission = 0.0;
//  o.Specular = 0.0;
//  o.Alpha = 0.0;
//  o.Gloss = 0.0;
//  fixed3 normalWorldVertex = fixed3(0,0,1);
//  o.Normal = IN.worldNormal;
//  normalWorldVertex = IN.worldNormal;

//  // call surface function
//  surf (surfIN, o);
//fixed3 originalNormal = o.Normal;
//  half atten = 1;

//  // Setup lighting environment
//  UnityGI gi;
//  UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
//  gi.indirect.diffuse = 0;
//  gi.indirect.specular = 0;
//  gi.light.color = 0;
//  gi.light.dir = half3(0,1,0);
//  // Call GI (lightmaps/SH/reflections) lighting function
//  UnityGIInput giInput;
//  UNITY_INITIALIZE_OUTPUT(UnityGIInput, giInput);
//  giInput.light = gi.light;
//  giInput.worldPos = worldPos;
//  giInput.atten = atten;
//  #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
//    giInput.lightmapUV = IN.lmap;
//  #else
//    giInput.lightmapUV = 0.0;
//  #endif
//  #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
//    giInput.ambient = IN.sh;
//  #else
//    giInput.ambient.rgb = 0.0;
//  #endif
//  giInput.probeHDR[0] = unity_SpecCube0_HDR;
//  giInput.probeHDR[1] = unity_SpecCube1_HDR;
//  #if defined(UNITY_SPECCUBE_BLENDING) || defined(UNITY_SPECCUBE_BOX_PROJECTION)
//    giInput.boxMin[0] = unity_SpecCube0_BoxMin; // .w holds lerp value for blending
//  #endif
//  #ifdef UNITY_SPECCUBE_BOX_PROJECTION
//    giInput.boxMax[0] = unity_SpecCube0_BoxMax;
//    giInput.probePosition[0] = unity_SpecCube0_ProbePosition;
//    giInput.boxMax[1] = unity_SpecCube1_BoxMax;
//    giInput.boxMin[1] = unity_SpecCube1_BoxMin;
//    giInput.probePosition[1] = unity_SpecCube1_ProbePosition;
//  #endif
//  LightingLambert_GI(o, giInput, gi);

//  // call lighting function to output g-buffer
//  outEmission = LightingLambert_Deferred (o, gi, outGBuffer0, outGBuffer1, outGBuffer2);
//  #if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
//    outShadowMask = UnityGetRawBakedOcclusions (IN.lmap.xy, worldPos);
//  #endif
//  #ifndef UNITY_HDR_ON
//  outEmission.rgb = exp2(-outEmission.rgb);
//  #endif
//}


//#endif


//ENDCG

//}

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
// vertex modifier: 'vert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 14 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
//#pragma surface surf Lambert vertex:vert finalcolor:FinalColor
// //#pragma exclude_renderers xbox360 ps3
// //#pragma multi_compile DOD_FOG_NONE DOD_FOG_LINEAR DOD_FOG_EXP DOD_FOG_EXP2
//#pragma target 3.0

// #include "CustomFog.cginc"
struct Input {
	float2 tc_Control : TEXCOORD0;
	float2 tc_Splat0 : TEXCOORD1;
	float2 tc_Splat1 : TEXCOORD2;
	float2 tc_Splat2 : TEXCOORD3;
	float2 tc_Splat3 : TEXCOORD4;
	// CUSTOM_FOG_COORDS(5)
	float3 worldPos : TEXCOORD6;
};
 
sampler2D _Control;
float4 _Control_ST;
sampler2D _Splat0,_Splat1,_Splat2,_Splat3;
float4 _Splat0_ST,_Splat1_ST,_Splat2_ST,_Splat3_ST;

void vert(inout appdata_full v, out Input data)
{
	UNITY_INITIALIZE_OUTPUT(Input, data);
	data.tc_Control.xy = TRANSFORM_TEX(v.texcoord, _Control);
	data.tc_Splat0 = TRANSFORM_TEX(v.texcoord, _Splat0);
	data.tc_Splat1 = TRANSFORM_TEX(v.texcoord, _Splat1);
	data.tc_Splat2 = TRANSFORM_TEX(v.texcoord, _Splat2);
	data.tc_Splat3 = TRANSFORM_TEX(v.texcoord, _Splat3);
	data.worldPos = mul(unity_ObjectToWorld, v.vertex);
	// CUSTOM_TRANSFER_FOG(data.fogCoord, v.vertex);

}

void FinalColor(Input IN, SurfaceOutput o, inout fixed4 color)
{
	// CUSTOM_APPLY_FOG(IN.fogCoord, IN.worldPos, color.rgb);
}
 
void surf (Input IN, inout SurfaceOutput o) {
	fixed4 splat_control = tex2D (_Control, IN.tc_Control).rgba;
		
	fixed3 lay1 = tex2D (_Splat0, IN.tc_Splat0);
	fixed3 lay2 = tex2D (_Splat1, IN.tc_Splat1);
	fixed3 lay3 = tex2D (_Splat2, IN.tc_Splat2);
	fixed3 lay4 = tex2D (_Splat3, IN.tc_Splat3);

	o.Alpha = 0.0;
	o.Albedo.rgb = (lay1 * splat_control.r + lay2 * splat_control.g + lay3 * splat_control.b + lay4 * splat_control.a);
}

#include "UnityMetaPass.cginc"

// vertex-to-fragment interpolation data
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldPos : TEXCOORD0;
  float4 custompack0 : TEXCOORD1; // tc_Control tc_Splat0
  float4 custompack1 : TEXCOORD2; // tc_Splat1 tc_Splat2
  float2 custompack2 : TEXCOORD3; // tc_Splat3
#ifdef EDITOR_VISUALIZATION
  float2 vizUV : TEXCOORD4;
  float4 lightCoord : TEXCOORD5;
#endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  vert (v, customInputData);
  o.custompack0.xy = customInputData.tc_Control;
  o.custompack0.zw = customInputData.tc_Splat0;
  o.custompack1.xy = customInputData.tc_Splat1;
  o.custompack1.zw = customInputData.tc_Splat2;
  o.custompack2.xy = customInputData.tc_Splat3;
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
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc_Control.x = 1.0;
  surfIN.tc_Splat0.x = 1.0;
  surfIN.tc_Splat1.x = 1.0;
  surfIN.tc_Splat2.x = 1.0;
  surfIN.tc_Splat3.x = 1.0;
  surfIN.worldPos.x = 1.0;
  surfIN.tc_Control = IN.custompack0.xy;
  surfIN.tc_Splat0 = IN.custompack0.zw;
  surfIN.tc_Splat1 = IN.custompack1.xy;
  surfIN.tc_Splat2 = IN.custompack1.zw;
  surfIN.tc_Splat3 = IN.custompack2.xy;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);

  // call surface function
  surf (surfIN, o);
  UnityMetaInput metaIN;
  UNITY_INITIALIZE_OUTPUT(UnityMetaInput, metaIN);
  metaIN.Albedo = o.Albedo;
  metaIN.Emission = o.Emission;
  metaIN.SpecularColor = o.Specular;
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
// vertex modifier: 'vert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 14 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
//#pragma surface surf Lambert vertex:vert finalcolor:FinalColor
// //#pragma exclude_renderers xbox360 ps3
// //#pragma multi_compile DOD_FOG_NONE DOD_FOG_LINEAR DOD_FOG_EXP DOD_FOG_EXP2
//#pragma target 3.0

// #include "CustomFog.cginc"
struct Input {
	float2 tc_Control : TEXCOORD0;
	float2 tc_Splat0 : TEXCOORD1;
	float2 tc_Splat1 : TEXCOORD2;
	float2 tc_Splat2 : TEXCOORD3;
	float2 tc_Splat3 : TEXCOORD4;
	// CUSTOM_FOG_COORDS(5)
	float3 worldPos : TEXCOORD6;
};
 
sampler2D _Control;
float4 _Control_ST;
sampler2D _Splat0,_Splat1,_Splat2,_Splat3;
float4 _Splat0_ST,_Splat1_ST,_Splat2_ST,_Splat3_ST;

void vert(inout appdata_full v, out Input data)
{
	UNITY_INITIALIZE_OUTPUT(Input, data);
	data.tc_Control.xy = TRANSFORM_TEX(v.texcoord, _Control);
	data.tc_Splat0 = TRANSFORM_TEX(v.texcoord, _Splat0);
	data.tc_Splat1 = TRANSFORM_TEX(v.texcoord, _Splat1);
	data.tc_Splat2 = TRANSFORM_TEX(v.texcoord, _Splat2);
	data.tc_Splat3 = TRANSFORM_TEX(v.texcoord, _Splat3);
	data.worldPos = mul(unity_ObjectToWorld, v.vertex);
	// CUSTOM_TRANSFER_FOG(data.fogCoord, v.vertex);

}

void FinalColor(Input IN, SurfaceOutput o, inout fixed4 color)
{
	// CUSTOM_APPLY_FOG(IN.fogCoord, IN.worldPos, color.rgb);
}
 
void surf (Input IN, inout SurfaceOutput o) {
	fixed4 splat_control = tex2D (_Control, IN.tc_Control).rgba;
		
	fixed3 lay1 = tex2D (_Splat0, IN.tc_Splat0);
	fixed3 lay2 = tex2D (_Splat1, IN.tc_Splat1);
	fixed3 lay3 = tex2D (_Splat2, IN.tc_Splat2);
	fixed3 lay4 = tex2D (_Splat3, IN.tc_Splat3);

	o.Alpha = 0.0;
	o.Albedo.rgb = (lay1 * splat_control.r + lay2 * splat_control.g + lay3 * splat_control.b + lay4 * splat_control.a);
}

#include "UnityMetaPass.cginc"

// vertex-to-fragment interpolation data
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldPos : TEXCOORD0;
  float4 custompack0 : TEXCOORD1; // tc_Control tc_Splat0
  float4 custompack1 : TEXCOORD2; // tc_Splat1 tc_Splat2
  float2 custompack2 : TEXCOORD3; // tc_Splat3
#ifdef EDITOR_VISUALIZATION
  float2 vizUV : TEXCOORD4;
  float4 lightCoord : TEXCOORD5;
#endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  vert (v, customInputData);
  o.custompack0.xy = customInputData.tc_Control;
  o.custompack0.zw = customInputData.tc_Splat0;
  o.custompack1.xy = customInputData.tc_Splat1;
  o.custompack1.zw = customInputData.tc_Splat2;
  o.custompack2.xy = customInputData.tc_Splat3;
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
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc_Control.x = 1.0;
  surfIN.tc_Splat0.x = 1.0;
  surfIN.tc_Splat1.x = 1.0;
  surfIN.tc_Splat2.x = 1.0;
  surfIN.tc_Splat3.x = 1.0;
  surfIN.worldPos.x = 1.0;
  surfIN.tc_Control = IN.custompack0.xy;
  surfIN.tc_Splat0 = IN.custompack0.zw;
  surfIN.tc_Splat1 = IN.custompack1.xy;
  surfIN.tc_Splat2 = IN.custompack1.zw;
  surfIN.tc_Splat3 = IN.custompack2.xy;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);

  // call surface function
  surf (surfIN, o);
  UnityMetaInput metaIN;
  UNITY_INITIALIZE_OUTPUT(UnityMetaInput, metaIN);
  metaIN.Albedo = o.Albedo;
  metaIN.Emission = o.Emission;
  metaIN.SpecularColor = o.Specular;
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

//#LINE 67
 
}
// Fallback to Diffuse
Fallback "Diffuse"
}
