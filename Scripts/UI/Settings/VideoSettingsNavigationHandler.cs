using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class VideoSettingsNavigationHandler : SelectorNavigationHandler
    {
        #region Override

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        protected override int InitializeValue(string[] values)
        {
            int value = 0;

            switch (_selector)
            {
                case VideoSettingsSelector.Fullscreen:
                    return RenderSettingsManager.instance.fullscreen ? 1 : 0;
                case VideoSettingsSelector.WindowScale:
                    value = RenderSettingsManager.instance.windowScale;
                    break;
                case VideoSettingsSelector.VerticalSync:
                    return RenderSettingsManager.instance.verticalSync;
            }

            var valueStr = value.ToString();

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i].Equals(valueStr))
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
            switch (_selector)
            {
                case VideoSettingsSelector.Fullscreen:
                    var fullscreenEnabled = bool.Parse(value);
                    RenderSettingsManager.instance.fullscreen = fullscreenEnabled;
                    var videoSettingsUITab = transform.parent.parent.GetComponent<VideoSettingsUITabNavigation>();
                    videoSettingsUITab.OnWindowScaleOptionChanged();
                    break;
                case VideoSettingsSelector.WindowScale:
                    var scale = int.Parse(value);
                    RenderSettingsManager.instance.windowScale = scale;
                    break;

                case VideoSettingsSelector.VerticalSync:
                    var vSyncCount = int.Parse(value);
                    RenderSettingsManager.instance.verticalSync = vSyncCount;
                    break;
            }
        }

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private VideoSettingsSelector _selector = default;

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public enum VideoSettingsSelector
    {
        None,
        Fullscreen,
        WindowScale,
        VerticalSync,
    }
}

