using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

namespace ProjectShovel
{
    public class OptimalSetupScreen : MonoBehaviour
    {
        #region Message

        /// <summary>
        /// 
        /// </summary>
        private void Start()
        {
            _controllerImage.DOFade(0f, 0f);
            _textMesh.DOFade(0f, 0f);
            _textDemo.DOFade(0f, 0f);
            foreach(var item in _images)
            {
                item.DOFade(0f, 0f);
            }

            StartAnimation();
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private void StartAnimation()
        {
            _currentSequence = DOTween.Sequence();

            _currentSequence.Append(_textDemo.DOFade(1f, 0.5f));

            foreach (var item in _images)
            {
                _currentSequence.Join(item.DOFade(1f, 0.5f));
            }

            _currentSequence.AppendInterval(1.0f);
            _currentSequence.Append(_textDemo.DOFade(0f, 0.5f));

            foreach (var item in _images)
            {
                _currentSequence.Join(item.DOFade(0f, 0.5f));
            }

            _currentSequence.Append(_textMesh.DOFade(1f, 0.5f));
            _currentSequence.Join(_controllerImage.DOFade(1f, 0.5f));

            _currentSequence.AppendInterval(1.0f);

            _currentSequence.Append(_textMesh.DOFade(0f, 0.5f));
            _currentSequence.Join(_controllerImage.DOFade(0f, 0.5f));

            _currentSequence.AppendCallback(() => GameplayStatics.TransitionToScene(GameConstants.TitleScreenScene));

        }

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Image _controllerImage = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private TextMeshProUGUI _textMesh = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private TextMeshProUGUI _textDemo = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private List<Image> _images = default;

        private Sequence _currentSequence;

        #endregion
    }
}

