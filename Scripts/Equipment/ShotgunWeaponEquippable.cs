using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class ShotgunWeaponEquippable : FireWeaponEquippable
    {
        #region Override

        /// <summary>
        /// 
        /// </summary>
        protected override void SpawnBullets()
        {
            var multiplier = _multiplierBase;
            var baseAngle = muzzleTransform.eulerAngles.z;

            for(var i = 0; i < _bulletCount; ++i)
            {
                var angle = multiplier * _anglePerBullet;
                angle += Random.Range(_angleKnockbackPerBullet * -1.0f, _angleKnockbackPerBullet);
                angle += baseAngle;

                var radians = Mathf.Deg2Rad * angle;

                var spawnPosition = GetBulletSpawnPosition();
                SpawnBullet(new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)) * transform.localScale.x, spawnPosition);

                multiplier += _multiplierStep;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        private void SpawnAdditionalBullet(Vector2 offset, Vector2 spawnPosition)
        {
            var num = 2.0f * Vector2.Dot(orientation, offset);
            SpawnBullet(new Vector2(num * offset.x + orientation.x, num * offset.y + orientation.y), spawnPosition);
        }

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private int _bulletCount = 3;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _multiplierBase = -1.0f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _multiplierStep = 1.0f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _anglePerBullet = 15.0f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _angleKnockbackPerBullet = 5.0f;

        #endregion
    }
}