using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Com.LuisPedroFonseca.ProCamera2D;
using UnityEngine.SceneManagement;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class SplashScreen : MonoBehaviour
    {
        #region Message

        /// <summary>
        /// 
        /// </summary>
        private void Awake()
        {
            _image = GetComponent<Image>();
            _image.sprite = _studioLogo;

            StartCoroutine(StartAnimation());
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
            _currentSequence.Append(_image.DOFade(0f, 0.0f));
            _currentSequence.Append(_image.DOFade(1f, 0.5f));
           
            yield return new WaitForSeconds(2);
            _currentSequence = DOTween.Sequence();
            _currentSequence.Append(_image.DOFade(0f, 0.5f));
            _currentSequence.AppendCallback(() => _image.sprite = _fmodLogo);
            _currentSequence.Append(_image.DOFade(1f, 0.5f));

            yield return new WaitForSeconds(2);
            _currentSequence = DOTween.Sequence();
            _currentSequence.Append(_image.DOFade(0f, 0.5f));
            _currentSequence.AppendCallback(() => SceneManager.LoadScene(GameConstants.OptimalSetupScreenScene));

        }

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Sprite _studioLogo = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Sprite _fmodLogo = default;

        private Sequence _currentSequence;

        private Image _image;

        #endregion
    }
}
