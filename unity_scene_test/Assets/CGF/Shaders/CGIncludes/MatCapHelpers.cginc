///
/// INFORMATION
/// 
/// Project: Chloroplast Games Framework
/// Game: Chloroplast Games Framework
/// Date: 01/10/2020
/// Author: Chloroplast Games
/// Website: http://www.chloroplastgames.com
/// Programmers: Pau Elias Soriano
/// Description: MatCap helper functions.
///

#if !defined(MATCAP_HELPERS_INCLUDED)
#define MATCAP_HELPERS_INCLUDED

#include "UnityShaderVariables.cginc"
#if !defined(CG_HELPERS_INCLUDED) 
#include "Assets/CGF/Shaders/CGIncludes/CGHelpers.cginc"
#endif

// Diffuse calculation.
float3 GetDiffuse (float2 uV, sampler2D mainTex, float3 color){
	float3 diffuse = tex2D(mainTex, uV).rgb * color;

	return diffuse;
}


// Alpha calculation.
float GetAlpha (float2 uV, sampler2D mainTex, float colorAlpha){
	float alpha = tex2D(mainTex, uV).a * colorAlpha;

	return alpha;
}


// MatCap calculation.
float3 GetMatCap (float2 uV, sampler2D matCapTexture){
	float3 matCapColor = tex2D(matCapTexture, uV).rgb;

	return matCapColor;
}


// Normal calculation.
float3 GetTangentSpaceNormal (float2 uV, sampler2D normalMap, fixed normalLevel){
	float3 normal = float3(0, 0, 1);

	#if defined(_NORMAL_ON)
		normal = UnpackScaleNormal(tex2D(normalMap, uV), normalLevel);
	#endif

	return normal;
}

// Binaromal calculation
float3 CreateBinormal (float3 normal, float3 tangent, float binormalSign) {
	return cross(normal, tangent.xyz) * (binormalSign * unity_WorldTransformParams.w);
}


// Ambient occlusion calculation.
float GetAmbientOcclusion (float2 uV, sampler2D ambientOcclusionMap, fixed ambientOcclusionLevel) {
	float3 ambientOcclusion = float3(1, 1, 1);

	#if defined(_AMBIENTOCCLUSION_ON)
		ambientOcclusion = lerp(1, tex2D(ambientOcclusionMap, uV).g, ambientOcclusionLevel);
		return ambientOcclusion;
	#else
		return ambientOcclusion;
	#endif
}


// Emission calculation.
float3 GetEmission (float2 uV, sampler2D emissionMap, float3 emissionColor) {
	float3 emission = float3(0, 0, 0);

	#if defined(_EMISSION_ON)
		emission = tex2D(emissionMap, uV) * emissionColor.rgb;

		return emission;
	#else
		return emission;
	#endif
}
#endif