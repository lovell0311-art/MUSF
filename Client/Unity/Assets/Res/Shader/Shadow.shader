
Shader "NKGMoba/CustomShadow"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        Shadow_Color("“ı”∞—’…´", Color) = (1,0,0,1)
        DirX("DirectionX", Range(-1, 1)) = 0.5
        DirZ("DirectionZ", Range(-1, 1)) = 0.5
        Fade_Factor("Fade_Factor", Range(0, 1)) = 0.5
        Ground_Height("Ground_Height", Range(0, 5)) = 0
        [Toggle(_EnableFade)]_Toggle("FadeByDistance", int) = 0
        Distanec_Fade_Factor("Distanec_Fade_Factor", Range(0, 1)) = 0.5
    }    
    SubShader
    {
        Tags { "RenderType"="Opaque" }
	Pass
        {
            Stencil
            {
            	Ref 0
            	Comp equal
            	pass IncrSat
            }
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #include "UnityCG.cginc"
            float Ground_Height;
            float DirX;
            float DirZ;
            float Fade_Factor;
            float Distanec_Fade_Factor;
            float4 _Color;
            float4 Shadow_Color;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            #pragma multi_compile _DisableFade _EnableFade
            #pragma vertex vert
            #pragma fragment frag
            // #include "UuShadow.cginc"
            
            struct a2v
            {
                float4 pos : SV_POSITION;
                float3 normal : NORMAL;
            };
            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 _color : TEXCOORD0;
            };
            v2f vert(a2v v)
            {
                v2f o;
                
                float3 worldPos = mul(unity_ObjectToWorld , v.pos).xyz;

                worldPos.x = worldPos.x - DirX * max(Ground_Height, worldPos.y);
                worldPos.z = worldPos.z - DirZ * max(Ground_Height, worldPos.y);
                worldPos.y = Ground_Height;
                o.pos = UnityWorldToClipPos(worldPos);
                float factor = 1 - Fade_Factor;

                #if _EnableFade
                {
                    // ignore y pos
                    float3 modelPos = float3(unity_ObjectToWorld[2].x , 0, unity_ObjectToWorld[2].w );
                    float fade = 1 - saturate(distance(modelPos, worldPos)) * Distanec_Fade_Factor;
                    factor = factor * fade;
                }
                #endif
                o._color = Shadow_Color;
                o._color.a *= factor;
                

                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                return fixed4(i._color);
            }


            ENDCG  
        }
    }
}                
