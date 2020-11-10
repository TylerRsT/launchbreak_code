using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu(fileName = "WPN_Name", menuName = "Shovel/Weapon/Throwaway Weapon Descriptor")]
    public class ThrowawayWeaponDescriptor : WeaponDescriptor
    {
        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public int defaultAmmo => _defaultAmmo;

        /// <summary>
        /// 
        /// </summary>
        public GameObject thrownPrefab => _thrownPrefab;

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private int _defaultAmmo = 1;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private GameObject _thrownPrefab = default;

        #endregion
    }
}