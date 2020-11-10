using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class WeaponDescriptor : LootableDescriptor
    {
        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public GameObject weaponPrefab => _weaponPrefab;

        /// <summary>
        /// 
        /// </summary>
        public IReadOnlyList<string> pickupSounds => _pickupSounds;

        /// <summary>
        /// 
        /// </summary>
        public Vector2 pickupButtonOffset => _pickupButtonOffset;

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private GameObject _weaponPrefab = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private List<string> _pickupSounds = new List<string>();

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Vector2 _pickupButtonOffset = new Vector2(-10.0f, -5.0f);

        #endregion
    }
}