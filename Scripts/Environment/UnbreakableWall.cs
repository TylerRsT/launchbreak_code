using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class UnbreakableWall : MonoBehaviour
    {
        #region Messages

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dashAttack"></param>
        private void OnReceivingDashAttack(CharacterDashAttack dashAttack)
        {
            dashAttack.Accept();
        }

        #endregion
    }
}