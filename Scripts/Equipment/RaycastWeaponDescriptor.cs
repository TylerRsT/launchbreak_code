using Elendow.SpritedowAnimator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu(fileName = "WPN_Name", menuName = "Shovel/Weapon/Raycast Weapon Descriptor")]
    public class RaycastWeaponDescriptor : HeatBasedWeaponDescriptor
    {
        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public SpriteAnimation impactAnimation => _impactAnimation;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        public TrailRenderer shotTrailRenderer => _shotTrailRenderer;

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private SpriteAnimation _impactAnimation = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private TrailRenderer _shotTrailRenderer = default;

        #endregion
    }
}