using Elendow.SpritedowAnimator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class KatanaBuff : CharacterBuff, IDamageProvider
    {
        #region Const

        private const string CharacterLayerName = "Character";
        private const string CharacterKatanaLayerName = "CharacterKatana";

        #endregion

        #region IDamageProvider

        /// <summary>
        /// 
        /// </summary>
        string IDamageProvider.providerName => "Katana";

        /// <summary>
        /// 
        /// </summary>
        CharacterSpawner IDamageProvider.providerSpawner => character.spawner;

        #endregion

        #region Override

        /// <summary>
        /// 
        /// </summary>
        protected override void Start()
        {
            base.Start();

            var katanaObject = GameObject.Instantiate(powerUpData.katanaPrefab);
            katanaObject.transform.SetParent(character.transform);
            katanaObject.transform.localPosition = Vector3.zero;

            _katanaAnimator = katanaObject.GetComponent<SpriteAnimator>();
            _katanaCollider = katanaObject.GetComponent<Collider2D>();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Update()
        {
            base.Update();

            if (!_isAttacking)
            {
                _katanaCollider.transform.localScale = new Vector3(character.GetComponent<SpriteRenderer>().flipX ? -1.0f : 1.0f, 1.0f, 1.0f);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnDestroy()
        {
            base.OnDestroy();

            Destroy(_katanaAnimator.gameObject);
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        public void SlashBegin()
        {
            _isAttacking = true;
            character.targetOrientationLocked = true;
            character.gameObject.layer = LayerMask.NameToLayer(CharacterKatanaLayerName);

            AttackOrientation();

            _katanaAnimator.Play(_katanaAnimator.animations[0], true);
            this.EmitSound(powerUpData.katanaSlashSounds);
        }

        /// <summary>
        /// 
        /// </summary>
        public void SlashUpdate()
        {
            var contactFilter = new ContactFilter2D();
            contactFilter.SetLayerMask(LayerMask.GetMask("Character"));
            var hits = new List<Collider2D>();

            _katanaCollider.OverlapCollider(contactFilter, hits);

            foreach(var item in hits)
            {
                var characterHit = item.gameObject.GetComponent<Character>();

                if(characterHit == null)
                {
                    continue;
                }

                if (!_characterHits.Contains(characterHit) && characterHit != character)
                {
                    _characterHits.Add(item.gameObject.GetComponent<Character>());

                    if (!characterHit.isInvincible)
                    {
                        characterHit.TakeDamages(new DamageInfo
                        {
                            provider = this,
                            damages = powerUpData.damages,
                            damageType = DamageType.Normal,
                        });
                        KnockbackCharacter(characterHit);
                    }
                }
            }    
        }

        /// <summary>
        /// 
        /// </summary>
        public void SlashEnd()
        {
            _characterHits.Clear();

            AttackOrientationReset();

            _isAttacking = false;
            character.targetOrientationLocked = false;
            character.gameObject.layer = LayerMask.NameToLayer(CharacterLayerName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
        private void KnockbackCharacter(Character character)
        {
            Vector2 diff = transform.position - character.transform.position;
            diff.Normalize();
            diff *= -1.0f;

            character.Push(diff, powerUpData.knockbackCurve, powerUpData.knockback, powerUpData.knockbackDuration);
        }

        /// <summary>
        /// 
        /// </summary>
        private void AttackOrientation()
        {
            var orientation = character.moveOrientation;
            if(orientation == Vector2.zero)
            {
                orientation = character.targetOrientation;
            }

            if (orientation.y > 0.5f)
            {
                _katanaCollider.transform.localScale = new Vector3(1.0f, -1.0f, 1.0f);
                Quaternion quat = Quaternion.Euler(0, 0, 90);
                _katanaAnimator.transform.localRotation = quat;
            }
            else if (orientation.y < -0.5f)
            {
                _katanaCollider.transform.localScale = new Vector3(1.0f, -1.0f, 1.0f);
                Quaternion quat = Quaternion.Euler(0, 0, -90);
                _katanaAnimator.transform.localRotation = quat;
            }
            else
            {
                _katanaCollider.transform.localScale = new Vector3(orientation.x < 0.0f ? -1.0f : 1.0f, 1.0f, 1.0f);
                character.GetComponent<SpriteRenderer>().flipX = orientation.x < 0.0f;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void AttackOrientationReset()
        {
            _katanaCollider.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            Quaternion quat = Quaternion.Euler(0, 0, 0);
            _katanaAnimator.transform.localRotation = quat;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public KatanaPowerUpData powerUpData { get; set; }

        #endregion

        #region Fields

        private SpriteAnimator _katanaAnimator;
        private Collider2D _katanaCollider;
        private List<Character> _characterHits = new List<Character>();
        private bool _isAttacking = false;

        #endregion
    }
}
