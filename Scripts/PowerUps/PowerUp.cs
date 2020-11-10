using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPowerUp
    {
        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        bool CanApply(Character character);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
        void Apply(Character character);

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        PowerUpData powerUpData { get; set; }

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public abstract class PowerUp<T> : IPowerUp where T : PowerUpData
    {
        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        protected PowerUp()
        { }

        #endregion

        #region IAbility

        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public virtual bool CanApply(Character character)
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
        public virtual void Apply(Character character)
        { }

        /// <summary>
        /// 
        /// </summary>
        PowerUpData IPowerUp.powerUpData
        {
            get => ((PowerUp<T>)(this)).powerUpData;
            set => ((PowerUp<T>)(this)).powerUpData = value as T;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public T powerUpData { get; set; }

        #endregion
    }
}