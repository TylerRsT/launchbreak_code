using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class PowerUpPickable : LootPickable
    {
        #region Override

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lootDescriptor"></param>
        public override void Initialize(LootableDescriptor lootableDescriptor)
        {
            base.Initialize(lootableDescriptor);
            _powerUpDescriptor = lootableDescriptor as PowerUpDescriptor;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Start()
        {
            base.Start();

            var powerUpType = PowerUpTable.Get(_powerUpDescriptor.powerUpId);
            Debug.Assert(powerUpType != null);

            _powerUp = PowerUpFactory.Instantiate(powerUpType, _powerUpDescriptor.powerUpData);
            Debug.Assert(_powerUp != null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="response"></param>
        protected override void OnPickup(PickupResponse response)
        {
            base.OnPickup(response);

            if(_powerUp.CanApply(response.character))
            {
                var emitor = Instantiate(_particleAttractor, transform.position, transform.rotation);
                emitor.SetTarget(response.character.transform);
                response.Accept();
                SpawnPickupText(_powerUpDescriptor.name);
                this.EmitSound(_powerUpDescriptor.pickupSounds);
                response.character.powerUpIds.Add(_powerUpDescriptor.powerUpId);
                _powerUp.Apply(response.character);
                Telemetry.game.Incr($"{_powerUpDescriptor.name}_pickup");
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public PowerUpDescriptor powerUpDescriptor => _powerUpDescriptor;

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private PowerUpDescriptor _powerUpDescriptor;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private PowerUpParticle _particleAttractor = default;

        private IPowerUp _powerUp;

        #endregion
    }
}