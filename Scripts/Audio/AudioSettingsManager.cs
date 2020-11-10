using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class AudioSettingsManager
    {
        #region Const

        private const string MusicVolumeKey = "MusicVolume";
        private const string SFXVolumeKey = "SFXVolume";

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        private AudioSettingsManager()
        {
            _musicVCA = RuntimeManager.GetVCA("vca:/Music");
            _sfxVCA = RuntimeManager.GetVCA("vca:/SFX");

            musicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 1.0f);
            sfxVolume = PlayerPrefs.GetFloat(SFXVolumeKey, 1.0f);
        }

        #endregion

        #region Methods
        
        /// <summary>
        /// 
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static void Initialize()
        {
            _instance = new AudioSettingsManager();
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public static AudioSettingsManager instance => _instance;

        /// <summary>
        /// 
        /// </summary>
        public float musicVolume
        {
            get
            {
                return _musicVolume;
            }
            set
            {
                _musicVolume = value;
                PlayerPrefs.SetFloat(MusicVolumeKey, value);
                _musicVCA.setVolume(value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public float sfxVolume
        {
            get
            {
                return _sfxVolume;
            }
            set
            {
                _sfxVolume = value;
                PlayerPrefs.SetFloat(SFXVolumeKey, value);
                _sfxVCA.setVolume(value);
            }
        }

        #endregion

        #region Fields

        private static AudioSettingsManager _instance;

        private VCA _musicVCA;
        private VCA _sfxVCA;

        private float _musicVolume;
        private float _sfxVolume;

        #endregion
    }
}
