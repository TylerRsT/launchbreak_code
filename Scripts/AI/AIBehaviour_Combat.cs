using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    [AIBehaviour]
    public class AIBehaviour_Combat : AIBehaviour
    {
        #region Messages

        /// <summary>
        /// 
        /// </summary>
        protected override void Update()
        {
            base.Update();

            var weapon = character.currentEquippable as HeatBasedWeaponEquippable;
            if(weapon != null)
            {
                if (weapon.isOverheating)
                {
                    _elapsedSinceOverheating += GameplayStatics.gameDeltaTime;
                    if (_elapsedSinceOverheating >= 1.0f)
                    {
                        if (character.weapons.Count(x => x != null) > 1)
                        {
                            character.Action(CharacterAction.Switch);
                        }
                    }
                }
                else
                {
                    _elapsedSinceOverheating = 0.0f;
                }
            }
        }

        #endregion

        #region Fields

        private float _elapsedSinceOverheating;

        #endregion
    }
}