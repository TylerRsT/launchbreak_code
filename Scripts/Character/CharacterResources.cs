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
    public class CharacterResources : MonoBehaviour
    {
        #region Const

        private const float ShowSupplyTimeout = 1.5f;

        #endregion

        #region Messages

        /// <summary>
        /// 
        /// </summary>
        private void Awake()
        {
            _character = GetComponent<Character>();

            _healthSpriteAnimator = _healthSpriteRenderer.GetComponent<SpriteAnimator>();
            _healthSpriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);

            _supplySpriteAnimator = _supplySpriteRenderer.GetComponent<SpriteAnimator>();
            _supplySpriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);

            _armorSpriteAnimator = _armorSpriteRenderer.GetComponent<SpriteAnimator>();
            _armorSpriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);

            _ammoSpriteAnimator = _ammoSpriteRenderer.GetComponent<SpriteAnimator>();
            _ammoSpriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);

            _health = _maxHealth;

            var supplyAnimation = _supplySpriteAnimator.animations[0];
            _missingSupplySprite = supplyAnimation.GetFrame(supplyAnimation.FramesCount - 2);
            _enoughSupplySprite = supplyAnimation.GetFrame(supplyAnimation.FramesCount - 1);
        }

        /// <summary>
        /// 
        /// </summary>
        private void Update()
        {
            if (_isInCombat || _isRegenerating)
            {
                _regenElapsed += GameplayStatics.gameDeltaTime;

                if (_isInCombat && _regenElapsed >= _regenerationDelay)
                {
                    _isInCombat = false;
                    _isRegenerating = true;

                    if (hasArmor || _health == _maxHealth)
                    {
                        _healthSpriteRenderer.DOFade(0.0f, 0.5f);
                        _armorSpriteRenderer.DOFade(0.0f, 0.5f);
                    }
                }

                if (_isRegenerating && _regenElapsed >= _regenerationInterval)
                {
                    health += 1;
                    _regenElapsed = 0.0f;

                    if (health == _maxHealth)
                    {
                        _isRegenerating = false;
                        _healthSpriteRenderer.DOFade(0.0f, 0.5f);
                        _armorSpriteRenderer.DOFade(0.0f, 0.5f);
                    }
                }
            }

            if (_character.triggerState == CharacterTriggerState.Construction)
            {
                _healthSpriteRenderer.color = Color.white;
                _supplySpriteRenderer.color = Color.white;
            }
            else if (_showSupply)
            {
                _supplyElapsed += GameplayStatics.gameDeltaTime;
                if(_supplyElapsed >= ShowSupplyTimeout)
                {
                    _showSupply = false;
                    _supplyElapsed = 0.0f;
                    _supplyFadeoutTween = _supplySpriteRenderer.DOFade(0.0f, 0.2f);
                    _supplyFadeoutTween.onComplete += (() =>
                    {
                        HideSupply();
                    });
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="damages"></param>
        public void TakeDamages(int damages)
        {
            if (armor > 0)
            {
                var remainingDamages = damages - armor;
                armor = Mathf.Max(0, armor - damages);
                damages = remainingDamages;

                if(armor == 0 && _character.powerUpIds.Contains(PowerUpId.Armor))
                {
                    var dropManagers = FindObjectsOfType<DropManager>();
                    foreach(var dropManager in dropManagers)
                    {
                        dropManager.Free(PowerUpId.Armor);
                    }
                }
            }

            if (damages > 0)
            {
                health = Mathf.Max(0, health - damages);
                if (health == 0)
                {
                    _healthSpriteRenderer.DOFade(0.0f, 0.5f);
                    enabled = false;
                    return;
                }
            }

            _isInCombat = true;

            if (!hasArmor)
            {
                _regenElapsed = 0.0f;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cost"></param>
        public void TakeSupply(int cost)
        {
            supply = Mathf.Max(0, supply - cost);
        }

        /// <summary>
        /// 
        /// </summary>
        public void ShowSupply()
        {
            KillSupplyTween();

            _showSupply = true;
            _supplyElapsed = 0.0f;
            _supplySpriteRenderer.color = Color.white;
        }

        /// <summary>
        /// 
        /// </summary>
        public void HideSupply()
        {
            _showSupply = false;
            _supplyElapsed = 0.0f;

            KillSupplyTween();
            _supplySpriteRenderer.color = _supplySpriteRenderer.color.MakeTransparent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reason"></param>
        /// <param name="showSupply"></param>
        public void SupplyAnim(SupplyAnimReason reason, bool showSupply = false)
        {
            if (_noSupplyCoroutine == null)
            {
                if (showSupply)
                {
                    ShowSupply();
                }
                _noSupplyCoroutine = StartCoroutine(DoSupplyAnim(reason == SupplyAnimReason.NotEnough ?
                    _missingSupplySprite : _enoughSupplySprite));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void HideHealth()
        {
            _healthSpriteRenderer.color = _healthSpriteRenderer.color.MakeTransparent();
            _armorSpriteRenderer.color = _armorSpriteRenderer.color.MakeTransparent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="replacementSprite"></param>
        private IEnumerator DoSupplyAnim(Sprite replacementSprite)
        {
            var elapsed = 0.0f;
            while(elapsed < 1.0f)
            {
                _supplySpriteRenderer.sprite = elapsed % 0.4f < 0.2f ? replacementSprite : _supplySpriteAnimator.animations[0].GetFrame(_supply);

                yield return new WaitForSeconds(0.2f);
                elapsed += 0.2f;
            }

            _noSupplyCoroutine = null;
            _supplySpriteRenderer.sprite = _supplySpriteAnimator.animations[0].GetFrame(_supply);
        }

        /// <summary>
        /// 
        /// </summary>
        private void KillSupplyTween()
        {
            if (_supplyFadeoutTween != null && _supplyFadeoutTween.IsActive())
            {
                _supplyFadeoutTween.Complete(false);
                _supplyFadeoutTween = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void ShowHealth()
        {
            _healthSpriteRenderer.color = Color.white;
            _healthSpriteRenderer.sprite = _healthSpriteAnimator.animations[0].GetFrame(health);

            if (armor > 0)
            {
                _armorSpriteRenderer.color = Color.white;
                _armorSpriteRenderer.sprite = _armorSpriteAnimator.animations[0].GetFrame(armor - 1);
            }
            else
            {
                _armorSpriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public bool hasArmor => _armor > 0;

        /// <summary>
        /// 
        /// </summary>
        public int maxHealth => _maxHealth;

        /// <summary>
        /// 
        /// </summary>
        public int maxSupply => _maxSupply;

        /// <summary>
        /// 
        /// </summary>
        public int maxArmor => _maxArmor;

        /// <summary>
        /// 
        /// </summary>
        public int health
        {
            get { return _health; }
            private set
            {
                value = Mathf.Min(value, _maxHealth);
                if(value != _health)
                {
                    _health = value;
                    ShowHealth();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int supply
        {
            get { return _supply; }
            set
            {
                value = Mathf.Min(value, _maxSupply);
                _supply = value;
                _supplySpriteRenderer.sprite = _supplySpriteAnimator.animations[0].GetFrame(value);

                ShowSupply();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int armor
        {
            get { return _armor; }
            set
            {
                value = Mathf.Min(value, _maxArmor);
                _armor = value;
                ShowHealth();
            }
        }

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private int _maxHealth = 4;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private int _maxSupply = 8;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private int _maxArmor = 4;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _regenerationDelay = 7.0f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _regenerationInterval = 1.0f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private SpriteRenderer _healthSpriteRenderer = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private SpriteRenderer _supplySpriteRenderer = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private SpriteRenderer _armorSpriteRenderer = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private SpriteRenderer _ammoSpriteRenderer = default;

        private int _health;
        private int _supply;
        private int _armor;

        private Character _character;

        private SpriteAnimator _healthSpriteAnimator;
        private SpriteAnimator _supplySpriteAnimator;
        private SpriteAnimator _armorSpriteAnimator;
        private SpriteAnimator _ammoSpriteAnimator;

        private Sprite _missingSupplySprite;
        private Sprite _enoughSupplySprite;
        private Coroutine _noSupplyCoroutine;

        private bool _isRegenerating = false;
        private bool _isInCombat = false;
        private float _regenElapsed = 0.0f;

        private bool _showSupply = false;
        private float _supplyElapsed = 0.0f;
        private Tween _supplyFadeoutTween;

        #endregion

        #region Nested

        /// <summary>
        /// 
        /// </summary>
        public enum SupplyAnimReason
        {
            NotEnough,
            Enough,
        }

        #endregion
    }
}
