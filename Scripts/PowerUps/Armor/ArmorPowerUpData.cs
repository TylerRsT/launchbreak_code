using Elendow.SpritedowAnimator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu(fileName = "PWRDATA_Armor", menuName = "Shovel/Power Up Data/Armor")]
    public class ArmorPowerUpData : PowerUpData
    {
        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public int armor => _armor;

        /// <summary>
        /// 
        /// </summary>
        public bool canPickupIfArmorFull => _canPickupIfArmorFull;

        /// <summary>
        /// 
        /// </summary>
        public SpriteAnimation armorAnimation => _armorAnimation;

        /// <summary>
        /// 
        /// </summary>
        public SpriteAnimation armorHitAnimation => _armorHitAnimation;

        /// <summary>
        /// 
        /// </summary>
        public IReadOnlyList<string> armorHitSounds => _armorHitSounds;

        /// <summary>
        /// 
        /// </summary>
        public IReadOnlyList<string> armorDestroySounds => _armorDestroySounds;

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private int _armor = 2;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private bool _canPickupIfArmorFull = false;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private SpriteAnimation _armorAnimation = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private SpriteAnimation _armorHitAnimation = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private List<string> _armorHitSounds = new List<string>();

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private List<string> _armorDestroySounds = new List<string>();

        #endregion
    }
}