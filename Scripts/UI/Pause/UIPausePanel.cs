using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class UIPausePanel : MonoBehaviour
    {

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        public void Close()
        {
            GameObject.Destroy(this.gameObject);
        }

        #endregion
    }
}