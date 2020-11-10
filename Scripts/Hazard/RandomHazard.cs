using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class RandomHazard : MonoBehaviour
    {
        #region Const

        protected const string PlaySoundKey = "Play";

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        public abstract void Play();

        #endregion

        #region Properties
        
        /// <summary>
        /// 
        /// </summary>
        public bool canTriggerMultipleTimes
        {
            get => _canTriggerMultipleTimes;
            protected set => _canTriggerMultipleTimes = value;
        }

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private bool _canTriggerMultipleTimes = false;

        #endregion
    }
}