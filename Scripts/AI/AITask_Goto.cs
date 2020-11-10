using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class AITask_Goto : AITask
    {
        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        public AITask_Goto(Vector2 target)
        {
            _target = target;
        }

        #endregion

        #region Override

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IEnumerator OnHandleController()
        {
            yield return navigationController.NavigateTo(_target);
        }

        #endregion

        #region Fields

        private Vector2 _target;

        #endregion
    }
}