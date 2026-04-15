// Made with Amplify Shader Editor v1.9.1
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "vfx"
{
	Properties
	{
		[Enum(Add,1,Alpha,10)]_Blend("Blend", Float) = 10
		_MainTex("MainTex", 2D) = "white" {}
		_MainPower("MainPower", Float) = 1
		[HDR]_MainCol("MainCol", Color) = (1,1,1,1)
		_MainSpeed_U("MainSpeed_U", Float) = 0
		_MainSpeed_V("MainSpeed_V", Float) = 0
		_MaskTex("MaskTex", 2D) = "white" {}
		_MaskPower("MaskPower", Float) = 1
		_MaskSpeed_U("MaskSpeed_U", Float) = 0
		_MaskSpeed_V("MaskSpeed_V", Float) = 0
		[Toggle(_DISTORT_MASK_ON)] _Distort_Mask("Distort_Mask", Float) = 0
		_Distort_Mask_Str("Distort_Mask_Str", Range( 0 , 1)) = 0
		_DistortTex("DistortTex", 2D) = "white" {}
		_DistortSpeed_U("DistortSpeed_U", Float) = 0
		_DistortSpeed_V("DistortSpeed_V", Float) = 0
		_DistortStr("DistortStr", Range( 0 , 1)) = 0
		_VertexTex("VertexTex", 2D) = "white" {}
		_VertexSpeed_U("VertexSpeed_U", Float) = 0
		_VertexSpeed_V("VertexSpeed_V", Float) = 0
		_VertexAnimStr("VertexAnimStr", Range( 0 , 5)) = 0
		_Fresnel_Scale("Fresnel_Scale", Float) = 1
		_Fresnel_Power("Fresnel_Power", Float) = 1
		_Fresnel_Bias("Fresnel_Bias", Range( 0 , 1)) = 0

	}
	
	SubShader
	{
		
		
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
	LOD 100

		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend SrcAlpha [_Blend]
		AlphaToMask Off
		Cull Off
		ColorMask RGBA
		ZWrite Off
		ZTest LEqual
		
		
		
		Pass
		{
			Name "Unlit"

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
			#include "UnityStandardBRDF.cginc"
			#define ASE_NEEDS_FRAG_COLOR
			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#pragma shader_feature_local _DISTORT_MASK_ON


			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 worldPos : TEXCOORD0;
				#endif
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_color : COLOR;
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			uniform float _Blend;
			uniform sampler2D _VertexTex;
			uniform float _VertexSpeed_U;
			uniform float _VertexSpeed_V;
			uniform float4 _VertexTex_ST;
			uniform float _VertexAnimStr;
			uniform sampler2D _MainTex;
			uniform sampler2D _DistortTex;
			uniform float _DistortSpeed_U;
			uniform float _DistortSpeed_V;
			uniform float4 _DistortTex_ST;
			uniform float _DistortStr;
			uniform float _MainSpeed_U;
			uniform float _MainSpeed_V;
			uniform float4 _MainTex_ST;
			uniform float _MainPower;
			uniform float4 _MainCol;
			uniform sampler2D _MaskTex;
			uniform float _MaskSpeed_U;
			uniform float _MaskSpeed_V;
			uniform float4 _MaskTex_ST;
			uniform float _Distort_Mask_Str;
			uniform float _MaskPower;
			uniform float _Fresnel_Power;
			uniform float _Fresnel_Scale;
			uniform float _Fresnel_Bias;

			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				float2 appendResult47 = (float2(_VertexSpeed_U , _VertexSpeed_V));
				float2 uv_VertexTex = v.ase_texcoord.xy * _VertexTex_ST.xy + _VertexTex_ST.zw;
				float2 panner41 = ( _Time.y * appendResult47 + uv_VertexTex);
				
				float3 ase_worldNormal = UnityObjectToWorldNormal(v.ase_normal);
				o.ase_texcoord2.xyz = ase_worldNormal;
				
				o.ase_texcoord1.xy = v.ase_texcoord.xy;
				o.ase_color = v.color;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord1.zw = 0;
				o.ase_texcoord2.w = 0;
				float3 vertexValue = float3(0, 0, 0);
				#if ASE_ABSOLUTE_VERTEX_POS
				vertexValue = v.vertex.xyz;
				#endif
				vertexValue = ( float4( v.ase_normal , 0.0 ) * tex2Dlod( _VertexTex, float4( panner41, 0, 0.0) ) * _VertexAnimStr ).rgb;
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
				float2 appendResult21 = (float2(_DistortSpeed_U , _DistortSpeed_V));
				float2 uv_DistortTex = i.ase_texcoord1.xy * _DistortTex_ST.xy + _DistortTex_ST.zw;
				float2 panner20 = ( _Time.y * appendResult21 + uv_DistortTex);
				float4 temp_cast_0 = (0.5).xxxx;
				float4 break28 = ( tex2D( _DistortTex, panner20 ) - temp_cast_0 );
				float2 appendResult24 = (float2(break28.r , break28.g));
				float2 appendResult15 = (float2(_MainSpeed_U , _MainSpeed_V));
				float2 uv_MainTex = i.ase_texcoord1.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				float2 panner13 = ( _Time.y * appendResult15 + uv_MainTex);
				float4 tex2DNode2 = tex2D( _MainTex, ( ( appendResult24 * _DistortStr ) + panner13 ) );
				float4 temp_cast_1 = (_MainPower).xxxx;
				float2 appendResult9 = (float2(_MaskSpeed_U , _MaskSpeed_V));
				float2 uv_MaskTex = i.ase_texcoord1.xy * _MaskTex_ST.xy + _MaskTex_ST.zw;
				float2 panner6 = ( _Time.y * appendResult9 + uv_MaskTex);
				#ifdef _DISTORT_MASK_ON
				float2 staticSwitch58 = ( ( appendResult24 * _Distort_Mask_Str ) + panner6 );
				#else
				float2 staticSwitch58 = panner6;
				#endif
				float3 ase_worldNormal = i.ase_texcoord2.xyz;
				float3 normalizedWorldNormal = normalize( ase_worldNormal );
				float3 ase_worldViewDir = UnityWorldSpaceViewDir(WorldPosition);
				ase_worldViewDir = Unity_SafeNormalize( ase_worldViewDir );
				float dotResult68 = dot( normalizedWorldNormal , ase_worldViewDir );
				float4 appendResult10 = (float4(( pow( tex2DNode2 , temp_cast_1 ) * i.ase_color * _MainCol ).rgb , ( tex2DNode2.a * pow( tex2D( _MaskTex, staticSwitch58 ).r , _MaskPower ) * i.ase_color.a * saturate( ( ( pow( abs( dotResult68 ) , _Fresnel_Power ) * _Fresnel_Scale ) + _Fresnel_Bias ) ) * _MainCol.a )));
				
				
				finalColor = appendResult10;
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	Fallback Off
}
/*ASEBEGIN
Version=19100
Node;AmplifyShaderEditor.SamplerNode;4;-805.8875,263.7899;Inherit;True;Property;_MaskTex;MaskTex;6;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;10;-126.8291,5.107666;Inherit;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;12;-1747.413,-26.19774;Inherit;False;0;2;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;13;-1476.413,-12.19775;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TimeNode;17;-1716.047,347.2573;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;15;-1694.079,145.8023;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;18;-1825.069,-464.7275;Inherit;True;Property;_DistortTex;DistortTex;12;0;Create;True;0;0;0;False;0;False;-1;None;b9953a434094999428e0a26ead902a08;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;19;-2222.069,-458.7275;Inherit;False;0;18;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;20;-2021.069,-331.7275;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;21;-2187.735,-228.7274;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;27;-1648.417,-254.2422;Inherit;False;Constant;_Float0;Float 0;10;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;28;-1269.417,-457.2422;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleSubtractOpNode;26;-1455.417,-453.2422;Inherit;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.DynamicAppendNode;24;-1091.663,-444.7712;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;29;-974.4165,-292.2422;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;25;-1106.663,-5.77124;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;34;-333.5879,-81.05841;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3;-242.8875,247.7899;Inherit;False;5;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;33;-534.5879,145.9416;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;35;-2414.696,-128.3271;Inherit;False;Property;_DistortSpeed_U;DistortSpeed_U;13;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;36;-2417.696,-41.32709;Inherit;False;Property;_DistortSpeed_V;DistortSpeed_V;14;0;Create;True;0;0;0;False;0;False;0;-0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;37;-1894.589,140.8553;Inherit;False;Property;_MainSpeed_U;MainSpeed_U;4;0;Create;True;0;0;0;False;0;False;0;-0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;38;-1883.575,229.3385;Inherit;False;Property;_MainSpeed_V;MainSpeed_V;5;0;Create;True;0;0;0;False;0;False;0;-1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;39;-756.7577,666.3702;Inherit;True;Property;_VertexTex;VertexTex;16;0;Create;True;0;0;0;False;0;False;-1;None;4b8d32341fb26ef409b2cb9c6e3b2f6f;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;40;-1257.505,687.7068;Inherit;False;0;39;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;41;-1001.505,698.7068;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;45;-1192.761,905.7051;Inherit;False;Property;_VertexSpeed_U;VertexSpeed_U;17;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;46;-1190.761,1005.705;Inherit;False;Property;_VertexSpeed_V;VertexSpeed_V;18;0;Create;True;0;0;0;False;0;False;0;-0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;47;-992.7607,930.7051;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;50;-405.5302,876.3614;Inherit;False;Property;_VertexAnimStr;VertexAnimStr;19;0;Create;True;0;0;0;False;0;False;0;0.14;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.NormalVertexDataNode;49;-327.8501,483.9152;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;48;-76.8501,627.9152;Inherit;False;3;3;0;FLOAT3;0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;898.9675,-13.7212;Float;False;True;-1;2;ASEMaterialInspector;100;5;vfx;0770190933193b94aaa3065e307002fa;True;Unlit;0;0;Unlit;2;True;True;1;5;False;;0;True;_Blend;0;1;False;;0;False;;True;0;False;;0;False;;False;False;False;False;False;False;False;False;False;True;0;False;;True;True;2;False;;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;True;True;2;False;;True;3;False;;True;False;0;False;;0;False;;True;2;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;True;2;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;0;;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;0;1;True;False;;False;0
Node;AmplifyShaderEditor.OneMinusNode;55;53.86668,1023.662;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;5;-1543.837,263.9891;Inherit;False;0;4;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;8;-1617.837,555.9891;Inherit;False;Property;_MaskSpeed_V;MaskSpeed_V;9;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;9;-1429.837,516.9891;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;7;-1623.837,471.9891;Inherit;False;Property;_MaskSpeed_U;MaskSpeed_U;8;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;6;-1306.717,286.4752;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;57;-1014.354,211.3585;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;30;-1319.417,-230.2422;Inherit;False;Property;_DistortStr;DistortStr;15;0;Create;True;0;0;0;False;0;False;0;0.235;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;59;-1347.039,114.442;Inherit;False;Property;_Distort_Mask_Str;Distort_Mask_Str;11;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;60;-1008.039,99.44202;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.StaticSwitch;58;-1027.897,378.1238;Inherit;False;Property;_Distort_Mask;Distort_Mask;10;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT2;0,0;False;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT2;0,0;False;6;FLOAT2;0,0;False;7;FLOAT2;0,0;False;8;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PowerNode;61;-461.8767,348.4452;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-821.7737,-79.16486;Inherit;True;Property;_MainTex;MainTex;1;0;Create;True;0;0;0;False;0;False;-1;None;d40975455bac9c5408223ecca75db44a;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;1;-650.0967,-363.0883;Inherit;False;Property;_Blend;Blend;0;1;[Enum];Create;True;0;2;Add;1;Alpha;10;0;True;0;False;10;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;63;-764.921,-267.4592;Inherit;False;Property;_MainCol;MainCol;3;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,1;2.828427,2.828427,2.828427,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;62;-668.8767,469.4452;Inherit;False;Property;_MaskPower;MaskPower;7;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;64;-509.7979,-73.12256;Inherit;False;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;65;-756.7979,141.8774;Inherit;False;Property;_MainPower;MainPower;2;0;Create;True;0;0;0;False;0;False;1;1.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;52;-978.3438,1144.319;Inherit;False;Property;_Fresnel_Bias;Fresnel_Bias;22;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;53;-873.3438,1223.319;Inherit;False;Property;_Fresnel_Scale;Fresnel_Scale;20;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;54;-872.3438,1319.319;Inherit;False;Property;_Fresnel_Power;Fresnel_Power;21;0;Create;True;0;0;0;False;0;False;1;1.24;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;71;-617.618,1410.248;Inherit;False;World;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.FresnelNode;51;-626.2342,1034.354;Inherit;False;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;66;-315.4851,1045.115;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;74;232.382,1287.248;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;69;-638.618,1250.248;Inherit;False;True;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DotProductOpNode;68;-393.618,1249.248;Inherit;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;72;-158.618,1251.248;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;73;25.38196,1262.248;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;75;401.382,1300.248;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;56;-87.38977,1027.131;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;76;580.818,1279.18;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
WireConnection;4;1;58;0
WireConnection;10;0;34;0
WireConnection;10;3;3;0
WireConnection;13;0;12;0
WireConnection;13;2;15;0
WireConnection;13;1;17;2
WireConnection;15;0;37;0
WireConnection;15;1;38;0
WireConnection;18;1;20;0
WireConnection;20;0;19;0
WireConnection;20;2;21;0
WireConnection;20;1;17;2
WireConnection;21;0;35;0
WireConnection;21;1;36;0
WireConnection;28;0;26;0
WireConnection;26;0;18;0
WireConnection;26;1;27;0
WireConnection;24;0;28;0
WireConnection;24;1;28;1
WireConnection;29;0;24;0
WireConnection;29;1;30;0
WireConnection;25;0;29;0
WireConnection;25;1;13;0
WireConnection;34;0;64;0
WireConnection;34;1;33;0
WireConnection;34;2;63;0
WireConnection;3;0;2;4
WireConnection;3;1;61;0
WireConnection;3;2;33;4
WireConnection;3;3;76;0
WireConnection;3;4;63;4
WireConnection;39;1;41;0
WireConnection;41;0;40;0
WireConnection;41;2;47;0
WireConnection;41;1;17;2
WireConnection;47;0;45;0
WireConnection;47;1;46;0
WireConnection;48;0;49;0
WireConnection;48;1;39;0
WireConnection;48;2;50;0
WireConnection;0;0;10;0
WireConnection;0;1;48;0
WireConnection;55;0;56;0
WireConnection;9;0;7;0
WireConnection;9;1;8;0
WireConnection;6;0;5;0
WireConnection;6;2;9;0
WireConnection;6;1;17;2
WireConnection;57;0;60;0
WireConnection;57;1;6;0
WireConnection;60;0;24;0
WireConnection;60;1;59;0
WireConnection;58;1;6;0
WireConnection;58;0;57;0
WireConnection;61;0;4;1
WireConnection;61;1;62;0
WireConnection;2;1;25;0
WireConnection;64;0;2;0
WireConnection;64;1;65;0
WireConnection;51;1;52;0
WireConnection;51;2;53;0
WireConnection;51;3;54;0
WireConnection;66;0;51;0
WireConnection;74;0;73;0
WireConnection;74;1;53;0
WireConnection;68;0;69;0
WireConnection;68;1;71;0
WireConnection;72;0;68;0
WireConnection;73;0;72;0
WireConnection;73;1;54;0
WireConnection;75;0;74;0
WireConnection;75;1;52;0
WireConnection;56;0;66;0
WireConnection;76;0;75;0
ASEEND*/
//CHKSM=B321E22D8CB766842DF3C6B14CA3178C16F638EF