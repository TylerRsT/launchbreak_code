using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class NoneGameMode : GameMode
    {
        #region Override

        /// <summary>
        /// 
        /// </summary>
        protected override void InitGame()
        {
            hasGameStarted = true;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Step()
        { }

        /// <summary>
        /// 
        /// </summary>
        public override int highestScore => default;

        #endregion
    }
}