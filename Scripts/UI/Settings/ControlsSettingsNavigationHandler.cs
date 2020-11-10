using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class ControlsSettingsNavigationHandler : SelectorNavigationHandler
    {
        #region Override

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        protected override void OnSelectedValueChanged(string value)
        {
            switch(_selector)
            {
                case ControlsSettingsSelector.GamepadRumble:
                    var percentage = float.Parse(value) / 100.0f;
                    GamepadRumbleValueUpdate(percentage);
                    break;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="percentage"></param>
        private void GamepadRumbleValueUpdate(float percentage)
        {
            StopAllCoroutines();
            InputHelper.StopAllVibrations();
            InputHelper.SetAllVibrations(percentage);

            StartCoroutine(GamepadRumbleDuration(_gamepadRumbleDuration));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="duration"></param>
        /// <returns></returns>
        private IEnumerator GamepadRumbleDuration(float duration)
        {
            yield return new WaitForSecondsRealtime(duration);

            InputHelper.StopAllVibrations();
        }

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private ControlsSettingsSelector _selector = default;

        private float _gamepadRumbleDuration = 0.5f;

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public enum ControlsSettingsSelector
    {
        None,
        GamepadRumble,
    }
}
