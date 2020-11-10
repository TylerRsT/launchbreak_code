using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    /// /!\ You must NEVER remove entries from this enum !!!
    public enum AbilityId
    {
        None,
        DashDeflect,
        Greedy,
        ConstructRogue,
    }

    /// <summary>
    /// 
    /// </summary>
    public static class AbilityTable
    {
        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="abilityId"></param>
        /// <returns></returns>
        public static System.Type Get(AbilityId abilityId)
        {
            System.Type abilityType;
            if (_table.TryGetValue(abilityId, out abilityType))
            {
                return abilityType;
            }
            return null;
        }

        #endregion

        #region Fields

        private static Dictionary<AbilityId, System.Type> _table = new Dictionary<AbilityId, System.Type>
        {
            { AbilityId.DashDeflect, typeof(DashDeflectAbility) },
        };

        #endregion
    }
}
