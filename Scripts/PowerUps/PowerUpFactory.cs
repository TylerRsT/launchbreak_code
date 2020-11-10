using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public static class PowerUpFactory
    {
        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TPowerUp"></typeparam>
        /// <typeparam name="TPowerUpData"></typeparam>
        /// <param name="powerUpData"></param>
        /// <returns></returns>
        public static TPowerUp Instantiate<TPowerUp, TPowerUpData>(TPowerUpData powerUpData)
            where TPowerUp : class, IPowerUp, new()
            where TPowerUpData : PowerUpData
        {
            return Instantiate(typeof(TPowerUp), powerUpData) as TPowerUp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TPowerUpData"></typeparam>
        /// <param name="powerUpType"></param>
        /// <param name="powerUpData"></param>
        /// <returns></returns>
        public static IPowerUp Instantiate<TPowerUpData>(System.Type powerUpType, TPowerUpData powerUpData)
            where TPowerUpData : PowerUpData
        {
            Debug.Assert(typeof(IPowerUp).IsAssignableFrom(powerUpType));

            var powerUp = System.Activator.CreateInstance(powerUpType) as IPowerUp;
            (powerUp as IPowerUp).powerUpData = powerUpData;

            return powerUp;
        }

        #endregion
    }
}