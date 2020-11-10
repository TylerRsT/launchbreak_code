using Com.LuisPedroFonseca.ProCamera2D;
using CreativeSpore.SuperTilemapEditor;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class FireWeaponEquippable : HeatBasedWeaponEquippable
    {
        #region Const

        private const int MaxRaycastHitCount = 4;

        #endregion

        #region Override

        /// <summary>
        /// 
        /// </summary>
        protected override void Start()
        {
            base.Start();

            elapsedSinceLastFire = fireWeaponDescriptor.rateOfFire;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Update()
        {
            base.Update();

            _recomputedBulletSpawnPosition = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override bool CanFire()
        {
            var condition = elapsedSinceLastFire >= fireWeaponDescriptor.rateOfFire;
            if (condition || isOverheating)
            {
                Vibrate(0.5f, 0.1f);
            }
            return condition;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Fire()
        {
            if (fireWeaponDescriptor.bulletPrefab == null)
            {
                return;
            }

            if (isOverheating)
            {
                this.EmitSound(fireWeaponDescriptor.emptySounds, false);
                return;
            }

            var angleKnockback = fireWeaponDescriptor.angleKnockback;
            var eulerAngles = transform.localEulerAngles;
            eulerAngles.z += Random.Range(angleKnockback * -1.0f, angleKnockback);
            transform.localEulerAngles = eulerAngles;

            SpawnBullets();
            DoKnockbackSequence();

            this.EmitSound(fireWeaponDescriptor.fireSounds, true);

            AddHeat(fireWeaponDescriptor.heatPerShot);
            elapsedSinceLastFire = 0.0f;
        }

        #endregion

        #region Messages

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collision"></param>
        private void OnTriggerEnter2D(Collider2D collision)
        {
            var tilemapChunk = collision.GetComponent<TilemapChunk>();
            if(tilemapChunk != null)
            {
                _currentCollidingChunk = tilemapChunk;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collision"></param>
        private void OnTriggerExit2D(Collider2D collision)
        {
            var tilemapChunk = collision.GetComponent<TilemapChunk>();
            if (tilemapChunk != null && tilemapChunk == _currentCollidingChunk)
            {
                _currentCollidingChunk = null;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        protected virtual void SpawnBullets()
        {
            SpawnBullet(shooter.targetOrientation, GetBulletSpawnPosition());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bulletOrientation"></param>
        protected GameObject SpawnBullet(Vector2 bulletOrientation)
        {
            return SpawnBullet(bulletOrientation, muzzleTransform.position);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bulletOrientation"></param>
        protected GameObject SpawnBullet(Vector2 bulletOrientation, Vector2 position)
        {
            var prefabBullet = fireWeaponDescriptor.bulletPrefab.GetComponent<Bullet>();
            var spawnPosition = prefabBullet != null && prefabBullet.useWeaponRotation ? transform.rotation : Quaternion.Euler(0.0f, 0.0f, 0.0f);
            var gameObject = Instantiate(fireWeaponDescriptor.bulletPrefab, position, spawnPosition);
            var bullet = gameObject.GetComponent<Bullet>();

            if (bullet != null)
            {
                bullet.orientation = bulletOrientation;
                bullet.gameplayTeam.teamIndex = shooter.GetComponent<GameplayTeam>().teamIndex;
                bullet.instigator = this;
                bullet.damages = (int)fireWeaponDescriptor.damages;
                bullet.speed = fireWeaponDescriptor.bulletSpeed;
                bullet.damageType = fireWeaponDescriptor.damageType;
            }

            // Spawn fire effect.
            if (fireWeaponDescriptor.fireEffectPrefab != null)
            {
                Instantiate(fireWeaponDescriptor.fireEffectPrefab, muzzleTransform.position, muzzleTransform.rotation, transform);
            }

            gameObject.layer = LayerMask.NameToLayer("Bullet");
            return gameObject;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected Vector2 GetBulletSpawnPosition()
        {
            if(_recomputedBulletSpawnPosition)
            {
                return _bulletSpawnPosition;
            }

            var distance = Vector2.Distance(_raycastOrigin.position, muzzleTransform.position);
            var spawnAtRaycastOrigin = false;

            var collisionCount = 0;

            if (_currentCollidingChunk != null)
            {
                spawnAtRaycastOrigin = true;
            }
            else if ((collisionCount = Physics2D.RaycastNonAlloc(_raycastOrigin.position, character.targetOrientation, _fireRaycastHits, distance)) > 0)
            {
                for (var i = 0; i < collisionCount; ++i)
                {
                    var bulletReaction = _fireRaycastHits[i].collider.GetComponent<BulletReaction>();
                    var tilemapChunk = _fireRaycastHits[i].collider.GetComponent<TilemapChunk>();
                    if (bulletReaction != null || tilemapChunk != null)
                    {
                        spawnAtRaycastOrigin = true;
                        break;
                    }
                }
            }

            _bulletSpawnPosition = spawnAtRaycastOrigin ? _raycastOrigin.position : muzzleTransform.position;
            _recomputedBulletSpawnPosition = true;
            return _bulletSpawnPosition;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        protected FireWeaponDescriptor fireWeaponDescriptor => (FireWeaponDescriptor)weaponDescriptor;

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Transform _raycastOrigin = default;

        private RaycastHit2D[] _fireRaycastHits = new RaycastHit2D[MaxRaycastHitCount];
        private TilemapChunk _currentCollidingChunk;

        private bool _recomputedBulletSpawnPosition = false;
        private Vector2 _bulletSpawnPosition;

        #endregion
    }
}