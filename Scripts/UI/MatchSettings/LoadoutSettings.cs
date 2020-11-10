using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class LoadoutSettings : MonoBehaviour
    {
        #region Const

        private const string ReadyActionName = "Ready";
        private const string LoadoutSlotActionName = "LoadoutSlot";

        #endregion

        #region Messages

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        private void OnSubmit(UINavigationData data)
        {
            switch (data.actionName)
            {
                case ReadyActionName:
                    playerLoadoutSettings.isReady = true;
                    GetComponent<UINavigation>().Unfocus();
                    break;
                case LoadoutSlotActionName:
                    var itemPresenter = data.navigable.GetComponent<LoadoutItemPresenter>();
                    playerLoadoutSettings.GoToArmory(itemPresenter);
                    break;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetPresenter"></param>
        public void ValidateItem(LoadoutItemPresenter targetPresenter)
        {
            var presenters = GetComponentsInChildren<LoadoutItemPresenter>();
            foreach(var presenter in presenters)
            {
                if(presenter == targetPresenter)
                {
                    continue;
                }

                if(presenter.item == targetPresenter.item)
                {
                    presenter.item = null;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="active"></param>
        public void SetFocus(bool active)
        {
            if(active)
            {
                GetComponent<UINavigation>().Focus();
            }
            else
            {
                GetComponent<UINavigation>().Unfocus();
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public PlayerLoadoutSettings playerLoadoutSettings { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public LoadoutItem[] loadout => GetComponentsInChildren<LoadoutItemPresenter>().Select(x => x.item).ToArray();

        /// <summary>
        /// 
        /// </summary>
        public int playerIndex
        {
            set { GetComponent<UINavigation>().userIndex = value; }
        }

        #endregion
    }
}