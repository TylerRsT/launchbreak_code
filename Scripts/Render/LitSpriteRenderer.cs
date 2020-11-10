using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class LitSpriteRenderer : MonoBehaviour
    {
        #region Const

        private const string MaskTexParamName = "_MaskTex";

        #endregion

        #region Messages

        /// <summary>
        /// 
        /// </summary>
        private void Awake()
        {
            var spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.material.SetTexture(MaskTexParamName, _lightMaskSprite.texture);
        }

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Sprite _lightMaskSprite = default;

        #endregion
    }
}