// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ASE/A_TwoTintMask"
{
	Properties
	{
		[Enum(UnityEngine.Rendering.CullMode)]_CullMode1("CullMode", Float) = 0
		[HDR]_Color2("Color 1", Color) = (3.890196,1.929412,0.7843137,0)
		[HDR]_Color3("Color 2", Color) = (0.5176471,1.411765,3.482353,0)
		_FlashSpeed1("FlashSpeed", Float) = 1
		_MainTex1("MainTex", 2D) = "white" {}
		_MainTexSpeed1("MainTexSpeed", Vector) = (0,0,0,0)
		_AddtiveTex1("AddtiveTex", 2D) = "white" {}
		[HDR]_AddtiveColor1("AddtiveColor", Color) = (1,0.6428995,0,0)
		_AddtiveSpeed1("AddtiveSpeed", Vector) = (0,0,0,0)
		_TwistTex1("TwistTex", 2D) = "black" {}
		_TwistSpeed1("TwistSpeed", Vector) = (0.1,0.1,0,0)
		_TwistScale1("TwistScale", Range( 0 , 0.2)) = 0.05600879
		_Mask("Mask", 2D) = "white" {}
		[Enum(R,0,A,1)]_RorA("R or A", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}

	}
	
	SubShader
	{
		
		
		Tags { "RenderType"="Opaque" }
	LOD 100

		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend One One
		AlphaToMask Off
		Cull Back
		ColorMask RGBA
		ZWrite Off
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

			uniform float _CullMode1;
			uniform sampler2D _Mask;
			uniform float4 _Mask_ST;
			uniform float _RorA;
			uniform float4 _Color2;
			uniform float4 _Color3;
			uniform float _FlashSpeed1;
			uniform sampler2D _MainTex1;
			uniform float2 _MainTexSpeed1;
			uniform float4 _MainTex1_ST;
			uniform float _TwistScale1;
			uniform sampler2D _TwistTex1;
			uniform float2 _TwistSpeed1;
			uniform sampler2D _AddtiveTex1;
			uniform float2 _AddtiveSpeed1;
			uniform float4 _AddtiveTex1_ST;
			uniform float4 _AddtiveColor1;

			
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
				float2 uv_Mask = i.ase_texcoord1.xy * _Mask_ST.xy + _Mask_ST.zw;
				float4 tex2DNode1 = tex2D( _Mask, uv_Mask );
				float lerpResult28 = lerp( tex2DNode1.r , tex2DNode1.a , _RorA);
				float4 lerpResult3_g3 = lerp( _Color2 , _Color3 , (0.0 + (sin( ( _Time.y * _FlashSpeed1 ) ) - -1.0) * (1.0 - 0.0) / (1.0 - -1.0)));
				float2 uv_MainTex1 = i.ase_texcoord1.xy * _MainTex1_ST.xy + _MainTex1_ST.zw;
				float2 panner9 = ( 1.0 * _Time.y * _MainTexSpeed1 + uv_MainTex1);
				float2 texCoord2_g2 = i.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float2 panner5_g2 = ( 1.0 * _Time.y * _TwistSpeed1 + texCoord2_g2);
				float4 temp_output_10_0 = ( _TwistScale1 * tex2D( _TwistTex1, panner5_g2 ) );
				float2 uv_AddtiveTex1 = i.ase_texcoord1.xy * _AddtiveTex1_ST.xy + _AddtiveTex1_ST.zw;
				float2 panner11 = ( 1.0 * _Time.y * _AddtiveSpeed1 + uv_AddtiveTex1);
				
				
				finalColor = ( lerpResult28 * ( ( lerpResult3_g3 * tex2D( _MainTex1, ( float4( panner9, 0.0 , 0.0 ) + temp_output_10_0 ).rg ) ) + ( tex2D( _AddtiveTex1, ( temp_output_10_0 + float4( panner11, 0.0 , 0.0 ) ).rg ) * _AddtiveColor1 ) ) );
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=18912
2029;150;1445;783;2464.926;1313.288;2.174843;True;False
Node;AmplifyShaderEditor.Vector2Node;4;-1999.487,131.964;Inherit;False;Property;_TwistSpeed1;TwistSpeed;10;0;Create;True;0;0;0;False;0;False;0.1,0.1;0.1,0.1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TexturePropertyNode;5;-2126.487,-161.736;Inherit;True;Property;_TwistTex1;TwistTex;9;0;Create;True;0;0;0;False;0;False;2e1b80183ef63184e9a3f5b38bb262ad;2e1b80183ef63184e9a3f5b38bb262ad;False;black;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.Vector2Node;6;-1718.431,287.6839;Inherit;False;Property;_AddtiveSpeed1;AddtiveSpeed;8;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;7;-2131.487,27.96402;Inherit;False;Property;_TwistScale1;TwistScale;11;0;Create;True;0;0;0;False;0;False;0.05600879;0.0261;0;0.2;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;2;-1946.141,-453.0213;Inherit;False;0;18;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;3;-1736.669,140.3967;Inherit;False;0;19;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;8;-1893.966,-307.024;Inherit;False;Property;_MainTexSpeed1;MainTexSpeed;5;0;Create;True;0;0;0;False;0;False;0,0;-0.5,-0.1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.PannerNode;9;-1562.088,-378.6965;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FunctionNode;10;-1688.538,-181.7494;Inherit;False;Twist;-1;;2;e8b1a858a14c80346a0094288f3886a3;0;3;4;SAMPLER2D;0;False;8;FLOAT;0.1;False;9;FLOAT2;0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.PannerNode;11;-1471.669,184.3967;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;15;-1480.892,-507.4337;Inherit;False;Property;_FlashSpeed1;FlashSpeed;3;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;16;-1496.048,-737.3489;Inherit;False;Property;_Color3;Color 2;2;1;[HDR];Create;True;0;0;0;False;0;False;0.5176471,1.411765,3.482353,0;0.1518325,0.8257051,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;13;-1498.048,-911.3488;Inherit;False;Property;_Color2;Color 1;1;1;[HDR];Create;True;0;0;0;False;0;False;3.890196,1.929412,0.7843137,0;0.682353,0.3547087,0.1937883,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;14;-1313.4,-303.418;Inherit;False;2;2;0;FLOAT2;0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;12;-1208.53,31.64532;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT2;0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;20;-1161.618,-792.7433;Inherit;False;TwoColor;-1;;3;f995fab11fb68024c924ef159cb30e44;0;3;11;COLOR;0.4768601,0.6269382,0.7169812,1;False;12;COLOR;0.2988163,0.4963823,0.8018868,1;False;13;FLOAT;5;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;18;-1165.838,-370.7781;Inherit;True;Property;_MainTex1;MainTex;4;0;Create;True;0;0;0;False;0;False;-1;8f855d3ce7eb9ab4aa0959df6a508cb5;8f855d3ce7eb9ab4aa0959df6a508cb5;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;17;-939.3034,153.0229;Inherit;False;Property;_AddtiveColor1;AddtiveColor;7;1;[HDR];Create;True;0;0;0;False;0;False;1,0.6428995,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;19;-1025.223,-64.19519;Inherit;True;Property;_AddtiveTex1;AddtiveTex;6;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;1;-954.029,-955.5302;Inherit;True;Property;_Mask;Mask;12;0;Create;True;0;0;0;False;0;False;-1;None;86c8810146a27704db014306e386dfe2;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;-800.7059,-467.0543;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;22;-689.3034,-121.9771;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;29;-764.1473,-728.2422;Inherit;False;Property;_RorA;R or A;13;1;[Enum];Create;True;0;2;R;0;A;1;0;False;0;False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;28;-538.7949,-901.8076;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;23;-481.0714,-289.9158;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;25;-228.559,-489.4448;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;24;-4.300723,-759.5366;Inherit;False;Property;_CullMode1;CullMode;0;1;[Enum];Create;True;0;0;1;UnityEngine.Rendering.CullMode;True;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;179.9429,-71.39474;Float;False;True;-1;2;ASEMaterialInspector;100;1;ASE/A_TwoTintMask;0770190933193b94aaa3065e307002fa;True;Unlit;0;0;Unlit;2;True;True;4;1;False;-1;1;False;-1;0;1;False;-1;0;False;-1;True;0;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;True;True;True;True;True;0;False;-1;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;True;2;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;RenderType=Opaque=RenderType;True;2;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=ForwardBase;False;False;0;;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;1;True;False;;False;0
WireConnection;9;0;2;0
WireConnection;9;2;8;0
WireConnection;10;4;5;0
WireConnection;10;8;7;0
WireConnection;10;9;4;0
WireConnection;11;0;3;0
WireConnection;11;2;6;0
WireConnection;14;0;9;0
WireConnection;14;1;10;0
WireConnection;12;0;10;0
WireConnection;12;1;11;0
WireConnection;20;11;13;0
WireConnection;20;12;16;0
WireConnection;20;13;15;0
WireConnection;18;1;14;0
WireConnection;19;1;12;0
WireConnection;21;0;20;0
WireConnection;21;1;18;0
WireConnection;22;0;19;0
WireConnection;22;1;17;0
WireConnection;28;0;1;1
WireConnection;28;1;1;4
WireConnection;28;2;29;0
WireConnection;23;0;21;0
WireConnection;23;1;22;0
WireConnection;25;0;28;0
WireConnection;25;1;23;0
WireConnection;0;0;25;0
ASEEND*/
//CHKSM=9136DEE6092C0CA4BB57FF1C303BC9FDBF94CFA3