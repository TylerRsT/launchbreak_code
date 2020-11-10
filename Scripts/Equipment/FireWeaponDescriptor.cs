using Com.LuisPedroFonseca.ProCamera2D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu(fileName = "WPN_Name", menuName = "Shovel/Weapon/Fire Weapon Descriptor")]
    public class FireWeaponDescriptor : HeatBasedWeaponDescriptor
    {
        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public GameObject bulletPrefab => _bulletPrefab;

        /// <summary>
        /// 
        /// </summary>
        public GameObject heatParticlePrefab => _heatParticlePrefab;

        /// <summary>
        /// 
        /// </summary>
        public float bulletSpeed => _bulletSpeed;

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private GameObject _bulletPrefab = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private GameObject _heatParticlePrefab = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _bulletSpeed = 500.0f;

        #endregion
    }
}