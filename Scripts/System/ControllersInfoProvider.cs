using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class ControllersInfoProvider : MonoBehaviour
    {
        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public abstract bool hasKeyboardOnly { get; }

        #endregion
    }
}