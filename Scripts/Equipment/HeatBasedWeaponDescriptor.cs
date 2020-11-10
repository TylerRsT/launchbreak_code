using Com.LuisPedroFonseca.ProCamera2D;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class HeatBasedWeaponDescriptor : WeaponDescriptor
    {
        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public float damages => _damages;

        /// <summary>
        /// 
        /// </summary>
        public DamageType damageType => _damageType;

        /// <summary>
        /// 
        /// </summary>
        public float rateOfFire => _rateOfFire;

        /// <summary>
        /// 
        /// </summary>
        public float heatPerShot => _heatPerShot;

        /// <summary>
        /// 
        /// </summary>
        public float recoveryDelay => _recoveryDelay;

        /// <summary>
        /// 
        /// </summary>
        public float overheatPenalty => _overheatPenalty;

        /// <summary>
        /// 
        /// </summary>
        public float recoveryAmount => _recoveryAmount;

        /// <summary>
        /// 
        /// </summary>
        public IReadOnlyList<float> loadLevels => _loadLevels;

        /// <summary>
        /// 
        /// </summary>
        public GameObject fireEffectPrefab => _fireEffectPrefab;

        /// <summary>
        /// 
        /// </summary>
        public float fireKnockback => _fireKnockback;

        /// <summary>
        /// 
        /// </summary>
        public float fireKnockbackDuration => _fireKnockbackDuration;

        /// <summary>
        /// 
        /// </summary>
        public float angleKnockback => _angleKnockback;

        /// <summary>
        /// 
        /// </summary>
        public ShakePreset cameraShake => _cameraShake;

        /// <summary>
        /// 
        /// </summary>
        public IReadOnlyList<string> fireSounds => _fireSounds;

        /// <summary>
        /// 
        /// </summary>
        public IReadOnlyList<string> emptySounds => _emptySounds;

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _damages = 1.0f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private DamageType _damageType = DamageType.Normal;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _rateOfFire = 0.4f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _heatPerShot = 0.2f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _recoveryDelay = 1.0f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _overheatPenalty = 2.0f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _recoveryAmount = 0.125f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private List<float> _loadLevels = new List<float>();

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private GameObject _fireEffectPrefab = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _fireKnockback = -5.0f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _fireKnockbackDuration = 0.2f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _angleKnockback = 10.0f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private ShakePreset _cameraShake = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private List<string> _fireSounds = new List<string>();

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private List<string> _emptySounds = new List<string>();

        #endregion
    }
}