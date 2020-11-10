using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class PowerUpBuff : CharacterBuff
    {
        #region Override

        /// <summary>
        /// 
        /// </summary>
        protected override void Start()
        {
            base.Start();

            character.powerUpIds.Add(_powerUpId);
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnDestroy()
        {
            base.OnDestroy();

            character.powerUpIds.Remove(_powerUpId);
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public PowerUpId powerUpId
        {
            get => _powerUpId;
            set => _powerUpId = value;
        }

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private PowerUpId _powerUpId = PowerUpId.None;

        #endregion
    }
}