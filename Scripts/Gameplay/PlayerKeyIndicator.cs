using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class PlayerKeyIndicator : MonoBehaviour
    {
        #region Messages

        /// <summary>
        /// 
        /// </summary>
        private void Start()
        {
            _tweenSequence = DOTween.Sequence();
            _tweenSequence.Append(transform.DOLocalMoveY(transform.localPosition.y + _offset, _interval));
            _tweenSequence.Append(transform.DOLocalMoveY(transform.localPosition.y, _interval));
            _tweenSequence.SetLoops(-1);
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnDestroy()
        {
            if(_tweenSequence != null && _tweenSequence.IsActive())
            {
                _tweenSequence.Kill();
                _tweenSequence = null;
            }
        }

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _offset = 5.0f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _interval = 0.2f;

        /// <summary>
        /// 
        /// </summary>
        private Sequence _tweenSequence;

        #endregion
    }
}