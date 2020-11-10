using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class InvincilityBuff : CharacterBuff
    {
        #region Override

        /// <summary>
        /// 
        /// </summary>
        protected override void Update()
        {
            base.Update();

            if (elapsed % 0.4f < 0.2f)
            {
                character.SetCosmeticAlpha(0.5f);
            }
            else
            {
                character.SetCosmeticAlpha(1.0f);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bullet"></param>
        /// <returns></returns>
        public override bool ReceiveBullet(Bullet bullet)
        {
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnDestroy()
        {
            base.OnDestroy();

            character.SetCosmeticAlpha(1.0f);
        }

        #endregion
    }
}