using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    [DisplayName("UI Image")]
    [RequireComponent(typeof(UnityEngine.UI.Image))]
    public class UIImage : UINavigable
    {
        #region Messages

        /// <summary>
        /// 
        /// </summary>
        private void Awake()
        {
            _image = GetComponent<UnityEngine.UI.Image>();
            if (_image != null)
            {
                _defaultSprite = _image.sprite;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="navigable"></param>
        public void OnGotFocus(UINavigable navigable)
        {
            if (_image != null)
            {
                _image.sprite = _hoverSprite;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="navigable"></param>
        private void OnLostFocus(UINavigable navigable)
        {
            if (_image != null)
            {
                _image.sprite = _defaultSprite;
            }
        }

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Sprite _hoverSprite = default;

        private Sprite _defaultSprite;

        private UnityEngine.UI.Image _image;

        #endregion
    }
}