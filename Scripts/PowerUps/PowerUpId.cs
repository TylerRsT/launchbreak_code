using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public enum PowerUpId
    {
        None,
        SpeedBoost,
        Cooler,
        Armor,
        Katana,
    }

    /// <summary>
    /// 
    /// </summary>
    public static class PowerUpTable
    {
        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="powerUpId"></param>
        /// <returns></returns>
        public static System.Type Get(PowerUpId powerUpId)
        {
            System.Type powerUpType;
            if (_table.TryGetValue(powerUpId, out powerUpType))
            {
                return powerUpType;
            }
            return null;
        }

        #endregion

        #region Fields

        private static Dictionary<PowerUpId, System.Type> _table = new Dictionary<PowerUpId, System.Type>
        {
            { PowerUpId.SpeedBoost, typeof(SpeedBoostPowerUp) },
            { PowerUpId.Cooler, typeof(CoolerPowerUp) },
            { PowerUpId.Armor, typeof(ArmorPowerUp) },
            { PowerUpId.Katana, typeof(KatanaPowerUp) },
        };

        #endregion
    }
}