using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class HitFlash : MonoBehaviour
    {
        #region Const

        private const string FlashMaterialAmountName = "_FlashAmount";

        #endregion

        #region Messages

        /// <summary>
        /// 
        /// </summary>
        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _defaultMaterial = _spriteRenderer.material;
        }

        /// <summary>
        /// 
        /// </summary>
        private void Update()
        {
            if(!_isFlashing)
            {
                return;
            }

            _elapsed += GameplayStatics.gameDeltaTime;

            if(_elapsed >= _duration)
            {
                _spriteRenderer.material = _defaultMaterial;
                _isFlashing = false;
                return;
            }

            var targetMaterial = _defaultMaterial;
            if((_elapsed % (_interval * 2.0f)) < _interval)
            {
                targetMaterial = _flashMaterial;
            }

            if(_spriteRenderer.material != targetMaterial)
            {
                _spriteRenderer.material = targetMaterial;
                if (targetMaterial == _flashMaterial)
                {
                    targetMaterial.SetFloat(FlashMaterialAmountName, _flashValue);
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        public void Flash()
        {
            Flash(1.0f, 0.2f, 0.05f);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="duration"></param>
        /// <param name="interval"></param>
        public void Flash(float value, float duration, float interval)
        {
            _isFlashing = true;

            _elapsed = 0.0f;
            _flashValue = value;
            _duration = duration;
            _interval = interval;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public Material defaultMaterial
        {
            get => _defaultMaterial;
            set => _defaultMaterial = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public Material flashMaterial
        {
            get => _flashMaterial;
            set => _flashMaterial = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public float interval
        {
            get => _interval;
            set => _interval = value;
        }

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Material _flashMaterial = default;

        private Material _defaultMaterial = default;

        private bool _isFlashing;

        private float _flashValue;
        private float _duration;
        private float _interval;

        private float _elapsed;

        private SpriteRenderer _spriteRenderer;

        #endregion
    }
}