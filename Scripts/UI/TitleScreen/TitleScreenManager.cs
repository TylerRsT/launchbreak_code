using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Com.LuisPedroFonseca.ProCamera2D;
using UnityEngine.UI;
using DG.Tweening;
using Elendow.SpritedowAnimator;
using TMPro;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class TitleScreenManager : MonoBehaviour
    {
        #region Const

        private const string TitleScreenSoundKey = "TitleScreen";

        #endregion

        #region Messages

        /// <summary>
        /// 
        /// </summary>
        private void Start()
        {
            if (!MenuMusicController.isPlaying)
            {
                MenuMusicController.StartMusic();
            }

            if (_titleScreenPanelOpened)
            {
                UITitleScreenPanel.OpenTitleScreenPanel();
                _titleImage.gameObject.SetActive(false);
                _anyButtonText.gameObject.SetActive(false);
            }
            else
            {
                _titleImage.DOFade(0f, 0.01f);
                _anyButtonText.gameObject.SetActive(false);
                StartCoroutine(StartAnimation());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void Update()
        {
            if(Input.anyKeyDown && !_titleScreenPanelOpened && _firstAnimationDone)
            {
                StartTitleFlicker();
                _titleScreenPanelOpened = true;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        private IEnumerator StartAnimation()
        {
            Sequence sequence = DOTween.Sequence();

            sequence.Join(_titleImage.DOFade(1f, 1f));
            sequence.Join(_titleImage.transform.DOMoveY(32f, 1f));

            yield return sequence.WaitForCompletion();

            var animator = GetComponent<UIAnimator>();
            animator.Play(true);

            yield return new WaitUntil(()=> !animator.IsPlaying);

            _anyButtonText.gameObject.SetActive(true);

            yield return sequence.WaitForCompletion();

            _firstAnimationDone = true;
        }

        /// <summary>
        /// 
        /// </summary>
        private void StartTitleFlicker()
        {
            this.EmitSound(TitleScreenSoundKey);

            Sequence sequence = DOTween.Sequence();
            for (var i = 0; i < _flickerCount; i++)
            {
                sequence.Append(_titleImage.DOFade(0f, 0.08f));
                sequence.Append(_titleImage.DOFade(1f, 0.08f));
            }
            sequence.Append(_titleImage.DOFade(0f, 0.5f));
            sequence.AppendCallback(() => UITitleScreenPanel.OpenTitleScreenPanel());
            _anyButtonText.gameObject.SetActive(false);
            for (var i = 0; i < GameConstants.MaxPlayerCount; ++i)
            {
                this.StartVibrationCoroutine(i + 1, 0.7f, 0.7f, 0.3f);
            }
        }

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Image _titleImage = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private TextMeshProUGUI _anyButtonText = default;

        private int _flickerCount = 3;
        private static bool _titleScreenPanelOpened = false;
        private static bool _firstAnimationDone = false;

        #endregion
    }
}
