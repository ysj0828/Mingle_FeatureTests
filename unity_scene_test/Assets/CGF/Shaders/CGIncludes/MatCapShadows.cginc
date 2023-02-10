// Upgrade NOTE: upgraded instancing buffer 'InstanceProperties' to new syntax.

///
/// INFORMATION
/// 
/// Project: Chloroplast Games Framework
/// Game: Chloroplast Games Framework
/// Date: 09/10/2020
/// Author: Chloroplast Games
/// Website: http://www.chloroplastgames.com
/// Programmers: Pau Elias Soriano
/// Description: MatCap shadow (shadowcaster) pass.
///

#if !defined(MATCAP_SHADOWS_INCLUDED)
#define MATCAP_SHADOWS_INCLUDED


/*** Includes ***/
#include "UnityCG.cginc"
#if !defined(CG_HELPERS_INCLUDED) 
	#include "Assets/CGF/Shaders/CGIncludes/CGHelpers.cginc"
#endif


/*** Defines ***/
#if defined(_ALPHABLEND_ON)
	#if defined(_SEMITRANSPARENTSHADOWS_ON)
		#define SHADOWS_SEMITRANSPARENT 1
	#endif
#endif

#if SHADOWS_SEMITRANSPARENT || defined(_ALPHATEST_ON) || defined(_ALPHABLEND_ON)
	#define SHADOWS_NEED_UV 1
#endif


/*** Variables ***/
//float4 _Color;
sampler2D _MainTex;
float4 _MainTex_ST;
//float _Cutoff;

sampler3D _DitherMaskLOD;

// Instance properties.
#if UNITY_VERSION <= 20172
	UNITY_INSTANCING_BUFFER_START(InstanceProperties)
		UNITY_DEFINE_INSTANCED_PROP(fixed4, _Color)
#define _Color_arr InstanceProperties
		UNITY_DEFINE_INSTANCED_PROP(fixed, _Cutoff)
#define _Cutoff_arr InstanceProperties
	UNITY_INSTANCING_BUFFER_END(InstanceProperties)
#endif

#if UNITY_VERSION >= 20173
	UNITY_INSTANCING_BUFFER_START(InstanceProperties)
		UNITY_DEFINE_INSTANCED_PROP(fixed4, _Color)
		UNITY_DEFINE_INSTANCED_PROP(fixed, _Cutoff)
	UNITY_INSTANCING_BUFFER_END(InstanceProperties)
#endif

struct InstancedProperties
{
	fixed4 Color;
	fixed4 Cutoff;
};


/*** Mesh data ***/
struct appdata {
	// GPU instancing
	// VR Single pass instanced rendering
	UNITY_VERTEX_INPUT_INSTANCE_ID

	float4 position : POSITION;
	float3 normal : NORMAL;
	float2 uv : TEXCOORD0;
};

/*** Vertex interpolators ***/
struct InterpolatorsVertex {
	// GPU instancing
	// VR Single pass instanced rendering
	UNITY_VERTEX_INPUT_INSTANCE_ID

	float4 position : SV_POSITION;
	
	#if SHADOWS_NEED_UV
		float2 uv : TEXCOORD0;
	#endif
	
	#if defined(SHADOWS_CUBE)
		float3 lightVec : TEXCOORD1;
	#endif
	
	#if defined(LOD_FADE_CROSSFADE)
		#if UNITY_VERSION <= 567
			#if defined(_LODFADEMODE_CROSSFADEDITHERING)
				half3 ditherScreenPosition : TEXCOORD2;
			#endif
		#endif
	#endif

	// VR Single pass stereo rendering
	UNITY_VERTEX_OUTPUT_STEREO
};

/*** Fragment interpolators ***/
struct v2f {
	// GPU instancing
	// VR Single pass instanced rendering
	UNITY_VERTEX_INPUT_INSTANCE_ID

	#if SHADOWS_SEMITRANSPARENT || defined(LOD_FADE_CROSSFADE)
		UNITY_VPOS_TYPE vpos : VPOS;
	#else
		float4 positions : SV_POSITION;
	#endif

