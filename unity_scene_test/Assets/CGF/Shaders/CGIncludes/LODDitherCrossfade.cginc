// Adding this in a shader replaces the default unity dither functionality on all passes with one that uses a custom dither.
// All you need to do is bang the texture in resources (so that the script can load it always), and include that cginc in any of your shaders which use the "dithercrossfade" generation option.
// https://github.com/keijiro/CrossFadingLod
// https://forum.unity.com/threads/blue-noise-crossfade-dither-lod.538905/#post-3561053


// ------------------------------------------------------------------
//  Dither LOD cross fade helpers
#ifndef CUSTOM_DITHER_CROSSFADE_INCLUDED
#define CUSTOM_DITHER_CROSSFADE_INCLUDED


//UNITY_SHADER_NO_UPGRADE


	sampler2D _BlueNoiseLODDitherCrossfadePattern;
	float4 _BlueNoiseLODDitherCrossfadePattern_TexelSize;

	sampler2D _FloydSteinbergLODDitherCrossfadePattern;
	float4 _FloydSteinbergLODDitherCrossfadePattern_TexelSize;

	#ifdef LOD_FADE_CROSSFADE
		// Undefine unity implementation and define our own
		#undef UNITY_APPLY_DITHER_CROSSFADE
		#define UNITY_APPLY_DITHER_CROSSFADE(vpos) UnityApplyCustomDitherCrossFade(vpos)

		// Custom Dither LOD cross fade.
		void UnityApplyCustomDitherCrossFade(float2 vpos, sampler2D ditherPattern)
		{
			vpos /= 16; // the dither mask texture is 16x16
			vpos.y = frac(vpos.y) * 0.0625 + unity_LODFade.y; // quantized lod fade by 16 levels
			clip(tex2D(ditherPattern, vpos).r - 0.5);
		}

	#else
		#define UNITY_APPLY_DITHER_CROSSFADE(vpos)
	#endif

#endif

