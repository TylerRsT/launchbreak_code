using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    [RequireComponent(typeof(GameplayTeam))]
    public abstract class ShooterBehaviour : DamageComponent, IDamageProvider
    {
        #region IDamageProvider

        /// <summary>
        /// 
        /// </summary>
        string IDamageProvider.providerName => damageProviderName;

        /// <summary>
        /// 
        /// </summary>
        CharacterSpawner IDamageProvider.providerSpawner => spawner;

        #endregion

        #region Override

        /// <summary>
        /// 
        /// </summary>
        /// <param name="damageInfo"></param>
        public override void TakeDamages(DamageInfo damageInfo)
        { }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public abstract string damageProviderName { get; }

        /// <summary>
        /// 
        /// </summary>
        public CharacterSpawner spawner { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Vector2 targetOrientation { get; protected set; } = Vector2.down;

        #endregion
    }
}