using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class SpeedBoostPowerUp : PowerUp<SpeedBoostPowerUpData>
    {
        #region Override

        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
        public override void Apply(Character character)
        {
            base.Apply(character);

            SpeedBuff speedBuff;
            if (character.TryAddBuff(out speedBuff))
            {
                speedBuff.duration = powerUpData.duration;
                speedBuff.speedMultiplier = powerUpData.speedMultiplier;
                speedBuff.hollowFrequency = powerUpData.hollowFrequency;
            }
        }

        #endregion
    }
}