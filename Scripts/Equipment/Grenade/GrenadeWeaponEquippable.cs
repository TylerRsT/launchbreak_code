using Com.LuisPedroFonseca.ProCamera2D;
using DG.Tweening;
using Elendow.SpritedowAnimator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class GrenadeWeaponEquippable : ThrowawayWeaponEquippable
    {
        #region Messages

        /// <summary>
        /// 
        /// </summary>
        private void OnValidate()
        {
            var spriteRenderer = GetComponent<SpriteRenderer>();
            if(spriteRenderer != null)
            {
                spriteRenderer.sprite = _offSprite;
            }
        }

        #endregion

        #region Override

        /// <summary>
        /// 
        /// </summary>
        public override void OnCharacterDied()
        {
            base.OnCharacterDied();

            Unpin();
            Throw(_throwInfo, 0.2f);
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Trigger()
        {
            base.Trigger();

            Unpin();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Release()
        {
            base.Release();

            Throw();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Start()
        {
            base.Start();

            if (_grenadeBehaviour == null)
            {
                _grenadeBehaviour = GetComponent<GrenadeBehaviour>();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        private void Unpin()
        {
            if (_isPinned)
            {
                _isPinned = false;
                _grenadeBehaviour = gameObject.AddComponent<GrenadeBehaviour>();
                _grenadeBehaviour.explosionDelay = _explosionDelay;
                _grenadeBehaviour.radius = _radius;
                _grenadeBehaviour.knockback = _knockback;
                _grenadeBehaviour.damages = _damages;
                _grenadeBehaviour.offSprite = _offSprite;
                _grenadeBehaviour.onSprite = _onSprite;
                _grenadeBehaviour.explosionAnimation = _explosionAnimation;
                _grenadeBehaviour.explosionFrequency = _explosionFrequency;
                _grenadeBehaviour.explosionDistance = _explosionDistance;
                _grenadeBehaviour.explosionShake = _explosionShake;
                _grenadeBehaviour.equippable = this;

                var pinObject = GameObject.Instantiate(Bootstrap.instance.data.emptyTileSpritePrefab, transform.position, Quaternion.identity);
                var pinRenderer = pinObject.GetComponent<SpriteRenderer>();
                pinRenderer.sprite = _pinSprite;

                pinObject.transform.DOJump(pinObject.transform.position + Vector3.right * 20.0f * MathExtensions.RandOne(),
                    15.0f, Random.Range(2, 4), 0.4f, true)
                    .onComplete += (() =>
                    {
                        IEnumerator disappear()
                        {
                            yield return new WaitForSeconds(2.0f);
                            pinRenderer.DOFade(0.0f, 0.2f)
                                .onComplete += (() =>
                                {
                                    Destroy(pinObject);
                                });
                        };
                        pinRenderer.GetComponent<MonoBehaviour>().StartCoroutine(disappear());
                    });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void Throw(GrenadeThrowInfo throwInfo, float multiplier)
        {
            var pickable = Instantiate(_pickablePrefab, character.transform.position, Quaternion.identity).GetComponent<GrenadePickable>();
            pickable.Initialize(weaponDescriptor);
            pickable.isPickable = false;

            var pickableRenderer = pickable.GetComponent<SpriteRenderer>();
            pickableRenderer.sprite = null;

            var behaviour = pickable.GetComponent<GrenadeBehaviour>();

            var renderer = behaviour.subRenderer;

            if (_grenadeBehaviour != null)
            {
                behaviour.CopyFrom(_grenadeBehaviour);
                behaviour.spriteRenderer = pickableRenderer;
            }

            behaviour.RollOnTheFloor(character.targetOrientation, throwInfo, multiplier);
            character.ClearWeapon(this, false);
        }

        /// <summary>
        /// 
        /// </summary>
        private void Throw()
        {
            Throw(_throwInfo, 1.0f);
        }

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _explosionDelay = 3.0f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _radius = 48.0f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _knockback = 36.0f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private int _damages = 3;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private GameObject _pickablePrefab = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Sprite _offSprite = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Sprite _onSprite = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private SpriteAnimation _explosionAnimation = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private int _explosionFrequency = 1;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private ShakePreset _explosionShake = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _explosionDistance = 12.0f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private GrenadeThrowInfo _throwInfo = new GrenadeThrowInfo();

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Sprite _pinSprite = default;

        private bool _isPinned = true;

        private GrenadeBehaviour _grenadeBehaviour;

        #endregion
    }

    [System.Serializable]
    public class GrenadeThrowInfo
    {
        #region Fields

        public AnimationCurve rollCurve;

        public float rollDuration;

        public float rollMoveSpeed;

        public float rollRotSpeed;

        public AnimationCurve throwCurve;

        public float throwMultiplier;

        #endregion
    }
}