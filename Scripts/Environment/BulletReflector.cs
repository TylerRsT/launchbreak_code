using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    [RequireComponent(typeof(Construct))]
    public class BulletReflector : DamageComponent
    {
        #region Override

        /// <summary>
        /// 
        /// </summary>
        /// <param name="damageInfo"></param>
        public override void TakeDamages(DamageInfo damageInfo)
        {
            TakeDamages(damageInfo, true);
        }

        #endregion

        #region Messages

        /// <summary>
        /// 
        /// </summary>
        private void Awake()
        {
            _hitFlash = GetComponent<HitFlash>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="construct"></param>
        private void OnConstructed(ConstructLoadoutItem construct)
        {
            _health = construct.health;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="response"></param>
        private void OnReceivingBullet(BulletResponse response)
        {
            response.received = false;
            if (this.IsInConstruction())
            {
                return;
            }

            _health -= response.bullet.damages;
            _hitFlash?.Flash();

            _expectedBullet = response.bullet;
            _expectedBullet.GetComponent<Collider2D>().isTrigger = false;

            if (_health <= 0)
            {
                _destroyAfterReflect = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dashAttack"></param>
        private void OnReceivingDashAttack(CharacterDashAttack dashAttack)
        {
            if (this.IsInConstruction())
            {
                return;
            }

            dashAttack.Accept();

            TakeDamages(dashAttack, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collision"></param>
        private void OnCollisionStay2D(Collision2D collision)
        {
            if(_expectedBullet == null)
            {
                return;
            }

            var contactPoint = collision.contacts.FirstOrDefault(x => x.collider.GetComponent<Bullet>() == _expectedBullet);
            if(contactPoint.collider != null)
            {
                contactPoint.collider.isTrigger = true;

                if(_destroyAfterReflect)
                {
                    var spawner = (_expectedBullet.instigator as Character)?.spawner;
                    var baseItem = GetComponent<Construct>().baseItem;
                    if (spawner != null
                        && spawner.HasAbility(AbilityId.ConstructRogue)
                        && baseItem != null
                        && spawner.character != null)
                    {
                        spawner.character.tempConstruct = baseItem;
                    }
                }

                //_expectedBullet.orientation = Vector2.Reflect(_expectedBullet.orientation, contactPoint.normal);
                _expectedBullet.orientation = _expectedBullet.orientation * -1.0f;
                _expectedBullet.gameplayTeam.teamIndex = -1;
                _expectedBullet.instigator = null;
                _expectedBullet = null;

                if(_destroyAfterReflect)
                {
                    this.DestroyConstruct();
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="damageInfo"></param>
        /// <param name="checkInConstruction"></param>
        private void TakeDamages(DamageInfo damageInfo, bool checkInConstruction)
        {
            if (checkInConstruction && this.IsInConstruction())
            {
                return;
            }

            _health -= damageInfo.damages;
            _hitFlash?.Flash();

            if (_health <= 0)
            {
                this.DestroyConstruct();
            }
        }

        #endregion

        #region Fields

        private int _health;
        private HitFlash _hitFlash;

        private Bullet _expectedBullet;

        private bool _destroyAfterReflect;

        #endregion
    }
}