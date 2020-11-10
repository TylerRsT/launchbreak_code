using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class AITask_Patrol : AITask
    {
        #region Override

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IEnumerator OnHandleController()
        {
            do
            {
                var nextNodeIndex = Random.Range(0, walkableNodes.Length);
                var targetPosition = walkableNodes[nextNodeIndex].worldPosition;
                yield return navigationController.NavigateTo(targetPosition);
            } while (true);
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public NavMeshNode[] walkableNodes { get; set; }

        #endregion
    }
}