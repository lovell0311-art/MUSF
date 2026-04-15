
Shader "DCG/Water Shader/Ocean - Planar Reflection" {
    Properties {
        _Normal ("Normal", 2D) = "bump" {}
        _WaterColor ("Water Color", Color) = (0.2941176,1,1,1)
        _Density ("Density", Float ) = 1
        _FadeLevel ("Fade Level", Float ) = 4
        _Specularity ("Specularity", Range(0, 1)) = 0
        _Gloss ("Gloss", Range(0, 1)) = 0.8988418
        _Fresnel ("Fresnel", Float ) = 3
        _Scale ("Scale", Float ) = 1
        _NormalIntensity ("Normal Intensity", Range(0, 2)) = 2
        _WaterSpeed ("Water Speed", Float ) = 1
        _WaterOpacity ("Water Opacity", Range(0, 1)) = 1
        _ShoreOpacity ("Shore Opacity", Float ) = 1
        _ShoreTransparency ("Shore Transparency", Float ) = 6
        _RefractionIntensity ("Refraction Intensity", Range(0, 1)) = 0
        _Foam ("Foam", 2D) = "white" {}
        _FoamScale ("Foam Scale", Float ) = 1
        _FoamSpeed ("Foam Speed", Float ) = 1
        _FoamIntensity ("Foam Intensity", Float ) = 1
        _FoamWavesIntensity ("Foam Waves Intensity", Float ) = 1
        _FoamDistance ("Foam Distance", Float ) = 2
        _DisplacementMask ("Displacement Mask", 2D) = "white" {}
        _DisplacementTile ("Displacement Tile", 2D) = "white" {}
        _WavesTileScale ("Waves Tile Scale", Float ) = 6
        _MaskIntensity ("Mask Intensity", Range(0, 1)) = 1
        _DisplacementSpeed ("Displacement Speed", Float ) = 1
        [MaterialToggle] _InverseWavesDirection ("Inverse Waves Direction", Float ) = 0
        _WavesAmplitude ("Waves Amplitude", Float ) = 1
        _ReflectionTex ("ReflectionTex", 2D) = "white" {}
        _ReflectionPower ("Reflection Power", Float ) = 1
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        //GrabPass{ }
        Pass {
            Name "FORWARD"
            Tags {
                //"LightMode"="UniversalForward"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            //#include "UnityCG.cginc"
            //#include "Lighting.cginc"
            //#include "UnityPBSLighting.cginc"
            //#include "UnityStandardBRDF.cginc"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            //#pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma target 3.0
            #pragma glsl
            //uniform sampler2D _GrabTexture;
            //uniform sampler2D _CameraDepthTexture;
            TEXTURE2D(_CameraDepthTexture);SAMPLER(sampler_CameraDepthTexture_linear_clamp);
            TEXTURE2D(_CameraOpaqueTexture); SAMPLER(sampler_CameraOpaqueTexture_linear_clamp);

            uniform float4 _TimeEditor;
            uniform float4 _WaterColor;
            uniform float _Density;
            uniform float _FadeLevel;
            uniform float _Specularity;
            uniform sampler2D _Normal; uniform float4 _Normal_ST;
            uniform float _Scale;
            uniform float _WaterSpeed;
            uniform float _NormalIntensity;
            uniform float _Fresnel;
            uniform float _ShoreOpacity;
            uniform float _ShoreTransparency;
            uniform float _RefractionIntensity;
            uniform sampler2D _Foam; uniform float4 _Foam_ST;
            uniform float _FoamScale;
            uniform float _FoamSpeed;
            uniform float _FoamIntensity;
            uniform float _FoamDistance;
            uniform float _Gloss;
            uniform sampler2D _DisplacementMask; uniform float4 _DisplacementMask_ST;
            uniform sampler2D _DisplacementTile; uniform float4 _DisplacementTile_ST;
            uniform float _WavesTileScale;
            uniform float _MaskIntensity;
            uniform float _DisplacementSpeed;
            uniform half _InverseWavesDirection;
            uniform float _WavesAmplitude;
            uniform float _FoamWavesIntensity;
            uniform float _WaterOpacity;
            uniform sampler2D _ReflectionTex; uniform float4 _ReflectionTex_ST;
            uniform float _ReflectionPower;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 bitangentDir : TEXCOORD4;
                float4 screenPos : TEXCOORD5;
                float4 projPos : TEXCOORD6;
                float fogFactor : TEXCOORD7;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = TransformObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                float3 recipObjScale = float3( length(unity_WorldToObject[0].xyz), length(unity_WorldToObject[1].xyz), length(unity_WorldToObject[2].xyz) );
                float3 objScale = 1.0/recipObjScale;
                float4 node_9767 = _Time + _TimeEditor;
                float node_1302 = length((o.uv0*2.0+-1.0));
                float2 node_7065 = (o.uv0*(-1.0));
                float4 _DisplacementMask_var = tex2Dlod(_DisplacementMask,float4(TRANSFORM_TEX(node_7065, _DisplacementMask),0.0,0));
                float2 node_6159 = ((float2(node_1302,node_1302)*_WavesTileScale*(_DisplacementMask_var.rgb*_MaskIntensity).rg)+(_DisplacementSpeed*node_9767.r*(_InverseWavesDirection*2.0+-1.0))*float2(1,1));
                float4 _DisplacementTile_var = tex2Dlod(_DisplacementTile,float4(TRANSFORM_TEX(node_6159, _DisplacementTile),0.0,0));
                float3 node_3816 = (_DisplacementTile_var.r*float3(0,1,0)*_WavesAmplitude);
                v.vertex.xyz += node_3816;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);

                float3 lightColor = _MainLightColor.rgb;


                o.pos = TransformObjectToHClip(v.vertex.xyz );
                o.fogFactor = ComputeFogFactor(o.pos.z);

                //UNITY_TRANSFER_FOG(o,o.pos);
                o.projPos = ComputeScreenPos (o.pos);
                o.projPos.z = LinearEyeDepth(o.projPos.z/o.projPos.w,_ZBufferParams);
                o.screenPos = o.pos;
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                float3 recipObjScale = float3( length(unity_WorldToObject[0].xyz), length(unity_WorldToObject[1].xyz), length(unity_WorldToObject[2].xyz) );
                float3 objScale = 1.0/recipObjScale;
                #if UNITY_UV_STARTS_AT_TOP
                    float grabSign = -_ProjectionParams.x;
                #else
                    float grabSign = _ProjectionParams.x;
                #endif
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                i.screenPos = float4( i.screenPos.xy / i.screenPos.w, 0, 0 );
                i.screenPos.y *= _ProjectionParams.x;
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float4 node_7509 = _Time + _TimeEditor;
                float node_4302 = (_WaterSpeed*node_7509.r);
                float2 node_7657 = (float2(objScale.r,objScale.b)*i.uv0*_Scale);
                float2 node_5018 = ((float2(0.333,0.666)+(0.666*node_7657))+node_4302*float2(-1,1));
                float3 node_1516 = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(node_5018, _Normal)));
                float2 node_9281 = (node_7657+node_4302*float2(1,-1));
                float3 node_5988 = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(node_9281, _Normal)));
                float3 node_3745 = lerp(float3(0,0,1),float3((node_1516.r+node_5988.r),(node_1516.g+node_5988.g),1.0),_NormalIntensity);
                float3 normalLocal = node_3745;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                //float sceneZ = max(0,LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)))) - _ProjectionParams.g);
                float sceneZ = max(0,LinearEyeDepth (SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture_linear_clamp, i.projPos.xy/i.projPos.w),_ZBufferParams) - _ProjectionParams.g);

                float partZ = max(0,i.projPos.z - _ProjectionParams.g);
                float node_9540 = i.normalDir.g;
                float2 sceneUVs = float2(1,grabSign)*i.screenPos.xy*0.5+0.5 + (node_3745.rg*_RefractionIntensity*((1.0 - node_9540)*1.0+1.0));
               // float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
               float4 sceneColor = SAMPLE_TEXTURE2D_LOD(_CameraOpaqueTexture, sampler_CameraOpaqueTexture_linear_clamp, sceneUVs, 0);
               //return sceneColor;
                Light light = GetMainLight();
	             
                float3 lightDirection = light.direction;
                float3 lightColor = _MainLightColor.rgb;;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = 1;
                float3 attenColor = attenuation * _MainLightColor.rgb;;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float gloss = clamp(_Gloss,0.1,1);
                float specPow = exp2( gloss * 10.0+1.0);
