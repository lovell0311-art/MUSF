// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "G/FX_raodong"
{
	Properties
	{
		[HDR]_Color0("Color 0", Color) = (1,1,1,1)
		_main_tex("main_tex", 2D) = "white" {}
		_u1("u1", Float) = 1.33
		_v1("v1", Float) = 0.57
		_noise_tex("noise_tex", 2D) = "white" {}
		_mask_tex("mask_tex", 2D) = "white" {}
		_u("u", Float) = 1
		_v("v", Float) = 0
		_raodong_qiangdu("raodong_qiangdu", Float) = 0.1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Custom"  "Queue" = "Transparent+0" "IsEmissive" = "true"  }
		Cull Off
		ZWrite Off
		Blend One One
		
		
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
#pragma multi_compile_fwdbase novertexlight noshadowmask nodynlightmap nodirlightmap nolightmap noshadow
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
// needs world space position for lighting: no
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: YES
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 1 texcoords actually used
//   float2 _texcoord
#include "UnityCG.cginc"
//Shader does not support lightmap thus we always want to fallback to SH.
#undef UNITY_SHOULD_SAMPLE_SH
#define UNITY_SHOULD_SAMPLE_SH (!defined(UNITY_PASS_FORWARDADD) && !defined(UNITY_PASS_PREPASSBASE) && !defined(UNITY_PASS_SHADOWCASTER) && !defined(UNITY_PASS_META))
#include "Lighting.cginc"
#include "AutoLight.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 25 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
		#include "UnityShaderVariables.cginc"
		//#pragma target 3.0
		//#pragma surface surf Unlit keepalpha noshadow noambient novertexlights nolightmap  nodynlightmap nodirlightmap nofog nometa noforwardadd 
		struct Input
		{
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
		};

		uniform sampler2D _main_tex;
		uniform float _u1;
		uniform float _v1;
		uniform float4 _main_tex_ST;
		uniform sampler2D _noise_tex;
		uniform float _u;
		uniform float _v;
		uniform float4 _noise_tex_ST;
		uniform float _raodong_qiangdu;
		uniform sampler2D _mask_tex;
		uniform float4 _mask_tex_ST;
		uniform float4 _Color0;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 appendResult25 = (float2(_u1 , _v1));
			float2 panner21 = ( 1.0 * _Time.y * appendResult25 + i.uv_texcoord);
			float2 uv_main_tex = i.uv_texcoord * _main_tex_ST.xy + _main_tex_ST.zw;
			float2 appendResult9 = (float2(_u , _v));
			float2 uv_noise_tex = i.uv_texcoord * _noise_tex_ST.xy + _noise_tex_ST.zw;
			float2 panner5 = ( 1.0 * _Time.y * appendResult9 + uv_noise_tex);
			float4 tex2DNode10 = tex2D( _main_tex, ( panner21 + ( uv_main_tex + ( tex2D( _noise_tex, panner5 ).r * _raodong_qiangdu ) ) ) );
			float2 uv_mask_tex = i.uv_texcoord * _mask_tex_ST.xy + _mask_tex_ST.zw;
			o.Emission = ( ( tex2DNode10 * tex2D( _mask_tex, uv_mask_tex ) * tex2DNode10.a ) * _Color0 * i.vertexColor * i.vertexColor.a ).rgb;
			o.Alpha = 1;
		}

		

// vertex-to-fragment interpolation data
// no lightmaps:
#ifndef LIGHTMAP_ON
// half-precision fragment shader registers:
#ifdef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
struct v2f_surf {
  UNITY_POSITION(pos);
  float2 pack0 : TEXCOORD0; // _texcoord
  float3 worldNormal : TEXCOORD1;
  float3 worldPos : TEXCOORD2;
  fixed4 color : COLOR0;
  DECLARE_LIGHT_COORDS(3)
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
// high-precision fragment shader registers:
#ifndef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
struct v2f_surf {
  UNITY_POSITION(pos);
  float2 pack0 : TEXCOORD0; // _texcoord
  float3 worldNormal : TEXCOORD1;
  float3 worldPos : TEXCOORD2;
  fixed4 color : COLOR0;
  DECLARE_LIGHT_COORDS(3)
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
  float2 pack0 : TEXCOORD0; // _texcoord
  float3 worldNormal : TEXCOORD1;
  float3 worldPos : TEXCOORD2;
  fixed4 color : COLOR0;
  float4 lmap : TEXCOORD3;
  DECLARE_LIGHT_COORDS(4)
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
// high-precision fragment shader registers:
#ifndef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
struct v2f_surf {
  UNITY_POSITION(pos);
  float2 pack0 : TEXCOORD0; // _texcoord
  float3 worldNormal : TEXCOORD1;
  float3 worldPos : TEXCOORD2;
  fixed4 color : COLOR0;
  float4 lmap : TEXCOORD3;
  DECLARE_LIGHT_COORDS(4)
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
#endif
float4 _texcoord_ST;

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  o.pos = UnityObjectToClipPos(v.vertex);
  o.pack0.xy = TRANSFORM_TEX(v.texcoord, _texcoord);
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
  o.color = v.color;
  #ifdef LIGHTMAP_ON
  o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
  #endif

  // SH/ambient and vertex lights
  #ifndef LIGHTMAP_ON
  #endif // !LIGHTMAP_ON

  COMPUTE_LIGHT_COORDS(o); // pass light cookie coordinates to pixel shader
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  // prepare and unpack data
  Input surfIN;
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.uv_texcoord.x = 1.0;
  surfIN.vertexColor.x = 1.0;
  surfIN.uv_texcoord = IN.pack0.xy;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  surfIN.vertexColor = IN.color;
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

  // lightmaps
  #ifdef LIGHTMAP_ON
    #if DIRLIGHTMAP_COMBINED
      // directional lightmaps
      fixed4 lmtex = UNITY_SAMPLE_TEX2D(unity_Lightmap, IN.lmap.xy);
      half3 lm = DecodeLightmap(lmtex);
    #else
      // single lightmap
      fixed4 lmtex = UNITY_SAMPLE_TEX2D(unity_Lightmap, IN.lmap.xy);
      fixed3 lm = DecodeLightmap (lmtex);
    #endif

  #endif // LIGHTMAP_ON


  // realtime lighting: call lighting function
  #ifndef LIGHTMAP_ON
  c += LightingUnlit (o, lightDir, atten);
  #else
    c.a = o.Alpha;
  #endif

  #ifdef LIGHTMAP_ON
  #endif // LIGHTMAP_ON

  c.rgb += o.Emission;
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
// needs world space position for lighting: no
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: YES
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 1 texcoords actually used
//   float2 _texcoord
#include "UnityCG.cginc"
//Shader does not support lightmap thus we always want to fallback to SH.
#undef UNITY_SHOULD_SAMPLE_SH
#define UNITY_SHOULD_SAMPLE_SH (!defined(UNITY_PASS_FORWARDADD) && !defined(UNITY_PASS_PREPASSBASE) && !defined(UNITY_PASS_SHADOWCASTER) && !defined(UNITY_PASS_META))
#include "Lighting.cginc"
#include "AutoLight.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 25 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
		#include "UnityShaderVariables.cginc"
		//#pragma target 3.0
		//#pragma surface surf Unlit keepalpha noshadow noambient novertexlights nolightmap  nodynlightmap nodirlightmap nofog nometa noforwardadd 
		struct Input
		{
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
		};

		uniform sampler2D _main_tex;
		uniform float _u1;
		uniform float _v1;
		uniform float4 _main_tex_ST;
		uniform sampler2D _noise_tex;
		uniform float _u;
		uniform float _v;
		uniform float4 _noise_tex_ST;
		uniform float _raodong_qiangdu;
		uniform sampler2D _mask_tex;
		uniform float4 _mask_tex_ST;
		uniform float4 _Color0;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 appendResult25 = (float2(_u1 , _v1));
			float2 panner21 = ( 1.0 * _Time.y * appendResult25 + i.uv_texcoord);
			float2 uv_main_tex = i.uv_texcoord * _main_tex_ST.xy + _main_tex_ST.zw;
			float2 appendResult9 = (float2(_u , _v));
			float2 uv_noise_tex = i.uv_texcoord * _noise_tex_ST.xy + _noise_tex_ST.zw;
			float2 panner5 = ( 1.0 * _Time.y * appendResult9 + uv_noise_tex);
			float4 tex2DNode10 = tex2D( _main_tex, ( panner21 + ( uv_main_tex + ( tex2D( _noise_tex, panner5 ).r * _raodong_qiangdu ) ) ) );
			float2 uv_mask_tex = i.uv_texcoord * _mask_tex_ST.xy + _mask_tex_ST.zw;
			o.Emission = ( ( tex2DNode10 * tex2D( _mask_tex, uv_mask_tex ) * tex2DNode10.a ) * _Color0 * i.vertexColor * i.vertexColor.a ).rgb;
			o.Alpha = 1;
		}

		

// vertex-to-fragment interpolation data
// no lightmaps:
#ifndef LIGHTMAP_ON
// half-precision fragment shader registers:
#ifdef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
struct v2f_surf {
  UNITY_POSITION(pos);
  float2 pack0 : TEXCOORD0; // _texcoord
  float3 worldNormal : TEXCOORD1;
  float3 worldPos : TEXCOORD2;
  fixed4 color : COLOR0;
  DECLARE_LIGHT_COORDS(3)
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
// high-precision fragment shader registers:
#ifndef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
struct v2f_surf {
  UNITY_POSITION(pos);
  float2 pack0 : TEXCOORD0; // _texcoord
  float3 worldNormal : TEXCOORD1;
  float3 worldPos : TEXCOORD2;
  fixed4 color : COLOR0;
  DECLARE_LIGHT_COORDS(3)
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
  float2 pack0 : TEXCOORD0; // _texcoord
  float3 worldNormal : TEXCOORD1;
  float3 worldPos : TEXCOORD2;
  fixed4 color : COLOR0;
  float4 lmap : TEXCOORD3;
  DECLARE_LIGHT_COORDS(4)
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
// high-precision fragment shader registers:
#ifndef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
struct v2f_surf {
  UNITY_POSITION(pos);
  float2 pack0 : TEXCOORD0; // _texcoord
  float3 worldNormal : TEXCOORD1;
  float3 worldPos : TEXCOORD2;
  fixed4 color : COLOR0;
  float4 lmap : TEXCOORD3;
  DECLARE_LIGHT_COORDS(4)
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
#endif
float4 _texcoord_ST;

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  o.pos = UnityObjectToClipPos(v.vertex);
  o.pack0.xy = TRANSFORM_TEX(v.texcoord, _texcoord);
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
  o.color = v.color;
  #ifdef LIGHTMAP_ON
  o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
  #endif

  // SH/ambient and vertex lights
  #ifndef LIGHTMAP_ON
  #endif // !LIGHTMAP_ON

  COMPUTE_LIGHT_COORDS(o); // pass light cookie coordinates to pixel shader
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  // prepare and unpack data
  Input surfIN;
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.uv_texcoord.x = 1.0;
  surfIN.vertexColor.x = 1.0;
  surfIN.uv_texcoord = IN.pack0.xy;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  surfIN.vertexColor = IN.color;
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

  // lightmaps
  #ifdef LIGHTMAP_ON
    #if DIRLIGHTMAP_COMBINED
      // directional lightmaps
      fixed4 lmtex = UNITY_SAMPLE_TEX2D(unity_Lightmap, IN.lmap.xy);
      half3 lm = DecodeLightmap(lmtex);
    #else
      // single lightmap
      fixed4 lmtex = UNITY_SAMPLE_TEX2D(unity_Lightmap, IN.lmap.xy);
      fixed3 lm = DecodeLightmap (lmtex);
    #endif

  #endif // LIGHTMAP_ON


  // realtime lighting: call lighting function
  #ifndef LIGHTMAP_ON
  c += LightingUnlit (o, lightDir, atten);
  #else
    c.a = o.Alpha;
  #endif

  #ifdef LIGHTMAP_ON
  #endif // LIGHTMAP_ON

  c.rgb += o.Emission;
  return c;
}


#endif


ENDCG

}

	// ---- end of surface shader generated code

#LINE 69

	}
	CustomEditor "ASEMaterialInspector"
}
