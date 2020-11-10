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
    public class CharacterPlayerIdBehaviour : MonoBehaviour
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
            if (playerIndex == -1)
            {
                _spriteRenderer.sprite = _cpuSprite;
            }
            else
            {
                _spriteRenderer.sprite = _playerIdSprites[playerIndex - 1];
            }
            _spriteRenderer.material.SetColor("_DesiredColor", color);

            _sequence = DOTween.Sequence();
            _sequence.Append(transform.DOLocalMoveY(transform.localPosition.y + 5.0f, 0.25f));
            _sequence.Append(transform.DOLocalMoveY(transform.localPosition.y, 0.25f));
            _sequence.SetLoops(2);
            _sequence.onComplete += (() =>
            {
                _sequence = null;
                _blinkStarted = true;
            });
        }

        /// <summary>
        /// 
        /// </summary>
        private void Update()
        {
            if(!_blinkStarted)
            {
                return;
            }

            _elapsed += GameplayStatics.gameDeltaTime;

            if(_elapsed >= _duration)
            {
                Destroy(gameObject);
                return;
            }

            var alpha = ((_elapsed % (_switchInterval * 2.0f) <= _switchInterval)) ? 1.0f : 0.0f;
            this.SetCosmeticAlpha(alpha);
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnDestroy()
        {
            if(_sequence != null && _sequence.IsActive())
            {
                _sequence.Kill();
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public int playerIndex { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Color color { get; set; }

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private List<Sprite> _playerIdSprites = new List<Sprite>();

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Sprite _cpuSprite = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _duration = 3.0f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _switchInterval = 0.5f;

        private SpriteRenderer _spriteRenderer;

        private Sequence _sequence;

        private float _elapsed = 0.0f;

        private bool _blinkStarted = false;

        #endregion
    }
}