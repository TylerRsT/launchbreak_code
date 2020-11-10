using System.Collections;
using UnityEngine;
using DG.Tweening;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class TitleScreenNavigationHandler : MonoBehaviour
    {
        #region Messages

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        private IEnumerator OnSubmit(UINavigationData data)
        {
            var shakeTime = 0.5f;
            var shakeStrenght = 5f;
            var shakeVibrato = 50;

            switch (data.actionName)
            {
                case "Start":
                    data.navigationHandler.Unfocus();
                    ShakeAllControllers();
                    MatchSettingsManager.showMaps = false;
                    yield return UITitleScreenPanel.titleScreenPanelInstance.playButton.transform.DOShakePosition(shakeTime, shakeStrenght, shakeVibrato).WaitForCompletion();
                    UITitleScreenPanel.CloseTitleScreenPanel();
                    break;
                case "Settings":
                    data.navigationHandler.Unfocus();
                    ShakeAllControllers();
                    yield return UITitleScreenPanel.titleScreenPanelInstance.settingsButton.transform.DOShakePosition(shakeTime, shakeStrenght, shakeVibrato).WaitForCompletion();
                    UITitleScreenPanel.titleScreenPanelInstance.gameObject.SetActive(false);

                    SettingsPanelNavigationHandler.OpenSettingsPanel(() =>
                    {
                        data.navigationHandler.Focus();
                        UITitleScreenPanel.titleScreenPanelInstance.gameObject.SetActive(true);
                    });
                    break;
                case "Credits":
                    data.navigationHandler.Unfocus();
                    ShakeAllControllers();
                    yield return UITitleScreenPanel.titleScreenPanelInstance.creditsButton.transform.DOShakePosition(shakeTime, shakeStrenght, shakeVibrato).WaitForCompletion();
                    UITitleScreenPanel.titleScreenPanelInstance.gameObject.SetActive(false);
                    var credits = FindObjectOfType<Credits>();
                    credits?.StartCredits(() =>
                    {
                        data.navigationHandler.Focus();
                        UITitleScreenPanel.titleScreenPanelInstance.gameObject.SetActive(true);
                    });
                    UITitleScreenPanel.titleScreenPanelInstance.gameObject.SetActive(false);
                    break;
                case "QuitGame":
                    data.navigationHandler.Unfocus();
                    ShakeAllControllers();
                    MenuMusicController.StopMusic();
                    yield return UITitleScreenPanel.titleScreenPanelInstance.quitButton.transform.DOShakePosition(shakeTime, shakeStrenght, shakeVibrato).WaitForCompletion();
                    GameplayStatics.TransitionToScene(GameConstants.SupportScene);
                    break;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        private void ShakeAllControllers()
        {
            for (var i = 0; i < GameConstants.MaxPlayerCount; ++i)
            {
                this.StartVibrationCoroutine(i + 1, 0.7f, 0.7f, 0.3f);
            }
        }

        #endregion
    }
}
