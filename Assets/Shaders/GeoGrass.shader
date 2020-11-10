// @Cyanilux
// Grass Geometry Shader, Written for Universal RP with help from https://roystan.net/articles/grass-shader.html

Shader "Unlit/GeoGrass" {
	Properties {
		_Color("Colour", Color) = (1,1,1,1)
		_Color2("Colour2", Color) = (1,1,1,1)
		_Width("Width", Float) = 1
		_RandomWidth("Random Width", Float) = 1
		_Height("Height", Float) = 1
		_RandomHeight("Random Height", Float) = 1
		_WindStrength("Wind Strength", Float) = 0.1
		_ShadowStrength("Shadow Strengh", Float) = 0.1
		[Space]
		_TessellationUniform("Tessellation Uniform", Range(1, 64)) = 1
	}

	SubShader {
		Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "Queue" = "Geometry"}
		LOD 300

		Cull[_Cull]

		Pass {
			Name "ForwardLit"
			Tags {"LightMode" = "UniversalForward"}

			HLSLPROGRAM
			// Required to compile gles 2.0 with standard srp library
			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x gles
			#pragma target 4.5

			#pragma require geometry
				
			#pragma vertex vert
			#pragma geometry geom
			#pragma fragment frag
			#pragma hull hull
			#pragma domain domain

            //include fog
            #pragma multi_compile_fog

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile _ REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            #pragma multi_compile _ LIGHTMAP_ON

            #pragma shader_feature _ALPHATEST_ON
            // Defines

			#define BLADE_SEGMENTS 1

			// Includes

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
			#include "grass_structs.hlsl" 
			#include "CustomTessellation.hlsl"
			#include "grass.hlsl"
			CBUFFER_START(Shadows)
				float _ShadowStrength;
			CBUFFER_END
            
			float4 frag(GeometryOutput input) : SV_Target {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                float NdotL = saturate(dot(normalize(_MainLightPosition.xyz), input.normal));
                float3 ambient = SampleSH(input.normal);

				Light mainLight = GetMainLight(input.shadowCoord);

				#if SHADOWS_SCREEN
					float4 ShadowAtten = SampleScreenSpaceShadowmap(input.shadowCoord);
				#else
					ShadowSamplingData shadowSamplingData = GetMainLightShadowSamplingData();
					half shadowStrength = GetMainLightShadowStrength();
					float4 ShadowAtten = SampleShadowmap(input.shadowCoord, TEXTURE2D_ARGS(_MainLightShadowmapTexture,
													                                       sampler_MainLightShadowmapTexture),
					                                    shadowSamplingData, shadowStrength, false);
				#endif
                float3 color = lerp(_Color, _Color2, input.uv.y);
                float3 rgb = color * ( _MainLightColor.rgb * mainLight.shadowAttenuation + ambient);

            #ifdef _ADDITIONAL_LIGHTS
                int additionalLightsCount = GetAdditionalLightsCount();
                for (int i = 0; i < additionalLightsCount; ++i)
                {
                    Light light = GetAdditionalLight(i, input.positionWS);
                    half3 attenuatedLightColor = light.color * (light.distanceAttenuation * light.shadowAttenuation);
                    rgb += attenuatedLightColor * color;
                }
            #endif


                //rgb = MixFog(rgb, input.fogCoord);

				return float4(rgb, 1.0);
			}

			ENDHLSL
		}
        
        Pass
        {
            Name "ShadowCaster"

            Tags{"LightMode" = "ShadowCaster"}

                Cull Back

                HLSLPROGRAM

                #pragma prefer_hlslcc gles
                #pragma exclude_renderers d3d11_9x gles
                #pragma target 4.5
                
                #pragma vertex ShadowPassVertex
                #pragma fragment ShadowPassFragment

               #pragma shader_feature _ALPHATEST_ON

                // GPU Instancing
                #pragma multi_compile_instancing

                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"


                CBUFFER_START(UnityPerMaterial)
                half4 _TintColor;
                sampler2D _MainTex;
                float4 _MainTex_ST;
                float   _Alpha;
                CBUFFER_END

                struct VertexInput
                {
                float4 vertex : POSITION;
                float4 normal : NORMAL;

                #if _ALPHATEST_ON
                float2 uv     : TEXCOORD0;
                #endif

                UNITY_VERTEX_INPUT_INSTANCE_ID
                };

                struct VertexOutput
                {
                float4 vertex : SV_POSITION;
                #if _ALPHATEST_ON
                float2 uv     : TEXCOORD0;
                #endif
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO

                };

                VertexOutput ShadowPassVertex(VertexInput v)
                {
                VertexOutput o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                float3 positionWS = TransformObjectToWorld(v.vertex.xyz);
                float3 normalWS = TransformObjectToWorldNormal(v.normal.xyz);

                float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, _MainLightPosition.xyz));

                o.vertex = positionCS;
                #if _ALPHATEST_ON
                o.uv = v.uv * _MainTex_ST.xy + _MainTex_ST.zw; ;
                #endif

                return o;
                }

                half4 ShadowPassFragment(VertexOutput i) : SV_TARGET
                {
                    UNITY_SETUP_INSTANCE_ID(i);
                    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

                    #if _ALPHATEST_ON
                    float4 col = tex2D(_MainTex, i.uv);
                    clip(col.a - _Alpha);
                    #endif

                    return 0;
                }

                ENDHLSL
            }
            
            Pass
            {
                Name "DepthOnly"
                Tags{"LightMode" = "DepthOnly"}

                ZWrite On
                ColorMask 0

                Cull Back

                HLSLPROGRAM

                #pragma prefer_hlslcc gles
                #pragma exclude_renderers d3d11_9x gles
                #pragma target 4.5

                // GPU Instancing
                #pragma multi_compile_instancing

                #pragma vertex vert
                #pragma fragment frag

                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

                CBUFFER_START(UnityPerMaterial)
                CBUFFER_END

                struct VertexInput
                {
                    float4 vertex : POSITION;
                    UNITY_VERTEX_INPUT_INSTANCE_ID
                };

                struct VertexOutput
                {
                float4 vertex : SV_POSITION;

                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
                };

                VertexOutput vert(VertexInput v)
                {
                    VertexOutput o;
                    UNITY_SETUP_INSTANCE_ID(v);
                    UNITY_TRANSFER_INSTANCE_ID(v, o);
                    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                    o.vertex = TransformObjectToHClip(v.vertex.xyz);

                    return o;
                }

                half4 frag(VertexOutput IN) : SV_TARGET
                {
                    return 0;
                }
                ENDHLSL
            }
        //}
	}
}
