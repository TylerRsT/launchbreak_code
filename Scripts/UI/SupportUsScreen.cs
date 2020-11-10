using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using TMPro;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class SupportUsScreen : MonoBehaviour
    {
        #region Messages

        /// <summary>
        /// 
        /// </summary>
        private void Start()
        {
            foreach (var item in _iconImages)
            {
                item.DOFade(0f, 0f);
            }

            foreach(var item in _textMeshes)
            {
                item.DOFade(0f, 0f);
            }

            StartCoroutine(StartAnimation());
        }

        /// <summary>
        /// 
        /// </summary>
        private void Update()
        {
            if(_completed)
            {
                if (Input.GetKeyDown(KeyCode.JoystickButton2))
                {
                    Telemetry.session.Set("quitLink", "steam");
                    Application.OpenURL("https://store.steampowered.com/app/1292160/Launch_Break/");
                }
                else if (Input.GetKeyDown(KeyCode.JoystickButton3))
                {
                    Telemetry.session.Set("quitLink", "itch");
                    Application.OpenURL("https://badrezgames.itch.io/launch-break");
                }
                else if (Input.GetKeyDown(KeyCode.JoystickButton1))
                {
                    Telemetry.session.Set("quitLink", "discord");
                    Application.OpenURL("https://discord.gg/x86BpGa");
                }

                if (Input.anyKeyDown)
                {
                    Application.Quit();
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private IEnumerator StartAnimation()
        {
            _currentSequence = DOTween.Sequence();

            // Fade In
            foreach (var item in _iconImages)
            {
                _currentSequence.Join(item.DOFade(1f, 0.5f));
            }

            foreach (var item in _textMeshes)
            {
                _currentSequence.Join(item.DOFade(1f, 0.5f));
            }

            yield return _currentSequence.WaitForCompletion();

            _completed = true;
        }

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private List<Image> _iconImages = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private List<TextMeshProUGUI> _textMeshes = default;

        private Sequence _currentSequence;

        private bool _completed = false;

        #endregion
    }
}
