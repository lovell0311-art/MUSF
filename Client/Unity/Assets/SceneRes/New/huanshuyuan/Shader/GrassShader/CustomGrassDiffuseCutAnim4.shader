// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "/Custom/Valkyrie/Toon/Scene/Other-GrassDiffuseCutAnim4"
{
	Properties
	{
		_MainTex("Main Tex", 2D) = "white" {}
		_AlphaCutoff("Alpha Cut Off", Float) = 0.9
		_Color("Main Color", Color) = (1,1,1,1)
		_LightPower("Power of Light", Float) = 1.0
		//xz表示振幅,y频率,w表示相速
		_WaveControl("WaveControl(x:XSpeed y:Hz z:ZSpeed w:PV)",vector) = (0.2,0.002,0.1,40)
		_Noise("Noise", 2D) = "white" {}
	}

	SubShader
	{
		//Tags { "IgnoreProjector" = "True" "RenderType" = "Opaque"}
		//Tags { "RenderType"="Opaque" "Queue"="Geometry"}

		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		LOD 100
		Pass
		{
			//Tags{"LightMode"="ForwardBase"}
			//ZWrite		On
			ZWrite		Off
			//Blend SrcAlpha OneMinusSrcAlpha
			Cull		Off
			Lighting 	Off

			CGPROGRAM
			#pragma vertex			vert
			#pragma fragment		frag
			#pragma multi_compile_fwdbase
			
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"

			sampler2D		_MainTex;
			half4			_MainTex_ST;
			half			_AlphaCutoff;

			half4			_Color;
			half			_LightPower;


			half4	_WaveControl;
			sampler2D _Noise;
			struct v2f
			{
				float4	pos		: POSITION;
				float2	uv		: TEXCOORD0;
				float3 positionWS	:TEXCOORD1;
			};
			
			v2f vert(appdata_full v)
			{
				v2f o;
				float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.positionWS = worldPos;
				
                //worldPos在_WindControl方向上与x轴相交点在一定范围内则一起摇曳
			//	worldPos
				
				float4 offset_WorldPos = worldPos;
				//绕Y旋转50度
				//float2x2 rotating = {
				//0.64279 ,-0.76604,
				//0.76604,0.64279
				//};
				//offset_WorldPos.xz = mul(rotating, offset_WorldPos.xz);

				//offset_WorldPos.xz += noise;
				//坐标影响sin
				//float factor = (offset_WorldPos.x+offset_WorldPos.z) * _WaveControl.w;

				//float percentage = offset_WorldPos.x/
				
				float factor = offset_WorldPos.x * _WaveControl.y + _WaveControl.w;
				float2 wave = sin(factor * _Time.x - offset_WorldPos.z) * _WaveControl.xz;
				offset_WorldPos.xz += wave;
				offset_WorldPos.x += 1;
				
            
				worldPos = lerp(worldPos, offset_WorldPos, pow(v.texcoord.y, 2));
			
				
				o.pos = UnityWorldToClipPos(worldPos);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);


				
				return o;
			}


			float4 frag(v2f i) : SV_Target
			{
				
				float noise = tex2D(_Noise, i.positionWS.xz*0.01).r;
				noise+=0.5;
				noise = pow(noise, 3);
				noise = clamp(noise, 0.1, 1);

				float excessive = pow(i.uv.y, 5) ;
				//lerp(noise, 1, i.uv.y);
				
				noise*=1.5;
				
				//;
				
				//noise+=0.1;
				//
				
				half4 fCol = tex2D(_MainTex, i.uv.xy);
		
				half4 c;
				c.rgb = (fCol.rgb - 0.001) * _Color * _LightPower * noise;
		
				
				c.rgb = lerp(c.rgb, fCol.rgb,  excessive);
				
				
				c.a = fCol.a;

				if (c.a < _AlphaCutoff)
				{
					discard;
				}
				c.a = 1;
				//UNITY_OPAQUE_ALPHA(c.a);
				

				
				//return float4(noise,noise,noise,1);
				return c;
			}


			
			ENDCG
		}
		
		//Pass { 
		//	Tags { "LightMode"="ForwardAdd" }
			
		//	Blend One One
			
			
		//	CGPROGRAM
		//	#pragma vertex			vert
		//	#pragma fragment		frag
		//	#pragma multi_compile_fwdadd
			
		//	#include "Lighting.cginc"
		//	#include "AutoLight.cginc"

		//	sampler2D		_MainTex;
		//	half4			_MainTex_ST;
		//	half			_AlphaCutoff;

		//	half4			_Color;
		//	half			_LightPower;


		//	half4	_WaveControl;
		//	sampler2D _Noise;
			
		//	struct v2f
		//	{
		//		float4	pos		: POSITION;
		//		float2	uv		: TEXCOORD0;
		//		float3 positionWS	:TEXCOORD1;
		//	};
			
		//	v2f vert(appdata_full v)
		//	{
		//		v2f o;
		//		float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
		//		o.positionWS = worldPos;
				
  //              //worldPos在_WindControl方向上与x轴相交点在一定范围内则一起摇曳
		//	//	worldPos
				
		//		float4 offset_WorldPos = worldPos;
		//		//offset_WorldPos.xz += noise;
		//		//坐标影响sin
		//		//float factor = (offset_WorldPos.x+offset_WorldPos.z) * _WaveControl.w;

		//		//float percentage = offset_WorldPos.x/
				
		//		float factor = offset_WorldPos.x * _WaveControl.y + _WaveControl.w;
		//		float2 wave = sin(factor * _Time.x - offset_WorldPos.z) * _WaveControl.xz;
		//		offset_WorldPos.xz += wave;
		//		offset_WorldPos.x += 1;
				
            
		//		worldPos = lerp(worldPos, offset_WorldPos, pow(v.texcoord.y, 2));
			
				
		//		o.pos = UnityWorldToClipPos(worldPos);
		//		o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);


				
		//		return o;
		//	}


		//	float4 frag(v2f i) : SV_Target
		//	{
				
		//		float noise = tex2D(_Noise, i.positionWS.xz*0.01).r;
		//		noise+=0.5;
		//		noise = pow(noise, 3);
		//		noise = clamp(noise, 0.1, 1);

		//		float excessive = pow(i.uv.y,5) ;
		//		//lerp(noise, 1, i.uv.y);
				
		//		noise*=1.5;
				
		//		//;
				
		//		//noise+=0.1;
		//		//
				
				
		//		half4 fCol = tex2D(_MainTex, i.uv.xy);
		
		//		half4 c;
		//		c.rgb = (fCol.rgb - 0.001) * _Color * _LightPower * noise;
		
				
		//		c.rgb = lerp(c.rgb, fCol.rgb,  excessive);
				
			
					
				
		//		c.a = fCol.a;

		//		if (c.a < _AlphaCutoff)
		//		{
		//			discard;
		//		}
		//		c.a = 1;
		//		//UNITY_OPAQUE_ALPHA(c.a);

				
		//	 	fixed3 diffuse = _LightColor0.rgb * c.rgb;
				
		//		UNITY_LIGHT_ATTENUATION(atten, 0, i.positionWS);

		//		//return atten;
		//		return fixed4(diffuse * atten, 1.0);


				
		//		//return float4(noise,noise,noise,1);
		//		//return c;
		//	}
			
		//	ENDCG
		//}
	}
	Fallback "Diffuse"
}
