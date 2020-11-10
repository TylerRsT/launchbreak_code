using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu(fileName = "ABIL_Name", menuName = "Shovel/Ability Loadout Item")]
    public class AbilityLoadoutItem : LoadoutItem
    {
        #region Override

        /// <summary>
        /// 
        /// </summary>
        public override void Load()
        {
            _abilityData?.Load();
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public AbilityId abilityId => _abilityId;

        /// <summary>
        /// 
        /// </summary>
        public AbilityData abilityData => _abilityData;

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private AbilityId _abilityId = AbilityId.None;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private AbilityData _abilityData = default;

        #endregion
    }
}