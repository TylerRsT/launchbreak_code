using Elendow.SpritedowAnimator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu(fileName = "ABILDATA_DashDeflect", menuName = "Shovel/Ability Data/Dash Deflect")]
    public class DashDeflectAbilityData : AbilityData
    {
        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public float multiplier => _multiplier;

        /// <summary>
        /// 
        /// </summary>
        public float maxVelocity => _maxVelocity;

        /// <summary>
        /// 
        /// </summary>
        public SpriteAnimation deflectAnimation => _deflectAnimation;

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _multiplier = 1.5f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _maxVelocity = 3000.0f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private SpriteAnimation _deflectAnimation = default;

        #endregion
    }
}