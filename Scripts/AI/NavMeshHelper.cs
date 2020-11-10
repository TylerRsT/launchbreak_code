using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    [RequireComponent(typeof(BoxCollider2D))]
    public class NavMeshHelper : MonoBehaviour
    {
        #region Messages

        /// <summary>
        /// 
        /// </summary>
        private void Awake()
        {
            _boxCollider = GetComponent<BoxCollider2D>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public Bounds bounds => _boxCollider.bounds;

        /// <summary>
        /// 
        /// </summary>
        public bool walkable => _walkable;

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private bool _walkable = true;

        private BoxCollider2D _boxCollider;

        #endregion
    }
}