// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ASE/A_TwoColor 2"
{
	Properties
	{
		[HDR]_Color1("Color 1", Color) = (3.890196,1.929412,0.7843137,0)
		[HDR]_Color2("Color 2", Color) = (0.5176471,1.411765,3.482353,0)
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
			uniform float4 _Color1;
			uniform float4 _Color2;
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
				float4 lerpResult3_g3 = lerp( _Color1 , _Color2 , (0.0 + (sin( ( _Time.y * _FlashSpeed ) ) - -1.0) * (1.0 - 0.0) / (1.0 - -1.0)));
				float2 uv_MainTex = i.ase_texcoord1.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				float2 panner5 = ( 1.0 * _Time.y * _MainTexSpeed + uv_MainTex);
				float2 texCoord2_g2 = i.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float2 panner5_g2 = ( 1.0 * _Time.y * _TwistSpeed + texCoord2_g2);
				float4 temp_output_13_0 = ( _TwistScale * tex2D( _TwistTex, panner5_g2 ) );
				float2 uv_AddtiveTex = i.ase_texcoord1.xy * _AddtiveTex_ST.xy + _AddtiveTex_ST.zw;
				float2 panner21 = ( 1.0 * _Time.y * _AddtiveSpeed + uv_AddtiveTex);
				
				
				finalColor = saturate( ( ( lerpResult3_g3 * tex2D( _MainTex, ( float4( panner5, 0.0 , 0.0 ) + temp_output_13_0 ).rg ) ) + ( tex2D( _AddtiveTex, ( temp_output_13_0 + float4( panner21, 0.0 , 0.0 ) ).rg ) * _AddtiveColor ) ) );
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=18912
2087;156;1339;663;1875.44;106.319;1.272265;True;False
Node;AmplifyShaderEditor.Vector2Node;11;-1415.583,-8.912441;Inherit;False;Property;_MainTexSpeed;MainTexSpeed;4;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;4;-1463.306,-163.8141;Inherit;False;0;1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexturePropertyNode;15;-1684.079,112.0637;Inherit;True;Property;_TwistTex;TwistTex;8;0;Create;True;0;0;0;False;0;False;2e1b80183ef63184e9a3f5b38bb262ad;2e1b80183ef63184e9a3f5b38bb262ad;False;black;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.RangedFloatNode;16;-1689.079,301.7637;Inherit;False;Property;_TwistScale;TwistScale;10;0;Create;True;0;0;0;False;0;False;0.05600879;0.0615;0;0.2;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;22;-1235.596,797.8911;Inherit;False;Property;_AddtiveSpeed;AddtiveSpeed;7;0;Create;True;0;0;0;False;0;False;0,0;0.3,0.1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;20;-1253.834,650.6039;Inherit;False;0;19;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;17;-1557.079,405.7637;Inherit;False;Property;_TwistSpeed;TwistSpeed;9;0;Create;True;0;0;0;False;0;False;0.1,0.1;0.1,0.1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.PannerNode;21;-988.8345,694.6039;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FunctionNode;13;-1198.703,241.8227;Inherit;False;Twist;-1;;2;e8b1a858a14c80346a0094288f3886a3;0;3;4;SAMPLER2D;0;False;8;FLOAT;0.1;False;9;FLOAT2;0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.PannerNode;5;-1097.306,-105.8141;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;25;-725.6951,378.8525;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT2;0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;12;-830.5654,43.78915;Inherit;False;2;2;0;FLOAT2;0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;9;-776.2818,-314.5972;Inherit;False;Property;_FlashSpeed;FlashSpeed;2;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;8;-791.438,-504.782;Inherit;False;Property;_Color2;Color 2;1;1;[HDR];Create;True;0;0;0;False;0;False;0.5176471,1.411765,3.482353,0;0.1610353,0.2644013,0.4627451,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;7;-793.438,-678.7819;Inherit;False;Property;_Color1;Color 1;0;1;[HDR];Create;True;0;0;0;False;0;False;3.890196,1.929412,0.7843137,0;0.5377358,0.3074299,0.08370417,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;24;-456.4685,500.2301;Inherit;False;Property;_AddtiveColor;AddtiveColor;6;1;[HDR];Create;True;0;0;0;False;0;False;1,0.6428995,0,0;0.7372549,1.443137,2.870588,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;6;-457.0078,-476.1313;Inherit;False;TwoColor;-1;;3;f995fab11fb68024c924ef159cb30e44;0;3;11;COLOR;0.4768601,0.6269382,0.7169812,1;False;12;COLOR;0.2988163,0.4963823,0.8018868,1;False;13;FLOAT;5;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;19;-542.3881,283.012;Inherit;True;Property;_AddtiveTex;AddtiveTex;5;0;Create;True;0;0;0;False;0;False;-1;None;fbddcaa3dbdb1424499eadbddac80ec2;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;1;-683.0034,-23.57096;Inherit;True;Property;_MainTex;MainTex;3;0;Create;True;0;0;0;False;0;False;-1;8f855d3ce7eb9ab4aa0959df6a508cb5;ad9f7c63232a63141a9faccdab1d2133;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3;-160.675,-138.2685;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;23;-206.4685,225.2301;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;18;76.76355,-16.48511;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;26;37.22656,-369.7472;Inherit;False;Property;_CullMode;CullMode;11;1;[Enum];Create;True;0;0;1;UnityEngine.Rendering.CullMode;True;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;27;282.4821,-13.84671;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;525,-52;Float;False;True;-1;2;ASEMaterialInspector;100;1;ASE/A_TwoColor 2;0770190933193b94aaa3065e307002fa;True;Unlit;0;0;Unlit;2;True;True;4;1;False;-1;1;False;-1;0;1;False;-1;0;False;-1;True;0;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;True;0;False;-1;True;True;0;True;26;False;True;True;True;True;True;0;False;-1;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;2;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;True;2;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=ForwardBase;False;False;0;;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;1;True;False;;False;0
WireConnection;21;0;20;0
WireConnection;21;2;22;0
WireConnection;13;4;15;0
WireConnection;13;8;16;0
WireConnection;13;9;17;0
WireConnection;5;0;4;0
WireConnection;5;2;11;0
WireConnection;25;0;13;0
WireConnection;25;1;21;0
WireConnection;12;0;5;0
WireConnection;12;1;13;0
WireConnection;6;11;7;0
WireConnection;6;12;8;0
WireConnection;6;13;9;0
WireConnection;19;1;25;0
WireConnection;1;1;12;0
WireConnection;3;0;6;0
WireConnection;3;1;1;0
WireConnection;23;0;19;0
WireConnection;23;1;24;0
WireConnection;18;0;3;0
WireConnection;18;1;23;0
WireConnection;27;0;18;0
WireConnection;0;0;27;0
ASEEND*/
//CHKSM=774BEA7410E3A5813ACD2111A5B9A3590DDD5BE8