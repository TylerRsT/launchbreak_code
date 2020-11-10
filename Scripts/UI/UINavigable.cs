using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    [DisplayName("UI Navigable")]
    public class UINavigable : MonoBehaviour
    {
        #region Messages

        /// <summary>
        /// 
        /// </summary>
        /// <param name="navigable"></param>
        private void OnGotFocus(UINavigable navigable)
        {
            hasFocus = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="navigable"></param>
        private void OnLostFocus(UINavigable navigable)
        {
            hasFocus = false;
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        public void InsertToNavigationMap()
        {
            if(_up != null)
            {
                _up._down = this;
            }
            if(_down != null)
            {
                _down._up = this;
            }

            if(_left != null)
            {
                _left._right = this;
            }
            if(_right != null)
            {
                _right._left = this;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void RemoveFromNavigationMap()
        {
            if(_up != null)
            {
                _up._down = _down;
            }
            if(_down != null)
            {
                _down._up = _up;
            }

            if(_left != null)
            {
                _left._right = _right;
            }
            if(_right != null)
            {
                _right._left = _left;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public bool hasFocus { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public UINavigable up => _up;

        /// <summary>
        /// 
        /// </summary>
        public UINavigable right => _right;

        /// <summary>
        /// 
        /// </summary>
        public UINavigable down => _down;

        /// <summary>
        /// 
        /// </summary>
        public UINavigable left => _left;

        /// <summary>
        /// 
        /// </summary>
        public string submitActionName => _submitActionName;

        /// <summary>
        /// 
        /// </summary>
        public string cancelActionName => _cancelActionName;

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private UINavigable _up = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private UINavigable _right = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private UINavigable _down = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private UINavigable _left = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private string _submitActionName = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private string _cancelActionName = string.Empty;

        #endregion
    }
}