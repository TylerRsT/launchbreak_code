using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class GameplayTeam : MonoBehaviour
    {
        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsEnemy(GameplayTeam other)
        {
            return _teamIndex == -1 || other.teamIndex != _teamIndex;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public int teamIndex
        {
            get { return _teamIndex; }
            set { _teamIndex = value; }
        }

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private int _teamIndex = -1;

        #endregion
    }
}