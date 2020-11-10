using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using System.Linq;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class FlickerButton : MonoBehaviour
    {
        #region Messages

        /// <summary>
        /// 
        /// </summary>
        private void Start()
        {
            FlickerAnimation();
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnDestroy()
        {
            TweenExtensions.SafeKill(ref _sequence);
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        private void FlickerAnimation()
        {
            TextMeshProUGUI textRenderer = GetComponentInChildren<TextMeshProUGUI>();

            _sequence = DOTween.Sequence();
            if (_image != null)
            {
                _sequence.Append(_image.DOFade(0, 0.5f))
                    .Join(_text.DOFade(0, 0.5f))
                    .Append(_image.DOFade(1, 0.5f))
                    .Join(_text.DOFade(1, 0.5f));
            }
            else
            {
                _sequence.Append(_text.DOFade(0, 0.5f))
                    .Append(_text.DOFade(1, 0.5f));
            }

            _sequence.SetLoops(-1)
                .SetEase(Ease.Linear);
            _sequence.SetLoops(-1);
        }

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Image _image = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private TextMeshProUGUI _text = default;

        private Sequence _sequence;

        #endregion
    }
}
