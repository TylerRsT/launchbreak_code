using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class UITab : MonoBehaviour
    {
        #region Messages

        /// <summary>
        /// 
        /// </summary>
        private void Awake()
        {
            _image = GetComponent<UnityEngine.UI.Image>();
            _defaultSprite = _image.sprite;
        }

        /// <summary>
        ///
        /// </summary>
        public void Focus()
        {
            if (_image != null)
            {
                _image.sprite = _hoverSprite;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Unfocus()
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
