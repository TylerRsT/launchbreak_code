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
    public abstract class HeatBasedWeaponEquippable : WeaponEquippable
    {
        #region Const

        private static readonly Color[] _loadLevelColors = new Color[]
        {
            new Color(0.149f, 0.973f, 1.000f),
            new Color(0.967f, 0.859f, 0.145f),
            new Color(1.000f, 0.561f, 0.149f),
            new Color(1.000f, 0.149f, 0.475f),
        };

        #endregion

        #region Override

        /// <summary>
        /// 
        /// </summary>
        public override void Trigger()
        {
            base.Trigger();

            if (CanFire())
            {
                Fire();
                shooter?.GetComponent<CharacterInput>()?.Vibrate(0.5f, 0.5f, 0.1f);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void OnEquip()
        {
            base.OnEquip();

            if (isOverheating)
            {
                _muzzleParticleSystem.Play();
            }

            if (character.powerUpIds.Contains(PowerUpId.Cooler))
            {
                ShowMuzzleCooler();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void OnUnequip()
        {
            base.OnUnequip();

            HideMuzzleCooler();
            _muzzleParticleSystem.Stop();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Cooldown()
        {
            base.Cooldown();

            isOverheating = false;
            _muzzleParticleSystem.Stop();
            _heat = 0.0f;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            _muzzleParticleSystem = _muzzleTransform.GetComponent<ParticleSystem>();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void FixedUpdate()
        {
            base.Update();

            var recoveryDelay = weaponDescriptor.recoveryDelay;

            elapsedSinceLastFire += GameplayStatics.gameFixedDeltaTime;
            if (isOverheating)
            {
                recoveryDelay += weaponDescriptor.overheatPenalty;
            }

            if (elapsedSinceLastFire >= recoveryDelay && !isFiring)
            {
                _heat = Mathf.Max(_heat - weaponDescriptor.recoveryAmount * GameplayStatics.gameFixedDeltaTime, 0.0f);
            }

            if (isOverheating && _heat == 0.0f)
            {
                Cooldown();
            }

            if (isOverheating)
            {
                _currentLoadLevel = weaponDescriptor.loadLevels.Count - 1;
            }
            else
            {
                for (var i = 0; i < weaponDescriptor.loadLevels.Count; ++i)
                {
                    if (_heat >= weaponDescriptor.loadLevels[i])
                    {
                        _currentLoadLevel = i;
                    }
                }
            }

            var nextColor = _loadLevelColors[_currentLoadLevel];
            spriteRenderer.material.SetColor("_AltDesiredColor", nextColor);
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (_weaponKnockbackSequence != null && _weaponKnockbackSequence.IsActive())
            {
                _weaponKnockbackSequence.Kill();
                _weaponKnockbackSequence = null;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        public void ShowMuzzleCooler()
        {
            spriteRenderer.material.SetColor("_DesiredColor", new Color(0.0f, 1.0f, 1.0f, 1.0f));
        }

        /// <summary>
        /// 
        /// </summary>
        public void HideMuzzleCooler()
        {
            spriteRenderer.material.SetColor("_DesiredColor", Pickable.blackOutlineColor);
        }

        /// <summary>
        /// 
        /// </summary>
        protected void Vibrate(float intensity, float duration)
        {
            shooter?.GetComponent<CharacterInput>()?.Vibrate(intensity, intensity, duration);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected bool AddHeat(float value)
        {
            if(value > 0 && character.powerUpIds.Contains(PowerUpId.Cooler))
            {
                value /= 2.0f;
            }
            heat += value;
            return _heat >= 1.0f;
        }

        /// <summary>
        /// 
        /// </summary>
        protected void DoKnockbackSequence()
        {
            if (_weaponKnockbackSequence != null && _weaponKnockbackSequence.IsActive())
            {
                _weaponKnockbackSequence.Complete();
            }

            if (_characterKnockbackSequence != null && _characterKnockbackSequence.IsActive())
            {
                _characterKnockbackSequence.Complete();
            }

            if (weaponDescriptor.fireKnockback != 0.0f && weaponDescriptor.fireKnockbackDuration > 0.0f)
            {
                _weaponKnockbackSequence = DOTween.Sequence();
                _weaponKnockbackSequence.Append(transform.DOLocalMove(orientation * weaponDescriptor.fireKnockback, weaponDescriptor.fireKnockbackDuration / 2.0f));
                _weaponKnockbackSequence.Append(transform.DOLocalMove(Vector2.zero, weaponDescriptor.fireKnockbackDuration / 2.0f));

                character.body.MovePosition(character.body.position + orientation * weaponDescriptor.fireKnockback);
            }

            var cameraShake = weaponDescriptor.cameraShake;
            if (cameraShake != null)
            {
                CameraShakeManager.instance.Shake(cameraShake);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected abstract bool CanFire();

        /// <summary>
        /// 
        /// </summary>
        protected abstract void Fire();

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public bool isOverheating { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        protected virtual bool isFiring => false;

        /// <summary>
        /// 
        /// </summary>
        protected float heat
        {
            get { return _heat; }
            set
            {
                _heat = Mathf.Min(value, 1.0f);
                if(value >= 1.0f)
                {
                    isOverheating = true;
                    _muzzleParticleSystem.Play();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected Transform muzzleTransform => _muzzleTransform;

        /// <summary>
        /// 
        /// </summary>
        protected float elapsedSinceLastFire { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private new HeatBasedWeaponDescriptor weaponDescriptor => base.weaponDescriptor as HeatBasedWeaponDescriptor;

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Transform _muzzleTransform = default;

        private float _heat = 0.0f;

        private ParticleSystem _muzzleParticleSystem;

        private int _currentLoadLevel = 0;

        private Sequence _characterKnockbackSequence = null;
        private Sequence _weaponKnockbackSequence = null;

        #endregion
    }
}