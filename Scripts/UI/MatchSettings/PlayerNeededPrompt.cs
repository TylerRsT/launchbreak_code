using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class PlayerNeededPrompt : MonoBehaviour
    {
        #region Messages

        /// <summary>
        /// 
        /// </summary>
        private void Start()
        {
            transform.DOScale(0f, 0.01f);
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        public void Open()
        {
            _isOpen = true;
            transform.DOScale(1f, 0.1f);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Close()
        {
            _isOpen = false;
            transform.DOScale(0f, 0.1f);
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public bool isOpen => _isOpen;

        #endregion

        #region Fields

        private bool _isOpen = false;

        #endregion

    }
}