Shader "Custom/UIOutline" {
	Properties{
		_MainTex("Sprite Texture", 2D) = "white"{}
		_LightColor("LightColor", Color) = (0.498,0.482,0.168,1)
		_Intensity("Intensity", Range(1, 20)) = 7
		_FlashRate("FlashRate", Range(1, 20)) = 10
	}
		SubShader{
			Tags {
				"Queue" = "Transparent"
				"RenderType" = "Transparent"
				"IgnoreProjector" = "True"
			}

			Cull Off
			Lighting Off
			ZWrite Off
			ZTest Off
			Blend SrcAlpha OneMinusSrcAlpha

			pass {
				CGPROGRAM

				#pragma vertex vert 
				#pragma fragment frag 

				#include "UnityCG.cginc"

				sampler2D _MainTex;
				float2 _MainTex_TexelSize;
				fixed4 _LightColor;
				float4 _MainTex_ST;
				float _Intensity;
				float _FlashRate;

				
				struct v2f {
					float4 vertex : POSITION;
					float2 uv[5]: TEXCOORD0;
				};

				v2f vert(appdata_base v) {
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					half2 uv = v.texcoord.xy;
					
					half _BlurSize = 1;
					o.uv[0] = uv;
					o.uv[1] = uv + float2(0, _MainTex_TexelSize.y * 1.0) * _BlurSize;
					o.uv[2] = uv - float2(0, _MainTex_TexelSize.y * 1.0) * _BlurSize;
					o.uv[3] = uv + float2(0, _MainTex_TexelSize.y * 2.0) * _BlurSize;
					o.uv[4] = uv - float2(0, _MainTex_TexelSize.y * 2.0) * _BlurSize;
					
					return o;
				}

				fixed4 frag(v2f i) : COLOR {
					float weight[3] = {0.4026,0.2442,0.0545};
					
					fixed4 mainTex =  tex2D(_MainTex, i.uv[0]);
					
					fixed sum = mainTex.a * weight[0];
					
					for(int it=1;it<3;it++)
					{
						sum += tex2D(_MainTex, i.uv[it*2-1]).a * weight[it];
						sum += tex2D(_MainTex, i.uv[it*2]).a * weight[it];
					}

					float4 lightColor = _LightColor * (sin(_Time.y * _FlashRate) + 1.0) *  _Intensity;
					
					
					return lerp(mainTex, lightColor * (1-mainTex.a) + mainTex, sum);
					
				}

				ENDCG
			}
		}
}