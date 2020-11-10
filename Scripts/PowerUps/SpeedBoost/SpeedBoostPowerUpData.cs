using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    [CreateAssetMenu(fileName = "PWRDATA_SpeedBoost", menuName = "Shovel/Power Up Data/Speed Boost")]
    public class SpeedBoostPowerUpData : PowerUpData
    {
        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public float duration => _duration;

        /// <summary>
        /// 
        /// </summary>
        public float speedMultiplier => _speedMultiplier;

        /// <summary>
        /// 
        /// </summary>
        public int hollowFrequency => _hollowFrequency;

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _duration = 1.0f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _speedMultiplier = 1.0f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private int _hollowFrequency = 3;

        #endregion
    }
}