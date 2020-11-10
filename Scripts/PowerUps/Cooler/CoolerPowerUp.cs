using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class CoolerPowerUp : PowerUp<DurationPowerUpData>
    {
        #region Override

        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
        public override void Apply(Character character)
        {
            base.Apply(character);

            foreach(var weapon in character.weapons)
            {
                weapon?.Cooldown();
            }

            var fireWeapon = character.currentEquippable as FireWeaponEquippable;
            fireWeapon?.ShowMuzzleCooler();
        }

        #endregion
    }
}