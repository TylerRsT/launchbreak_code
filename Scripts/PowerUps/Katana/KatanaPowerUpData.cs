using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu(fileName = "PWRDATA_Katana", menuName = "Shovel/Power Up Data/Katana")]
    public class KatanaPowerUpData : PowerUpData
    {
        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public GameObject katanaPrefab => _katanaPrefab;

        /// <summary>
        /// 
        /// </summary>
        public int damages => _damages;

        /// <summary>
        /// 
        /// </summary>
        public DamageType damageType => _damageType;

        /// <summary>
        /// 
        /// </summary>
        public float knockback => _knockback;

        /// <summary>
        /// 
        /// </summary>
        public float knockbackDuration => _knockbackDuration;

        /// <summary>
        /// 
        /// </summary>
        public AnimationCurve knockbackCurve => _knockbackCurve;

        /// <summary>
        /// 
        /// </summary>
        public DashProperties dashProperties => _dashProperties;

        /// <summary>
        /// 
        /// </summary>
        public IReadOnlyList<string> katanaSlashSounds => _katanaSlashSounds;

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private GameObject _katanaPrefab = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private int _damages = 2;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private DamageType _damageType = DamageType.Normal;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _knockback = 1000f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _knockbackDuration = 0.15f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private AnimationCurve _knockbackCurve = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private DashProperties _dashProperties = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private List<string> _katanaSlashSounds = new List<string>();

        #endregion
    }
}
