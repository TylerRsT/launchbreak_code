using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class AudioSettingsNavigationHandler : SelectorNavigationHandler
    {
        #region Override

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        protected override int InitializeValue(string[] values)
        {
            float percentage = 0.0f;

            switch(_selector)
            {
                case AudioSettingsSelector.Music:
                    percentage = AudioSettingsManager.instance.musicVolume;
                    break;
                case AudioSettingsSelector.Sfx:
                    percentage = AudioSettingsManager.instance.sfxVolume;
                    break;
            }

            var decimalPercentage = System.Convert.ToInt32(percentage * 100);
            var percentageStr = decimalPercentage.ToString();

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i].Equals(percentageStr))
                {
                    return i;
                }
            }

            return base.InitializeValue(values);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        protected override void OnSelectedValueChanged(string value)
        {
            var percentage = float.Parse(value) / 100.0f;

            switch (_selector)
            {
                case AudioSettingsSelector.Music:
                    AudioSettingsManager.instance.musicVolume = percentage;
                    break;

                case AudioSettingsSelector.Sfx:
                    AudioSettingsManager.instance.sfxVolume = percentage;
                    break;
            }
        }

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private AudioSettingsSelector _selector = default;

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public enum AudioSettingsSelector
    {
        None,
        Music,
        Sfx,
    }
}



