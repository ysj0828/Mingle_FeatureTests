///
/// INFORMATION
/// 
/// Project: Chloroplast Games Framework
/// Game: Chloroplast Games Framework
/// Date: 01/10/2020
/// Author: Chloroplast Games
/// Website: http://www.chloroplastgames.com
/// Programmers: Pau Elias Soriano
/// Description: Unlit/MatCap.
///

Shader "CG Framework/Unlit/MatCap" {

	Properties {
		[Header(Diffuse)]
		_MainTex("Diffuse Map (RGBA)", 2D) = "white" {}
		[Toggle(_SEMITRANSPARENTSHADOWS_ON)]_SemitransparentShadows("Use Semitransparent Shadows", Float) = 0
		_Cutoff("Alpha cutoff", Range( 0 , 1)) = 0.5
		_Color("Color (RGBA)", Color) = (1,1,1,1)
		
		[Header(MatCap)]
		_MatCap("MatCap (RGB)", 2D) = "white" {}
		[Toggle]_DesaturateMatCap("Desaturate MatCap", Float) = 0

		[Header(Ambient Occlusion)]
		[Toggle(_AMBIENTOCCLUSION_ON)]_AmbientOcclusion("Ambient Occlusion", Float) = 0
		_AmbientOcclusionMap("Ambient Occlusion Map (RGB)", 2D) = "white" {}
		_AmbientOcclusionLevel("Ambient Occlusion Level", Range(0, 1)) = 1

		[Header(Ambient Light)]
		[Toggle(_AMBIENTLIGHT_ON)] _AmbientLight("Enable Ambient Light", Float) = 0

		[Header(Normal)]
		[Toggle(_NORMAL_ON)]_Normal("Normal", Float) = 0
		_NormalMap("Normal Map (RGB)", 2D) = "bump" {}
		_NormalLevel("Normal Level", Float) = 1

		[Header(Emission)]
		[Toggle(_EMISSION_ON)]_Emission("Emission", Float) = 0
		_EmissionMap("Emission Map (RGB)", 2D) = "white" {}
		[HDR]_EmissionColor("Emission Color (RGB)", Color) = (0, 0, 0)

		[Header(LOD Fade)]
		[KeywordEnum(None,CrossFadeBlending,CrossFadeDithering)] _LODFadeMode("LOD Fade Mode", Float) = 0
		[KeywordEnum(Unity,BlueNoise,FloydSteinberg)] _LODDitherType("LOD Dither Type", Float) = 0

		[Header(Stencil Options)]
		_StencilReference("Stencil Reference", Range( 0 , 255)) = 0
		_StencilReadMask ("Stencil Read Mask", Range( 0 , 255)) = 255
		_StencilWriteMask ("Stencil Write Mask", Range( 0 , 255)) = 255
		[Enum(UnityEngine.Rendering.CompareFunction)]_StencilComparisonFunction ("Stencil Comparison Function", Range( 0 , 8)) = 8
		[Enum(UnityEngine.Rendering.StencilOp)]_StencilPassOperation("Stencil Pass Operation", Range( 0 , 7)) = 2
		[Enum(UnityEngine.Rendering.StencilOp)]_StencilFailOperation("Stencil Fail Operation", Range( 0 , 7)) = 0
		[Enum(UnityEngine.Rendering.StencilOp)]_StencilZFailOperation("Stencil ZFail Operation", Range( 0 , 7)) = 0

		[HideInInspector]_RenderMode("Render Mode", Float) = 0
		[HideInInspector]_SrcBlend("SrcBlend", Float) = 1
		[HideInInspector]_DstBlend("DstBlend", Float) = 0
		[HideInInspector]_ZWrite("ZWrite", Float) = 1
		[HideInInspector][Enum(UnityEngine.Rendering.CullMode)]_Cull ("Cull", Float) = 2
	}

	
	CGINCLUDE

	#define BINORMAL_PER_FRAGMENT
	#define FOG_DISTANCE

	ENDCG

	
	SubShader {
		
		// ------------------------------------------------------------------
		//  MatCap rendering pass
		Pass {
			// Pass information
			Name "MatCap"
			Tags {
				"RenderType"="Custom"
				"Queue"="Geometry"
				"LightMode"="ForwardBase"
			}
			Blend [_SrcBlend] [_DstBlend]
			Cull [_Cull]
			ColorMask RGBA
			ZWrite [_ZWrite]
			ZTest LEqual
			Offset 0, 0
			Stencil
            {
                Ref [_StencilReference]
				ReadMask [_StencilReadMask]
				WriteMask [_StencilWriteMask]
				Comp [_StencilComparisonFunction]
				Pass [_StencilPassOperation]
				Fail [_StencilFailOperation]
				ZFail [_StencilZFailOperation]
            }

			CGPROGRAM

			// Shader compilation target level
			#pragma target 3.0

			// Shader variants
			#pragma shader_feature_local _ _ALPHATEST_ON _ALPHABLEND_ON
			#pragma shader_feature_local _AMBIENTLIGHT_ON
			#pragma shader_feature_local _AMBIENTOCCLUSION_ON
			#pragma shader_feature_local _NORMAL_ON
			#pragma shader_feature_local _EMISSION_ON
		
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma shader_feature_local _LODFADEMODE_NONE _LODFADEMODE_CROSSFADEBLENDING _LODFADEMODE_CROSSFADEDITHERING
			#pragma shader_feature_local _LODDITHERTYPE_UNITY _LODDITHERTYPE_BLUENOISE _LODDITHERTYPE_FLOYDSTEINBERG
			#include "Assets/CGF/Shaders/CGIncludes/LODDitherCrossfade.cginc"
			
			// Untiy fog
			#pragma multi_compile_fog
			
			// GPU instancing
			#pragma multi_compile_instancing
			#pragma instancing_options maxcount:511
			#pragma instancing_options lodfade

			// VR defines
			#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
			//only defining to not throw compilation error over Unity 5.5
			#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
			#endif

			// Pass functions
			#pragma vertex vert
			#pragma fragment frag

			// Pass body
			#include "Assets/CGF/Shaders/CGIncludes/MatCapPass.cginc"

			ENDCG
		}


		// ------------------------------------------------------------------
		//  Shadow rendering pass
		Pass {
			// Pass information
			Name "ShadowCaster"
			Tags {
				"Lightmode" = "ShadowCaster"
			}

			CGPROGRAM

			// Shader compilation target level
			#pragma target 3.0

			// Shader variants
			#pragma shader_feature_local _ _ALPHATEST_ON _ALPHABLEND_ON
			#pragma shader_feature_local _SEMITRANSPARENTSHADOWS_ON

			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma shader_feature_local _LODFADEMODE_NONE _LODFADEMODE_CROSSFADEBLENDING _LODFADEMODE_CROSSFADEDITHERING
			#pragma shader_feature_local _LODDITHERTYPE_UNITY _LODDITHERTYPE_BLUENOISE _LODDITHERTYPE_FLOYDSTEINBERG
			#include "Assets/CGF/Shaders/CGIncludes/LODDitherCrossfade.cginc"

			#pragma multi_compile_shadowcaster

			// GPU instancing
			#pragma multi_compile_instancing
			#pragma instancing_options maxcount:511
			#pragma instancing_options lodfade

			// Pass functions
			#pragma vertex vertShadow
			#pragma fragment fragShadow

			// Pass body
			#include "Assets/CGF/Shaders/CGIncludes/MatCapShadows.cginc"

			ENDCG
		}
	}
	CustomEditor "CGFUnlitMatCapMaterialEditor"
}