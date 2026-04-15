// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:3,bdst:0,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:33684,y:32443,varname:node_3138,prsc:2|emission-6763-OUT;n:type:ShaderForge.SFN_Color,id:7241,x:32687,y:32687,ptovrint:False,ptlb:Main_color,ptin:_Main_color,varname:node_7241,prsc:2,glob:False,taghide:False,taghdr:True,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Tex2d,id:4448,x:31968,y:32418,varname:node_4448,prsc:2,ntxv:0,isnm:False|UVIN-3006-OUT,TEX-1031-TEX;n:type:ShaderForge.SFN_Slider,id:9614,x:32609,y:32919,ptovrint:False,ptlb:Depth_power,ptin:_Depth_power,varname:node_9614,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_DepthBlend,id:5761,x:33036,y:32890,varname:node_5761,prsc:2|DIST-9614-OUT;n:type:ShaderForge.SFN_VertexColor,id:2758,x:32668,y:32299,varname:node_2758,prsc:2;n:type:ShaderForge.SFN_Multiply,id:2363,x:33062,y:32481,varname:node_2363,prsc:2|A-2758-RGB,B-3534-OUT,C-7241-RGB;n:type:ShaderForge.SFN_Multiply,id:5110,x:33127,y:32684,varname:node_5110,prsc:2|A-2758-A,B-4448-R,C-7241-A,D-5761-OUT,E-4448-A;n:type:ShaderForge.SFN_Multiply,id:6763,x:33312,y:32550,varname:node_6763,prsc:2|A-2363-OUT,B-5110-OUT,C-4016-R;n:type:ShaderForge.SFN_TexCoord,id:7842,x:30805,y:32629,varname:node_7842,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Time,id:9766,x:30805,y:32400,varname:node_9766,prsc:2;n:type:ShaderForge.SFN_Multiply,id:318,x:31042,y:32281,varname:node_318,prsc:2|A-9766-T,B-4552-OUT;n:type:ShaderForge.SFN_ValueProperty,id:4552,x:30805,y:32550,ptovrint:False,ptlb:U_speed,ptin:_U_speed,varname:node_4552,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Add,id:2154,x:31124,y:32602,varname:node_2154,prsc:2|A-318-OUT,B-7842-U;n:type:ShaderForge.SFN_Time,id:9671,x:30793,y:32786,varname:node_9671,prsc:2;n:type:ShaderForge.SFN_Multiply,id:2065,x:31037,y:32892,varname:node_2065,prsc:2|A-9671-T,B-4099-OUT;n:type:ShaderForge.SFN_ValueProperty,id:4099,x:30793,y:32952,ptovrint:False,ptlb:V_speed,ptin:_V_speed,varname:node_4099,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Add,id:4946,x:31127,y:32717,varname:node_4946,prsc:2|A-7842-V,B-2065-OUT;n:type:ShaderForge.SFN_Append,id:3006,x:31352,y:32512,varname:node_3006,prsc:2|A-2154-OUT,B-4946-OUT;n:type:ShaderForge.SFN_Tex2d,id:4016,x:32384,y:33228,ptovrint:False,ptlb:Tex_B,ptin:_Tex_B,varname:node_4016,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-6431-OUT;n:type:ShaderForge.SFN_TexCoord,id:3859,x:31632,y:33132,varname:node_3859,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Time,id:5926,x:31635,y:32988,varname:node_5926,prsc:2;n:type:ShaderForge.SFN_Multiply,id:5191,x:31811,y:32999,varname:node_5191,prsc:2|A-2942-OUT,B-5926-T;n:type:ShaderForge.SFN_ValueProperty,id:2942,x:31635,y:32929,ptovrint:False,ptlb:B_U_speed,ptin:_B_U_speed,varname:node_2942,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_ValueProperty,id:7307,x:31659,y:33595,ptovrint:False,ptlb:B_V_speed,ptin:_B_V_speed,varname:node_7307,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Time,id:9323,x:31659,y:33675,varname:node_9323,prsc:2;n:type:ShaderForge.SFN_Multiply,id:2824,x:31801,y:33485,varname:node_2824,prsc:2|A-7307-OUT,B-9323-T;n:type:ShaderForge.SFN_Add,id:9513,x:31897,y:33107,varname:node_9513,prsc:2|A-5191-OUT,B-3859-U;n:type:ShaderForge.SFN_Add,id:5736,x:31939,y:33338,varname:node_5736,prsc:2|A-3859-V,B-2824-OUT;n:type:ShaderForge.SFN_Append,id:6431,x:32118,y:33192,varname:node_6431,prsc:2|A-9513-OUT,B-5736-OUT;n:type:ShaderForge.SFN_Tex2dAsset,id:1031,x:30500,y:31826,ptovrint:False,ptlb:Main_tex,ptin:_Main_tex,varname:node_1031,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:3042,x:31649,y:31581,varname:node_3042,prsc:2,ntxv:0,isnm:False|UVIN-4521-OUT,TEX-1031-TEX;n:type:ShaderForge.SFN_Tex2d,id:9406,x:31493,y:32212,varname:node_9406,prsc:2,ntxv:0,isnm:False|UVIN-1250-OUT,TEX-1031-TEX;n:type:ShaderForge.SFN_Add,id:4521,x:31471,y:31704,varname:node_4521,prsc:2|A-3006-OUT,B-430-OUT;n:type:ShaderForge.SFN_Subtract,id:1250,x:31376,y:32051,varname:node_1250,prsc:2|A-3006-OUT,B-430-OUT;n:type:ShaderForge.SFN_Append,id:3534,x:32299,y:32269,varname:node_3534,prsc:2|A-3042-R,B-9406-B,C-4448-G;n:type:ShaderForge.SFN_ValueProperty,id:430,x:31067,y:31858,ptovrint:False,ptlb:Pianyi_power,ptin:_Pianyi_power,varname:node_430,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;proporder:1031-7241-9614-4552-4099-4016-2942-7307-430;pass:END;sub:END;*/

