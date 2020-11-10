using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class ContextualSpriteButton : MonoBehaviour
    {
        #region Messages

        /// <summary>
        /// 
        /// </summary>
        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
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

        private SpriteRenderer _renderer;

        private ControllersInfoProvider _infoProvider;

        #endregion
    }
}