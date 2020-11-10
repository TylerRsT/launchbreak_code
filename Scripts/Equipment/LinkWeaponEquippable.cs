using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class LinkWeaponEquippable : HeatBasedWeaponEquippable
    {
        #region Const

        private const string SolidLayerName = "Solid";
        private const string CharacterLayerName = "Character";

        private const int CooldownCapacity = 8;

        #endregion

        #region Override

        /// <summary>
        /// 
        /// </summary>
        public override void OnUnequip()
        {
            base.OnUnequip();
            Release();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Release()
        {
            base.Release();

            if(_shakeTween != null)
            {
                _shakeTween.Kill();
                _shakeTween = null;
            }

            _isFiring = false;
            if (_link != null)
            {
                Destroy(_link);
                _link = null;
            }

            if (_linkEnd != null)
            {
                Destroy(_linkEnd);
                _linkEnd = null;
            }

            _currentDamageComponent = null;
            _elapsedSinceSameTarget = 0.0f;
            _elapsedSinceFire = 0.0f;
            _isLoaded = false;

            spriteRenderer.sprite = _standardSprite;
            StopSounds();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orientation"></param>
        public override void SetTargetOrientation(Vector2 orientation)
        {
            if (_isFiring)
            {
                var diff = orientation - _lastTargetOrientation;
                diff /= linkWeaponDescriptor.orientationSpeedReducer;
                orientation = _lastTargetOrientation + diff;
            }

            base.SetTargetOrientation(orientation);
            _lastTargetOrientation = orientation;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            _solidLayerMask = LayerMask.GetMask(SolidLayerName, CharacterLayerName);
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            var secondsPerDamage = 1.0f / linkWeaponDescriptor.damagesPerSecond;
            for (var i = 0; i < _cooldowns.Count; ++i)
            {
                var cooldown = _cooldowns[i];
                cooldown.elapsed += GameplayStatics.gameFixedDeltaTime;
                _cooldowns[i] = cooldown;
            }

            _cooldowns.RemoveAll((cooldown) =>
            {
                return cooldown.elapsed >= secondsPerDamage;
            });

            if (_isFiring)
            {
                _elapsedSinceFire += GameplayStatics.gameFixedDeltaTime;
                if (_elapsedSinceFire >= linkWeaponDescriptor.startupTime)
                {
                    Link();
                }
                else if(_elapsedSinceFire >= linkWeaponDescriptor.startupTime * (2.0f / 3.0f))
                {
                    spriteRenderer.sprite = _loadEndSprite;
                }
                else
                {
                    spriteRenderer.sprite = _loadBeginSprite;
                }
            }
            else
            {
                _elapsedSinceFire = 0.0f;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override bool CanFire()
        {
            Vibrate(0.5f, 0.1f);
            return !isOverheating && elapsedSinceLastFire >= linkWeaponDescriptor.rateOfFire;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Fire()
        {
            if (!_isFiring)
            {
                _isFiring = true;

                if (_shakeTween != null)
                {
                    _shakeTween.Kill();
                }

                _shakeTween = DOTween.Sequence();
                _shakeTween.Append(transform.DOShakePosition(1.0f, 1.0f, 15, fadeOut: false));
                _shakeTween.SetLoops(-1);

                _soundInstances.AddRange(this.EmitSound(linkWeaponDescriptor.startupSounds));
                _soundInstances.RegisterAsGameplayLooping();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override bool isFiring => _isFiring;

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        private void Link()
        {
            var gameObjects = FindObjectsOfType<GameObject>().Where(x => x.layer == LayerMask.NameToLayer(SolidLayerName)).ToArray();

            if (AddHeat(linkWeaponDescriptor.heatPerShot * GameplayStatics.gameFixedDeltaTime))
            {
                Release();
                return;
            }

            if (!_isLoaded)
            {
                if (_shakeTween != null)
                {
                    _shakeTween.Kill();
                }

                _shakeTween = DOTween.Sequence();
                _shakeTween.Append(transform.DOShakePosition(1.0f, 1.5f, 30, fadeOut: false));
                _shakeTween.SetLoops(-1);

                spriteRenderer.sprite = _standardSprite;
                _isLoaded = true;
                StopSounds();
                _soundInstances.AddRange(this.EmitSound(linkWeaponDescriptor.fireSounds));
                _soundInstances.RegisterAsGameplayLooping();
            }
            Vibrate(0.75f, 0.1f);

            elapsedSinceLastFire = 0.0f;
            var offset = new Vector2(0.0f, linkWeaponDescriptor.beamHeight / 2.0f);
            var raycastHit = Physics2D.Raycast((Vector2)muzzleTransform.position + offset, _lastTargetOrientation, Mathf.Infinity, _solidLayerMask);

            offset *= -1.0f;
            var bottomRaycastHit = Physics2D.Raycast((Vector2)muzzleTransform.position + offset, _lastTargetOrientation, Mathf.Infinity, _solidLayerMask);

            if (raycastHit.collider == null || (bottomRaycastHit.collider != null && bottomRaycastHit.distance < raycastHit.distance))
            {
                raycastHit = bottomRaycastHit;
                offset *= -1.0f;
            }

            if (raycastHit.collider == null)
            {
                return;
            }

            if (_link == null)
            {
                _link = Instantiate(_linkPrefab, muzzleTransform);
            }
            if(_linkEnd == null)
            {
                _linkEnd = Instantiate(_linkEndPrefab, raycastHit.point, Quaternion.identity, character.transform);
            }

            var heightScale = Random.Range(linkWeaponDescriptor.beamMinScale, linkWeaponDescriptor.beamMaxScale);
            _link.transform.localScale = new Vector2(raycastHit.distance / 3.0f, heightScale);
            _link.transform.localPosition = new Vector2(raycastHit.distance / 2.0f, 0.0f);

            _linkEnd.transform.position = raycastHit.point + offset;
            _linkEnd.transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles);
            _linkEnd.transform.localScale = new Vector2(transform.localScale.x, heightScale);

            var damageComponent = raycastHit.collider.GetComponent<DamageComponent>();
            if(damageComponent == null)
            {
                _currentDamageComponent = null;
                return;
            }

            if(damageComponent != _currentDamageComponent)
            {
                _currentDamageComponent = damageComponent;
                _elapsedSinceSameTarget = 0.0f;
                SendDamages(damageComponent, (int)linkWeaponDescriptor.damages);
            }
            else
            {
                _currentDamageComponent = damageComponent;
                _elapsedSinceSameTarget += GameplayStatics.gameFixedDeltaTime;
            }

            var secondsPerDamage = 1.0f / linkWeaponDescriptor.damagesPerSecond;
            if(_elapsedSinceSameTarget >= secondsPerDamage)
            {
                SendDamages(damageComponent, 1);
                _elapsedSinceSameTarget -= secondsPerDamage;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="damageComponent"></param>
        /// <param name="damages"></param>
        private void SendDamages(DamageComponent damageComponent, int damages)
        {
            var cooldown = _cooldowns.FirstOrDefault(x => x.damageComponent == damageComponent);
            if (cooldown != null)
            {
                return;
            }

            if(damageComponent.isInvincible)
            {
                return;
            }

            damageComponent.TakeDamages(new DamageInfo
            {
                provider = this,
                damages = damages,
                damageType = linkWeaponDescriptor.damageType,
            });

            var character = damageComponent as Character;
            if(character != null)
            {
                character.Push(orientation, Random.Range(800.0f, 1000.0f), 0.1f);
            }

            cooldown = new LinkCooldown
            {
                damageComponent = damageComponent,
                elapsed = 0.0f,
            };
            _cooldowns.Add(cooldown);
        }

        /// <summary>
        /// 
        /// </summary>
        private void StopSounds()
        {
            foreach(var soundInstance in _soundInstances)
            {
                soundInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                soundInstance.UnregisterAsGameplayLooping();
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        protected LinkWeaponDescriptor linkWeaponDescriptor => weaponDescriptor as LinkWeaponDescriptor;

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private GameObject _linkPrefab = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private GameObject _linkEndPrefab = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Sprite _standardSprite = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Sprite _loadBeginSprite = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Sprite _loadEndSprite = default;

        private DamageComponent _currentDamageComponent;
        private float _elapsedSinceSameTarget = 0.0f;

        private int _solidLayerMask;

        private GameObject _link;
        private GameObject _linkEnd;

        private bool _isFiring;
        private bool _isLoaded = false;
        private float _elapsedSinceFire = 0.0f;
        private Vector2 _lastTargetOrientation;

        private List<LinkCooldown> _cooldowns = new List<LinkCooldown>(CooldownCapacity);

        private Sequence _shakeTween;

        private List<FMOD.Studio.EventInstance> _soundInstances = new List<FMOD.Studio.EventInstance>();

        #endregion

        #region Nested

        /// <summary>
        /// 
        /// </summary>
        private class LinkCooldown
        {
            public DamageComponent damageComponent;
            public float elapsed;
        }

        #endregion
    }
}