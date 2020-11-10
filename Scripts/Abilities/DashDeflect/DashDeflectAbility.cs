using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class DashDeflectAbility : Ability<DashDeflectAbilityData>
    {
        #region Const

        private const string DeflectSoundKey = "Deflect";

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dashState"></param>
        /// <param name="bullet"></param>
        /// <returns></returns>
        public bool TryDeflectBullet(CharacterGameplayState_Dash dashState, Bullet bullet)
        {
            if(dashState.isActive || dashState.isDeflectActive)
            {
                bullet.speed = Mathf.Min(abilityData.multiplier * bullet.speed, abilityData.maxVelocity);
                bullet.instigator = dashState.character;
                bullet.gameplayTeam.teamIndex = -1;
                bullet.orientation *= -1.0f;
                bullet.isDashReflected = true;

                dashState.isDeflectActive = true;

                SpawnDeflectAnimation(dashState, bullet.transform);

                return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dashState"></param>
        /// <param name="transform"></param>
        public void SpawnDeflectAnimation(CharacterGameplayState_Dash dashState, Transform transform)
        {
            dashState.character.EmitSound(DeflectSoundKey);
            GameplayStatics.SpawnFireAndForgetAnimation(abilityData.deflectAnimation, transform.position, transform.rotation, dashState.character.transform);
        }

        #endregion
    }
}