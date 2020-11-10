using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class KeySlot : MonoBehaviour
    {
        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public int value => _value;
        
        /// <summary>
        /// 
        /// </summary>
        public bool isOn
        {
            get => _isOn;
            set => _isOn = value;
        }

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private int _value = 0;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private bool _isOn = false;

        #endregion
    }
}