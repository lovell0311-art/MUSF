// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ASE/A_ZFDTSZN 1"
{
	Properties
	{
		[HDR]_BaseColor("BaseColor", Color) = (1,1,1,1)
		_MainTex("MainTex", 2D) = "white" {}
		[HDR]_Noise1Color1("Noise1Color1", Color) = (0.4245283,0.2715882,0.2022517,0)
		[HDR]_Noise1Color2("Noise1Color2", Color) = (0.1732823,0.2837319,0.6226415,0)
		_NoiseTex1("NoiseTex1", 2D) = "white" {}
		_NoiseTex1Speed("NoiseTex1Speed", Vector) = (0,0,0,0)
		[HDR]_Noise2Color("Noise2Color", Color) = (0,0,0,0)
		_NoiseTex2("NoiseTex2", 2D) = "white" {}
		_NoiseTex2Speed("NoiseTex2Speed", Vector) = (0,0,0,0)
		_TwistTex("TwistTex", 2D) = "white" {}
		_TwistSpeed("TwistSpeed", Vector) = (0,0,0,0)
		_TwistIntensity("TwistIntensity", Range( 0 , 1)) = 0

	}
	
	SubShader
	{
		
		
		Tags { "RenderType"="Opaque" }
	LOD 100

		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend Off
		AlphaToMask Off
		Cull Back
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

			uniform float4 _BaseColor;
			uniform sampler2D _MainTex;
			uniform float4 _Noise1Color1;
			uniform float4 _Noise1Color2;
			uniform sampler2D _NoiseTex1;
			uniform float2 _NoiseTex1Speed;
			uniform float4 _NoiseTex1_ST;
			uniform float _TwistIntensity;
			uniform sampler2D _TwistTex;
			uniform float2 _TwistSpeed;
			uniform float4 _TwistTex_ST;
			uniform float4 _Noise2Color;
			uniform sampler2D _NoiseTex2;
			uniform float2 _NoiseTex2Speed;
			uniform float4 _NoiseTex2_ST;

			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

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
				float2 texCoord2 = i.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float4 lerpResult27 = lerp( _Noise1Color1 , _Noise1Color2 , (0.0 + (sin( ( _Time.y * 1.0 ) ) - -1.0) * (1.0 - 0.0) / (1.0 - -1.0)));
				float2 uv_NoiseTex1 = i.ase_texcoord1.xy * _NoiseTex1_ST.xy + _NoiseTex1_ST.zw;
				float2 panner9 = ( 1.0 * _Time.y * _NoiseTex1Speed + uv_NoiseTex1);
				float2 uv_TwistTex = i.ase_texcoord1.xy * _TwistTex_ST.xy + _TwistTex_ST.zw;
				float2 panner23 = ( 1.0 * _Time.y * _TwistSpeed + uv_TwistTex);
				float temp_output_21_0 = ( _TwistIntensity * tex2D( _TwistTex, panner23 ).r );
				float2 uv_NoiseTex2 = i.ase_texcoord1.xy * _NoiseTex2_ST.xy + _NoiseTex2_ST.zw;
				float2 panner14 = ( 1.0 * _Time.y * _NoiseTex2Speed + uv_NoiseTex2);
				
				
				finalColor = ( ( _BaseColor * tex2D( _MainTex, texCoord2 ) ) + ( lerpResult27 * tex2D( _NoiseTex1, ( panner9 + temp_output_21_0 ) ).r ) + ( _Noise2Color * tex2D( _NoiseTex2, ( panner14 + temp_output_21_0 ) ).r ) );
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=18912
2029;150;1445;783;2735.097;-592.8148;1;True;False
Node;AmplifyShaderEditor.TextureCoordinatesNode;24;-2090.845,717.8126;Inherit;False;0;20;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;25;-2044.794,866.2128;Inherit;False;Property;_TwistSpeed;TwistSpeed;10;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleTimeNode;29;-1300.948,143.5958;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;23;-1798.174,803.7903;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;30;-1278.963,222.4692;Inherit;False;Constant;_FlashSpeed;FlashSpeed;11;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;22;-1504.761,698.0449;Inherit;False;Property;_TwistIntensity;TwistIntensity;11;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;20;-1528.611,812.8446;Inherit;True;Property;_TwistTex;TwistTex;9;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;8;-1720.404,272.6165;Inherit;False;0;6;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;15;-1538.957,1088.378;Inherit;False;0;12;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;31;-1120.177,201.2353;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;16;-1529.857,1241.778;Inherit;False;Property;_NoiseTex2Speed;NoiseTex2Speed;8;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;10;-1711.304,426.0166;Inherit;False;Property;_NoiseTex1Speed;NoiseTex1Speed;5;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;-1199.246,795.3743;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;14;-1171.451,1173.497;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;9;-1399.304,351.9164;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SinOpNode;33;-997.1772,238.2353;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;34;-819.1772,320.2353;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;-1;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;19;-975.9401,1173.168;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ColorNode;11;-839.4786,-24.54732;Inherit;False;Property;_Noise1Color1;Noise1Color1;2;1;[HDR];Create;True;0;0;0;False;0;False;0.4245283,0.2715882,0.2022517,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;18;-964.8765,667.3864;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;2;-1031.036,-243.7675;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;26;-843.075,148.2669;Inherit;False;Property;_Noise1Color2;Noise1Color2;3;1;[HDR];Create;True;0;0;0;False;0;False;0.1732823,0.2837319,0.6226415,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;4;-620.9357,-450.1675;Inherit;False;Property;_BaseColor;BaseColor;0;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;27;-609.0859,125.0885;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;1;-756.0841,-254.1884;Inherit;True;Property;_MainTex;MainTex;1;0;Create;True;0;0;0;False;0;False;-1;1ba634ad55cbd6242a61f49bca2d94ee;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;12;-798.3309,961.0953;Inherit;True;Property;_NoiseTex2;NoiseTex2;7;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;6;-824.2946,565.5657;Inherit;True;Property;_NoiseTex1;NoiseTex1;4;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;17;-732.7311,790.4954;Inherit;False;Property;_Noise2Color;Noise2Color;6;1;[HDR];Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3;-326.2159,-184.1655;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-314.925,238.0058;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;13;-452.5305,923.3953;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;5;-22.53252,42.26043;Inherit;True;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;211.0684,97.72342;Float;False;True;-1;2;ASEMaterialInspector;100;1;ASE/A_ZFDTSZN 1;0770190933193b94aaa3065e307002fa;True;Unlit;0;0;Unlit;2;False;True;0;1;False;-1;0;False;-1;0;1;False;-1;0;False;-1;True;0;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;True;True;True;True;True;0;False;-1;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;RenderType=Opaque=RenderType;True;2;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=ForwardBase;False;False;0;;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;1;True;False;;False;0
WireConnection;23;0;24;0
WireConnection;23;2;25;0
WireConnection;20;1;23;0
WireConnection;31;0;29;0
WireConnection;31;1;30;0
WireConnection;21;0;22;0
WireConnection;21;1;20;1
WireConnection;14;0;15;0
WireConnection;14;2;16;0
WireConnection;9;0;8;0
WireConnection;9;2;10;0
WireConnection;33;0;31;0
WireConnection;34;0;33;0
WireConnection;19;0;14;0
WireConnection;19;1;21;0
WireConnection;18;0;9;0
WireConnection;18;1;21;0
WireConnection;27;0;11;0
WireConnection;27;1;26;0
WireConnection;27;2;34;0
WireConnection;1;1;2;0
WireConnection;12;1;19;0
WireConnection;6;1;18;0
WireConnection;3;0;4;0
WireConnection;3;1;1;0
WireConnection;7;0;27;0
WireConnection;7;1;6;1
WireConnection;13;0;17;0
WireConnection;13;1;12;1
WireConnection;5;0;3;0
WireConnection;5;1;7;0
WireConnection;5;2;13;0
WireConnection;0;0;5;0
ASEEND*/
//CHKSM=196E516502D7FD25C4B60CCB883B131043A33527