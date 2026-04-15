// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "UI/UIMask" {
    Properties  
        {  
            [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}  
            _Color ("Tint", Color) = (1,1,1,1)  
              
            _StencilComp ("Stencil Comparison", Float) = 8  
            _Stencil ("Stencil ID", Float) = 0  
            _StencilOp ("Stencil Operation", Float) = 0  
            _StencilWriteMask ("Stencil Write Mask", Float) = 255  
            _StencilReadMask ("Stencil Read Mask", Float) = 255  
      
            _ColorMask ("Color Mask", Float) = 15  
      
            [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0  
      
            //[HideInInspector]  
            _MaskCull ("Mask Cull", Vector) = (0,0,100,200)  
            _bNeedSet ("Need Mask Cull", Int) = 0  
        }  
      
        SubShader  
        {  
            Tags  
            {   
                "Queue"="Transparent"   
                "IgnoreProjector"="True"   
                "RenderType"="Transparent"   
                "PreviewType"="Plane"  
                "CanUseSpriteAtlas"="True"  
            }  
              
            Stencil  
            {  
                Ref [_Stencil]  
                Comp [_StencilComp]  
                Pass [_StencilOp]   
                ReadMask [_StencilReadMask]  
                WriteMask [_StencilWriteMask]  
            }  
      
            Cull Off  
            Lighting Off  
            ZWrite Off  
            ZTest [unity_GUIZTestMode]  
            Blend SrcAlpha OneMinusSrcAlpha  
            ColorMask [_ColorMask]  
      
            Pass  
            {  
                CGPROGRAM  
                #pragma vertex vert  
                #pragma fragment frag  
      
                #include "UnityCG.cginc"  
                #include "UnityUI.cginc"  
      
                #pragma multi_compile __ UNITY_UI_ALPHACLIP  
                  
                struct appdata_t  
                {  
                    float4 vertex   : POSITION;  
                    float4 color    : COLOR;  
                    float2 texcoord : TEXCOORD0;  
                };  
      
                struct v2f  
                {  
                    float4 vertex   : SV_POSITION;  
                    fixed4 color    : COLOR;  
                    half2 texcoord  : TEXCOORD0;  
                    float4 worldPosition : TEXCOORD1;  
                    //-------------------add----------------------  
                    float2 vScenePos : TEXCOORD2;  
                    //-------------------add----------------------  
                };  
                  
                fixed4 _Color;  
                fixed4 _TextureSampleAdd;  
                float4 _ClipRect;  
                //-------------------add----------------------  
                float4 _MaskCull;  
                int _bNeedSet;  
                //-------------------add----------------------  
                v2f vert(appdata_t IN)  
                {  
                    v2f OUT;  
                    OUT.worldPosition = IN.vertex;  
                    OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);  
      
                    OUT.texcoord = IN.texcoord;  
                      
                    #ifdef UNITY_HALF_TEXEL_OFFSET  
                    OUT.vertex.xy += (_ScreenParams.zw-1.0)*float2(-1,1);  
                    #endif  
                    if (_bNeedSet > 0)  
                    {     
                        //模拟硬件除W的过程，转换到NDC空间  
                        OUT.vScenePos = OUT.vertex / OUT.vertex.w;  
                        //NDC空间转换到屏幕空间，得到实际屏幕上位置的点  
                        OUT.vScenePos  = (OUT.vScenePos*_ScreenParams.xy + _ScreenParams.xy)/2;  
                    }  
                    OUT.color = IN.color * _Color;  
                    return OUT;  
                }  
      
                sampler2D _MainTex;  
      
                fixed4 frag(v2f IN) : SV_Target  
                {  
                    half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;  
                    float Alpha = 1;  
                    if (_bNeedSet > 0)  
                    {  
                        Alpha *= (IN.vScenePos.x >= _MaskCull.x);
                        Alpha *= (IN.vScenePos.x <= _MaskCull.z);
                        Alpha *= (IN.vScenePos.y >= _MaskCull.y);
                        Alpha *= (IN.vScenePos.y <= _MaskCull.w);
                    }  
                    color.a *= (1-Alpha);  
                      
                    color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);  
                      
                      
                      
                    #ifdef UNITY_UI_ALPHACLIP  
                    clip (color.a - 0.001);  
                    #endif  
                    return color;  
                }  
                ENDCG  
            }  
        }  
    }  
