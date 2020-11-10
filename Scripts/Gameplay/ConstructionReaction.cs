using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class ConstructionReaction : MonoBehaviour
    {
        #region Messages

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        private void OnConstructionSpaceRequested(ConstructionSpaceRequest request)
        {
            if (!_acceptConstruction)
            {
                request.Decline();
            }
        }

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private bool _acceptConstruction = false;

        #endregion
    }
}
