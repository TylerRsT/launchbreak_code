using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class Loadout
    {
        #region Const

        public const int LoadoutSize = 4;

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public LoadoutItem this[int index]
        {
            get { return items[index]; }
            set { items[index] = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public LoadoutItem[] items { get; } = new LoadoutItem[LoadoutSize];

        #endregion
    }
}