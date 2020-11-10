using Elendow.SpritedowAnimator;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public enum CharacterAction
    {
        Fire,
        Release,
        Dash,
        Pick,
        Drop,
        Switch,
        Activate,
        Throw,
    }

    /// <summary>
    /// 
    /// </summary>
    [RequireComponent(typeof(SpriteAnimator))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(CapsuleCollider2D))]
    public class Character : ShooterBehaviour
    {
        #region Const

        private const int MaxWeaponsNumber = 2;
        private const float DamageFlashDivider = 4.0f;

        private const string OnReceivingDashAttackMethodName = "OnReceivingDashAttack";
        private const float DashCancelAngleTolerance = 0.35f;

        #endregion

        #region Override

        /// <summary>
        /// 
        /// </summary>
        /// <param name="damageInfo"></param>
        public override void TakeDamages(DamageInfo damageInfo)
        {
            base.TakeDamages(damageInfo);

            var input = GetComponent<CharacterInput>();
            if (input != null)
            {
                input.Vibrate();
            }

            gameplayStateHandler?.OnTakeDamages(damageInfo);
            _resources.TakeDamages(damageInfo.damages);

            if (damageInfo.provider != null)
            {
                Telemetry.game.Incr($"{damageInfo.provider.providerName}_char_dmg", damageInfo.damages);
            }

            if (_resources.health > 0)
            {
                var hitFlash = GetComponent<HitFlash>();
                hitFlash?.Flash(1.0f, damageInfo.damages / DamageFlashDivider, 0.05f);
            }
            else if(!GameMode.instance.isGameOver)
            {
                if (damageInfo.provider != null)
                {
                    damageInfo.provider.providerSpawner.killCount++;
                    Telemetry.game.Incr($"{damageInfo.provider.providerName}_char_kill");
                }
                deathDamageType = damageInfo.damageType;
                currentEquippable?.OnCharacterDied();
                SetGameplayState(CharacterGameplayState.Dead);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override string damageProviderName => "Character";

        #endregion

        #region Messages

        /// <summary>
        /// 
        /// </summary>
        private void Reset()
        {
            var rigidbody = GetComponent<Rigidbody2D>();

            rigidbody.bodyType = RigidbodyType2D.Kinematic;
            rigidbody.isKinematic = true;
        }

        /// <summary>
        /// 
        /// </summary>
        private void Awake()
        {
            _body = GetComponent<Rigidbody2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _resources = GetComponent<CharacterResources>();
            _capsuleCollider = GetComponent<CapsuleCollider2D>();

            foreach (var handler in _gameplayStates.Values)
            {
                handler.Initialize(this);
            }

            foreach (var handler in _triggerStates.Values)
            {
                handler.Initialize(this);
            }

            SetGameplayState(gameplayState, true);
            SetTriggerState(triggerState, true);
        }

        /// <summary>
        /// 
        /// </summary>
        private void Start()
        {
            GameMode.instance?.characterTransformSorter?.AddCharacter(this);
        }

        /// <summary>
        /// 
        /// </summary>
        private void Update()
        {
            gameplayStateHandler?.Update();
            triggerStateHandler?.Update();

            dashCooldown = Mathf.Max(0.0f, dashCooldown - GameplayStatics.gameDeltaTime);
        }

        /// <summary>
        /// 
        /// </summary>
        private void FixedUpdate()
        {
            gameplayStateHandler?.FixedUpdate();
            triggerStateHandler?.FixedUpdate();
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnDestroy()
        {
            GameMode.instance?.characterTransformSorter?.RemoveCharacter(this);

            var dropManagers = FindObjectsOfType<DropManager>();
            foreach (var dropManager in dropManagers)
            {
                dropManager.Free(powerUpIds);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collision"></param>
        private void OnCollisionStay2D(Collision2D collision)
        {
            if(_isMoveBeingAdjusted)
            {
                return;
            }

            var dashState = gameplayStateHandler as CharacterGameplayState_Dash;
            if (dashState == null)
            {
                return;
            }

            var normal = collision.contacts.FirstOrDefault(x => x.otherRigidbody == _body).normal;
            var moveOrientation = dashState.moveOrientation;

            if ((Mathf.Abs(normal.x - moveOrientation.y) <= DashCancelAngleTolerance)
                || (Mathf.Abs(normal.y - moveOrientation.x) <= DashCancelAngleTolerance))
            {
                dashState.moveOrientation = moveOrientation.Straighten();
                return;
            }

            if (dashState != null && dashState.isActive && !dashState.dashAttack.received)
            {
                collision.gameObject.SendMessage(OnReceivingDashAttackMethodName, dashState.dashAttack, SendMessageOptions.DontRequireReceiver);
                if (dashState.dashAttack.received)
                {
                    dashState.Cancel();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collision"></param>
        private void OnTriggerStay2D(Collider2D collision)
        {
            var wasMoveBeingAdjusted = _isMoveBeingAdjusted;
            _isMoveBeingAdjusted = CheckPositionAdjustment(collision);

            if(_isMoveBeingAdjusted && !wasMoveBeingAdjusted)
            {
                Telemetry.game.Incr("moveAdjusts");
            }

            if(collision.GetComponent<Character>() != null)
            {
                var dashState = gameplayStateHandler as CharacterGameplayState_Dash;
                if (dashState == null)
                {
                    return;
                }

                if (dashState != null && dashState.isActive && !dashState.dashAttack.received)
                {
                    collision.gameObject.SendMessage(OnReceivingDashAttackMethodName, dashState.dashAttack, SendMessageOptions.DontRequireReceiver);
                    if (dashState.dashAttack.received)
                    {
                        dashState.Cancel();
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collision"></param>
        private void OnTriggerExit2D(Collider2D collision)
        {
            _isMoveBeingAdjusted = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bulletResponse"></param>
        private void OnReceivingBullet(BulletResponse bulletResponse)
        {
            if (gameplayStateHandler != null)
            {
                var received = gameplayStateHandler.ReceiveBullet(bulletResponse.bullet);
                if (received)
                {
                    foreach (var buff in GetComponents<CharacterBuff>())
                    {
                        received = buff.ReceiveBullet(bulletResponse.bullet);
                        if (!received)
                        {
                            break;
                        }
                    }
                }

                bulletResponse.received = received;

                if (received)
                {
                    TakeDamages(bulletResponse.bullet);
                    if (!resources.hasArmor)
                    {
                        Push(bulletResponse.bullet.orientation, Random.Range(600.0f, 800.0f), 0.1f);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dashAttack"></param>
        private void OnReceivingDashAttack(CharacterDashAttack dashAttack)
        {
            if(dashAttack.character.powerUpIds.Contains(PowerUpId.Katana))
            {
                return;
            }

            dashAttack.Accept();

            var input = GetComponent<CharacterInput>();
            if(input != null)
            {
                input.Vibrate(0.5f, 0.5f, 0.2f);
            }

            Push(dashAttack.orientation,
                dashAttack.character.gameplayState == CharacterGameplayState.Dash ? 2500.0f : 4000.0f,
                0.25f);

            var launchKeyStateHandler = gameplayStateHandler as CharacterGameplayState_LaunchKey;
            if (launchKeyStateHandler != null)
            {
                launchKeyStateHandler.DropInPlace();
                SetGameplayState(CharacterGameplayState.Default);
            }

            GetComponent<HitFlash>()?.Flash();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        private void OnConstructionValidation(ConstructionSpaceRequest request)
        {
            if (request.contactingColliders.Contains(_capsuleCollider))
            {
                request.Decline();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="move"></param>
        /// <param name="target"></param>
        public void Orientate(Vector2 move, Vector2 target)
        {
            gameplayStateHandler?.Orientate(move, target);
            triggerStateHandler?.Orientate(move, target);
        }

        public void HandleConstructionMode(float constructionModeAxis)
        {
            gameplayStateHandler?.HandleConstructionMode(constructionModeAxis);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Action(CharacterAction action)
        {
            gameplayStateHandler?.Action(action);
        }

        /// <summary>
        /// 
        /// </summary>
        public void PerformTrigger()
        {
            triggerStateHandler?.Fire();
        }

        /// <summary>
        /// 
        /// </summary>
        public void PerformRelease()
        {
            triggerStateHandler?.Release();
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Dash()
        {
            if (canDash)
            {
                SetGameplayState(CharacterGameplayState.Dash);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Switch()
        {
            switch (triggerState)
            {
                case CharacterTriggerState.Construction:
                    (triggerStateHandler as CharacterTriggerState_Construction).Switch();
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool TryPick(System.Action<Character, Pickable> action)
        {
            var foundPickables = new HashSet<Pickable>();

            foreach (var collider in GetComponents<Collider2D>())
            {
                var collisions = new List<Collider2D>();
                collider.GetContacts(new ContactFilter2D
                {
                    useTriggers = true
                }, collisions);

                foreach (var pickable in collisions.Select(x => x.GetComponent<Pickable>()))
                {
                    if (pickable != null)
                    {
                        foundPickables.Add(pickable);
                    }
                }
            }

            foreach (var foundPickable in foundPickables.OrderBy(x => x.priority))
            {
                if (foundPickable.CanBePickedUpBy(this))
                {
                    action(this, foundPickable);
                    if (foundPickable.isPickedUp)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nextGameplayState"></param>
        /// <param name="forceReset"></param>
        public void SetGameplayState(CharacterGameplayState nextGameplayState, bool forceReset = false)
        {
            if (nextGameplayState == gameplayState && !forceReset)
            {
                return;
            }
            gameplayStateHandler?.Exit();
            gameplayState = nextGameplayState;
            gameplayStateHandler?.Enter();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nextTriggerState"></param>
        /// <param name="forceReset"></param>
        public void SetTriggerState(CharacterTriggerState nextTriggerState, bool forceReset = false)
        {
            if (nextTriggerState == triggerState && !forceReset)
            {
                return;
            }
            triggerStateHandler?.Exit();
            triggerState = nextTriggerState;
            triggerStateHandler?.Enter();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="moveOrientation"></param>
        /// <param name="targetOrientation"></param>
        public void SetOrientations(Vector2 moveOrientation, Vector2 targetOrientation)
        {
            _moveOrientation = moveOrientation;

            float angle = Vector2.SignedAngle(Vector2.down, targetOrientation);
            float radians = Mathf.Deg2Rad * angle;

            _spriteRenderer.flipX = radians < 0.0f;

            if (targetOrientationLocked)
            {
                return;
            }
            this.targetOrientation = targetOrientation;

            currentEquippable?.SetTargetOrientation(targetOrientation);
            foreach (var weapon in weapons)
            {
                if (weapon != null && weapon != currentEquippable)
                {
                    weapon.SetTargetOrientation(targetOrientation);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loadout"></param>
        public void SetConstructsFromLoadout(Loadout loadout)
        {
            _constructs = loadout.items.OfType<ConstructLoadoutItem>().ToArray();
            _constructsOriginalLength = _constructs.Length;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="buff"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public bool TryAddBuff<T>(out T buff, float duration = -1.0f) where T : CharacterBuff
        {
            if(gameplayStateHandler.acceptsBuffs && GetComponent<T>() == null)
            {
                buff = gameObject.AddComponent<T>();
                buff.duration = duration;
                return true;
            }

            buff = null;
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="duration"></param>
        /// <returns></returns>
        public bool TryAddBuff<T>(float duration) where T : CharacterBuff
        {
            T buff;
            return TryAddBuff(out buff, duration);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="equippablePrefab"></param>
        /// <returns></returns>
        public Equippable SetEquippable(Equippable equippablePrefab)
        {
            Debug.Assert(equippablePrefab != null);

            currentEquippable = equippablePrefab;
            currentEquippable.transform.localPosition = currentEquippable.offset;

            return currentEquippable;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="weaponPrefab"></param>
        /// <returns></returns>
        public WeaponEquippable SetWeapon(int index, WeaponEquippable weaponPrefab)
        {
            Debug.Assert(weaponPrefab != null);
            Debug.Assert(index >= 0 && index < MaxWeaponsNumber);

            if (_weapons[index] != null)
            {
                Destroy(_weapons[index].gameObject);
            }

            var characterInput = GetComponent<CharacterInput>();
            if (currentEquippable != null && characterInput != null && characterInput.isFiring)
            {
                _weapons[index] = weaponPrefab;
                weaponPrefab.spriteRenderer.enabled = false;
            }
            else
            {
                _weapons[index] = SetEquippable(weaponPrefab) as WeaponEquippable;
                _lastWeaponIndex = index;
            }
            return _weapons[index];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="weaponDescriptor"></param>
        /// <returns></returns>
        public WeaponEquippable SetWeapon(int index, WeaponDescriptor weaponDescriptor)
        {
            Debug.Assert(weaponDescriptor != null);

            ClearWeapon(index);

            var prefab = weaponDescriptor.weaponPrefab;
            var weapon = SetWeapon(index, Instantiate(prefab, transform.position, Quaternion.identity, transform).GetComponent<WeaponEquippable>());

            weapon.weaponDescriptor = weaponDescriptor;
            weapon.EmitSound(weaponDescriptor.pickupSounds);

            return weapon;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="spawnPickable"></param>
        public bool ClearWeapon(int index, bool spawnPickable = true)
        {
            if (_weapons[index] == null)
            {
                return false;
            }

            // Spawn current weapon as pickable
            return ClearWeapon(_weapons[index], spawnPickable);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="weapon"></param>
        /// <param name="spawnPickable"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool ClearWeapon(WeaponEquippable weapon, bool spawnPickable = true, int index = -1)
        {
            if (index == -1)
            {
                for (var i = 0; i < _weapons.Length; ++i)
                {
                    if (_weapons[i] == weapon)
                    {
                        index = i;
                    }
                }

                Debug.Assert(index >= 0);
            }

            var switchWeapon = _currentEquippable is WeaponEquippable;

            Destroy(weapon.gameObject);
            if (spawnPickable && weapon.weaponDescriptor != spawner.defaultWeapon)
            {
                GameplayStatics.SpawnWeaponPickable(weapon.weaponDescriptor, transform.position);
            }

            _weapons[index] = null;

            if (switchWeapon)
            {
                _currentEquippable = null;
                SwitchWeapon();
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spawnPickable"></param>
        public bool ClearCurrentWeapon(bool spawnPickable = true)
        {
            return ClearWeapon(_lastWeaponIndex, spawnPickable);
        }

        /// <summary>
        /// 
        /// </summary>
        public void SwitchWeapon()
        {
            var destroy = true;
            for (var i = 0; i < MaxWeaponsNumber; ++i)
            {
                if (currentEquippable != null && currentEquippable == _weapons[i])
                {
                    destroy = false;
                }
            }

            if (destroy && currentEquippable != null)
            {
                Destroy(currentEquippable.gameObject);
            }

            var originalWeaponIndex = _lastWeaponIndex;
            var nextWeaponIndex = _lastWeaponIndex;
            do
            {
                nextWeaponIndex = (nextWeaponIndex + 1) % MaxWeaponsNumber;
            } while (_weapons[nextWeaponIndex] == null && nextWeaponIndex != originalWeaponIndex);
            _lastWeaponIndex = nextWeaponIndex;

            currentEquippable = _weapons[_lastWeaponIndex];
            GetComponentInChildren<SwitchWeaponIndicator>().SetEnabled(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dropDefault"></param>
        public void DropAllWeapons(bool dropDefault = false)
        {
            foreach (var weapon in weapons)
            {
                if (weapon != null && weapon.weaponDescriptor != null)
                {
                    if (!dropDefault && weapon.weaponDescriptor == spawner.defaultWeapon)
                    {
                        continue;
                    }
                    weapon.OnUnequip();
                    GameplayStatics.SpawnWeaponPickable(weapon.weaponDescriptor, transform.position);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orientation"></param>
        /// <param name="curve"></param>
        /// <param name="power"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public PushBuff Push(Vector2 orientation, AnimationCurve curve, float power, float duration)
        {
            var currentPushBuff = GetComponent<PushBuff>();
            if (currentPushBuff == null || currentPushBuff.power <= power)
            {
                PushBuff pushBuff;
                if (TryAddBuff(out pushBuff))
                {
                    pushBuff.orientation = orientation;
                    pushBuff.power = power;
                    pushBuff.curve = curve;
                    pushBuff.duration = duration;
                    return pushBuff;
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orientation"></param>
        /// <param name="power"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public PushBuff Push(Vector2 orientation, float power, float duration)
        {
            return Push(orientation, null, power, duration);
        }

        /// <summary>
        /// 
        /// </summary>
        public bool ScatterSupply()
        {
            var supply = resources.supply;
            if (supply > 0 && GetComponent<NoSupplyPickupBuff>() == null)
            {
                resources.TakeSupply(supply);
                GameplayStatics.ScatterSupply(transform, supply);

                TryAddBuff<NoSupplyPickupBuff>(0.5f);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="alpha"></param>
        public void SetCosmeticAlpha(float alpha)
        {
            var color = _spriteRenderer.color;
            color.a = alpha;
            _spriteRenderer.color = color;
        }

        /// <summary>
        /// 
        /// </summary>
        public void ToggleMarketingZoom()
        {
            foreach (var marketingZoom in FindObjectsOfType<CharacterMarketingZoom>())
            {
                if (marketingZoom.GetComponent<Character>() != this)
                {
                    DestroyImmediate(marketingZoom);
                }
            }

            var characterMarketingZoom = GetComponent<CharacterMarketingZoom>();
            if (characterMarketingZoom != null)
            {
                DestroyImmediate(characterMarketingZoom);
            }
            else
            {
                gameObject.AddComponent<CharacterMarketingZoom>();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collision"></param>
        /// <returns></returns>
        private bool CheckPositionAdjustment(Collider2D collision)
        {
            //const float tolerance = 18.0f;
            const float reckoning = 3.0f;

            if (!isMoving && gameplayState != CharacterGameplayState.Dash)
            {
                return false;
            }

            if(!collision.bounds.Intersects(_capsuleCollider.bounds))
            {
                return false;
            }

            // Move adjustment
            var colliderBounds = collision.bounds;
            var closestCorner = collision.GetComponent<LocomotionCorner>();

            if (closestCorner == null || closestCorner.isCollidingCorners)
            {
                return false;
            }

            if (moveOrientation.y != 0.0f)
            {
                if(moveOrientation.x != 0.0f && !MathExtensions.AreSameSign(moveOrientation.y, closestCorner.adjustY))
                {
                    return false;
                }
                _body.MovePosition(_body.position + Vector2.right * reckoning * closestCorner.adjustX);
                return true;
            }
            else if(moveOrientation.x != 0.0f)
            {
                if(moveOrientation.y != 0.0f && !MathExtensions.AreSameSign(moveOrientation.x, closestCorner.adjustX))
                {
                    return false;
                }
                _body.MovePosition(_body.position + Vector2.up * reckoning * closestCorner.adjustY);
                return true;
            }

            return false;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public CharacterDescriptor characterDescriptor { get; set; }

        /// <summary>
        /// 
        /// </summary> 
        public CharacterSkinDescriptor skinDescriptor { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public CharacterStatsDescriptor statsDescriptor => _statsDescriptor;

        /// <summary>
        /// 
        /// </summary>
        public SpriteAnimation deathEffectAnimation => _deathEffectAnimation;

        /// <summary>
        /// 
        /// </summary>
        public GameObject constructionSpacePrefab => _constructionSpacePrefab;

        /// <summary>
        /// 
        /// </summary>
        public GameObject playerKeyIndicatorPrefab => _playerKeyIndicatorPrefab;

        /// <summary>
        /// 
        /// </summary>
        public Vector2 moveOrientation => _moveOrientation;

        /// <summary>
        /// 
        /// </summary>
        public ParticleSystem runParticleSystem => _runParticleSystem;

        /// <summary>
        /// 
        /// </summary>
        public ParticleSystem dashParticleSystem => _dashTrailParticleSystem;

        /// <summary>
        /// 
        /// </summary>
        public SpriteRenderer floorShadowRenderer => _floorShadowRenderer;

        /// <summary>
        /// 
        /// </summary>
        public SpriteAnimation burnedToDeathAnimation => _burnedToDeathAnimation;

        /// <summary>
        /// 
        /// </summary>
        public Equippable currentEquippable
        {
            get { return _currentEquippable; }
            set
            {
                if (_currentEquippable == value)
                {
                    return;
                }

                _currentEquippable?.OnUnequip();
                _currentEquippable = value;
                for (var i = 0; i < MaxWeaponsNumber; ++i)
                {
                    if (_weapons[i] != null && _weapons[i] == value)
                    {
                        _lastWeaponIndex = i;
                    }
                }

                value?.OnEquip();

                var weaponEquippable = value as WeaponEquippable;
                if (weaponEquippable != null && weaponEquippable.weaponDescriptor != null)
                {
                    weaponEquippable.EmitSound(weaponEquippable.weaponDescriptor.pickupSounds);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int currentWeaponIndex
        {
            get => _lastWeaponIndex;
            set => _lastWeaponIndex = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public DamageType deathDamageType { get; set; } = DamageType.Normal;

        /// <summary>
        /// 
        /// </summary>
        public bool isMoving { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool isGlued { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool targetOrientationLocked { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public override bool isInvincible =>
            gameplayStateHandler is CharacterGameplayState_Dash || GetComponent<InvincilityBuff>() != null;

        /// <summary>
        /// 
        /// </summary>
        public float speed
        {
            get
            {
                var value = statsDescriptor.speed;
                foreach(var multiplier in speedMultipliers)
                {
                    value *= multiplier;
                }
                return value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public List<float> speedMultipliers { get; } = new List<float>();

        /// <summary>
        /// 
        /// </summary>
        public List<PowerUpId> powerUpIds { get; } = new List<PowerUpId>();

        /// <summary>
        /// 
        /// </summary>
        public bool canDash => dashCooldown == 0.0f;

        /// <summary>
        /// 
        /// </summary>
        public float dashCooldown { get; set; } = 0.0f;

        /// <summary>
        /// 
        /// </summary>
        public Rigidbody2D body => _body;

        /// <summary>
        /// 
        /// </summary>
        public CharacterResources resources => _resources;

        /// <summary>
        /// 
        /// </summary>
        public ConstructLoadoutItem[] constructs
        {
            get => _constructs;
            set => _constructs = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public ConstructLoadoutItem tempConstruct
        {
            get { return _tempConstruct; }
            set
            {
                if(_constructs.Contains(value) || _tempConstruct == value)
                {
                    return;
                }

                _tempConstruct = value;
                var constructs = new List<ConstructLoadoutItem>();
                for(var i = 0; i < _constructsOriginalLength; ++i)
                {
                    constructs.Add(_constructs[i]);
                }

                if (value != null)
                {
                    constructs.Add(value);
                }
                _constructs = constructs.ToArray();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public WeaponEquippable[] weapons => _weapons;

        /// <summary>
        /// 
        /// </summary>
        public CharacterGameplayState gameplayState { get; private set; } = CharacterGameplayState.Default;

        /// <summary>
        /// 
        /// </summary>
        public CharacterTriggerState triggerState { get; private set; } = CharacterTriggerState.Weapon;

        /// <summary>
        /// 
        /// </summary>
        public CharacterGameplayStateHandler gameplayStateHandler => _gameplayStates[gameplayState];

        /// <summary>
        /// 
        /// </summary>
        public CharacterTriggerStateHandler triggerStateHandler => _triggerStates[triggerState];

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private CharacterStatsDescriptor _statsDescriptor = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private SpriteAnimation _deathEffectAnimation = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private GameObject _constructionSpacePrefab = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private GameObject _playerKeyIndicatorPrefab = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Vector2 _moveOrientation = Vector2.down;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private ParticleSystem _runParticleSystem = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private ParticleSystem _dashTrailParticleSystem = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private SpriteRenderer _floorShadowRenderer = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private SpriteAnimation _burnedToDeathAnimation = default;

        private Dictionary<CharacterGameplayState, CharacterGameplayStateHandler> _gameplayStates = new Dictionary<CharacterGameplayState, CharacterGameplayStateHandler>
        {
            { CharacterGameplayState.Default, new CharacterGameplayState_Default() },
            { CharacterGameplayState.Dash, new CharacterGameplayState_Dash() },
            { CharacterGameplayState.Dead, new CharacterGameplayState_Dead() },
            { CharacterGameplayState.LaunchKey, new CharacterGameplayState_LaunchKey() },
            { CharacterGameplayState.Leaving, new CharacterGameplayState_Leaving() },
            { CharacterGameplayState.Panicking, new CharacterGameplayState_Panicking() },
        };

        private Dictionary<CharacterTriggerState, CharacterTriggerStateHandler> _triggerStates = new Dictionary<CharacterTriggerState, CharacterTriggerStateHandler>
        {
            { CharacterTriggerState.None, new CharacterTriggerStateHandler() },
            { CharacterTriggerState.Weapon, new CharacterTriggerState_Weapon() },
            { CharacterTriggerState.Construction, new CharacterTriggerState_Construction() },
        };

        private WeaponEquippable[] _weapons = new WeaponEquippable[MaxWeaponsNumber];
        private int _lastWeaponIndex = 0;

        private ConstructLoadoutItem[] _constructs = new ConstructLoadoutItem[0];
        private ConstructLoadoutItem _tempConstruct = null;

        private int _constructsOriginalLength;

        private Equippable _currentEquippable;

        private Rigidbody2D _body;
        private SpriteRenderer _spriteRenderer;
        private CharacterResources _resources;
        private CapsuleCollider2D _capsuleCollider;

        private ContactPoint2D[] _moveAdjustContacts = new ContactPoint2D[8];

        private bool _isMoveBeingAdjusted;

        #endregion
    }
}