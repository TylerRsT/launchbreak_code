using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class AITask_Pickable : AITask
    {
        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pickable"></param>
        public AITask_Pickable(Pickable pickable)
        {
            _pickable = pickable;
        }

        #endregion

        #region Override

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IEnumerator OnHandleController()
        {
            yield return navigationController.NavigateTo(_pickable.transform.position);
            if (_pickable.isPickable && !_pickable.autoPickup)
            {
                character.Action(CharacterAction.Pick);
            }
            else if(_pickable is WeaponPickable && character.weapons.All(x => x != null))
            {
                character.Action(CharacterAction.Pick);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if(_pickable == null)
            {
                behaviourManager.StopTask(this);
            }
        }

        #endregion

        #region Fields

        private Pickable _pickable;

        #endregion
    }
}