/////// GI Data:
                //Light light;
                #ifdef LIGHTMAP_OFF
                    light.color = lightColor;
                    //light.dir = lightDirection;
                    //light.ndotl = LambertTerm (normalDirection, light.dir);
                #else
                    light.color = half3(0.f, 0.f, 0.f);
                    //light.ndotl = 0.0f;
                    //light.dir = half3(0.f, 0.f, 0.f);
                #endif
                //UnityGIInput d;
                //d.light = light;
                //d.worldPos = i.posWorld.xyz;
                //d.worldViewDir = viewDirection;
                //d.atten = attenuation;
                //Unity_GlossyEnvironmentData ugls_en_data;
                //ugls_en_data.roughness = 1.0 - gloss;
                //ugls_en_data.reflUVW = viewReflectDirection;
                //UnityGI gi = UnityGlobalIllumination(d, 1, normalDirection, ugls_en_data );

                //lightDirection = //gi.light.dir;
                //lightColor = //gi.light.color;
////// Specular:
                //float NdotL = max(0, dot( normalDirection, lightDirection ));
                float LdotH = max(0.0,dot(lightDirection, halfDirection));
                float node_1628 = pow(1.0-max(0,dot(normalDirection, viewDirection)),_Fresnel);
                float node_2311 = (_Specularity*node_1628);
                float3 specularColor = float3(node_2311,node_2311,node_2311);
                float specularMonochrome = max( max(specularColor.r, specularColor.g), specularColor.b);
                float NdotV = max(0.0,dot( normalDirection, viewDirection ));
                //float NdotH = max(0.0,dot( normalDirection, halfDirection ));
                //float VdotH = max(0.0,dot( viewDirection, halfDirection ));
                //float visTerm = SmithBeckmannVisibilityTerm( NdotL, NdotV, 1.0-gloss );
                //float normTerm = max(0.0, NDFBlinnPhongNormalizedTerm(NdotH, RoughnessToSpecPower(1.0-gloss)));
                //float specularPBL = max(0, (NdotL*visTerm*normTerm) * (UNITY_PI / 4) );
                //float3 directSpecular = (floor(attenuation) * _MainLightColor.rgb;) * pow(max(0,dot(halfDirection,normalDirection)),specPow)*specularPBL*lightColor*FresnelTerm(specularColor, LdotH);
                
                float3 specular =  LightingSpecular(lightColor,lightDirection, normalDirection,  viewDirection,half4(1.0f,1.0f,1.0f,0.0f), 1000.0f) * lightColor;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float node_5971 = 1.0;
                float3 w = float3(node_5971,node_5971,node_5971)*0.5; // Light wrapping
                float3 NdotLWrap = NdotL * ( 1.0 - w );
                float3 forwardLight = max(float3(0.0,0.0,0.0), NdotLWrap + w );
                float3 backLight = max(float3(0.0,0.0,0.0), -NdotLWrap + w ) * float3(node_5971,node_5971,node_5971);
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                NdotLWrap = max(float3(0,0,0), NdotLWrap);
                float3 directDiffuse = ((forwardLight+backLight) + ((1 +(fd90 - 1)*pow((1.00001-NdotLWrap), 5)) * (1 + (fd90 - 1)*pow((1.00001-NdotV), 5)) * NdotL))*(0.5-max(w.r,max(w.g,w.b))*0.5) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; // Ambient Light
                float node_9504 = (-0.1);
                float node_3478 = 1.0;
                float4 node_9786 = _Time + _TimeEditor;
                float node_9023 = (_FoamSpeed*node_9786.r);
                float2 node_2369 = (float2(objScale.r,objScale.b)*i.uv0*_FoamScale);
                float2 node_8859 = (node_2369+node_9023*float2(1,-1));
                float4 node_2758 = tex2D(_Foam,TRANSFORM_TEX(node_8859, _Foam));
                float2 node_5346 = (((node_2369*0.666)+float2(0.333,0.666))+node_9023*float2(-1,1));
                float4 node_5223 = tex2D(_Foam,TRANSFORM_TEX(node_5346, _Foam));
                float3 node_4593 = (node_2758.rgb*node_5223.rgb);
                float4 node_9767 = _Time + _TimeEditor;
                float node_1302 = length((i.uv0*2.0+-1.0));
                float2 node_7065 = (i.uv0*(-1.0));
                float4 _DisplacementMask_var = tex2D(_DisplacementMask,TRANSFORM_TEX(node_7065, _DisplacementMask));
                float2 node_6159 = ((float2(node_1302,node_1302)*_WavesTileScale*(_DisplacementMask_var.rgb*_MaskIntensity).rg)+(_DisplacementSpeed*node_9767.r*(_InverseWavesDirection*2.0+-1.0))*float2(1,1));
                float4 _DisplacementTile_var = tex2D(_DisplacementTile,TRANSFORM_TEX(node_6159, _DisplacementTile));
                float3 node_3816 = (_DisplacementTile_var.r*float3(0,1,0)*_WavesAmplitude);
                float3 node_9205 = ((_MainLightColor.rgb*attenuation*max(0,dot(lightDirection,i.normalDir)))*(clamp((node_4593*(1.0 - saturate((sceneZ-partZ)/_FoamDistance))*_FoamIntensity),0,1)+clamp((saturate(dot(node_3816,float3(0.3,0.59,0.11)))*_FoamWavesIntensity*node_4593),0,10)));
                float4 node_9323 = tex2D(_ReflectionTex,TRANSFORM_TEX(sceneUVs.rg, _ReflectionTex));
                float3 node_1190 = (node_9323.rgb*node_1628*_ReflectionPower);
                float3 diffuseColor = saturate((1.0-(1.0-((saturate((pow(saturate((node_3478 + ( (((sceneZ-partZ)*_Density) - node_9504) * (0.0 - node_3478) ) / ((_WaterColor.rgb*6.28318530718) - node_9504))),_FadeLevel*-1)+(1.0 - saturate((sceneZ-partZ)/_ShoreTransparency))))*sceneColor.rgb)+node_9205))*(1.0-node_1190)));
                diffuseColor *= 1-specularMonochrome;
                float3 diffuse = (directDiffuse + indirectDiffuse) * (diffuseColor);
