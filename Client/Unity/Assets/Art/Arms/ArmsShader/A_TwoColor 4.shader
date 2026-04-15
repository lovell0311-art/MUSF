// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ASE/A_TwoColor 4"
{
	Properties
	{
		[IntRange]_EffectIntensity("EffectIntensity", Range( 1 , 15)) = 15
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
		[HDR]_46Color1("4-6Color1", Color) = (3.890196,1.929412,0.7843137,0)
		[HDR]_46Color2("4-6Color2", Color) = (0.5176471,1.411765,3.482353,0)
		_79Color2("7-9Color2", Color) = (0.1071556,0.2724442,0.5283019,0)
		_79Color1("7-9Color1", Color) = (0.01779993,0.04205022,0.1509434,0)
		_1012Color1("10-12Color1", Color) = (0.1886792,0.09813809,0.03648984,0)
		_1012Color2("10-12Color2", Color) = (0.5471698,0.2325777,0.05420078,0)
		[HDR]_1315Color1("13-15Color1", Color) = (0.1871753,0.6239178,1.06066,0)
		[HDR]_1315Color2("13-15Color2", Color) = (1.5,0.9647059,0,0)

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
				float4 ase_texcoord1 : TEXCOORD1;
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
			uniform float4 _46Color1;
			uniform float _EffectIntensity;
			uniform float4 _79Color1;
			uniform float4 _1012Color1;
			uniform float4 _1315Color1;
			uniform float4 _46Color2;
			uniform float4 _79Color2;
			uniform float4 _1012Color2;
			uniform float4 _1315Color2;
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
				o.ase_texcoord1.zw = v.ase_texcoord1.xy;
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
				float temp_output_47_0 = (0.0 + (_EffectIntensity - 1.0) * (1.0 - 0.0) / (15.0 - 1.0));
				float temp_output_49_0 = step( 0.15 , temp_output_47_0 );
				float4 lerpResult50 = lerp( float4( 0,0,0,0 ) , _46Color1 , temp_output_49_0);
				float temp_output_51_0 = step( 0.4 , temp_output_47_0 );
				float4 lerpResult53 = lerp( lerpResult50 , _79Color1 , temp_output_51_0);
				float temp_output_56_0 = step( 0.6 , temp_output_47_0 );
				float4 lerpResult60 = lerp( lerpResult53 , _1012Color1 , temp_output_56_0);
				float temp_output_61_0 = step( 0.85 , temp_output_47_0 );
				float4 lerpResult65 = lerp( lerpResult60 , _1315Color1 , temp_output_61_0);
				float4 lerpResult48 = lerp( float4( 0,0,0,0 ) , _46Color2 , temp_output_49_0);
				float4 lerpResult52 = lerp( lerpResult48 , _79Color2 , temp_output_51_0);
				float4 lerpResult57 = lerp( lerpResult52 , _1012Color2 , temp_output_56_0);
				float4 lerpResult62 = lerp( lerpResult57 , _1315Color2 , temp_output_61_0);
				float4 lerpResult3_g9 = lerp( lerpResult65 , lerpResult62 , (0.0 + (sin( ( _Time.y * _FlashSpeed ) ) - -1.0) * (1.0 - 0.0) / (1.0 - -1.0)));
				float2 uv_MainTex = i.ase_texcoord1.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				float2 panner5 = ( 1.0 * _Time.y * _MainTexSpeed + uv_MainTex);
				float2 texCoord2_g8 = i.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float2 panner5_g8 = ( 1.0 * _Time.y * _TwistSpeed + texCoord2_g8);
				float4 temp_output_68_0 = ( _TwistScale * tex2D( _TwistTex, panner5_g8 ) );
				float2 uv2_AddtiveTex = i.ase_texcoord1.zw * _AddtiveTex_ST.xy + _AddtiveTex_ST.zw;
				float2 panner21 = ( 1.0 * _Time.y * _AddtiveSpeed + uv2_AddtiveTex);
				
				
				finalColor = saturate( ( ( ( lerpResult3_g9 * tex2D( _MainTex, ( float4( panner5, 0.0 , 0.0 ) + temp_output_68_0 ).rg ) ) + ( tex2D( _AddtiveTex, ( float4( panner21, 0.0 , 0.0 ) + temp_output_68_0 ).rg ) * _AddtiveColor * step( 0.95 , temp_output_47_0 ) ) ) * (0.3 + (_EffectIntensity - 1.0) * (1.5 - 0.3) / (15.0 - 1.0)) ) );
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=18912
1944;67;1687;901;3383.393;1654.541;1;True;False
Node;AmplifyShaderEditor.RangedFloatNode;30;-2878.66,-1448.279;Inherit;False;Property;_EffectIntensity;EffectIntensity;0;1;[IntRange];Create;True;0;0;0;True;0;False;15;15;1;15;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;47;-2303.098,-1596.191;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;15;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;8;-2477.056,-2897.227;Inherit;False;Property;_46Color2;4-6Color2;12;1;[HDR];Create;True;0;0;0;False;0;False;0.5176471,1.411765,3.482353,0;0.66,0.66,0.66,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StepOpNode;49;-2453.655,-2643.71;Inherit;True;2;0;FLOAT;0.15;False;1;FLOAT;0.3;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;7;-2441.48,-3148.791;Inherit;False;Property;_46Color1;4-6Color1;11;1;[HDR];Create;True;0;0;0;False;0;False;3.890196,1.929412,0.7843137,0;0.1,0.1,0.1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;55;-2082.455,-2314.759;Inherit;False;Property;_79Color2;7-9Color2;13;0;Create;True;0;0;0;False;0;False;0.1071556,0.2724442,0.5283019,0;0.1071556,0.2724442,0.5283019,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StepOpNode;51;-2202.864,-1977.348;Inherit;True;2;0;FLOAT;0.4;False;1;FLOAT;0.3;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;50;-1932.383,-2879.01;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;48;-1982.717,-2691.54;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;54;-2094.878,-2523.549;Inherit;False;Property;_79Color1;7-9Color1;14;0;Create;True;0;0;0;False;0;False;0.01779993,0.04205022,0.1509434,0;0.01779993,0.04205022,0.1509434,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;17;-2709.91,25.93853;Inherit;False;Property;_TwistSpeed;TwistSpeed;8;0;Create;True;0;0;0;False;0;False;0.1,0.1;0.1,-0.1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TexturePropertyNode;15;-2812.718,-285.3553;Inherit;True;Property;_TwistTex;TwistTex;7;0;Create;True;0;0;0;False;0;False;2e1b80183ef63184e9a3f5b38bb262ad;c438d1f73d13e254284eb2baef1abca7;False;black;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.LerpOp;53;-1615.823,-2710.546;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;20;-2063.636,461.8657;Inherit;False;1;19;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;16;-2841.91,-78.06152;Inherit;False;Property;_TwistScale;TwistScale;9;0;Create;True;0;0;0;False;0;False;0.05600879;0.043;0;0.2;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;22;-2051.397,610.1529;Inherit;False;Property;_AddtiveSpeed;AddtiveSpeed;6;0;Create;True;0;0;0;False;0;False;0,0;0.5,0.1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;4;-2020.226,-609.4712;Inherit;False;0;1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;52;-1695.676,-2440.251;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StepOpNode;56;-1784.855,-1708.118;Inherit;True;2;0;FLOAT;0.6;False;1;FLOAT;0.3;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;58;-1680.426,-1964.007;Inherit;False;Property;_1012Color2;10-12Color2;16;0;Create;True;0;0;0;False;0;False;0.5471698,0.2325777,0.05420078,0;0.7,0.3793848,0.06999997,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;11;-1986.787,-402.6287;Inherit;False;Property;_MainTexSpeed;MainTexSpeed;3;0;Create;True;0;0;0;False;0;False;0,0;-1,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.ColorNode;59;-1672.231,-2163.468;Inherit;False;Property;_1012Color1;10-12Color1;15;0;Create;True;0;0;0;False;0;False;0.1886792,0.09813809,0.03648984,0;0.1886792,0.09813809,0.03648984,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;21;-1794.354,460.1037;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;5;-1731.346,-470.5664;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LerpOp;57;-1355.611,-1724.145;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;64;-1176.816,-1484.036;Inherit;False;Property;_1315Color2;13-15Color2;18;1;[HDR];Create;True;0;0;0;False;0;False;1.5,0.9647059,0,0;0.454,0.1498117,0.03223399,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;63;-977.6036,-2393.147;Inherit;False;Property;_1315Color1;13-15Color1;17;1;[HDR];Create;True;0;0;0;False;0;False;0.1871753,0.6239178,1.06066,0;0.277,0.250523,0.194454,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StepOpNode;61;-1768.777,-1442.737;Inherit;True;2;0;FLOAT;0.85;False;1;FLOAT;0.3;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;60;-1343.249,-1966.468;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;68;-2372.279,-136.8092;Inherit;False;Twist;-1;;8;e8b1a858a14c80346a0094288f3886a3;0;3;4;SAMPLER2D;0;False;8;FLOAT;0.1;False;9;FLOAT2;0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;12;-1510.919,-424.1909;Inherit;False;2;2;0;FLOAT2;0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;25;-1423.892,310.5496;Inherit;False;2;2;0;FLOAT2;0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;9;-739.2424,-1357.021;Inherit;False;Property;_FlashSpeed;FlashSpeed;1;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;62;-803.7081,-1620.16;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;65;-706.4694,-1949.834;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;1;-1328.232,-492.5748;Inherit;True;Property;_MainTex;MainTex;2;0;Create;True;0;0;0;False;0;False;-1;8f855d3ce7eb9ab4aa0959df6a508cb5;2e1b80183ef63184e9a3f5b38bb262ad;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;6;-354.6279,-1549.85;Inherit;False;TwoColor;-1;;9;f995fab11fb68024c924ef159cb30e44;0;3;11;COLOR;0.4768601,0.6269382,0.7169812,1;False;12;COLOR;0.2988163,0.4963823,0.8018868,1;False;13;FLOAT;5;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;24;-872.2468,551.6182;Inherit;False;Property;_AddtiveColor;AddtiveColor;5;1;[HDR];Create;True;0;0;0;False;0;False;1,0.6428995,0,0;1.286275,3.634953,5.992157,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;19;-963.88,195.4388;Inherit;True;Property;_AddtiveTex;AddtiveTex;4;0;Create;True;0;0;0;False;0;False;-1;None;d469fa5916039c84c8712b21e893f84e;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StepOpNode;46;-879.6072,829.2651;Inherit;True;2;0;FLOAT;0.95;False;1;FLOAT;0.3;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3;5.080956,-775.2604;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;23;-464.9127,166.9141;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCRemapNode;33;-491.6559,-257.0928;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;15;False;3;FLOAT;0.3;False;4;FLOAT;1.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;18;449.0964,-71.25868;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;29;660.2737,-65.83013;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;27;805.3574,-54.96138;Inherit;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;26;186.7763,-1819.299;Inherit;False;Property;_CullMode;CullMode;10;1;[Enum];Create;True;0;0;1;UnityEngine.Rendering.CullMode;True;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;1037.007,-126.4558;Float;False;True;-1;2;ASEMaterialInspector;100;1;ASE/A_TwoColor 4;0770190933193b94aaa3065e307002fa;True;Unlit;0;0;Unlit;2;True;True;4;1;False;-1;1;False;-1;0;1;False;-1;0;False;-1;True;0;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;True;0;False;-1;True;True;0;True;26;False;True;True;True;True;True;0;False;-1;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;2;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;True;2;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=ForwardBase;False;False;0;;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;1;True;False;;False;0
WireConnection;47;0;30;0
WireConnection;49;1;47;0
WireConnection;51;1;47;0
WireConnection;50;1;7;0
WireConnection;50;2;49;0
WireConnection;48;1;8;0
WireConnection;48;2;49;0
WireConnection;53;0;50;0
WireConnection;53;1;54;0
WireConnection;53;2;51;0
WireConnection;52;0;48;0
WireConnection;52;1;55;0
WireConnection;52;2;51;0
WireConnection;56;1;47;0
WireConnection;21;0;20;0
WireConnection;21;2;22;0
WireConnection;5;0;4;0
WireConnection;5;2;11;0
WireConnection;57;0;52;0
WireConnection;57;1;58;0
WireConnection;57;2;56;0
WireConnection;61;1;47;0
WireConnection;60;0;53;0
WireConnection;60;1;59;0
WireConnection;60;2;56;0
WireConnection;68;4;15;0
WireConnection;68;8;16;0
WireConnection;68;9;17;0
WireConnection;12;0;5;0
WireConnection;12;1;68;0
WireConnection;25;0;21;0
WireConnection;25;1;68;0
WireConnection;62;0;57;0
WireConnection;62;1;64;0
WireConnection;62;2;61;0
WireConnection;65;0;60;0
WireConnection;65;1;63;0
WireConnection;65;2;61;0
WireConnection;1;1;12;0
WireConnection;6;11;65;0
WireConnection;6;12;62;0
WireConnection;6;13;9;0
WireConnection;19;1;25;0
WireConnection;46;1;47;0
WireConnection;3;0;6;0
WireConnection;3;1;1;0
WireConnection;23;0;19;0
WireConnection;23;1;24;0
WireConnection;23;2;46;0
WireConnection;33;0;30;0
WireConnection;18;0;3;0
WireConnection;18;1;23;0
WireConnection;29;0;18;0
WireConnection;29;1;33;0
WireConnection;27;0;29;0
WireConnection;0;0;27;0
ASEEND*/
//CHKSM=F61F03725E8EF68B50E94FE8770222DA0541D3C5