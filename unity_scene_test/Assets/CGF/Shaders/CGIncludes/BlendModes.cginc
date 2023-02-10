///
/// INFORMATION
/// 
/// Project: Chloroplast Games Framework
/// Game: Chloroplast Games Framework
/// Date: 19/03/2018
/// Author: Chloroplast Games
/// Website: http://www.chloroplastgames.com
/// Programmers: Pau Elias Soriano
/// Description: Blend mode helper functions.
///

#ifndef BLEND_MODES_INCLUDED
#define BLEND_MODES_INCLUDED

#if !defined(CG_HELPERS_INCLUDED) 
#include "Assets/CGF/Shaders/CGIncludes/CGHelpers.cginc"
#endif

	/// \english
    /// <summary>
    /// Calcule the color of blend modes.
    /// </summary>
    /// <param name="sourceColor">Source color.</param>
	/// <param name="destinationColor">Destination color.</param>
	/// <returns>Color of blend mode.</returns>
    /// \endenglish
    /// \spanish
    /// <summary>
    /// Calcula el color de los modos de fusión.
    /// </summary>
    /// <param name="sourceColor">Color fuente.</param>
	/// <param name="destinationColor">Color de destino.</param>
	/// <returns>Color del modo de fusión.</returns>
    /// \endspanish
	fixed4 CalculateBlendMode (fixed4 sourceColor, fixed4 destinationColor)
	{ 
		
		// Normal
		#ifdef _BLENDMODE_NORMAL

			fixed4 result = destinationColor;

			return result;

		// Darken
		#elif _BLENDMODE_DARKEN

			fixed4 result = min(sourceColor, destinationColor);

			result.a = destinationColor.a;

			return result;

		#elif _BLENDMODE_MULTIPLY

			fixed4 result = sourceColor * destinationColor;

			result.a = destinationColor.a;

			return result;

		#elif _BLENDMODE_COLORBURN

			fixed4 result = 1.0 - (1.0 - sourceColor) / destinationColor;
			//fixed4 result = 1.0 - (1.0 - destinationColor) / sourceColor;

			result.a = destinationColor.a;

			return saturate(result);

		#elif _BLENDMODE_LINEARBURN

			fixed4 result = sourceColor + destinationColor - 1.0;

			result.a = destinationColor.a;

			return result;

		#elif _BLENDMODE_DARKERCOLOR

			fixed4 result = DesaturateColorSimple(sourceColor) < DesaturateColorSimple(destinationColor) ? sourceColor : destinationColor;

			result.a = destinationColor.a;

			return result; 

		// Lighten
		#elif _BLENDMODE_LIGHTEN

			fixed4 result = max(sourceColor, destinationColor);

			result.a = destinationColor.a;

			return result;

		#elif _BLENDMODE_SCREEN

			fixed4 result = 1.0 - (1.0 - sourceColor) * (1.0 - destinationColor);

			result.a = destinationColor.a;

			return result;

		#elif _BLENDMODE_COLORDODGE

			fixed4 result = sourceColor / (1.0 - destinationColor);

			result.a = destinationColor.a;

			return saturate(result);

		#elif _BLENDMODE_LINEARDODGE

			fixed4 result = sourceColor + destinationColor;

			result.a = destinationColor.a;

			return result;

		#elif _BLENDMODE_LIGHTERCOLOR

			fixed4 result = DesaturateColorSimple(sourceColor) > DesaturateColorSimple(destinationColor) ? sourceColor : destinationColor;

			result.a = destinationColor.a;

			return result; 

		// Contrast
		#elif _BLENDMODE_OVERLAY

			fixed4 result = sourceColor > 0.5 ? 1.0 - 2.0 * (1.0 - sourceColor) * (1.0 - destinationColor) : 2.0 * sourceColor * destinationColor;
			result.a = destinationColor.a;
			return result;

		#elif _BLENDMODE_SOFTLIGHT

			fixed4 result = (1.0 - sourceColor) * sourceColor * destinationColor + sourceColor * (1.0 - (1.0 - sourceColor) * (1.0 - destinationColor));

			result.a = destinationColor.a;

			return result;

		#elif _BLENDMODE_HARDLIGHT

			fixed4 result = destinationColor > 0.5 ? 1.0 - (1.0 - sourceColor) * (1.0 - 2.0 * (destinationColor - 0.5)) : sourceColor * (2.0 * destinationColor);

			result.a = destinationColor.a;

			return result;

		#elif _BLENDMODE_VIVIDLIGHT

			fixed4 result = destinationColor > 0.5 ? sourceColor / (1.0 - (destinationColor - 0.5) * 2.0) : 1.0 - (1.0 - sourceColor) / (destinationColor * 2.0);

			result.a = destinationColor.a;

			return saturate(result);

		#elif _BLENDMODE_LINEARLIGHT

			fixed4 result = destinationColor > 0.5 ? sourceColor + 2.0 * (destinationColor - 0.5) : sourceColor + 2.0 * destinationColor - 1.0;

			result.a = destinationColor.a;

			return result;

		#elif _BLENDMODE_PINLIGHT

			fixed4 result = destinationColor > 0.5 ? max(sourceColor, 2.0 * (destinationColor - 0.5)) : min(sourceColor, 2.0 * destinationColor);

			result.a = destinationColor.a;

			return result;

		#elif _BLENDMODE_HARDMIX

			fixed4 result = (destinationColor > 1.0 - sourceColor) ? 1.0 : 0.0;

			result.a = destinationColor.a;

			return result;

		// Inversion
		#elif _BLENDMODE_DIFFERENCE

			fixed4 result = abs(sourceColor - destinationColor);

			result.a = destinationColor.a;

			return result;

		#elif _BLENDMODE_EXCLUSION

			fixed4 result = sourceColor + destinationColor - 2.0 * sourceColor * destinationColor;

			result.a = destinationColor.a;

			return result; 

		#elif _BLENDMODE_SUBTRACT

			fixed4 result = sourceColor - destinationColor;

			result.a = destinationColor.a;

			return result;

		#elif _BLENDMODE_DIVIDE

			fixed4 result = sourceColor / destinationColor;

			result.a = destinationColor.a;

			return result;

		// Component
		#elif _BLENDMODE_HUE

			fixed3 aHsl = RgbToHsl(sourceColor.rgb);

			fixed3 bHsl = RgbToHsl(destinationColor.rgb);

			fixed3 rHsl = fixed3(bHsl.x, aHsl.y, aHsl.z);

			return fixed4(HslToRgb(rHsl), destinationColor.a);

		#elif _BLENDMODE_SATURATION

			fixed3 aHsl = RgbToHsl(sourceColor.rgb);

			fixed3 bHsl = RgbToHsl(destinationColor.rgb);

			fixed3 rHsl = fixed3(aHsl.x, bHsl.y, aHsl.z);

			return fixed4(HslToRgb(rHsl), destinationColor.a);

		#elif _BLENDMODE_COLOR

			fixed3 aHsl = RgbToHsl(sourceColor.rgb);

			fixed3 bHsl = RgbToHsl(destinationColor.rgb);

			fixed3 rHsl = fixed3(bHsl.x, bHsl.y, aHsl.z);

			return fixed4(HslToRgb(rHsl), destinationColor.a);

		#elif _BLENDMODE_LUMINOSITY

			fixed3 aHsl = RgbToHsl(sourceColor.rgb);

			fixed3 bHsl = RgbToHsl(destinationColor.rgb);

			fixed3 rHsl = fixed3(aHsl.x, aHsl.y, bHsl.z);

			return fixed4(HslToRgb(rHsl), destinationColor.a);

		#else

			return destinationColor;

		#endif

	}

	/// \english
    /// <summary>
    /// Calcule the color of blend modes without keywords.
    /// </summary>
    /// <param name="sourceColor">Source color.</param>
	/// <param name="destinationColor">Destination color.</param>
	/// <param name="operationIndex">Index of the blend operation</param>
	/// <returns>Color of blend mode.</returns>
    /// \endenglish
    /// \spanish
    /// <summary>
    /// Calcula el color de los modos de fusión sin keywords.
    /// </summary>
    /// <param name="sourceColor">Color fuente.</param>
	/// <param name="destinationColor">Color de destino.</param>
	/// <param name="operationIndex">Índice de la operación de fusión.</param>
	/// <returns>Color del modo de fusión.</returns>
    /// \endspanish
	fixed4 CalculateBlendModeWithoutKeyword (fixed4 sourceColor, fixed4 destinationColor, fixed operationIndex)
	{
		fixed4 result = sourceColor;

		fixed3 aHsl;
		fixed3 bHsl;
		fixed3 rHsl;

		UNITY_BRANCH
		switch(operationIndex)
		{
		case 0:
			result = destinationColor;
			break;
		case 1:
			result = min(sourceColor, destinationColor);
			result.a = destinationColor.a;
			break;
		case 2:
			result = sourceColor * destinationColor;
			result.a = destinationColor.a;
			break;
		case 3:
			result = 1.0 - (1.0 - sourceColor) / destinationColor;
			result.a = destinationColor.a;
			break;
		case 4:
			result = sourceColor + destinationColor - 1.0;
			result.a = destinationColor.a;
			break;
		case 5:
			result = DesaturateColorSimple(sourceColor) < DesaturateColorSimple(destinationColor) ? sourceColor : destinationColor;
			result.a = destinationColor.a;
			break;
		case 6:
			result = max(sourceColor, destinationColor);
			result.a = destinationColor.a;
			break;
		case 7:
			result = 1.0 - (1.0 - sourceColor) * (1.0 - destinationColor);
			result.a = destinationColor.a;
			break;
		case 8:
			result = sourceColor / (1.0 - destinationColor);
			result.a = destinationColor.a;
			break;
		case 9:
			result = sourceColor + destinationColor;
			result.a = destinationColor.a;
			break;
		case 10:
			result = DesaturateColorSimple(sourceColor) > DesaturateColorSimple(destinationColor) ? sourceColor : destinationColor;
			result.a = destinationColor.a;
			break;
		case 11:
			result = sourceColor > 0.5 ? 1.0 - 2.0 * (1.0 - sourceColor) * (1.0 - destinationColor) : 2.0 * sourceColor * destinationColor;
			result.a = destinationColor.a;
			break;
		case 12:
			result = (1.0 - sourceColor) * sourceColor * destinationColor + sourceColor * (1.0 - (1.0 - sourceColor) * (1.0 - destinationColor));
			result.a = destinationColor.a;
			break;
		case 13:
			result = destinationColor > 0.5 ? 1.0 - (1.0 - sourceColor) * (1.0 - 2.0 * (destinationColor - 0.5)) : sourceColor * (2.0 * destinationColor);
			result.a = destinationColor.a;
			break;
		case 14:
			result = destinationColor > 0.5 ? sourceColor / (1.0 - (destinationColor - 0.5) * 2.0) : 1.0 - (1.0 - sourceColor) / (destinationColor * 2.0);
			result.a = destinationColor.a;
			break;
		case 15:
			result = destinationColor > 0.5 ? sourceColor + 2.0 * (destinationColor - 0.5) : sourceColor + 2.0 * destinationColor - 1.0;
			result.a = destinationColor.a;
			break;
		case 16:
			result = destinationColor > 0.5 ? max(sourceColor, 2.0 * (destinationColor - 0.5)) : min(sourceColor, 2.0 * destinationColor);
			result.a = destinationColor.a;
			break;
		case 17:
			result = (destinationColor > 1.0 - sourceColor) ? 1.0 : 0.0;
			result.a = destinationColor.a;
			break;
		case 18:
			result = abs(sourceColor - destinationColor);
			result.a = destinationColor.a;
			break;
		case 19:
			result = sourceColor + destinationColor - 2.0 * sourceColor * destinationColor;
			result.a = destinationColor.a;
			break;
		case 20:
			result = sourceColor - destinationColor;
			result.a = destinationColor.a;
			break;
		case 21:
			result = sourceColor / destinationColor;
			result.a = destinationColor.a;
			break;
		case 22:
			aHsl = RgbToHsl(sourceColor.rgb);
			bHsl = RgbToHsl(destinationColor.rgb);
			rHsl = fixed3(bHsl.x, aHsl.y, aHsl.z);
			result = fixed4(HslToRgb(rHsl), destinationColor.a);
			break;
		case 23:
			aHsl = RgbToHsl(sourceColor.rgb);
			bHsl = RgbToHsl(destinationColor.rgb);
			rHsl = fixed3(aHsl.x, bHsl.y, aHsl.z);
			result = fixed4(HslToRgb(rHsl), destinationColor.a);
			break;
		case 24:
			aHsl = RgbToHsl(sourceColor.rgb);
			bHsl = RgbToHsl(destinationColor.rgb);
			rHsl = fixed3(bHsl.x, bHsl.y, aHsl.z);
			result = fixed4(HslToRgb(rHsl), destinationColor.a);
			break;
		case 25:
			aHsl = RgbToHsl(sourceColor.rgb);
			bHsl = RgbToHsl(destinationColor.rgb);
			rHsl = fixed3(aHsl.x, aHsl.y, bHsl.z);
			result = fixed4(HslToRgb(rHsl), destinationColor.a);
			break;
		default:
			return destinationColor;
		}

		return result;
	}

#endif