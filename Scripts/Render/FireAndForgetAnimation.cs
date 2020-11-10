using Elendow.SpritedowAnimator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    [RequireComponent(typeof(SpriteAnimator))]
    public class FireAndForgetAnimation : MonoBehaviour
    {
        #region Messages

        /// <summary>
        /// 
        /// </summary>
        private void Awake()
        {
            _spriteAnimator = GetComponent<SpriteAnimator>();
        }

        /// <summary>
        /// 
        /// </summary>
        private void Start()
        {
            _spriteAnimator.Play(spriteAnimation, true);
        }

        /// <summary>
        /// 
        /// </summary>
        private void Update()
        {
            if(!_spriteAnimator.IsPlaying)
            {
                Destroy(gameObject);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public SpriteAnimation spriteAnimation { get; set; }

        #endregion

        #region Fields

        private SpriteAnimator _spriteAnimator;

        #endregion
    }
}