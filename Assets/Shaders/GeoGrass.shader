// @Cyanilux
// Grass Geometry Shader, Written for Universal RP with help from https://roystan.net/articles/grass-shader.html
// Note, doesn't include Lighting or Tessellation
Shader "Unlit/GeoGrass" {
    Properties {
		_Color ("Colour", Color) = (1,1,1,1)
		_Color2 ("Colour2", Color) = (1,1,1,1)
		_Width ("Width", Float) = 1
		_RandomWidth ("Random Width", Float) = 1
		_Height ("Height", Float) = 1
		_RandomHeight ("Random Height", Float) = 1
    }
    SubShader {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline" }
		LOD 300
 
		Cull Off
 
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
 
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
			#pragma multi_compile _ _SHADOWS_SOFT
 
			// Defines
 
			#define BLADE_SEGMENTS 3
 
			// Includes
 
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
			#include "grass.hlsl"
 
			// Fragment
 
			float4 frag (GeometryOutput input) : SV_Target {
				#if SHADOWS_SCREEN
					float4 clipPos = TransformWorldToHClip(input.positionWS);
					float4 shadowCoord = ComputeScreenPos(clipPos);
				#else
					float4 shadowCoord = TransformWorldToShadowCoord(input.positionWS);
				#endif
 
				Light mainLight = GetMainLight(shadowCoord);
 
				return lerp(_Color, _Color2, input.uv.y) * mainLight.shadowAttenuation;
			}
 
            ENDHLSL
        }
  
		// Used for rendering shadowmaps
		//UsePass "Universal Render Pipeline/Lit/ShadowCaster"
 
		Pass {
            Name "ShadowCaster"
            Tags {"LightMode" = "ShadowCaster"}
 
            ZWrite On
            ZTest LEqual
 
            HLSLPROGRAM
            // Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x gles
            #pragma target 4.5
 
            // -------------------------------------
            // Material Keywords
            //#pragma shader_feature _ALPHATEST_ON
 
            //--------------------------------------
            // GPU Instancing
            //#pragma multi_compile_instancing
            //#pragma shader_feature _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
 
			#pragma require geometry
 
            #pragma vertex vert
			#pragma geometry geom
            #pragma fragment ShadowPassFragment2
 
			#define BLADE_SEGMENTS 3
			#define SHADOW
 
            //#include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
 
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
			#include "grass.hlsl"
 
			half4 ShadowPassFragment2(GeometryOutput input) : SV_TARGET{
				//Alpha(SampleAlbedoAlpha(input.uv, TEXTURE2D_ARGS(_BaseMap, sampler_BaseMap)).a, _BaseColor, _Cutoff);
				return 0;
			}
 
            ENDHLSL
        }
 
		// Used for depth prepass
		// If shadows cascade are enabled we need to perform a depth prepass. 
		// We also need to use a depth prepass in some cases camera require depth texture
		// (e.g, MSAA is enabled and we can't resolve with Texture2DMS
		//UsePass "Universal Render Pipeline/Lit/DepthOnly"
 
		// Note, can't UsePass + SRP Batcher due to UnityPerMaterial CBUFFER having incosistent size between subshaders..
		// Had to comment this out for now so it doesn't break SRP Batcher.
		// Would have to copy the pass in here and use the same UnityPerMaterial buffer the other passes use
 
    }
}