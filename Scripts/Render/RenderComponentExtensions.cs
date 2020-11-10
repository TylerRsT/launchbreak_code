using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public static class RenderComponentExtensions
    {
        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="component"></param>
        /// <param name="alpha"></param>
        public static void SetCosmeticAlpha(this Component component, float alpha)
        {
            SetCosmeticAlpha(component.gameObject, alpha);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="alpha"></param>
        public static void SetCosmeticAlpha(this GameObject gameObject, float alpha)
        {
            var spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            if(spriteRenderer != null)
            {
                var color = spriteRenderer.color;
                color.a = alpha;
                spriteRenderer.color = color;
            }
        }

        #endregion
    }
}