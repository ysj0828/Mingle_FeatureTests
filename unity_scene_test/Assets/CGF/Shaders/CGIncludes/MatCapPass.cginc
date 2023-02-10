// Upgrade NOTE: upgraded instancing buffer 'InstanceProperties' to new syntax.

///
/// INFORMATION
/// 
/// Project: Chloroplast Games Framework
/// Game: Chloroplast Games Framework
/// Date: 01/10/2020
/// Author: Chloroplast Games
/// Website: http://www.chloroplastgames.com
/// Programmers: Pau Elias Soriano
/// Description: MatCap pass.
///

#if !defined(MATCAP_INCLUDED)
#define MATCAP_INCLUDED


/*** Includes ***/
#include "Lighting.cginc"
#if !defined(CG_HELPERS_INCLUDED) 
	#include "Assets/CGF/Shaders/CGIncludes/CGHelpers.cginc"
#endif
#include "Assets/CGF/Shaders/CGIncludes/MatCapHelpers.cginc"
#if defined(BLEND_OPERATION)
	#include "Assets/CGF/Shaders/CGIncludes/BlendModes.cginc"
#endif


/*** Defines ***/
#if defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2)
	#if !defined(FOG_DISTANCE)
		#define FOG_DEPTH 1
	#endif
	#define FOG_ON 1
#endif


/*** Variables ***/
sampler2D _MainTex;
float4 _MainTex_ST;
float4 _MainTex_TexelSize;
//float _Cutoff;
//float4 _Color;

sampler2D _MatCap;
float4 _MatCap_ST;
fixed _DesaturateMatCap;

sampler2D _AmbientOcclusionMap;
fixed _AmbientOcclusionLevel;

sampler2D _NormalMap;
fixed _NormalLevel;

sampler2D _EmissionMap;
//float3 _EmissionColor;

#if defined(BLEND_OPERATION)
	sampler2D _GrabTexture;
#endif

// Instance properties.
#if UNITY_VERSION <= 20172
	UNITY_INSTANCING_BUFFER_START(InstanceProperties)
		UNITY_DEFINE_INSTANCED_PROP(fixed4, _Color)
#define _Color_arr InstanceProperties
		UNITY_DEFINE_INSTANCED_PROP(fixed, _Cutoff)
#define _Cutoff_arr InstanceProperties

		UNITY_DEFINE_INSTANCED_PROP(float4, _EmissionColor)
#define _EmissionColor_arr InstanceProperties
	UNITY_INSTANCING_BUFFER_END(InstanceProperties)
#endif

#if UNITY_VERSION >= 20173
	UNITY_INSTANCING_BUFFER_START(InstanceProperties)
		UNITY_DEFINE_INSTANCED_PROP(fixed4, _Color)
		UNITY_DEFINE_INSTANCED_PROP(fixed, _Cutoff)

		UNITY_DEFINE_INSTANCED_PROP(float4, _EmissionColor)
	UNITY_INSTANCING_BUFFER_END(InstanceProperties)
#endif

struct InstancedProperties
{
	fixed4 Color;
	fixed4 Cutoff;

	float4 EmissionColor;
};


/*** Vertex data ***/
struct appdata {
	// GPU instancing
	// VR Single pass instanced rendering
	UNITY_VERTEX_INPUT_INSTANCE_ID
	
	float4 vertex : POSITION;
	float3 normal : NORMAL;
	float4 tangent : TANGENT;
	float2 uv : TEXCOORD0;
	float2 uv1 : TEXCOORD1;
	float2 uv2 : TEXCOORD2;
	float4 color : COLOR0;
};

/*** Vertex interpolators ***/
struct InterpolatorsVertex {
	// GPU instancing
	// VR Single pass instanced rendering
	UNITY_VERTEX_INPUT_INSTANCE_ID
	
	float4 pos : SV_POSITION;
	float4 uv : TEXCOORD0;
	float3 normal : TEXCOORD1;

