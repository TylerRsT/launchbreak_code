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
    public class CharacterDashAttack
    {
        #region Const

        private const int DashDamages = 1;

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
        /// <param name="orientation"></param>
        public CharacterDashAttack(Character character, Vector2 orientation)
        {
            this.character = character;
            this.orientation = orientation;
        }

        #endregion

        #region Operators

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dashAttack"></param>
        public static implicit operator DamageInfo(CharacterDashAttack dashAttack)
        {
            return new DamageInfo
            {
                provider = dashAttack.character,
                damages = DashDamages,
            };
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        public void Accept()
        {
            received = true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public Character character { get; }

        /// <summary>
        /// 
        /// </summary>
        public Vector2 orientation { get; }

        /// <summary>
        /// 
        /// </summary>
        public bool received { get; private set; } = false;

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public class CharacterGameplayState_Dash : CharacterGameplayStateHandler
    {
        #region Const

        private const string DashSoundKey = "Dash";
        private const string DashCancelSoundKey = "DashCancel";

        private const string DashHitShakeName = "SHK_DashHitShake";

        private const string MaterialFlashColorParamName = "_FlashColor";
        private const int TrailFrameInterval = 3;

        private static readonly Color TrailBeginColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        private static readonly Color TrailEndColor = new Color(0.149f, 0.616f, 1.0f, 1.0f);

        #endregion

        #region Override

        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
        public override void Initialize(Character character)
        {
            base.Initialize(character);

            _rigidbody = character.GetComponent<Rigidbody2D>();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Enter()
        {
            base.Enter();

            _dashProperties = character.statsDescriptor.dashProperties;

            if(_dashDeflectAbility.abilityData == null)
            {
                _dashDeflectAbility.abilityData = character.spawner.dashDeflectAbilityData;
            }

            if(character.moveOrientation == Vector2.zero)
            {
                moveOrientation = character.targetOrientation;
            }
            else
            {
                moveOrientation = character.moveOrientation;
            }

            character.SetTriggerState(CharacterTriggerState.None);
            character.SetCosmeticAlpha(0.5f);

            character.dashParticleSystem.Play();
            character.GetComponent<SpriteAnimator>().Play(character.skinDescriptor.dashAnimation, true);

            _hasKatana = character.powerUpIds.Contains(PowerUpId.Katana);
            if(_hasKatana)
            {
                _hasKatana = true;
                _katana = character.GetComponent<KatanaBuff>();
                _dashProperties = _katana.powerUpData.dashProperties;
                _katana.SlashBegin();
            }

            _elapsed = 0.0f;
            isDeflectActive = false;

            dashAttack = new CharacterDashAttack(character, moveOrientation);

            character.EmitSound(DashSoundKey);

            Telemetry.game.Incr("dashes");
        }

        /// <summary>
        /// 
        /// </summary>
        public override void FixedUpdate()
        {
            base.FixedUpdate();

            float lastElapsed = _elapsed;
            _elapsed += GameplayStatics.gameFixedDeltaTime;

            if (_elapsed >= _dashProperties.dashDuration)
            {
                character.SetGameplayState(CharacterGameplayState.Default);
                return;
            }

            // Trail code
            if (_trailFrame % TrailFrameInterval == 0)
            {
                var trailSprite = GameObject.Instantiate(Bootstrap.instance.data.emptyTileSpritePrefab, character.transform.position, character.transform.rotation);
                var trailSpriteRenderer = trailSprite.GetComponent<SpriteRenderer>();
                trailSpriteRenderer.sprite = character.GetComponent<SpriteRenderer>().sprite;
                trailSpriteRenderer.flipX = character.GetComponent<SpriteRenderer>().flipX;
                trailSpriteRenderer.material = Bootstrap.instance.data.spriteFlashMaterial;
                trailSpriteRenderer.material.SetColor(MaterialFlashColorParamName, character.characterDescriptor.mainColor);
                var tween = trailSpriteRenderer.DOFade(0.0f, _dashProperties.dashDuration);
                _dashTweens.Add(tween);
                tween.onComplete += (() =>
                {
                    GameObject.Destroy(trailSprite);
                    _dashTweens.Remove(tween);
                });
            }
            _trailFrame = (_trailFrame + 1) % TrailFrameInterval;

            float lastValue = _dashProperties.dashCurve.Evaluate(lastElapsed);
            float currentValue = _dashProperties.dashCurve.Evaluate(_elapsed);

            float diff = (currentValue - lastValue) * _dashProperties.dashSpeedMultiplier;

            float radians = Mathf.Deg2Rad * Vector2.SignedAngle(Vector2.right, moveOrientation);
            _rigidbody.velocity = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)) * diff;

            if(_hasKatana)
            {
                _katana.SlashUpdate();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Exit()
        {
            base.Exit();

            character.dashCooldown = _dashProperties.dashCooldown;
            character.SetCosmeticAlpha(1.0f);

            if (_hasKatana)
            {
                _katana.SlashEnd();
            }

            character.dashParticleSystem.Stop();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        public override void Action(CharacterAction action)
        {
            base.Action(action);

            switch (action)
            {
                case CharacterAction.Activate:
                    character.spawner.TriggerAllTraps();
                    break;
                case CharacterAction.Switch:
                    character.SwitchWeapon();
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bullet"></param>
        /// <returns></returns>
        public override bool ReceiveBullet(Bullet bullet)
        {
            if(_dashDeflectAbility.TryDeflectBullet(this, bullet))
            {
                Telemetry.game.Incr("dashDeflects");
            }

            return false;
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        public void Cancel()
        {
            character.EmitSound(DashCancelSoundKey);
            CameraShakeManager.instance.Shake(DashHitShakeName);

            var characterInput = character.GetComponent<CharacterInput>();
            if (characterInput != null)
            {
                character.GetComponent<CharacterInput>()?.Vibrate();
            }

            character.SetGameplayState(CharacterGameplayState.Default);
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public float elapsed => _elapsed;

        /// <summary>
        /// 
        /// </summary>
        public bool isActive
        {
            get
            {
                return _elapsed >= _dashProperties.dashActiveStart &&
                    _elapsed <= (_dashProperties.dashActiveStart + _dashProperties.dashActiveDuration);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public CharacterDashAttack dashAttack { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public bool isDeflectActive { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Vector2 moveOrientation { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DashDeflectAbility dashDeflectAbility => _dashDeflectAbility;

        #endregion

        #region Fields

        private Rigidbody2D _rigidbody;

        private float _elapsed = 0.0f;

        private int _trailFrame = 0;

        private List<Tween> _dashTweens = new List<Tween>();

        private DashProperties _dashProperties;

        private DashDeflectAbility _dashDeflectAbility = new DashDeflectAbility();

        private bool _hasKatana = false;
        private KatanaBuff _katana;

        #endregion
    }
}