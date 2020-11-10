using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    ///
    /// </summary>
    public class VideoSettingsUITabNavigation : MonoBehaviour
    {
        #region Messages

        /// <summary>
        /// 
        /// </summary>
        private void Start()
        {
            OnWindowScaleOptionChanged();
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        public void OnWindowScaleOptionChanged()
        {
            if (RenderSettingsManager.instance.fullscreen)
            {
                _windowScaleSelector.GetComponent<UIButton>().RemoveFromNavigationMap();
                _windowScaleSelector.gameObject.SetActive(false);
                _windowScaleCaption.gameObject.SetActive(false);
            }
            else
            {
                _windowScaleSelector.GetComponent<UIButton>().InsertToNavigationMap();
                _windowScaleSelector.gameObject.SetActive(true);
                _windowScaleCaption.gameObject.SetActive(true);

                _windowScaleSelector.optionIndex = RenderSettingsManager.instance.scaleFromResolution - 1;
            }
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private SelectorNavigationHandler _windowScaleSelector = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private GameObject _windowScaleCaption = default;
    }
}