	#if defined(BINORMAL_PER_FRAGMENT)
		float4 tangent : TEXCOORD2;
	#else
		float3 tangent : TEXCOORD2;
		float3 binormal : TEXCOORD3;
	#endif

	#if FOG_DEPTH
		float4 worldPos : TEXCOORD4;
	#else
		float3 worldPos : TEXCOORD4;
	#endif


	float3 indirectDiffuseLight : TEXCOORD7;
	

	#if defined(LOD_FADE_CROSSFADE)
		#if UNITY_VERSION <= 567
			#if defined(_LODFADEMODE_CROSSFADEDITHERING)
				half3 ditherScreenPosition : TEXCOORD6;
			#endif
		#endif
	#endif
	
	#if defined(BLEND_OPERATION)
		float4 grabScreenPosition : TEXCOORD8;
	#endif

	// VR Single pass stereo rendering
	UNITY_VERTEX_OUTPUT_STEREO
};

/*** Fragment interpolators ***/
struct v2f {
	// GPU instancing
	// VR Single pass instanced rendering
	UNITY_VERTEX_INPUT_INSTANCE_ID
	
	#if defined(LOD_FADE_CROSSFADE)
		UNITY_VPOS_TYPE vpos : VPOS;
	#else
		float4 pos : SV_POSITION;
	#endif

	float4 uv : TEXCOORD0;
	float3 normal : TEXCOORD1;

	#if defined(BINORMAL_PER_FRAGMENT)
		float4 tangent : TEXCOORD2;
	#else
		float3 tangent : TEXCOORD2;
		float3 binormal : TEXCOORD3;
	#endif

	#if FOG_DEPTH
		float4 worldPos : TEXCOORD4;
	#else
		float3 worldPos : TEXCOORD4;
	#endif


	float3 indirectDiffuseLight : TEXCOORD6;
	

	#if defined(LOD_FADE_CROSSFADE)
		#if UNITY_VERSION <= 567
			#if defined(_LODFADEMODE_CROSSFADEDITHERING)
				half3 ditherScreenPosition : TEXCOORD7;
			#endif
		#endif
	#endif
	
	#if defined(BLEND_OPERATION)
		float4 grabScreenPosition : TEXCOORD8;
	#endif

	// VR Single pass stereo rendering
	// VR Single pass instanced rendering
	UNITY_VERTEX_OUTPUT_STEREO
};


// Indirect diffuse light - Vertex diffuse light.
void ComputeVertexLightColor (inout v2f o) {
	#if defined(VERTEXLIGHT_ON)
		o.indirectDiffuseLight = Shade4PointLights(
			unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
			unity_LightColor[0].rgb, unity_LightColor[1].rgb,
			unity_LightColor[2].rgb, unity_LightColor[3].rgb,
			unity_4LightAtten0, o.worldPos.xyz, o.normal
		);
	#endif
}


/*** Vertex function. ***/
InterpolatorsVertex vert (appdata v) {
	InterpolatorsVertex o;

	// GPU instancing
	// VR Single pass instanced rendering
	UNITY_SETUP_INSTANCE_ID(v);
	UNITY_TRANSFER_INSTANCE_ID(v, o);
	

	UNITY_INITIALIZE_OUTPUT(v2f, o);


	// VR Single pass stereo rendering
	// VR Single pass instanced rendering
	UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

	
	o.pos = UnityObjectToClipPos(v.vertex);


	o.worldPos.xyz = mul(unity_ObjectToWorld, v.vertex);
	
	#if FOG_DEPTH
		o.worldPos.w = o.pos.z;
	#endif
	

	o.normal = UnityObjectToWorldNormal(v.normal);

	#if defined(BINORMAL_PER_FRAGMENT)
		o.tangent = float4(UnityObjectToWorldDir(v.tangent.xyz), v.tangent.w);
	#else
		o.tangent = UnityObjectToWorldDir(v.tangent.xyz);
		o.binormal = CreateBinormal(o.normal, o.tangent, v.tangent.w);
	#endif


	o.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);


	// Compute indirect diffuse light (ambient light).
	#if UNITY_SHOULD_SAMPLE_SH
		ComputeVertexLightColor(o);

		//o.indirectDiffuseLight = ShadeSHPerVertex(o.normal, o.indirectDiffuseLight);
	#endif
	

	#if defined(LOD_FADE_CROSSFADE)
		#if UNITY_VERSION <= 567
			#if defined(_LODFADEMODE_CROSSFADEDITHERING)
				o.ditherScreenPosition = ComputeDitherScreenPos(o.pos);
			#endif
		#endif
	#endif


	#if defined(BLEND_OPERATION)
		float4 screenPos = ComputeScreenPos(o.pos);
		o.grabScreenPosition = screenPos;
	#endif

	return o;
}



