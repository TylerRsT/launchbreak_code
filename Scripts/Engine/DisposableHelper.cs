using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public static class DisposableHelper
    {
        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposable"></param>
        public static void SafeDispose<T>(ref T disposable)
            where T : class, System.IDisposable
        {
            disposable?.Dispose();
            disposable = null;
        }

        #endregion
    }
}