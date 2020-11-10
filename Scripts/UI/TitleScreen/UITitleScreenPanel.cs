using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class UITitleScreenPanel : MonoBehaviour
    {
        #region Methods

        /// <summary>
        /// 
        /// </summary>
        public static void OpenTitleScreenPanel()
        {
            _titleScreenPanelInstance = Instantiate(Bootstrap.instance.data.titleScreenPanelPrefab, GameObject.FindObjectOfType<Canvas>().transform)
                .GetComponent<UITitleScreenPanel>();
#if PLATFORM_ITCH
            Instantiate(_titleScreenPanelInstance._itchBannerPrefab, new Vector3(-500, -138, 0), Quaternion.identity, GameObject.FindObjectOfType<Canvas>().transform);
#endif // PLATFORM_ITCH

            _titleScreenPanelInstance.StartCoroutine(_titleScreenPanelInstance.OpenInternal());
        }

        /// <summary>
        ///
        /// </summary>
        private IEnumerator OpenInternal()
        {
            _playButton.transform.position = new Vector2(_playButton.transform.position.x + 500, _playButton.transform.position.y);
            _settingsButton.transform.position = new Vector2(_settingsButton.transform.position.x - 500, _settingsButton.transform.position.y);
            _creditsButton.transform.position = new Vector2(_creditsButton.transform.position.x + 500, _creditsButton.transform.position.y);
            _quitButton.transform.position = new Vector2(_quitButton.transform.position.x - 500, _quitButton.transform.position.y);

            _playButton.transform.DOMoveX(0f, 0.3f);

            yield return new WaitForSeconds(0.2f);

            _settingsButton.transform.DOMoveX(0f, 0.3f);

            yield return new WaitForSeconds(0.2f);

            _creditsButton.transform.DOMoveX(0f, 0.3f);

            yield return new WaitForSeconds(0.2f);

            yield return _quitButton.transform.DOMoveX(0f, 0.3f).WaitForCompletion();

            GetComponent<UINavigation>().Focus();
        }

        /// <summary>
        ///
        /// </summary>
        private IEnumerator CloseInternal()
        {
            _playButton.transform.DOMoveX(500f, 0.3f);

            yield return new WaitForSeconds(0.2f);

            _settingsButton.transform.DOMoveX(-500f, 0.3f);

            yield return new WaitForSeconds(0.2f);

            _creditsButton.transform.DOMoveX(500f, 0.3f);

            yield return new WaitForSeconds(0.2f);

            yield return _quitButton.transform.DOMoveX(-500f, 0.3f).WaitForCompletion();

            GameplayStatics.TransitionToScene(GameConstants.MatchSettingsScene);
        }

        /// <summary>
        ///
        /// </summary>
        public static void CloseTitleScreenPanel()
        {
            _titleScreenPanelInstance.StartCoroutine(_titleScreenPanelInstance.CloseInternal());
        }

#endregion

#region Properties

        /// <summary>
        /// 
        /// </summary>
        public UIButton playButton => _playButton;

        /// <summary>
        /// 
        /// </summary>
        public UIButton settingsButton => _settingsButton;

        /// <summary>
        /// 
        /// </summary>
        public UIButton creditsButton => _creditsButton;

        /// <summary>
        /// 
        /// </summary>
        public UIButton quitButton => _quitButton;

#endregion

#region Fields

        /// <summary>
        /// 
        /// </summary>
        public static UITitleScreenPanel titleScreenPanelInstance => _titleScreenPanelInstance;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private UIButton _playButton = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private UIButton _settingsButton = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private UIButton _creditsButton = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private UIButton _quitButton = default;

#pragma warning disable 414
        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private GameObject _itchBannerPrefab = default;
#pragma warning restore 414

        private static UITitleScreenPanel _titleScreenPanelInstance;
        private System.Action _closeCallback;

#endregion
    }
}