// Indirect diffuse light - Ambient light.
UnityIndirect CreateIndirectLight (v2f o, float3 viewDir) {
	UnityIndirect indirectLight;
	indirectLight.diffuse = 0;
	indirectLight.specular = 0;

	// Ambient light.
	UnityGIInput dataGI;
	UNITY_INITIALIZE_OUTPUT(UnityGIInput, dataGI);

	#if UNITY_SHOULD_SAMPLE_SH
		dataGI.ambient = o.indirectDiffuseLight;
	#endif

	//UnityGI globalIllumination = UnityGI_Base(dataGI, 1, o.normal);

	//indirectLight.diffuse = globalIllumination.indirect.diffuse;
	indirectLight.diffuse = ShadeSH9(float4(o.normal, 1));

	// End

	return indirectLight;
}

// Fragment function normals.
void InitializeFragmentNormal(inout v2f o) {
	float3 tangentSpaceNormal = GetTangentSpaceNormal(o.uv.xy, _NormalMap, _NormalLevel);
	
	#if defined(BINORMAL_PER_FRAGMENT)
		float3 binormal = CreateBinormal(o.normal, o.tangent.xyz, o.tangent.w);
	#else
		float3 binormal = o.binormal;
	#endif
	
	o.normal = 
		tangentSpaceNormal.x * o.tangent +
		tangentSpaceNormal.y * binormal +
		tangentSpaceNormal.z * o.normal;
	
	o.normal = normalize(o.normal);
}

// Fog application.
float4 ApplyFog (float4 color, v2f o) {
	#if FOG_ON
		float viewDistance = length(_WorldSpaceCameraPos - o.worldPos.xyz);
		
		#if FOG_DEPTH
			viewDistance = UNITY_Z_0_FAR_FROM_CLIPSPACE(o.worldPos.w);
		#endif
		
		UNITY_CALC_FOG_FACTOR_RAW(viewDistance);
		
		float3 fogColor = 0;
		
		#if defined(FORWARD_BASE_PASS)
			fogColor = unity_FogColor.rgb;
		#endif
		
		color.rgb = lerp(fogColor, color.rgb, saturate(unityFogFactor));
	#endif
	
	return color;
}


