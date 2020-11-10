using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    [RequireComponent(typeof(ItchBuildInfoManager))]
    public class ItchVersionBanner : MonoBehaviour
    {
        #region Messages

        /// <summary>
        /// 
        /// </summary>
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.JoystickButton3) && _itchBannerOn)
            {
                Application.OpenURL("https://badrezgames.itch.io/launch-break");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newBuildInfo"></param>
        private void OnNewerVersionFound(BuildInfo newBuildInfo)
        {
            StartCoroutine(StartAnimation());
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        private IEnumerator StartAnimation()
        {
            yield return new WaitForSeconds(1f);

            transform.DOMoveX(-295, 0.5f);
            _itchBannerOn = true;
        }

        #endregion

        #region Fields

        private bool _itchBannerOn = false;

        #endregion
    }
}
