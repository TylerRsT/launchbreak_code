using Elendow.SpritedowAnimator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    [RequireComponent(typeof(BulletReaction))]
    [RequireComponent(typeof(SpriteAnimator))]
    public class AnimationBulletReaction : MonoBehaviour
    {
        #region Messages

        /// <summary>
        /// 
        /// </summary>
        private void Awake()
        {
            _spriteAnimator = GetComponent<SpriteAnimator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _startSprite = _spriteRenderer.sprite;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="response"></param>
        private void OnReceivingBullet(BulletResponse response)
        {
            PlayAnim();
            response.received = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dashAttack"></param>
        private void OnReceivingDashAttack(CharacterDashAttack dashAttack)
        {
            PlayAnim();
            dashAttack.Accept();
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        private void PlayAnim()
        {
            _spriteAnimator.Stop();
            _spriteAnimator.Play(_animToPlay, _playOneShot);
            if (_resetAtFinish)
            {
                _spriteAnimator.onFinish.AddListener(() =>
                {
                    _spriteRenderer.sprite = _startSprite;
                });
            }
        }

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private SpriteAnimation _animToPlay = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private bool _playOneShot = true;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private bool _resetAtFinish = true;

        private SpriteRenderer _spriteRenderer;
        private SpriteAnimator _spriteAnimator;

        private Sprite _startSprite;

        #endregion
    }
}