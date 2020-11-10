using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    [RequireComponent(typeof(BoxCollider2D))]
    public class LocomotionCorner : MonoBehaviour
    {
        #region Messages

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collision"></param>
        private void OnTriggerEnter2D(Collider2D collision)
        {
            var corner = collision.GetComponent<LocomotionCorner>();
            if (corner != null)
            {
                _collidingCorners.Add(corner);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collision"></param>
        private void OnTriggerExit2D(Collider2D collision)
        {
            var corner = collision.GetComponent<LocomotionCorner>();
            if (corner != null)
            {
                _collidingCorners.Remove(corner);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public float adjustX
        {
            get
            {
                switch(_side)
                {
                    case Side.TopLeft:
                    case Side.BottomLeft:
                        return -1.0f;
                    case Side.TopRight:
                    case Side.BottomRight:
                        return 1.0f;
                }
                Debug.Assert(false);
                return 0.0f;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public float adjustY
        {
            get
            {
                switch (_side)
                {
                    case Side.TopLeft:
                    case Side.TopRight:
                        return 1.0f;
                    case Side.BottomLeft:
                    case Side.BottomRight:
                        return -1.0f;
                }
                Debug.Assert(false);
                return 0.0f;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool isCollidingCorners => _collidingCorners.Any(x => x);

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Side _side = default;

        private List<LocomotionCorner> _collidingCorners = new List<LocomotionCorner>();

        #endregion

        #region Nested

        /// <summary>
        /// 
        /// </summary>
        public enum Side
        {
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight,
        }

        #endregion
    }
}