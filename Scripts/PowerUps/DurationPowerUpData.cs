using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu(fileName = "PWRDATA_Name", menuName = "Shovel/Power Up Data/Duration")]
    public class DurationPowerUpData : PowerUpData
    {
        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public float duration => _duration;

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _duration = 0.0f;

        #endregion
    }
}