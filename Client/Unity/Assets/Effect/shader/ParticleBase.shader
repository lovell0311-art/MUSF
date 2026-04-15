Shader "Shader Forge/ParticleBase" {
    Properties {
        _texMain ("texMain", 2D) = "white" {}
        [HDR]_MainColor ("MainColor", Color) = (0.5019608,0.5019608,0.5019608,1)
        _texFlow ("texFlow", 2D) = "white" {}
        [HDR]_FlowColor ("FlowColor", Color) = (0,0,0,1)
        [MaterialToggle] _Flow_Mask ("Flow_Mask", Float ) = 0
        _texMask ("texMask", 2D) = "white" {}
        _texNoise ("texNoise", 2D) = "white" {}
        _UVmain_dist_ange ("UVmain_dist_ange", Vector) = (0,0,0,0)
        _UVflow_dist_alADD ("UVflow_dist_alADD", Vector) = (0,0,0,0)
        _MinMax_ange_Scale ("MinMax_ange_Scale", Vector) = (0,0,0,1)
        _mkR_dist_Min_Max ("mkR_dist_Min_Max", Vector) = (0,0,-0.5,0.5)
        _UVnoise_rSpd_Scale ("UVnoise_rSpd_Scale", Vector) = (0,0,0,1)
        [HDR]_dissColor ("dissColor", Color) = (1,0.4316475,0,0)
        [Toggle(_USESOFTDIS_ON)] _useSoftDis("useSoftDis",Float) = 0
        _softDiss("softDiss",Range(0,1)) = 1
        _diss ("diss", Range(0, 2)) = 0
        _dissOutLine ("dissOutLine", Range(0, 1)) = 0
        [HDR]_FresnelColor ("FresnelColor", Color) = (0,0,0,1)
        _FresnelExp ("FresnelExp", Float ) = 1
        [MaterialToggle] _Ns_Mk ("Ns_Mk", Float ) = 0
        [MaterialToggle] _polar ("polar", Float ) = 0
        _pointX ("pointX", Float ) = 0
        _pointY ("pointY", Float ) = 0
        [MaterialToggle] _par_diss ("par_diss", Float ) = 0
        [MaterialToggle] _UV1 ("UV1", Float ) = 0
        [MaterialToggle] _Main_uv0 ("Main_uv0", Float ) = 0
        [MaterialToggle] _Mask_uv0 ("Mask_uv0", Float ) = 0
        [MaterialToggle] _Noise_polar ("Noise_polar", Float ) = 0
        [Toggle(_USEWAVE_ON)] _useWave ("useWave", Float ) = 0
        _WaveLength ("WaveLength", Float ) = 10
        _wave ("wave", Float ) = 0
        _WaveSpeed ("WaveSpeed", Float ) = 0
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
        
        [Toggle(_USECUSTOMDATA_ON)] _useCustomData("useCustomData",Float) = 0

        [Header(Rendering)]
        [Enum(UnityEngine.Rendering.CullMode)]_Cull("Cull",Float) = 2
        [Enum(On,1,Off,0)]_ZWrite("Zwrite", Float) = 0
        [Enum(UnityEngine.Rendering.BlendMode)]_SrcBlend("SrcBlend",Float) = 5
        [Enum(UnityEngine.Rendering.BlendMode)]_DstBlend("DstBlend",Float) = 10
        //[Toggle(_BLENDOFF_ON)]  _BlendOff("BlendOff", Float ) = 0
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
            Blend [_SrcBlend][_DstBlend]
            ZWrite [_ZWrite]
            Cull [_Cull]
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma target 3.0
            uniform sampler2D _texMain; uniform float4 _texMain_ST;
            uniform sampler2D _texFlow; uniform float4 _texFlow_ST;
            uniform sampler2D _texNoise; uniform float4 _texNoise_ST;
            uniform sampler2D _texMask; uniform float4 _texMask_ST;
            UNITY_INSTANCING_BUFFER_START( Props )
                UNITY_DEFINE_INSTANCED_PROP( half4, _MainColor)
                UNITY_DEFINE_INSTANCED_PROP( half4, _FlowColor)
                UNITY_DEFINE_INSTANCED_PROP( half, _diss)
                UNITY_DEFINE_INSTANCED_PROP( half, _dissOutLine)
                UNITY_DEFINE_INSTANCED_PROP( half4, _dissColor)
                UNITY_DEFINE_INSTANCED_PROP( half4, _UVflow_dist_alADD)
                UNITY_DEFINE_INSTANCED_PROP( half4, _UVmain_dist_ange)
                UNITY_DEFINE_INSTANCED_PROP( half4, _FresnelColor)
                UNITY_DEFINE_INSTANCED_PROP( half, _Ns_Mk)
                UNITY_DEFINE_INSTANCED_PROP( half, _FresnelExp)
                UNITY_DEFINE_INSTANCED_PROP( half, _polar)
                UNITY_DEFINE_INSTANCED_PROP( half, _par_diss)
                UNITY_DEFINE_INSTANCED_PROP( half, _UV1)
                UNITY_DEFINE_INSTANCED_PROP( half, _Main_uv0)
                UNITY_DEFINE_INSTANCED_PROP( half, _Mask_uv0)
                UNITY_DEFINE_INSTANCED_PROP( half, _Noise_polar)
                UNITY_DEFINE_INSTANCED_PROP( half4, _UVnoise_rSpd_Scale)
                UNITY_DEFINE_INSTANCED_PROP( half4, _MinMax_ange_Scale)
                UNITY_DEFINE_INSTANCED_PROP( half4, _mkR_dist_Min_Max)
                UNITY_DEFINE_INSTANCED_PROP( half, _Flow_Mask)
                UNITY_DEFINE_INSTANCED_PROP( half, _pointX)
                UNITY_DEFINE_INSTANCED_PROP( half, _pointY)
                UNITY_DEFINE_INSTANCED_PROP( half, _WaveLength)
                UNITY_DEFINE_INSTANCED_PROP( half, _wave)
                UNITY_DEFINE_INSTANCED_PROP( half, _WaveSpeed)
                //UNITY_DEFINE_INSTANCED_PROP( half, _useWave)
                UNITY_DEFINE_INSTANCED_PROP( half, _softDiss)
            UNITY_INSTANCING_BUFFER_END( Props )

            #pragma skip_variants DIRECTIONAL SHADOWS_SHADOWMASK DYNAMICLIGHTMAP_ON LIGHTMAP_SHADOW_MIXING DIRLIGHTMAP_COMBINED SHADOWS_SCREEN VERTEXLIGHT_ON LIGHTMAP_ON LIGHTPROBE_SH
            #pragma multi_compile __ _USEWAVE_ON
            #pragma multi_compile __ _USESOFTDIS_ON
            #pragma multi_compile __ _USECUSFORDIS_ON
            #pragma multi_compile __ _USECUSTOMDATA_ON

            struct VertexInput {
                UNITY_VERTEX_INPUT_INSTANCE_ID
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 texcoord0 : TEXCOORD0;
                float4 texcoord1 : TEXCOORD1;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                float4 uv0 : TEXCOORD0;
                float4 uv1 : TEXCOORD1;
                float4 posWorld : TEXCOORD2;
                float3 normalDir : TEXCOORD3;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                UNITY_SETUP_INSTANCE_ID( v );
                UNITY_TRANSFER_INSTANCE_ID( v, o );
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                UNITY_SETUP_INSTANCE_ID( i );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                fixed3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                fixed3 normalDirection = i.normalDir;
                half _diss_var = UNITY_ACCESS_INSTANCED_PROP( Props, _diss );
#ifdef _USECUSTOMDATA_ON
                half _par_diss_var = lerp( _diss_var * i.uv0.z, ((1.0 - i.vertexColor.a)*_diss_var) * i.uv0.z, UNITY_ACCESS_INSTANCED_PROP( Props, _par_diss ) );
#else
                half _par_diss_var = lerp( _diss_var, ((1.0 - i.vertexColor.a)*_diss_var), UNITY_ACCESS_INSTANCED_PROP( Props, _par_diss ) );
#endif
                half _dissOutLine_var = UNITY_ACCESS_INSTANCED_PROP( Props, _dissOutLine );
                half fDissVar = (_par_diss_var-_dissOutLine_var);
                
                float2 _UV1_var = lerp( i.uv0.xy, i.uv1.xy, UNITY_ACCESS_INSTANCED_PROP( Props, _UV1 ) );

                half _pointX_var = UNITY_ACCESS_INSTANCED_PROP( Props, _pointX );
                half _pointY_var = UNITY_ACCESS_INSTANCED_PROP( Props, _pointY );
                float2 point_uv1 = (float2(_pointX_var,_pointY_var)+_UV1_var)*2.0-1.0;
                float2 _polar_var = lerp( _UV1_var, float2(length(point_uv1),((atan2(point_uv1.r,point_uv1.g)/6.28)+0.5)), UNITY_ACCESS_INSTANCED_PROP( Props, _polar ) );
                
                half4 _UVnoise_rSpd_Scale_var = UNITY_ACCESS_INSTANCED_PROP( Props, _UVnoise_rSpd_Scale );
                float b_ns = _UVnoise_rSpd_Scale_var.b * _Time.y;
                float b_ns_cos = cos(b_ns);
                float b_ns_sin = sin(b_ns);
                float2 mul_noisetex_uv = mul(_UVnoise_rSpd_Scale_var.rg * _Time.y + _polar_var - fixed2(0.5,0.5),float2x2( b_ns_cos, -b_ns_sin, b_ns_sin, b_ns_cos))+fixed2(0.5,0.5);
                half2 a_ns_rcp = fixed2(1.0,1.0)/half2( _UVnoise_rSpd_Scale_var.a, _UVnoise_rSpd_Scale_var.a );
                float4 _texNoise_var = tex2D( _texNoise , TRANSFORM_TEX(( mul_noisetex_uv * a_ns_rcp - (( 0.5 / _UVnoise_rSpd_Scale_var.a ) - 0.5 )), _texNoise ));
                
                half4 _mkR_dist_Min_Max_var = UNITY_ACCESS_INSTANCED_PROP( Props, _mkR_dist_Min_Max );
                float r_mk_dist = _mkR_dist_Min_Max_var.r*0.005555556*3.141592654;
                float r_mk_dist_cos = cos(r_mk_dist);
                float r_mk_dist_sin = sin(r_mk_dist);
                float2 _Mask_uv0_var = lerp( _polar_var, i.uv0.xy, UNITY_ACCESS_INSTANCED_PROP( Props, _Mask_uv0 ) );
                float2 mul_mask_uv = (mul(_Mask_uv0_var-fixed2(0.5,0.5),float2x2( r_mk_dist_cos, -r_mk_dist_sin, r_mk_dist_sin, r_mk_dist_cos))+fixed2(0.5,0.5));
                float4 _texMask_var = tex2D(_texMask,TRANSFORM_TEX((( _texNoise_var.rg * _mkR_dist_Min_Max_var.g ) + mul_mask_uv ), _texMask));

                float _Ns_Mk_var = lerp( _texNoise_var.r, _texMask_var.r, UNITY_ACCESS_INSTANCED_PROP( Props, _Ns_Mk ) );

                float fd_nm = step(fDissVar,_Ns_Mk_var);

#ifdef _USESOFTDIS_ON
                float _softDiss_var = UNITY_ACCESS_INSTANCED_PROP(Props,_softDiss);
                float fdis = saturate(_Ns_Mk_var-fDissVar+fd_nm*(1-_softDiss_var)+_softDiss_var);
#else
                float fdis = 1;
                clip(fd_nm - 0.5);
#endif
                

                half4 _UVmain_dist_ange_var = UNITY_ACCESS_INSTANCED_PROP( Props, _UVmain_dist_ange );
                half a_m_dist = _UVmain_dist_ange_var.a*0.005555556*3.141592654;
                float a_m_dist_cos = cos(a_m_dist);
                float a_m_dist_sin = sin(a_m_dist);
                float2 _Noise_polar_var = lerp( _polar_var, _UV1_var, UNITY_ACCESS_INSTANCED_PROP( Props, _Noise_polar ) );
                float2 _Main_uv0_var = lerp( _Noise_polar_var, i.uv0.xy, UNITY_ACCESS_INSTANCED_PROP( Props, _Main_uv0 ) );
                
                half4 _MinMax_ange_Scale_var = UNITY_ACCESS_INSTANCED_PROP( Props, _MinMax_ange_Scale );
                float b_mmas = _MinMax_ange_Scale_var.b*0.005555556*3.141592654;
                float b_mmas_cos = cos(b_mmas);
                float b_mmas_sin = sin(b_mmas);
                half4 _UVflow_dist_alADD_var = UNITY_ACCESS_INSTANCED_PROP( Props, _UVflow_dist_alADD );
                float2 mul_flowtex_uv = (mul(((_UV1_var-0.5)*(_texNoise_var.rg*_UVflow_dist_alADD_var.b) + _UVflow_dist_alADD_var.rg*_Time.y + _Noise_polar_var)-fixed2(0.5,0.5),float2x2( b_mmas_cos, -b_mmas_sin, b_mmas_sin, b_mmas_cos))+fixed2(0.5,0.5));
                half2 a_mmas_rcp = fixed2(1.0,1.0)/half2( _MinMax_ange_Scale_var.a, _MinMax_ange_Scale_var.a );
                float4 _texFlow_var = tex2D(_texFlow,TRANSFORM_TEX((mul_flowtex_uv * a_mmas_rcp-((0.5/_MinMax_ange_Scale_var.a)-0.5)), _texFlow));
                
                half4 _FresnelColor_var = UNITY_ACCESS_INSTANCED_PROP( Props, _FresnelColor );
                half4 _dissColor_var = UNITY_ACCESS_INSTANCED_PROP( Props, _dissColor );
                half4 _MainColor_var = UNITY_ACCESS_INSTANCED_PROP( Props, _MainColor );
                half4 _FlowColor_var = UNITY_ACCESS_INSTANCED_PROP( Props, _FlowColor );
                float pd_nm = step(_par_diss_var,_Ns_Mk_var);
                float diss_var_s = saturate(step(fDissVar*1.2,_Ns_Mk_var)-pd_nm);
                float fresnelexp = pow(1.0-max(0,dot(normalDirection, viewDirection)),UNITY_ACCESS_INSTANCED_PROP( Props, _FresnelExp ));
                
                

#ifdef _USEWAVE_ON
                half _WaveSpeed_var = UNITY_ACCESS_INSTANCED_PROP( Props, _WaveSpeed );
                half _WaveLength_var = UNITY_ACCESS_INSTANCED_PROP( Props, _WaveLength );
                half _wave_var = UNITY_ACCESS_INSTANCED_PROP( Props, _wave );
                float2 main_uv = sin((_WaveSpeed_var * _Time.y) + (_WaveLength_var * i.uv0.x)) * _wave_var + ( i.uv0.xy * (_texNoise_var.rg * _UVmain_dist_ange_var.b + 1) );
#else
                float2 main_uv = (mul(((i.uv0.xy-0.5)*(_texNoise_var.rg*_UVmain_dist_ange_var.b) + _UVmain_dist_ange_var.rg*_Time.y + _Main_uv0_var)-fixed2(0.5,0.5),float2x2( a_m_dist_cos, -a_m_dist_sin, a_m_dist_sin, a_m_dist_cos))+fixed2(0.5,0.5));
#endif
                float4 _texMain_var = tex2D(_texMain,TRANSFORM_TEX(main_uv, _texMain));
                
                
                float fd_pd_nm = saturate(fd_nm-pd_nm);
                float3 finalColor = (i.vertexColor.rgb * (( _texMain_var.rgb * _MainColor_var.rgb ) + (( _texFlow_var.rgb * _FlowColor_var.rgb * _FlowColor_var.a * saturate(( -1.0 + ((( _texFlow_var.r * _texFlow_var.a ) - _MinMax_ange_Scale_var.r ) * 2 ) / ( _MinMax_ange_Scale_var.g - _MinMax_ange_Scale_var.r )))) * _texFlow_var.a) + (( fd_pd_nm * _dissColor_var.rgb ) + ( diss_var_s * _dissColor_var.rgb )) + ( fresnelexp * _FresnelColor_var.rgb )));
                float a_fm = ((_texMain_var.a*_MainColor_var.a)+(_texFlow_var.r*_UVflow_dist_alADD_var.a*_texFlow_var.a));
                float _Flow_Mask_var = lerp( a_fm, (a_fm*_texFlow_var.a), UNITY_ACCESS_INSTANCED_PROP( Props, _Flow_Mask ) );

                float a_diss = saturate(i.vertexColor.a+_dissColor_var.a);
                
                return half4(finalColor*fdis,fdis*((((-1.0 + ( (_Flow_Mask_var*_texMask_var.r - _mkR_dist_Min_Max_var.b) * 2 ) / (_mkR_dist_Min_Max_var.a - _mkR_dist_Min_Max_var.b))*a_diss)+((fd_pd_nm+diss_var_s)*_Flow_Mask_var*_texMask_var.r*a_diss))*saturate((fresnelexp+_FresnelColor_var.a))));
            }
            ENDCG
        }
    }
}
