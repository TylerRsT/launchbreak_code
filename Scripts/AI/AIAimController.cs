using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    [RequireComponent(typeof(CircleCollider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class AIAimController : MonoBehaviour
    {
        #region Const

        private const float IntervalBetweenFires = 0.2f;
        private const float IntervalBetweenDashes = 0.6f;
        private const float IntervalBetweenRaycasts = 0.4f;

        private const float IntervalBetweenFires_Easy = 0.5f;
        private const float IntervalBetweenDashes_Easy = 2.0f;

        private const float DashDistance = 60.0f;

        private const float StartTime = 2.0f; // So AI don't shoot as soon as they spawn.
        private const float ShootOdds = 0.6f;
        private const float CharacterDashOdds = 0.3f;
        private const float ConstructDashOdds = 0.4f;

        private const float ShootOdds_Easy = 0.2f;
        private const float CharacterDashOdds_Easy = 0.1f;

        #endregion

        #region Messages

        /// <summary>
        /// 
        /// </summary>
        private void Awake()
        {
            GetComponent<Rigidbody2D>().gravityScale = 0.0f;

            _circleCollider = GetComponent<CircleCollider2D>();
            _circleCollider.isTrigger = true;
            _circleCollider.radius = GameConstants.SceneWidth / 2.0f;

            if(AIBehaviour.easyMode)
            {
                _intervalBetweenFires = IntervalBetweenFires_Easy;
                _intervalBetweenDashes = IntervalBetweenDashes_Easy;
                _shootOdds = ShootOdds_Easy;
                _characterDashOdds = CharacterDashOdds_Easy;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void Start()
        {
            _character = GetComponentInParent<Character>();
        }

        /// <summary>
        /// 
        /// </summary>
        private void Update()
        {
            if (_elapsedSinceStart < StartTime)
            {
                _elapsedSinceStart += GameplayStatics.gameDeltaTime;
                return;
            }

            _elapsedSinceLastFire += GameplayStatics.gameDeltaTime;
            _elapsedSinceLastDash += GameplayStatics.gameDeltaTime;
            _elapsedSinceLastRaycast += GameplayStatics.gameDeltaTime;

            if (_elapsedSinceLastFire < _intervalBetweenFires && _elapsedSinceLastDash < _intervalBetweenDashes)
            {
                return;
            }

            if (_currentTarget != null)
            {
                var shouldShoot = _elapsedSinceLastRaycast < IntervalBetweenRaycasts;
                RaycastHit2D raycastHit = default;

                if (!shouldShoot)
                {
                    var difference = (_currentTarget.transform.position - _character.transform.position).normalized;
                    raycastHit = Physics2D.Raycast(_character.transform.position + difference * GameConstants.HalfTileSize, difference, _circleCollider.radius, LayerMask.GetMask("Character", "Solid"));
                    shouldShoot = raycastHit.collider != null;
                }

                if (shouldShoot)
                {
                    targetOrientation = (_currentTarget.transform.position - _character.transform.position).normalized;

                    var canShoot = true;
                    var weapon = _character.currentEquippable as HeatBasedWeaponEquippable;
                    if(weapon != null && weapon.isOverheating)
                    {
                        canShoot = false;
                    }

                    if (canShoot)
                    {
                        var shootRandGo = _shootOdds;
                        if (_elapsedSinceLastFire > _intervalBetweenFires && Random.Range(0.0f, 1.0f) <= shootRandGo)
                        {
                            _character.Action(CharacterAction.Fire);
                            _elapsedSinceLastFire = 0.0f;
                        }
                    }
                    else if (raycastHit.distance <= DashDistance && _elapsedSinceLastDash > _intervalBetweenDashes)
                    {
                        var dashRandGo = _characterDashOdds;
                        if (_currentTarget.GetComponent<BreakableConstruct>() != null)
                        {
                            dashRandGo = ConstructDashOdds;
                        }

                        if (Random.Range(0.0f, 1.0f) <= dashRandGo)
                        {
                            _character.Action(CharacterAction.Dash);
                            _elapsedSinceLastDash = 0.0f;
                        }
                    }
                    return;
                }
            }

            if(_elapsedSinceLastRaycast < IntervalBetweenRaycasts)
            {
                return;
            }

            ShooterBehaviour closestTarget = null;
            var closestDistance = float.MaxValue;

            foreach (var potentialTarget in _potentialTargets)
            {
                if (potentialTarget == null)
                {
                    continue;
                }

                var difference = (potentialTarget.transform.position - _character.transform.position).normalized;

                var raycastHit = Physics2D.Raycast(_character.transform.position + difference * GameConstants.HalfTileSize, difference, _circleCollider.radius, LayerMask.GetMask("Character", "Solid"));
                if (raycastHit.collider != null && raycastHit.collider.GetComponent<ShooterBehaviour>() == potentialTarget)
                {
                    var shooterCharacter = potentialTarget as Character;
                    if (shooterCharacter != null)
                    {
                        if (shooterCharacter.gameplayState == CharacterGameplayState.Dead
                            || shooterCharacter.isInvincible)
                        {
                            continue;
                        }
                    }

                    if (raycastHit.distance < closestDistance)
                    {
                        closestDistance = raycastHit.distance;
                        closestTarget = potentialTarget;
                    }
                }
            }

            _currentTarget = closestTarget;
            if (_currentTarget != null)
            {
                targetOrientation = (closestTarget.transform.position - _character.transform.position).normalized;
            }

            _elapsedSinceLastRaycast = 0.0f;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collision"></param>
        private void OnTriggerEnter2D(Collider2D collision)
        {
            var shooterBehaviour = collision.GetComponent<ShooterBehaviour>();
            if (shooterBehaviour != null && shooterBehaviour.GetComponent<GameplayTeam>().teamIndex != _character.GetComponent<GameplayTeam>().teamIndex)
            {
                _potentialTargets.Add(shooterBehaviour);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collision"></param>
        private void OnTriggerExit2D(Collider2D collision)
        {
            var shooterBehaviour = collision.GetComponent<ShooterBehaviour>();
            if (shooterBehaviour != null && shooterBehaviour.GetComponent<GameplayTeam>().teamIndex != _character.GetComponent<GameplayTeam>().teamIndex)
            {
                _potentialTargets.Remove(shooterBehaviour);
                if(shooterBehaviour == _currentTarget)
                {
                    _currentTarget = null;
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public bool hasTarget => _currentTarget != null;

        /// <summary>
        /// 
        /// </summary>
        public DamageComponent currentTarget
        {
            get => _currentTarget;
            set => _currentTarget = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public Vector2 targetOrientation { get; private set; }

        #endregion

        #region Fields

        private CircleCollider2D _circleCollider;

        private HashSet<ShooterBehaviour> _potentialTargets = new HashSet<ShooterBehaviour>();

        private DamageComponent _currentTarget;
        private Character _character;

        private float _elapsedSinceStart;
        private float _elapsedSinceLastFire;
        private float _elapsedSinceLastDash;
        private float _elapsedSinceLastRaycast;

        private float _intervalBetweenFires = IntervalBetweenFires;
        private float _intervalBetweenDashes = IntervalBetweenDashes;

        private float _shootOdds = ShootOdds;
        private float _characterDashOdds = CharacterDashOdds;

        #endregion
    }
}