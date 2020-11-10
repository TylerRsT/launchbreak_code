using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class EndGameNavigationHandler : MonoBehaviour
    {
        #region Messages

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        private void OnSubmit(UINavigationData action)
        {
            action.navigationHandler.Unfocus();
            StopMusic();
            switch (action.actionName)
            {
                case "Rematch":
                    if (GameModeParams.instance.useRandomMap)
                    {
                        GameModeParams.instance.selectedMap = GetRandomMap(GameModeParams.instance.selectedMap);
                    }
                    GameplayStatics.TransitionToScene(GameModeParams.instance.selectedMap);
                    break;
                case "QuitGame":
                    GameplayStatics.TransitionToScene(
                        GameConstants.TitleScreenScene
                    );
                    break;
                default:
                    MatchSettingsManager.showMaps = true;
                    GameplayStatics.TransitionToScene(
                        GameConstants.MatchSettingsScene
                    );
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        private void OnCancel(UINavigationData action)
        {
            var victoryScreen = FindObjectOfType<VictoryScreen>();
            victoryScreen?.WaitForDisplayRematchPanel();
            Destroy(transform.parent.parent.parent.gameObject);
        }

            #endregion

            #region Methods

            /// <summary>
            /// 
            /// </summary>
            /// <param name="currentMap"></param>
            /// <returns></returns>
            private string GetRandomMap(string currentMap)
        {
            var sceneCount = SceneManager.sceneCountInBuildSettings;
            var availableMaps = new List<string>();

            for (var i = 0; i < sceneCount; ++i)
            {
                var scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                var sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
                if (!sceneName.StartsWith(GameConstants.MapPrefixe) || sceneName == currentMap)
                {
                    continue;
                }

                availableMaps.Add(sceneName);
            }

            if(availableMaps.Count == 0)
            {
                return currentMap;
            }

            return availableMaps[Random.Range(0, availableMaps.Count)];
        }

        /// <summary>
        /// 
        /// </summary>
        private void StopMusic()
        {
            foreach (var controller in FindObjectsOfType<MusicController>())
            {
                controller.StopMusic();
            }
        }

        #endregion
    }
}