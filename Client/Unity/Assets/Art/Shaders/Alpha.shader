// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:597,x:32961,y:32820,varname:node_597,prsc:2|emission-1260-OUT,alpha-9797-OUT;n:type:ShaderForge.SFN_Tex2d,id:8250,x:32167,y:32803,ptovrint:False,ptlb:Main_tex,ptin:_Main_tex,varname:_Main_tex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-8487-OUT;n:type:ShaderForge.SFN_Multiply,id:1260,x:32478,y:32825,varname:node_1260,prsc:2|A-8250-RGB,B-806-RGB,C-4146-RGB;n:type:ShaderForge.SFN_Color,id:806,x:32167,y:33003,ptovrint:False,ptlb:Main_color,ptin:_Main_color,varname:_Main_color,prsc:2,glob:False,taghide:False,taghdr:True,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_VertexColor,id:4146,x:32037,y:33117,varname:node_4146,prsc:2;n:type:ShaderForge.SFN_Multiply,id:8419,x:32478,y:33122,varname:node_8419,prsc:2|A-6188-OUT,B-806-A,C-4146-A,D-3695-OUT;n:type:ShaderForge.SFN_DepthBlend,id:180,x:32213,y:33316,varname:node_180,prsc:2|DIST-3381-OUT;n:type:ShaderForge.SFN_Slider,id:3381,x:31832,y:33425,ptovrint:False,ptlb:DepthBlend_power,ptin:_DepthBlend_power,varname:_DepthBlend_power,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.1,max:1;n:type:ShaderForge.SFN_ValueProperty,id:674,x:31055,y:32334,ptovrint:False,ptlb:U_speed,ptin:_U_speed,varname:_U_speed,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Time,id:5028,x:31055,y:32429,varname:node_5028,prsc:2;n:type:ShaderForge.SFN_ValueProperty,id:1628,x:31055,y:32584,ptovrint:False,ptlb:V_speed,ptin:_V_speed,varname:_V_speed,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Multiply,id:2010,x:31249,y:32366,varname:node_2010,prsc:2|A-674-OUT,B-5028-T;n:type:ShaderForge.SFN_Multiply,id:7758,x:31268,y:32508,varname:node_7758,prsc:2|A-5028-T,B-1628-OUT;n:type:ShaderForge.SFN_Append,id:7600,x:31402,y:32417,varname:node_7600,prsc:2|A-2010-OUT,B-7758-OUT;n:type:ShaderForge.SFN_TexCoord,id:3970,x:31379,y:32660,varname:node_3970,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Add,id:1307,x:31605,y:32473,varname:node_1307,prsc:2|A-7600-OUT,B-3970-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:2709,x:32180,y:33634,ptovrint:False,ptlb:Tex_mask,ptin:_Tex_mask,varname:_Tex_mask,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:9797,x:32650,y:33233,varname:node_9797,prsc:2|A-8419-OUT,B-2709-R,C-7137-OUT,D-217-OUT;n:type:ShaderForge.SFN_SwitchProperty,id:3695,x:32324,y:33455,ptovrint:False,ptlb:DepthBlend,ptin:_DepthBlend,varname:_DepthBlend,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-1038-OUT,B-180-OUT;n:type:ShaderForge.SFN_Vector1,id:1038,x:32116,y:33455,varname:node_1038,prsc:2,v1:1;n:type:ShaderForge.SFN_Step,id:8486,x:32667,y:33953,varname:node_8486,prsc:2|A-2910-U,B-1974-R;n:type:ShaderForge.SFN_Tex2d,id:1974,x:32350,y:34021,ptovrint:False,ptlb:Dissove_tex,ptin:_Dissove_tex,varname:_node_1974,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_TexCoord,id:2910,x:32350,y:33831,varname:node_2910,prsc:2,uv:1,uaff:True;n:type:ShaderForge.SFN_SwitchProperty,id:7137,x:32851,y:33868,ptovrint:False,ptlb:Dissove,ptin:_Dissove,varname:node_7137,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-7085-OUT,B-8486-OUT;n:type:ShaderForge.SFN_Vector1,id:7085,x:32654,y:33832,varname:node_7085,prsc:2,v1:1;n:type:ShaderForge.SFN_SwitchProperty,id:6188,x:32478,y:32992,ptovrint:False,ptlb:Texture_Alpha,ptin:_Texture_Alpha,varname:node_6188,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-8250-A,B-8250-R;n:type:ShaderForge.SFN_TexCoord,id:785,x:31052,y:32829,varname:node_785,prsc:2,uv:1,uaff:True;n:type:ShaderForge.SFN_Append,id:723,x:31327,y:32887,varname:node_723,prsc:2|A-785-Z,B-785-W;n:type:ShaderForge.SFN_Add,id:9688,x:31601,y:32869,varname:node_9688,prsc:2|A-3970-UVOUT,B-723-OUT;n:type:ShaderForge.SFN_SwitchProperty,id:4270,x:31685,y:32707,ptovrint:False,ptlb:Once_uv,ptin:_Once_uv,varname:node_4270,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-1307-OUT,B-9688-OUT;n:type:ShaderForge.SFN_Step,id:8642,x:32554,y:33631,varname:node_8642,prsc:2|A-1974-R,B-4146-A;n:type:ShaderForge.SFN_Tex2d,id:3956,x:31121,y:33262,ptovrint:False,ptlb:noise_tex,ptin:_noise_tex,varname:node_3956,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:c1dedb91a621ad74da49b8033bdbd42d,ntxv:0,isnm:False;n:type:ShaderForge.SFN_SwitchProperty,id:8487,x:31873,y:32942,ptovrint:False,ptlb:Noise_on,ptin:_Noise_on,varname:node_8487,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-4270-OUT,B-6641-OUT;n:type:ShaderForge.SFN_Append,id:2287,x:31445,y:33127,varname:node_2287,prsc:2|A-2130-U,B-3956-G;n:type:ShaderForge.SFN_TexCoord,id:2130,x:31121,y:33098,varname:node_2130,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Lerp,id:6641,x:31688,y:33235,varname:node_6641,prsc:2|A-2287-OUT,B-2130-UVOUT,T-452-OUT;n:type:ShaderForge.SFN_SwitchProperty,id:217,x:32769,y:33431,ptovrint:False,ptlb:Dissove_alpha,ptin:_Dissove_alpha,varname:node_217,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-6111-OUT,B-8642-OUT;n:type:ShaderForge.SFN_Vector1,id:6111,x:32564,y:33465,varname:node_6111,prsc:2,v1:1;n:type:ShaderForge.SFN_RemapRange,id:452,x:31509,y:33439,varname:node_452,prsc:2,frmn:0,frmx:1,tomn:2,tomx:1|IN-4146-A;proporder:8250-806-6188-674-1628-2709-7137-1974-3695-3381-4270-3956-8487-217;pass:END;sub:END;*/

