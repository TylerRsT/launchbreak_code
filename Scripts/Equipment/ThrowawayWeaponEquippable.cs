using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class ThrowawayWeaponEquippable : WeaponEquippable
    {
        #region Override

        /// <summary>
        /// 
        /// </summary>
        public override void Trigger()
        {
            base.Trigger();
            timeSinceHeld += GameplayStatics.gameFixedDeltaTime;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        protected ThrowawayWeaponDescriptor throwawayWeaponDescriptor => (ThrowawayWeaponDescriptor)weaponDescriptor;

        /// <summary>
        /// 
        /// </summary>
        protected float timeSinceHeld { get; private set; }

        #endregion
    }
}