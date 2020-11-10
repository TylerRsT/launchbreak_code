using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class SettingsTabUINavigation : UINavigation
    {
        #region Const

        private const string StartActionName = "UI_Start";

        #endregion

        #region Messages

        /// <summary>
        /// 
        /// </summary>
        private void Update()
        {
            if (InputHelper.AnyButtonDown(StartActionName))
            {
                SettingsPanelNavigationHandler.CloseSettingsPanel();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        private void OnNavigate(UINavigationData data)
        {
            if(data.direction == UINavigationDirection.Up
                && currentNavigable != null 
                && currentNavigable.up == null
                && panelNavigationHandler != null)
            {
                Unfocus(true);
                panelNavigationHandler.isNavigatingTabs = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        private void OnCancel(UINavigationData action)
        {
            SettingsPanelNavigationHandler.CloseSettingsPanel();
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public SettingsPanelNavigationHandler panelNavigationHandler { get; set; }

        #endregion
    }
}
