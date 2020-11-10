using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class ShipShakeHazardManager : HazardManager<KeyBasedGameMode>
    {
        #region Const

        private const int ShipShakePriority = 10;
        private const string ShipShakeSoundKey = "ShipShake";

        private const float PowerOffAnnouncementDelay = 0.2f;
        private const float HazardStartDelay = 0.5f;
        private const float HazardInterval = 0.25f;

        #endregion

        #region Override

        /// <summary>
        /// 
        /// </summary>
        public override void OnGameModeStep()
        {
            base.OnGameModeStep();

            if (_lastHighestScore != gameMode.highestScore)
            {
                _lastHighestScore = gameMode.highestScore;
                DoNextShake();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            _randomHazardPlayer = GetComponent<RandomHazardPlayer>();
            _powerOffManager = GetComponent<PowerOffManager>();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Update()
        {
            base.Update();

            if (_shipShake == null)
            {
                return;
            }

            _elapsedSinceLastShake += GameplayStatics.gameDeltaTime;
            if (_elapsedSinceLastShake >= _nextShakeTime - PowerOffAnnouncementDelay && _powerOffManager != null)
            {
                if(!_powerOffManager.isPoweringOff)
                {
                    _powerOffManager.PowerOff(gameMode.highestScore);
                }
            }

            if (_elapsedSinceLastShake >= _nextShakeTime)
            {
                DoNextShake();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameMode"></param>
        protected override void Initialize(GameMode gameMode)
        {
            base.Initialize(gameMode);
            PlanNextShake();
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        protected void PlanNextShake()
        {
            _elapsedSinceLastShake = 0.0f;

            var highestScore = gameMode.highestScore;
            _shipShake = _shipShakeDescriptor.GetShipShakeAt(highestScore);

            if (_shipShake != null)
            {
                _nextShakeTime = Random.Range(_shipShake.intervalMin, _shipShake.intervalMax);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected void DoNextShake()
        {
            if (_shipShake == null || !_shipShake.isValid)
            {
                return;
            }

            if (_randomHazardPlayer != null)
            {
                var hazardCountToPlay = Random.Range(1, gameMode.highestScore + 1);
                _randomHazardPlayer.Play(hazardCountToPlay, HazardInterval, HazardStartDelay);
            }

            _powerOffManager?.PowerOff(gameMode.highestScore);

            var multiplier = Random.Range(_shipShake.multiplierMin, _shipShake.multiplierMax);
            CameraShakeManager.instance.Shake(_shipShake.shakePreset, multiplier, ShipShakePriority);
            this.EmitSound(ShipShakeSoundKey, false);
            PlanNextShake();
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        protected ShipShakeDescriptor shipShakeDescriptor => _shipShakeDescriptor;

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private ShipShakeDescriptor _shipShakeDescriptor = default;

        private float _elapsedSinceLastShake = 0.0f;
        private float _nextShakeTime = 0.0f;
        private ShipShake _shipShake;

        private int _lastHighestScore;

        private RandomHazardPlayer _randomHazardPlayer;
        private PowerOffManager _powerOffManager;

        #endregion
    }
}