///
/// INFORMATION
/// 
/// Project: Chloroplast Games Framework
/// Game: Chloroplast Games Framework
/// Date: 09/05/2018
/// Author: Chloroplast Games
/// Web: http://www.chloroplastgames.com
/// Programmers: David Cuenca
/// Description: Manager that allows manage global shader properties that should exist in every single rendering state, editor and runtime.
///

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace CGF.Systems.Shaders.Managers
{
    /// \english
    /// <summary>
    /// Manager that allows manage global shader properties that should exist in every single rendering state, editor and runtime.
    /// </summary>
    /// \endenglish
    /// \spanish
    /// <summary>
    /// Gestor que permite gestionar las propieades globales de los shaders que deben existir en todos los estado de renderizado, editor y en tiempo de ejecución.
    /// </summary>
    /// \endspanish
    #if UNITY_EDITOR
    [InitializeOnLoad]
    #endif
    public static class CGFGlobalShaderPropertyManager
    {

        #if UNITY_EDITOR
            static CGFGlobalShaderPropertyManager()
            {
                SetGlobalShaderProperties();
            }
        #endif

        #region Public Variables
            private static Texture2D _blueNoiseLODDitherCrossfadePattern;

            private static Texture2D _floydSteinbergLODDitherCrossfadePattern;
        #endregion


        #region Private Variables
            public static Texture2D BlueNoiseLODDitherCrossfadePattern
            {
                get
                {
                    if (!_blueNoiseLODDitherCrossfadePattern){
                        _blueNoiseLODDitherCrossfadePattern = Resources.Load<Texture2D>("Shaders/GlobalProperties/bluenoise_loddithercrossfade_pattern");
                    }
                        
                    return _blueNoiseLODDitherCrossfadePattern;
                }
            }

            public static Texture2D FloydSteinbergLODDitherCrossfadePattern
            {
                get
                {
                    if (!_floydSteinbergLODDitherCrossfadePattern){
                        _floydSteinbergLODDitherCrossfadePattern = Resources.Load<Texture2D>("Shaders/GlobalProperties/floydsteinberg_loddithercrossfade_pattern");
                    }
                        
                    return _floydSteinbergLODDitherCrossfadePattern;
                }
            }
        #endregion


        #region Main Methods
            [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
            static void OnBeforeSceneLoadRuntimeMethod()
            {

                SetGlobalShaderProperties();

            }
        #endregion


        #region Utility Methods
            public static void SetGlobalShaderProperties()
            {

                Shader.SetGlobalTexture("_BlueNoiseLODDitherCrossfadePattern", BlueNoiseLODDitherCrossfadePattern);
                Shader.SetGlobalTexture("_FloydSteinbergLODDitherCrossfadePattern", FloydSteinbergLODDitherCrossfadePattern);

            }
        #endregion


        #region Utility Events

        #endregion
    }
}
