using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class SwitchWeaponIndicator : MonoBehaviour
    {
        #region Messages

        /// <summary>
        /// 
        /// </summary>
        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        /// <summary>
        /// 
        /// </summary>
        private void Start()
        {
            _sequence = DOTween.Sequence();
            _sequence.Append(transform.DOLocalMoveY(transform.localPosition.y + 2.0f, 0.2f));
            _sequence.Append(transform.DOLocalMoveY(transform.localPosition.y, 0.2f));
            _sequence.SetLoops(-1);

            var input = GetComponentInParent<CharacterInput>();
            if (input != null)
            {
                var sprite = input.userIndex == 0 ? _keyboardSwitchSprite : _gamepadSwitchSprite;
                _spriteRenderer.sprite = sprite;
            }
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
        /// <param name="enabled"></param>
        public void SetEnabled(bool enabled)
        {
            _spriteRenderer.enabled = enabled;
        }

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Sprite _gamepadSwitchSprite = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Sprite _keyboardSwitchSprite = default;

        private SpriteRenderer _spriteRenderer;
        private Sequence _sequence;

        #endregion
    }
}
