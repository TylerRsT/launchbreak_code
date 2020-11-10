using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public static class AbilityFactory
    {
        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TAbility"></typeparam>
        /// <typeparam name="TAbilityData"></typeparam>
        /// <param name="abilityData"></param>
        /// <returns></returns>
        public static TAbility Instantiate<TAbility, TAbilityData>(TAbilityData abilityData)
            where TAbility : class, IAbility, new()
            where TAbilityData : AbilityData
        {
            return Instantiate(typeof(TAbility), abilityData) as TAbility;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TAbilityData"></typeparam>
        /// <param name="abilityType"></param>
        /// <param name="abilityData"></param>
        /// <returns></returns>
        public static IAbility Instantiate<TAbilityData>(System.Type abilityType, TAbilityData abilityData)
            where TAbilityData : AbilityData
        {
            Debug.Assert(typeof(IAbility).IsAssignableFrom(abilityType));

            var ability = System.Activator.CreateInstance(abilityType) as IAbility;
            (ability as IAbility).abilityData = abilityData;

            return ability;
        }

        #endregion
    }
}