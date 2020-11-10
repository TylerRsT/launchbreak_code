using Com.LuisPedroFonseca.ProCamera2D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu(fileName = "SSD_Name", menuName = "Shovel/Hazards/Ship Shake Descriptor")]
    public class ShipShakeDescriptor : ScriptableObject
    {
        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ShipShake GetShipShakeAt(int index)
        {
            if(_shipShakes.Count <= index)
            {
                return null;
            }

            return _shipShakes[index];
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public IReadOnlyList<ShipShake> shipShakes => _shipShakes;

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private List<ShipShake> _shipShakes = new List<ShipShake>();

        #endregion
    }

    /// <summary>
    /// /
    /// </summary>
    [System.Serializable]
    public class ShipShake
    {
        public bool isValid => shakePreset != null;

        public ShakePreset shakePreset = default;

        public float intervalMin = 30.0f;

        public float intervalMax = 45.0f;

        public float multiplierMin = 1.0f;

        public float multiplierMax = 2.0f;
    }
}