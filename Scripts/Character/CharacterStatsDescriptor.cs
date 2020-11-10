using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu(fileName = "STATS_Name", menuName = "Shovel/Character Stats Descriptor")]
    public class CharacterStatsDescriptor : ScriptableObject
    {
        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public float speed => _speed;

        /// <summary>
        /// 
        /// </summary>
        public DashProperties dashProperties => _dashProperties;

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _speed = 150.0f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private DashProperties _dashProperties = new DashProperties();
        
        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    [System.Serializable]
    public class DashProperties
    {
        public float dashSpeedMultiplier = 10000.0f;

        public float dashDuration = 0.5f;

        public float dashActiveStart = 0.0f;

        public float dashActiveDuration = 0.2f;

        public float dashCooldown = 1.0f;

        public AnimationCurve dashCurve = default;
    }
}