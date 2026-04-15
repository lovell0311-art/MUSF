// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom1/Water2"
{
    Properties
    {
        _Surface ("Surface", 2D) = "white" {}
        _Noise("Surface", 2D) = "white" {}
        _frothRange("泡沫范围", Range(0,1)) = 1
    	_waterDirection("水: xz方向,y速度 ", vector) = (1,1,0.5,1)
    	_noiseDirection("noise: xz方向,y速度,w强度 ", vector) = (1,1,1,0.5)
    	
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "LightMode"="ForwardBase" }
       // Tags { "IgnoreProjector" = "True" "RenderType" = "Opaque" }
    	
        Pass
        {
        	Tags { "LightMode"="ForwardBase" }
        	ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha
        	
        	Name "Unlit"
            CGPROGRAM
            #pragma vertex vert
			#pragma fragment frag
			
			#include "Lighting.cginc"

            float4 _waterDirection;
            float4 _noiseDirection;
            
			float4 _frothColor;
            float _frothRange;
            sampler2D _Surface; float4 _Surface_ST;
            sampler2D _Noise; float4 _Noise_ST;
			UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
			uniform float4 _CameraDepthTexture_TexelSize;
            
            struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				
			};
			
			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uvTileWater : TEXCOORD0;
				float2 uvNoise : TEXCOORD1;
				float4 screenPos :TEXCOORD2;
			};

           
            v2f vert (appdata v)
            {
                v2f o;
            	o.pos = UnityObjectToClipPos(v.vertex);
            	o.uvTileWater = TRANSFORM_TEX(v.uv, _Surface);
                o.uvNoise = TRANSFORM_TEX(v.uv, _Noise);
            	float4 screenPos = ComputeScreenPos(o.pos);
				o.screenPos = screenPos;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 noise = tex2D(_Noise,i.uvNoise + _Time.x * _noiseDirection.xz * _noiseDirection.y);
                noise *= _noiseDirection.w;
                float4 surface = tex2D(_Surface,i.uvTileWater + noise + _Time.x * _waterDirection.xz * _waterDirection.y);
            	
            	float4 screenPos = i.screenPos;
				screenPos = screenPos / screenPos.w;
				screenPos.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? screenPos.z : screenPos.z * 0.5 + 0.5;
            	
				float screenDepth28 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, screenPos.xy));
				//float screenDepth28 = Linear01Depth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, screenPos.xy ));
				float distanceDepth28 = abs(screenDepth28 - LinearEyeDepth(screenPos.z));
            	distanceDepth28 = pow(distanceDepth28, 1 ) * _frothRange;
            	float3 color = surface;
                return float4(color, saturate(distanceDepth28)) ;
            	
			    //return float4(screenDepth28,screenDepth28,screenDepth28,1);
            	//return float4(surface.xyz,1);
            }
            ENDCG
        }
    }
}
