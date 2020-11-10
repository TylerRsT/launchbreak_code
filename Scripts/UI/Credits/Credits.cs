using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class Credits : MonoBehaviour
    {
        #region Messages

        /// <summary>
        /// 
        /// </summary>
        private void Start()
        {
            _initTransform = transform.position;
        }

        /// <summary>
        /// 
        /// </summary>
        private void LateUpdate()
        {
            if(Input.anyKeyDown && _creditsSequence != null && (_creditsSequence.IsPlaying() || _creditsIsOver))
            {
                StopCredits();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callback"></param>
        public void StartCredits(System.Action callback = null)
        {
            _callback = callback;
            _creditsSequence = DOTween.Sequence();
            _creditsSequence.Append(transform.DOLocalMoveY(_endPosition, _scrollSpeed).SetEase(Ease.Linear));
            _creditsSequence.AppendCallback(() => _creditsIsOver = true);
        }

        /// <summary>
        /// 
        /// </summary>
        public void StopCredits()
        {
            TweenExtensions.SafeKill(ref _creditsSequence);
            transform.position = new Vector2(_initTransform.x, _initTransform.y);
            _callback?.Invoke();
            _creditsIsOver = false;
        }

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _endPosition = 500f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _scrollSpeed = 50f;

        private Sequence _creditsSequence;
        private Vector3 _initTransform;
        private System.Action _callback;
        private bool _creditsIsOver = false;

        #endregion
    }
}
