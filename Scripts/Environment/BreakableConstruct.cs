using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    [RequireComponent(typeof(Construct))]
    [RequireComponent(typeof(HitFlash))]
    public class BreakableConstruct : DamageComponent
    {
        #region Override

        /// <summary>
        /// 
        /// </summary>
        /// <param name="damageInfo"></param>
        public override void TakeDamages(DamageInfo damageInfo)
        {
            if (this.IsInConstruction())
            {
                return;
            }

            TakeDamagesInternal(damageInfo);
        }

        #endregion

        #region Messages

        /// <summary>
        /// 
        /// </summary>
        private void Awake()
        {
            _hitFlash = GetComponent<HitFlash>();
            _currentHealth = _health;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        private void OnConstructed(ConstructLoadoutItem item)
        {
            _health = item.health;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="response"></param>
        private void OnReceivingBullet(BulletResponse response)
        {
            if (this.IsInConstruction() || response.collider.isTrigger)
            {
                response.received = false;
                return;
            }

            response.received = true;
            if(TakeDamagesInternal(response.bullet))
            {
                var shooter = response.bullet.instigator as ShooterBehaviour;
                var spawner = shooter?.spawner;
                var baseItem = GetComponent<Construct>().baseItem;
                if(spawner != null 
                    && spawner.HasAbility(AbilityId.ConstructRogue)
                    && baseItem != null
                    && spawner.character != null)
                {
                    spawner.character.tempConstruct = baseItem;
                }
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
            TakeDamagesInternal(dashAttack);
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="damageInfo"></param>
        private bool TakeDamagesInternal(DamageInfo damageInfo)
        {
            _health -= damageInfo.damages;

            if (damageInfo.provider != null)
            {
                Telemetry.game.Incr($"{damageInfo.provider.providerName}_cons_dmg", damageInfo.damages);
            }

            _hitFlash.Flash();
            var renderer = GetComponent<SpriteRenderer>();
            renderer.sprite = _damagedSprite;
            if (_health <= 0)
            {
                if (damageInfo.provider != null)
                {
                    Telemetry.game.Incr($"{damageInfo.provider.providerName}_cons_kill");
                }

                this.DestroyConstruct();
                return true;
            }
            return false;
        }

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private int _health;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Sprite _damagedSprite = default;

        private int _currentHealth;

        private HitFlash _hitFlash;

        #endregion
    }
}