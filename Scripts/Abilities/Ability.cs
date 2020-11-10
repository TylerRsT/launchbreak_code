using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public interface IAbility
    {
        #region Properties

        /// <summary>
        /// 
        /// </summary>
        AbilityData abilityData { get; set; }

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public abstract class Ability<T> : IAbility where T : AbilityData
    {
        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        protected Ability()
        { }

        #endregion

        #region IAbility

        /// <summary>
        /// 
        /// </summary>
        AbilityData IAbility.abilityData
        {
            get => ((Ability<T>)(this)).abilityData;
            set => ((Ability<T>)(this)).abilityData = value as T;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public T abilityData { get; set; }

        #endregion
    }
}