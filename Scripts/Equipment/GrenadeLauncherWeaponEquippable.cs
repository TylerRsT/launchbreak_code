using Com.LuisPedroFonseca.ProCamera2D;
using Elendow.SpritedowAnimator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class GrenadeLauncherWeaponEquippable : FireWeaponEquippable
    {
        #region Override

        /// <summary>
        /// 
        /// </summary>
        protected override void SpawnBullets()
        {
            var gameObject = SpawnBullet(character.targetOrientation, GetBulletSpawnPosition());
            var grenadeBehaviour = gameObject.GetComponent<GrenadeBehaviour>();
            gameObject.layer = LayerMask.NameToLayer("Grenade");

            grenadeBehaviour.explosionDelay = _explosionDelay;
            grenadeBehaviour.radius = _radius;
            grenadeBehaviour.knockback = _knockback;
            grenadeBehaviour.knockbackDuration = _knockbackDuration;
            grenadeBehaviour.knockbackCurve = _knockbackCurve;
            grenadeBehaviour.damages = _damages;
            grenadeBehaviour.damageType = fireWeaponDescriptor.damageType;
            grenadeBehaviour.explosionAnimation = _explosionAnimation;
            grenadeBehaviour.explosionFrequency = _explosionFrequency;
            grenadeBehaviour.explosionDistance = _explosionDistance;
            grenadeBehaviour.explosionShake = _explosionShake;
            grenadeBehaviour.equippable = this;
            grenadeBehaviour.RollOnTheFloor(character.targetOrientation, _throwInfo, 1.0f);
        }

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _explosionDelay = 3.0f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _radius = 48.0f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _knockback = 36.0f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _knockbackDuration = 0.2f;
        
        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private AnimationCurve _knockbackCurve = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private int _damages = 3;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private SpriteAnimation _explosionAnimation = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private int _explosionFrequency = 1;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private ShakePreset _explosionShake = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _explosionDistance = 12.0f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private GrenadeThrowInfo _throwInfo = default;

        #endregion
    }
}