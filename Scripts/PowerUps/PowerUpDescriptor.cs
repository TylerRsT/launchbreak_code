using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu(fileName = "PWR_Name", menuName = "Shovel/Power Up Descriptor")]
    public class PowerUpDescriptor : LootableDescriptor
    {
        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public PowerUpId powerUpId => _powerUpId;

        /// <summary>
        /// 
        /// </summary>
        public PowerUpData powerUpData => _powerUpData;

        /// <summary>
        /// 
        /// </summary>
        public IReadOnlyList<string> pickupSounds => _pickupSounds;

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private PowerUpId _powerUpId = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private PowerUpData _powerUpData = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private List<string> _pickupSounds = new List<string>();

        #endregion
    }
}