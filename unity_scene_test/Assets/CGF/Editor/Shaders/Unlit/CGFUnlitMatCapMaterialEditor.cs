///
/// INFORMATION
/// 
/// Project: Chloroplast Games Framework
/// Game: Chloroplast Games Framework
/// Date: 01/10/2020
/// Author: Chloroplast Games
/// Website: http://www.chloroplastgames.com
/// Programmers: Pau Elias Soriano
/// Description: Material editor of the shader Unlit/MatCap.
///

using UnityEngine;
using UnityEditor;
//using CGF.Systems.Shaders.Unlit;

/// \english
/// <summary>
/// Material editor of the shader Unlit/MatCap.
/// </summary>
/// \endenglish
/// \spanish
/// <summary>
/// Editor del material del shader Unlit/MatCap.
/// </summary>
/// \endspanish
public class CGFUnlitMatCapMaterialEditor : CGFMaterialEditorClass
{

    #region Private Variables

        private bool _compactMode;
        private bool _renderModeHeaderGroup = true;
        private bool _mainHeaderGroup = true;
        private bool _matcapHeaderGroup = true;       
        private bool _ambientOcclusionHeaderGroup;
        private bool _normalHeaderGroup;
        private bool _emissionHeaderGroup;
        private bool _lodFadeModeHeaderGroup;
        private bool _stencilOptionsHeaderGroup;
        private bool _otherSettingsHeaderGroup;
        
        // General
        MaterialProperty _MainTex;
        MaterialProperty _SemitransparentShadows;
        MaterialProperty _Cutoff;
        MaterialProperty _Color;

        // MatCap
        MaterialProperty _MatCap;
        MaterialProperty _DesaturateMatCap;

        // Ambient Occlusion
        MaterialProperty _AmbientOcclusion;
        MaterialProperty _AmbientOcclusionMap;
        MaterialProperty _AmbientOcclusionLevel;

        // Ambient Light
        MaterialProperty _AmbientLight;

        // Normal
        MaterialProperty _Normal;
        MaterialProperty _NormalMap;
        MaterialProperty _NormalLevel;

        // Emission
        MaterialProperty _Emission;
        MaterialProperty _EmissionMap;
        MaterialProperty _EmissionColor;

        // LOD Fade
        MaterialProperty _LODFadeMode;
        MaterialProperty _LODDitherType;

        // Stencil Options
        MaterialProperty _StencilReference;
        MaterialProperty _StencilReadMask;
        MaterialProperty _StencilWriteMask;
        MaterialProperty _StencilComparisonFunction;
        MaterialProperty _StencilPassOperation;
        MaterialProperty _StencilFailOperation;
        MaterialProperty _StencilZFailOperation;

        // Render Mode
        MaterialProperty _RenderMode;
        MaterialProperty _Cull;

    #endregion


    #region Main Methods

        protected override void GetProperties()
        {

            // General
            _MainTex = FindProperty("_MainTex");
            _SemitransparentShadows = FindProperty("_SemitransparentShadows");
            _Cutoff = FindProperty("_Cutoff");
            _Color = FindProperty("_Color");

            // MatCap
            _MatCap = FindProperty("_MatCap");
            _DesaturateMatCap = FindProperty("_DesaturateMatCap");

            // Ambient Occlusion
            _AmbientOcclusion = FindProperty("_AmbientOcclusion");
            _AmbientOcclusionMap = FindProperty("_AmbientOcclusionMap");
            _AmbientOcclusionLevel = FindProperty("_AmbientOcclusionLevel");

            // Ambient Light
            _AmbientLight = FindProperty("_AmbientLight");

            // Normal
            _Normal = FindProperty("_Normal");
            _NormalMap = FindProperty("_NormalMap");
            _NormalLevel = FindProperty("_NormalLevel");

            // Emission
            _Emission = FindProperty("_Emission");
            _EmissionMap = FindProperty("_EmissionMap");
            _EmissionColor = FindProperty("_EmissionColor");

            // LOD Fade
            _LODFadeMode = FindProperty("_LODFadeMode");
            _LODDitherType = FindProperty("_LODDitherType");

            // Stencil Options
            _StencilReference = FindProperty("_StencilReference");
            _StencilReadMask = FindProperty("_StencilReadMask");
            _StencilWriteMask = FindProperty("_StencilWriteMask");
            _StencilComparisonFunction = FindProperty("_StencilComparisonFunction");
            _StencilPassOperation = FindProperty("_StencilPassOperation");
            _StencilFailOperation = FindProperty("_StencilFailOperation");
            _StencilZFailOperation = FindProperty("_StencilZFailOperation");
            
            // Render Mode
            _RenderMode = FindProperty("_RenderMode");
            _Cull = FindProperty("_Cull");
        }

        public override void Awake() {

            base.Awake();

            CGFMaterialEditorUtilitiesClass.SetInitialRenderMode(this.target as Material, "opaque");

        }