Shader "Sion/Add" {
    Properties {
        _Main_tex ("Main_tex", 2D) = "white" {}
        [HDR]_Main_color ("Main_color", Color) = (1,1,1,1)
        _Depth_power ("Depth_power", Range(0, 1)) = 0
        _U_speed ("U_speed", Float ) = 0
        _V_speed ("V_speed", Float ) = 0
        _Tex_B ("Tex_B", 2D) = "white" {}
        _B_U_speed ("B_U_speed", Float ) = 0
        _B_V_speed ("B_V_speed", Float ) = 0
        _Pianyi_power ("Pianyi_power", Float ) = 0
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha One
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _CameraDepthTexture;
            uniform float4 _Main_color;
            uniform float _Depth_power;
            uniform float _U_speed;
            uniform float _V_speed;
            uniform sampler2D _Tex_B; uniform float4 _Tex_B_ST;
            uniform float _B_U_speed;
            uniform float _B_V_speed;
            uniform sampler2D _Main_tex; uniform float4 _Main_tex_ST;
            uniform float _Pianyi_power;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
                float4 projPos : TEXCOORD1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos( v.vertex );
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
                float4 node_9766 = _Time;
                float4 node_9671 = _Time;
                float2 node_3006 = float2(((node_9766.g*_U_speed)+i.uv0.r),(i.uv0.g+(node_9671.g*_V_speed)));
                float2 node_4521 = (node_3006+_Pianyi_power);
                float4 node_3042 = tex2D(_Main_tex,TRANSFORM_TEX(node_4521, _Main_tex));
                float2 node_1250 = (node_3006-_Pianyi_power);
                float4 node_9406 = tex2D(_Main_tex,TRANSFORM_TEX(node_1250, _Main_tex));
                float4 node_4448 = tex2D(_Main_tex,TRANSFORM_TEX(node_3006, _Main_tex));
                float4 node_5926 = _Time;
                float4 node_9323 = _Time;
                float2 node_6431 = float2(((_B_U_speed*node_5926.g)+i.uv0.r),(i.uv0.g+(_B_V_speed*node_9323.g)));
                float4 _Tex_B_var = tex2D(_Tex_B,TRANSFORM_TEX(node_6431, _Tex_B));
                float3 emissive = ((i.vertexColor.rgb*float3(node_3042.r,node_9406.b,node_4448.g)*_Main_color.rgb)*(i.vertexColor.a*node_4448.r*_Main_color.a*saturate((sceneZ-partZ)/_Depth_power)*node_4448.a)*_Tex_B_var.r);
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
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
