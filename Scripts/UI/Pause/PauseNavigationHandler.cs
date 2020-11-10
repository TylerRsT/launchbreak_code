using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class PauseNavigationHandler : MonoBehaviour
    {
        #region Messages

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        private void OnSubmit(UINavigationData data)
        {
            switch (data.actionName)
            {
                case "Resume":
                    GameplayStatics.UnpauseGame();
                    break;
                case "Rematch":
                    StopMusic();
                    GameplayStatics.TransitionToScene(
                        UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
                    );
                    break;
                case "Settings":
                    data.navigationHandler.Unfocus();
                    _pausePanel.gameObject.SetActive(false);

                    SettingsPanelNavigationHandler.OpenSettingsPanel(() =>
                    {
                        _pausePanel.gameObject.SetActive(true);
                        data.navigationHandler.Focus();
                    });
                        break;
                case "QuitGame":
                    StopMusic();
                    GameplayStatics.TransitionToScene(
                        GameConstants.TitleScreenScene
                    );
                    break;
                case "ChangeMap":
                    MatchSettingsManager.showMaps = true;
                    StopMusic();
                    GameplayStatics.TransitionToScene(
                        GameConstants.MatchSettingsScene
                    );
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        private void OnCancel(UINavigationData data)
        {
            GameplayStatics.UnpauseGame();
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        private void StopMusic()
        {
            foreach(var controller in FindObjectsOfType<MusicController>())
            {
                controller.StopMusic();
            }
        }

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private UIPausePanel _pausePanel = default;

        #endregion
    }
}