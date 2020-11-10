using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class KatanaPowerUp : PowerUp<KatanaPowerUpData>
    {
        #region Override

        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
        public override void Apply(Character character)
        {
            base.Apply(character);

            KatanaBuff katanaBuff;
            if(character.TryAddBuff(out katanaBuff))
            {
                katanaBuff.powerUpData = powerUpData;
            }
        }

        #endregion
    }
}
