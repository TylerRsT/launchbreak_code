using Elendow.SpritedowAnimator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu(fileName = "SKIN_Name", menuName = "Shovel/Character Skin Descriptor")]
    public class CharacterSkinDescriptor : ScriptableObject
    {
        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public SpriteAnimation idleAnimation => _idleAnimation;

        /// <summary>
        /// 
        /// </summary>
        public SpriteAnimation moveAnimation => _moveAnimation;

        /// <summary>
        /// 
        /// </summary>
        public SpriteAnimation dashAnimation => _dashAnimation;

        /// <summary>
        /// 
        /// </summary>
        public SpriteAnimation deathAnimation => _deathAnimation;

        /// <summary>
        /// 
        /// </summary>
        public SpriteAnimation jumpAnimation => _jumpAnimation;
        /// <summary>
        /// 
        /// </summary>
        public Sprite headSprite => _headSprite;


        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private SpriteAnimation _idleAnimation = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private SpriteAnimation _moveAnimation = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private SpriteAnimation _dashAnimation = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private SpriteAnimation _deathAnimation = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private SpriteAnimation _jumpAnimation = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Sprite _headSprite = default;

        #endregion
    }
}