Shader "Sion/Alpha" {
    Properties {
        _Main_tex ("Main_tex", 2D) = "white" {}
        [HDR]_Main_color ("Main_color", Color) = (0.5,0.5,0.5,1)
        [MaterialToggle] _Texture_Alpha ("Texture_Alpha", Float ) = 0
        _U_speed ("U_speed", Float ) = 0
        _V_speed ("V_speed", Float ) = 0
        _Tex_mask ("Tex_mask", 2D) = "white" {}
        [MaterialToggle] _Dissove ("Dissove", Float ) = 1
        _Dissove_tex ("Dissove_tex", 2D) = "white" {}
        [MaterialToggle] _DepthBlend ("DepthBlend", Float ) = 1
        _DepthBlend_power ("DepthBlend_power", Range(0, 1)) = 0.1
        [MaterialToggle] _Once_uv ("Once_uv", Float ) = 0
        _noise_tex ("noise_tex", 2D) = "white" {}
        [MaterialToggle] _Noise_on ("Noise_on", Float ) = 0
        [MaterialToggle] _Dissove_alpha ("Dissove_alpha", Float ) = 1
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            //Tags {
            //    "LightMode"="ForwardBase"
            //}
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _CameraDepthTexture;
            uniform sampler2D _Main_tex; uniform float4 _Main_tex_ST;
            uniform float4 _Main_color;
            uniform float _DepthBlend_power;
            uniform float _U_speed;
            uniform float _V_speed;
            uniform sampler2D _Tex_mask; uniform float4 _Tex_mask_ST;
            uniform fixed _DepthBlend;
            uniform sampler2D _Dissove_tex; uniform float4 _Dissove_tex_ST;
            uniform fixed _Dissove;
            uniform fixed _Texture_Alpha;
            uniform fixed _Once_uv;
            uniform sampler2D _noise_tex; uniform float4 _noise_tex_ST;
            uniform fixed _Noise_on;
            uniform fixed _Dissove_alpha;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 texcoord1 : TEXCOORD1;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 uv1 : TEXCOORD1;
                float4 vertexColor : COLOR;
                float4 projPos : TEXCOORD2;
                UNITY_FOG_COORDS(3)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                float sceneZ = max(0,LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)))) - _ProjectionParams.g);
                float partZ = max(0,i.projPos.z - _ProjectionParams.g);
////// Lighting:
////// Emissive:
                float4 node_5028 = _Time;
                float4 _noise_tex_var = tex2D(_noise_tex,TRANSFORM_TEX(i.uv0, _noise_tex));
                float2 _Noise_on_var = lerp( lerp( (float2((_U_speed*node_5028.g),(node_5028.g*_V_speed))+i.uv0), (i.uv0+float2(i.uv1.b,i.uv1.a)), _Once_uv ), lerp(float2(i.uv0.r,_noise_tex_var.g),i.uv0,(i.vertexColor.a*-1.0+2.0)), _Noise_on );
                float4 _Main_tex_var = tex2D(_Main_tex,TRANSFORM_TEX(_Noise_on_var, _Main_tex));
                float3 emissive = (_Main_tex_var.rgb*_Main_color.rgb*i.vertexColor.rgb);
                float3 finalColor = emissive;
                float4 _Tex_mask_var = tex2D(_Tex_mask,TRANSFORM_TEX(i.uv0, _Tex_mask));
                float4 _Dissove_tex_var = tex2D(_Dissove_tex,TRANSFORM_TEX(i.uv0, _Dissove_tex));
                fixed4 finalRGBA = fixed4(finalColor,((lerp( _Main_tex_var.a, _Main_tex_var.r, _Texture_Alpha )*_Main_color.a*i.vertexColor.a*lerp( 1.0, saturate((sceneZ-partZ)/_DepthBlend_power), _DepthBlend ))*_Tex_mask_var.r*lerp( 1.0, step(i.uv1.r,_Dissove_tex_var.r), _Dissove )*lerp( 1.0, step(_Dissove_tex_var.r,i.vertexColor.a), _Dissove_alpha )));
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            struct VertexInput {
                float4 vertex : POSITION;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.pos = UnityObjectToClipPos( v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
