using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class ContextualImageButton : MonoBehaviour
    {
        #region Messages

        /// <summary>
        /// 
        /// </summary>
        private void Awake()
        {
            _renderer = GetComponent<Image>();
        }

        /// <summary>
        /// 
        /// </summary>
        private void Start()
        {
            _infoProvider = FindObjectOfType<ControllersInfoProvider>();
        }

        /// <summary>
        /// 
        /// </summary>
        private void Update()
        {
            _renderer.sprite = _infoProvider.hasKeyboardOnly ? _keyboardSprite : _gamepadSprite;
        }

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Sprite _keyboardSprite = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Sprite _gamepadSprite = default;

        private Image _renderer;

        private ControllersInfoProvider _infoProvider;

        #endregion
    }
}