	#if SHADOWS_NEED_UV
		float2 uv : TEXCOORD0;
	#endif
	
	#if defined(SHADOWS_CUBE)
		float3 lightVec : TEXCOORD1;
	#endif
	
	#if defined(LOD_FADE_CROSSFADE)
		#if UNITY_VERSION <= 567
			#if defined(_LODFADEMODE_CROSSFADEDITHERING)
				half3 ditherScreenPosition : TEXCOORD2;
			#endif
		#endif
	#endif

	// VR Single pass stereo rendering
	// VR Single pass instanced rendering
	UNITY_VERTEX_OUTPUT_STEREO
};


/*** Vertex function ***/
InterpolatorsVertex vertShadow (appdata v) {
	InterpolatorsVertex i;
	
	// GPU instancing
	// VR Single pass instanced rendering
	UNITY_SETUP_INSTANCE_ID(v);
	UNITY_TRANSFER_INSTANCE_ID(v, i);


	UNITY_INITIALIZE_OUTPUT(InterpolatorsVertex, i);


	// VR Single pass stereo rendering
	// VR Single pass instanced rendering
	UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
	
	#if defined(SHADOWS_CUBE)
		i.position = UnityObjectToClipPos(v.position);
		i.lightVec = mul(unity_ObjectToWorld, v.position).xyz - _LightPositionRange.xyz;
	#else
		i.position = UnityClipSpaceShadowCasterPos(v.position.xyz, v.normal);
		i.position = UnityApplyLinearShadowBias(i.position);
	#endif

	#if SHADOWS_NEED_UV
		i.uv = TRANSFORM_TEX(v.uv, _MainTex);
	#endif
	
	#if defined(LOD_FADE_CROSSFADE)
		#if UNITY_VERSION <= 567
			#if defined(_LODFADEMODE_CROSSFADEDITHERING)
				i.ditherScreenPosition = ComputeDitherScreenPos(i.position);
			#endif
		#endif
	#endif
	
	return i;
}



/*** Fragment function ***/
float4 fragShadow (v2f i) : SV_TARGET {
	// GPU instancing.
	// VR Single pass instanced rendering
	UNITY_SETUP_INSTANCE_ID(i);
	// VR Single pass instanced rendering
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);


	// Initialize the instanciated properties struct.
	InstancedProperties insPro;

	#if UNITY_VERSION <= 20172
		insPro.Color = UNITY_ACCESS_INSTANCED_PROP(_Color_arr, _Color);
		insPro.Cutoff = UNITY_ACCESS_INSTANCED_PROP(_Cutoff_arr, _Cutoff);
	#endif

	#if UNITY_VERSION >= 20173
		insPro.Color = UNITY_ACCESS_INSTANCED_PROP(InstanceProperties, _Color);
		insPro.Cutoff = UNITY_ACCESS_INSTANCED_PROP(InstanceProperties, _Cutoff);
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

	
	// Apply opacity.
	float alpha = insPro.Color.a;

	#if SHADOWS_NEED_UV
		alpha = alpha * tex2D(_MainTex, i.uv).a;
	#endif

	// Clip completely the shadow if the opacity of the mesh is too low.
	#if defined(_ALPHABLEND_ON)
		clip(alpha - 0.1);
	#endif

	#if defined(_ALPHATEST_ON)
		clip(alpha - insPro.Cutoff);
	#endif
	// End

	#if SHADOWS_SEMITRANSPARENT
		float dither = tex3D(_DitherMaskLOD, float3(i.vpos.xy * 0.25, alpha * 0.9375)).a;
		clip(dither - 0.01);
	#endif
	
	#if defined(SHADOWS_CUBE)
		float depth = length(i.lightVec) + unity_LightShadowBias.x;
		depth = depth * _LightPositionRange.w;
		return UnityEncodeCubeShadowDepth(depth);
	#else
		return 0;
	#endif
}

#endif