        protected override void InspectorGUI()
        {

            // Render Settings
            float useAlpha = _RenderMode.floatValue == 2 ? 1 : 0;

            float useAlphaClip =_RenderMode.floatValue == 1 ? 1 : 0;

            float useAlphaAndAlphaClip = _RenderMode.floatValue == 1 || _RenderMode.floatValue == 2 ? 1 : 0;

            //CGFMaterialEditorUtilitiesClass.BuildMaterialComponent(typeof(CGFUnlitMatCapBehavior));

            CGFMaterialEditorUtilitiesClass.BuildMaterialTools("http://chloroplastgames.com/cg-framework-user-manual/");

            CGFMaterialEditorUtilitiesClass.ManageMaterialValues(this);

            _compactMode = CGFMaterialEditorUtilitiesClass.BuildTextureCompactMode(_compactMode, m_HeaderStateKey);

            GUILayout.Space(5);

            _renderModeHeaderGroup = CGFMaterialEditorUtilitiesClass.BuildFoldOutHeaderGroup("Render Mode", "Rendering mode.", true, _renderModeHeaderGroup, m_HeaderStateKey);

            if (_renderModeHeaderGroup)
            {
                CGFMaterialEditorUtilitiesClass.BuildRenderModeEnum(_RenderMode, this, false);
                CGFMaterialEditorUtilitiesClass.BuildRenderFace(_Cull);
            }

            GUILayout.Space(5);

            // General
            _mainHeaderGroup = CGFMaterialEditorUtilitiesClass.BuildFoldOutHeaderGroup("Main", "Main features.", true, _mainHeaderGroup, m_HeaderStateKey);

            if (_mainHeaderGroup)
            {
                CGFMaterialEditorUtilitiesClass.BuildTexture("Diffuse Map " + CGFMaterialEditorUtilitiesExtendedClass.CheckRenderModeStandard(_RenderMode.floatValue), "Main texture.", _MainTex, this, true, _compactMode);
                CGFMaterialEditorUtilitiesClass.BuildKeyword("Use Semitransparent Shadows", "Enable the semitransparent shadows, apply a dithering to the sahdows based on the opacity of the mesh, only works with transparent render mode.", _SemitransparentShadows, true, useAlpha);
                CGFMaterialEditorUtilitiesClass.BuildSlider("Alpha Cutoff", "Alpha Cutoff value.", _Cutoff, useAlphaClip);
                CGFMaterialEditorUtilitiesClass.BuildColor("Color " + CGFMaterialEditorUtilitiesExtendedClass.CheckRenderModeStandard(_RenderMode.floatValue), "Main color.", _Color);
            }

            GUILayout.Space(5);

            // MatCap
            CGFMaterialEditorUtilitiesExtendedClass.BuildMatCap(_MatCap, _DesaturateMatCap, this, false, _matcapHeaderGroup, m_HeaderStateKey, _compactMode);

            GUILayout.Space(5);

            // Ambient Occlusion
            CGFMaterialEditorUtilitiesExtendedClass.BuildAmbientOcclusion(_AmbientOcclusion, _AmbientOcclusionMap, _AmbientOcclusionLevel, this, false, _ambientOcclusionHeaderGroup, m_HeaderStateKey, _compactMode);
 
            GUILayout.Space(5);

            // Ambient Light
            CGFMaterialEditorUtilitiesExtendedClass.BuildAmbientLight(_AmbientLight, _ambientOcclusionHeaderGroup, m_HeaderStateKey);

            GUILayout.Space(5);

            // Normal
            CGFMaterialEditorUtilitiesExtendedClass.BuildNormal(_Normal, _NormalMap, _NormalLevel, this, false, _normalHeaderGroup, m_HeaderStateKey,_compactMode);

            GUILayout.Space(5);

            // Emission
            CGFMaterialEditorUtilitiesExtendedClass.BuildEmissionStandard(_Emission, _EmissionMap, _EmissionColor, this, _emissionHeaderGroup, m_HeaderStateKey);

            GUILayout.Space(5);

            // LOD Fade
            CGFMaterialEditorUtilitiesExtendedClass.BuildLODFade(_LODFadeMode, _LODDitherType, this, _lodFadeModeHeaderGroup, m_HeaderStateKey);

            GUILayout.Space(5);

            // Stencil Mask
            CGFMaterialEditorUtilitiesExtendedClass.BuildStencilOptions(_StencilReference, _StencilReadMask, _StencilWriteMask, _StencilComparisonFunction, _StencilPassOperation, _StencilFailOperation, _StencilZFailOperation, this, _stencilOptionsHeaderGroup, m_HeaderStateKey);

            GUILayout.Space(5);

            CGFMaterialEditorUtilitiesClass.BuildOtherSettings(true, true, false, false, this, _otherSettingsHeaderGroup, m_HeaderStateKey);
        }
    #endregion
}