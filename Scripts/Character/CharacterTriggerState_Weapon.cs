using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class CharacterTriggerState_Weapon : CharacterTriggerStateHandler
    {
        #region Override
        
        /// <summary>
        /// 
        /// </summary>
        public override void Enter()
        {
            base.Enter();
            character.currentEquippable?.OnEquip();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Exit()
        {
            base.Exit();
            character.currentEquippable?.OnUnequip();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Fire()
        {
            base.Fire();
            (character.currentEquippable as WeaponEquippable)?.Trigger();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Release()
        {
            base.Release();
            (character.currentEquippable as WeaponEquippable)?.Release();
        }

        #endregion
    }
}