/*** Fragmen function. ***/
float4 frag (v2f i) : SV_Target {
	
	// GPU instancing.
	UNITY_SETUP_INSTANCE_ID(i);

	// VR Single pass instanced rendering
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);


	// Initialize the instanciated properties struct.
	InstancedProperties insPro;

	#if UNITY_VERSION <= 20172
		insPro.Color = UNITY_ACCESS_INSTANCED_PROP(_Color_arr, _Color);
		insPro.Cutoff = UNITY_ACCESS_INSTANCED_PROP(_Cutoff_arr, _Cutoff);

		insPro.EmissionColor = UNITY_ACCESS_INSTANCED_PROP(_EmissionColor_arr, _EmissionColor);
	#endif

	#if UNITY_VERSION >= 20173
		insPro.Color = UNITY_ACCESS_INSTANCED_PROP(InstanceProperties, _Color);
		insPro.Cutoff = UNITY_ACCESS_INSTANCED_PROP(InstanceProperties, _Cutoff);
		
		insPro.EmissionColor = UNITY_ACCESS_INSTANCED_PROP(InstanceProperties, _EmissionColor);
	#endif
	

	// LOD fade application, dither mode.
	#if defined(LOD_FADE_CROSSFADE)
		#if defined(_LODFADEMODE_CROSSFADEDITHERING)
			#if defined(_LODDITHERTYPE_UNITY)
				#if UNITY_VERSION <= 567
					ApplyDitherCrossFade(i.ditherScreenPosition);
				#else
					UnityApplyDitherCrossFade(i.vpos);
				#endif
			#elif defined(_LODDITHERTYPE_BLUENOISE)
				UnityApplyCustomDitherCrossFade(i.vpos, _BlueNoiseLODDitherCrossfadePattern);
			#elif defined(_LODDITHERTYPE_FLOYDSTEINBERG)
				UnityApplyCustomDitherCrossFade(i.vpos, _FloydSteinbergLODDitherCrossfadePattern);	
			#endif
		#endif
	#endif
		

	float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos.xyz);
	
	
	
	
	// Light data.
	//UnityLight light = CreateLight(i);
	InitializeFragmentNormal(i);
	UnityIndirect indirectLight = CreateIndirectLight(i, viewDir);

	
	// Apply the base color.
	float3 baseColor = GetDiffuse(i.uv.xy, _MainTex, insPro.Color.rgb);
	// End


	// Apply the MatCap.
	float2 matCapUV = mul(UNITY_MATRIX_V, i.normal * float3(0.5,0.5,0.5)) + float3(0.5,0.5,0.5).xy;
	float3 matCap = GetMatCap(matCapUV, _MatCap);
	matCap = lerp(matCap, DesaturateColor(matCap, 1), _DesaturateMatCap);
	baseColor = baseColor * matCap;
	// End


	// Apply the ambient light (indirect diffuse).
	#if _AMBIENTLIGHT_ON
		baseColor = baseColor + indirectLight.diffuse;				
	#endif
	// End


	// Apply the ambient occlusion.
	#if _AMBIENTOCCLUSION_ON
		float occlusion = GetAmbientOcclusion(i.uv.xy, _AmbientOcclusionMap, _AmbientOcclusionLevel);
		baseColor = baseColor * occlusion;				
	#endif
	// End

	
	// Apply all diffuse lights (direct diffuse and indirect diffuse) to the base color.
	fixed4 finalColor;
	finalColor.rgb = baseColor;
	// End

	
	// Apply the opacity.
	fixed alpha = GetAlpha(i.uv.xy, _MainTex, insPro.Color.a);

	// Apply the texture LOD opacity.
	#if defined(LOD_FADE_CROSSFADE)
		#if defined(_LODFADEMODE_CROSSFADEBLENDING)
			//alpha = saturate(alpha * unity_LODFade.x);

			//https://forum.unity.com/threads/how-to-correctly-use-unity_lodfade-x-in-cross-fade-mode.318778/
			float crossfading = max(step(unity_LODFade.x, 0), unity_LODFade.x);
			alpha = alpha * saturate(crossfading + crossfading);
		#endif
	#endif
	// End
	
	// Alpha clip application.
	#if _ALPHATEST_ON
		clip(alpha - insPro.Cutoff);
	#endif
	
	finalColor.a = alpha;
	// End
	
	
	// Blend application.
	#if defined(BLEND_OPERATION)
		float4 screenPos = i.grabScreenPosition;
		float4 grabScreenPos = CGComputeGrabScreenPosNew(screenPos);
		float4 grabScreenColor = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_GrabTexture, grabScreenPos);
		finalColor = CalculateBlendMode(grabScreenColor, finalColor);
	#endif
	// End
	

	// Emission application.
	#ifdef _EMISSION_ON
		finalColor.rgb = finalColor.rgb + GetEmission(i.uv.xy, _EmissionMap, insPro.EmissionColor.rgb);
	#endif
	// End


	// Fog application.
	finalColor = ApplyFog(finalColor, i);

	
	return finalColor;
}

#endif