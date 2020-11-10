using Elendow.SpritedowAnimator;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public static class SpriteComponentExtensions
    {
        #region Const

        private const string BaseAnimatorPlayingFieldName = "playing";

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        static SpriteComponentExtensions()
        {
            _baseAnimatorPlayingField = typeof(BaseAnimator).GetField(BaseAnimatorPlayingFieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spriteAnimator"></param>
        public static void Pause(this BaseAnimator spriteAnimator)
        {
            if (spriteAnimator != null)
            {
                _baseAnimatorPlayingField.SetValue(spriteAnimator, false);
            }
        }

        #endregion

        #region Fields

        private static FieldInfo _baseAnimatorPlayingField;

        #endregion
    }
}