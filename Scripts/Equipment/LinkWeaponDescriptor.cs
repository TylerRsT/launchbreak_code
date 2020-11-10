using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu(fileName = "WPN_Name", menuName = "Shovel/Weapon/Link Weapon Descriptor")]
    public class LinkWeaponDescriptor : HeatBasedWeaponDescriptor
    {
        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public float damagesPerSecond => _damagesPerSecond;

        /// <summary>
        /// 
        /// </summary>
        public float startupTime => _startupTime;

        /// <summary>
        /// 
        /// </summary>
        public float orientationSpeedReducer => _orientationSpeedReducer;

        /// <summary>
        /// 
        /// </summary>
        public float beamHeight => _beamHeight;

        /// <summary>
        /// 
        /// </summary>
        public float beamMinScale => _beamMinScale;

        /// <summary>
        /// 
        /// </summary>
        public float beamMaxScale => _beamMaxScale;

        /// <summary>
        /// 
        /// </summary>
        public IReadOnlyList<string> startupSounds => _startupSounds;

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _damagesPerSecond = 2.0f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _startupTime = 1.5f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _orientationSpeedReducer = 10.0f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _beamHeight = 1.0f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _beamMinScale = 0.7f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _beamMaxScale = 1.0f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private List<string> _startupSounds = new List<string>();

        #endregion
    }
}