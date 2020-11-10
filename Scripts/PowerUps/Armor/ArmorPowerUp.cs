using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class ArmorPowerUp : PowerUp<ArmorPowerUpData>
    {
        #region Override

        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public override bool CanApply(Character character)
        {
            if (character.resources.armor >= character.resources.maxArmor)
            {
                return powerUpData.canPickupIfArmorFull;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
        public override void Apply(Character character)
        {
            base.Apply(character);

            Apply(character, powerUpData.armor);
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
        /// <param name="armor"></param>
        public void Apply(Character character, int armor)
        {
            character.resources.armor += armor;

            ArmorBuff armorBuff;
            if (character.TryAddBuff(out armorBuff))
            {
                armorBuff.powerUpData = powerUpData;
            }
        }

        #endregion
    }
} 