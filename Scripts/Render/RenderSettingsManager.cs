using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class RenderSettingsManager
    {
        #region Const

        private const int NativeScreenWidth = 640;
        private const int NativeScreenHeight = 360;

        private const string FirstLaunchKey = "FirstLaunch";
        private const string WindowScaleKey = "WindowScale";
        private const string VSyncCountKey = "VSyncCount";

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        private RenderSettingsManager()
        {
            _fullscreen = Screen.fullScreen;
            windowScale = scaleFromResolution;
#if !UNITY_EDITOR
            if (PlayerPrefs.GetInt(FirstLaunchKey, 1) != 0)
            {
                _fullscreen = true;
                windowScale = scaleFromResolution;
                PlayerPrefs.SetInt(FirstLaunchKey, 0);
            }
#endif // !UNITY_EDITOR
            verticalSync = PlayerPrefs.GetInt(VSyncCountKey, 1);
        }

#endregion

#region Methods

        /// <summary>
        /// 
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static void Initialize()
        {
            _instance = new RenderSettingsManager();
        }

#endregion

#region Properties

        /// <summary>
        /// 
        /// </summary>
        public static RenderSettingsManager instance => _instance;

        /// <summary>
        /// 
        /// </summary>
        public bool fullscreen
        {
            get 
            {
                return _fullscreen;
            }
            set 
            {
                if(value)
                {
                    windowScale = fittestScale;
                }

                _fullscreen = Screen.fullScreen = value;
                PlayerPrefs.Save();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int windowScale
        {
            get
            {
                return _windowScale;
            }
            set
            {
                _windowScale = value;
                Screen.SetResolution(NativeScreenWidth * value, NativeScreenHeight * value, fullscreen);
                LightRenderer.RenderScale = _windowScale;
                PlayerPrefs.Save();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int verticalSync
        {
            get
            {
                return QualitySettings.vSyncCount;
            }
            set
            {

                QualitySettings.vSyncCount = value;
                PlayerPrefs.SetInt(VSyncCountKey, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int scaleFromResolution
        {
            get 
            {
                var resolutionWidth = Screen.width;
                return Mathf.FloorToInt(resolutionWidth / NativeScreenWidth);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int fittestScale
        {
            get
            {
                var highestWidth = Screen.resolutions.Max(x => x.width);
                return Mathf.FloorToInt(highestWidth / NativeScreenWidth);
            }
        }

#endregion

#region Fields

        private static RenderSettingsManager _instance;
        private bool _fullscreen;
        private int _windowScale = 1;

#endregion

    }
}