////// Emissive:
                float3 emissive = (node_1190+node_9205);
/// Final Color:
                float3 finalColor = diffuse*0.7 + specular + emissive;//
                half4 finalRGBA = half4(lerp(sceneColor.rgb, finalColor,(saturate((saturate((sceneZ-partZ)/_ShoreOpacity)*_WaterOpacity))*clamp(node_9540,0.3,1))),1);
                //UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                finalRGBA.xyz = MixFog(finalRGBA.xyz, i.fogFactor);
                return finalRGBA;
            }
            ENDHLSL
        }
//        Pass {
//            Name "FORWARD_DELTA"
//            Tags {
//                //"LightMode"="ForwardAdd"
//            }
//            Blend One One
//            Cull Off
//            ZWrite Off
            
//            CGPROGRAM
//            #pragma vertex vert
//            #pragma fragment frag
//            #define UNITY_PASS_FORWARDADD
//            #include "UnityCG.cginc"
//            #include "AutoLight.cginc"
//            #include "Lighting.cginc"
//            #include "UnityPBSLighting.cginc"
//            #include "UnityStandardBRDF.cginc"
//            #pragma multi_compile_fwdadd
//            #pragma multi_compile_fog
//            #pragma target 3.0
//            #pragma glsl
//            uniform sampler2D _GrabTexture;
//            uniform sampler2D _CameraDepthTexture;
//            uniform float4 _TimeEditor;
//            uniform float4 _WaterColor;
//            uniform float _Density;
//            uniform float _FadeLevel;
//            uniform float _Specularity;
//            uniform sampler2D _Normal; uniform float4 _Normal_ST;
//            uniform float _Scale;
//            uniform float _WaterSpeed;
//            uniform float _NormalIntensity;
//            uniform float _Fresnel;
//            uniform float _ShoreOpacity;
//            uniform float _ShoreTransparency;
//            uniform float _RefractionIntensity;
//            uniform sampler2D _Foam; uniform float4 _Foam_ST;
//            uniform float _FoamScale;
//            uniform float _FoamSpeed;
//            uniform float _FoamIntensity;
//            uniform float _FoamDistance;
//            uniform float _Gloss;
//            uniform sampler2D _DisplacementMask; uniform float4 _DisplacementMask_ST;
//            uniform sampler2D _DisplacementTile; uniform float4 _DisplacementTile_ST;
//            uniform float _WavesTileScale;
//            uniform float _MaskIntensity;
//            uniform float _DisplacementSpeed;
//            uniform fixed _InverseWavesDirection;
//            uniform float _WavesAmplitude;
//            uniform float _FoamWavesIntensity;
//            uniform float _WaterOpacity;
//            uniform sampler2D _ReflectionTex; uniform float4 _ReflectionTex_ST;
//            uniform float _ReflectionPower;
//            struct VertexInput {
//                float4 vertex : POSITION;
//                float3 normal : NORMAL;
//                float4 tangent : TANGENT;
//                float2 texcoord0 : TEXCOORD0;
//            };
//            struct VertexOutput {
//                float4 pos : SV_POSITION;
//                float2 uv0 : TEXCOORD0;
//                float4 posWorld : TEXCOORD1;
//                float3 normalDir : TEXCOORD2;
//                float3 tangentDir : TEXCOORD3;
//                float3 bitangentDir : TEXCOORD4;
//                float4 screenPos : TEXCOORD5;
//                float4 projPos : TEXCOORD6;
//                LIGHTING_COORDS(7,8)
//                UNITY_FOG_COORDS(9)
//            };
//            VertexOutput vert (VertexInput v) {
//                VertexOutput o = (VertexOutput)0;
//                o.uv0 = v.texcoord0;
//                o.normalDir = UnityObjectToWorldNormal(v.normal);
//                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
//                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
//                float3 recipObjScale = float3( length(unity_WorldToObject[0].xyz), length(unity_WorldToObject[1].xyz), length(unity_WorldToObject[2].xyz) );
//                float3 objScale = 1.0/recipObjScale;
//                float4 node_9767 = _Time + _TimeEditor;
//                float node_1302 = length((o.uv0*2.0+-1.0));
//                float2 node_7065 = (o.uv0*(-1.0));
//                float4 _DisplacementMask_var = tex2Dlod(_DisplacementMask,float4(TRANSFORM_TEX(node_7065, _DisplacementMask),0.0,0));
//                float2 node_6159 = ((float2(node_1302,node_1302)*_WavesTileScale*(_DisplacementMask_var.rgb*_MaskIntensity).rg)+(_DisplacementSpeed*node_9767.r*(_InverseWavesDirection*2.0+-1.0))*float2(1,1));
//                float4 _DisplacementTile_var = tex2Dlod(_DisplacementTile,float4(TRANSFORM_TEX(node_6159, _DisplacementTile),0.0,0));
//                float3 node_3816 = (_DisplacementTile_var.r*float3(0,1,0)*_WavesAmplitude);
//                v.vertex.xyz += node_3816;
//                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
//                float3 lightColor = _LightColor0.rgb;
//                o.pos = UnityObjectToClipPos(v.vertex );
//                UNITY_TRANSFER_FOG(o,o.pos);
//                o.projPos = ComputeScreenPos (o.pos);
//                COMPUTE_EYEDEPTH(o.projPos.z);
//                o.screenPos = o.pos;
//                TRANSFER_VERTEX_TO_FRAGMENT(o)
//                return o;
//            }
//            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
//                float isFrontFace = ( facing >= 0 ? 1 : 0 );
//                float faceSign = ( facing >= 0 ? 1 : -1 );
//                float3 recipObjScale = float3( length(unity_WorldToObject[0].xyz), length(unity_WorldToObject[1].xyz), length(unity_WorldToObject[2].xyz) );
//                float3 objScale = 1.0/recipObjScale;
//                #if UNITY_UV_STARTS_AT_TOP
//                    float grabSign = -_ProjectionParams.x;
//                #else
//                    float grabSign = _ProjectionParams.x;
//                #endif
//                i.normalDir = normalize(i.normalDir);
//                i.normalDir *= faceSign;
//                i.screenPos = float4( i.screenPos.xy / i.screenPos.w, 0, 0 );
//                i.screenPos.y *= _ProjectionParams.x;
//                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
//                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
//                float4 node_7509 = _Time + _TimeEditor;
//                float node_4302 = (_WaterSpeed*node_7509.r);
//                float2 node_7657 = (float2(objScale.r,objScale.b)*i.uv0*_Scale);
//                float2 node_5018 = ((float2(0.333,0.666)+(0.666*node_7657))+node_4302*float2(-1,1));
//                float3 node_1516 = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(node_5018, _Normal)));
//                float2 node_9281 = (node_7657+node_4302*float2(1,-1));
//                float3 node_5988 = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(node_9281, _Normal)));
//                float3 node_3745 = lerp(float3(0,0,1),float3((node_1516.r+node_5988.r),(node_1516.g+node_5988.g),1.0),_NormalIntensity);
//                float3 normalLocal = node_3745;
//                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
//                float sceneZ = max(0,LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)))) - _ProjectionParams.g);
//                float partZ = max(0,i.projPos.z - _ProjectionParams.g);
//                float node_9540 = i.normalDir.g;
//                float2 sceneUVs = float2(1,grabSign)*i.screenPos.xy*0.5+0.5 + (node_3745.rg*_RefractionIntensity*((1.0 - node_9540)*1.0+1.0));
//                float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
//                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
//                float3 lightColor = _LightColor0.rgb;
//                float3 halfDirection = normalize(viewDirection+lightDirection);
//////// Lighting:
//                float attenuation = LIGHT_ATTENUATION(i);
//                float3 attenColor = attenuation * _LightColor0.xyz;
//                float Pi = 3.141592654;
//                float InvPi = 0.31830988618;
/////////// Gloss:
//                float gloss = clamp(_Gloss,0.1,1);
//                float specPow = exp2( gloss * 10.0+1.0);
//////// Specular:
//                float NdotL = max(0, dot( normalDirection, lightDirection ));
//                float LdotH = max(0.0,dot(lightDirection, halfDirection));
//                float node_1628 = pow(1.0-max(0,dot(normalDirection, viewDirection)),_Fresnel);
//                float node_2311 = (_Specularity*node_1628);
//                float3 specularColor = float3(node_2311,node_2311,node_2311);
//                float specularMonochrome = max( max(specularColor.r, specularColor.g), specularColor.b);
//                float NdotV = max(0.0,dot( normalDirection, viewDirection ));
//                float NdotH = max(0.0,dot( normalDirection, halfDirection ));
//                float VdotH = max(0.0,dot( viewDirection, halfDirection ));
//                float visTerm = SmithBeckmannVisibilityTerm( NdotL, NdotV, 1.0-gloss );
//                float normTerm = max(0.0, NDFBlinnPhongNormalizedTerm(NdotH, RoughnessToSpecPower(1.0-gloss)));
//                float specularPBL = max(0, (NdotL*visTerm*normTerm) * (UNITY_PI / 4) );
//                float3 directSpecular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),specPow)*specularPBL*lightColor*FresnelTerm(specularColor, LdotH);
//                float3 specular = directSpecular;
///////// Diffuse:
//                NdotL = dot( normalDirection, lightDirection );
//                float node_5971 = 1.0;
//                float3 w = float3(node_5971,node_5971,node_5971)*0.5; // Light wrapping
//                float3 NdotLWrap = NdotL * ( 1.0 - w );
//                float3 forwardLight = max(float3(0.0,0.0,0.0), NdotLWrap + w );
//                float3 backLight = max(float3(0.0,0.0,0.0), -NdotLWrap + w ) * float3(node_5971,node_5971,node_5971);
//                NdotL = max(0.0,dot( normalDirection, lightDirection ));
//                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
//                NdotLWrap = max(float3(0,0,0), NdotLWrap);
//                float3 directDiffuse = ((forwardLight+backLight) + ((1 +(fd90 - 1)*pow((1.00001-NdotLWrap), 5)) * (1 + (fd90 - 1)*pow((1.00001-NdotV), 5)) * NdotL))*(0.5-max(w.r,max(w.g,w.b))*0.5) * attenColor;
//                float node_9504 = (-0.1);
//                float node_3478 = 1.0;
//                float4 node_9786 = _Time + _TimeEditor;
//                float node_9023 = (_FoamSpeed*node_9786.r);
//                float2 node_2369 = (float2(objScale.r,objScale.b)*i.uv0*_FoamScale);
//                float2 node_8859 = (node_2369+node_9023*float2(1,-1));
//                float4 node_2758 = tex2D(_Foam,TRANSFORM_TEX(node_8859, _Foam));
//                float2 node_5346 = (((node_2369*0.666)+float2(0.333,0.666))+node_9023*float2(-1,1));
//                float4 node_5223 = tex2D(_Foam,TRANSFORM_TEX(node_5346, _Foam));
//                float3 node_4593 = (node_2758.rgb*node_5223.rgb);
//                float4 node_9767 = _Time + _TimeEditor;
//                float node_1302 = length((i.uv0*2.0+-1.0));
//                float2 node_7065 = (i.uv0*(-1.0));
//                float4 _DisplacementMask_var = tex2D(_DisplacementMask,TRANSFORM_TEX(node_7065, _DisplacementMask));
//                float2 node_6159 = ((float2(node_1302,node_1302)*_WavesTileScale*(_DisplacementMask_var.rgb*_MaskIntensity).rg)+(_DisplacementSpeed*node_9767.r*(_InverseWavesDirection*2.0+-1.0))*float2(1,1));
//                float4 _DisplacementTile_var = tex2D(_DisplacementTile,TRANSFORM_TEX(node_6159, _DisplacementTile));
//                float3 node_3816 = (_DisplacementTile_var.r*float3(0,1,0)*_WavesAmplitude);
//                float3 node_9205 = ((_LightColor0.rgb*attenuation*max(0,dot(lightDirection,i.normalDir)))*(clamp((node_4593*(1.0 - saturate((sceneZ-partZ)/_FoamDistance))*_FoamIntensity),0,1)+clamp((saturate(dot(node_3816,float3(0.3,0.59,0.11)))*_FoamWavesIntensity*node_4593),0,10)));
//                float4 node_9323 = tex2D(_ReflectionTex,TRANSFORM_TEX(sceneUVs.rg, _ReflectionTex));
//                float3 node_1190 = (node_9323.rgb*node_1628*_ReflectionPower);
//                float3 diffuseColor = saturate((1.0-(1.0-((saturate((pow(saturate((node_3478 + ( (((sceneZ-partZ)*_Density) - node_9504) * (0.0 - node_3478) ) / ((_WaterColor.rgb*6.28318530718) - node_9504))),_FadeLevel)+(1.0 - saturate((sceneZ-partZ)/_ShoreTransparency))))*sceneColor.rgb)+node_9205))*(1.0-node_1190)));
//                diffuseColor *= 1-specularMonochrome;
//                float3 diffuse = directDiffuse * diffuseColor;
///// Final Color:
//                float3 finalColor = diffuse + specular;
//                fixed4 finalRGBA = fixed4(finalColor * (saturate((saturate((sceneZ-partZ)/_ShoreOpacity)*_WaterOpacity))*clamp(node_9540,0.3,1)),0);
//                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
//                return finalRGBA;
//            }
//            ENDCG
//        }
        //Pass {
        //    Name "ShadowCaster"
        //    Tags {
        //        "LightMode"="ShadowCaster"
        //    }
        //    Offset 1, 1
            
        //    CGPROGRAM
        //    #pragma vertex vert
        //    #pragma fragment frag
        //    #define UNITY_PASS_SHADOWCASTER
        //    #include "UnityCG.cginc"
        //    #include "Lighting.cginc"
        //    #include "UnityPBSLighting.cginc"
        //    #include "UnityStandardBRDF.cginc"
        //    #pragma fragmentoption ARB_precision_hint_fastest
        //    #pragma multi_compile_shadowcaster
        //    #pragma multi_compile_fog
        //    #pragma target 3.0
        //    #pragma glsl
        //    uniform float4 _TimeEditor;
        //    uniform sampler2D _DisplacementMask; uniform float4 _DisplacementMask_ST;
        //    uniform sampler2D _DisplacementTile; uniform float4 _DisplacementTile_ST;
        //    uniform float _WavesTileScale;
        //    uniform float _MaskIntensity;
        //    uniform float _DisplacementSpeed;
        //    uniform fixed _InverseWavesDirection;
        //    uniform float _WavesAmplitude;
        //    struct VertexInput {
        //        float4 vertex : POSITION;
        //        float2 texcoord0 : TEXCOORD0;
        //    };
        //    struct VertexOutput {
        //        V2F_SHADOW_CASTER;
        //        float2 uv0 : TEXCOORD1;
        //    };
        //    VertexOutput vert (VertexInput v) {
        //        VertexOutput o = (VertexOutput)0;
        //        o.uv0 = v.texcoord0;
        //        float4 node_9767 = _Time + _TimeEditor;
        //        float node_1302 = length((o.uv0*2.0+-1.0));
        //        float2 node_7065 = (o.uv0*(-1.0));
        //        float4 _DisplacementMask_var = tex2Dlod(_DisplacementMask,float4(TRANSFORM_TEX(node_7065, _DisplacementMask),0.0,0));
        //        float2 node_6159 = ((float2(node_1302,node_1302)*_WavesTileScale*(_DisplacementMask_var.rgb*_MaskIntensity).rg)+(_DisplacementSpeed*node_9767.r*(_InverseWavesDirection*2.0+-1.0))*float2(1,1));
        //        float4 _DisplacementTile_var = tex2Dlod(_DisplacementTile,float4(TRANSFORM_TEX(node_6159, _DisplacementTile),0.0,0));
        //        float3 node_3816 = (_DisplacementTile_var.r*float3(0,1,0)*_WavesAmplitude);
        //        v.vertex.xyz += node_3816;
        //        o.pos = UnityObjectToClipPos(v.vertex );
        //        TRANSFER_SHADOW_CASTER(o)
        //        return o;
        //    }
        //    float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
        //        float isFrontFace = ( facing >= 0 ? 1 : 0 );
        //        float faceSign = ( facing >= 0 ? 1 : -1 );
        //        SHADOW_CASTER_FRAGMENT(i)
        //    }
        //    ENDCG
        //}
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
