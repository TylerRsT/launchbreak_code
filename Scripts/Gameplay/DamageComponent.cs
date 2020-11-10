using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public struct DamageInfo
    {
        #region Fields

        /// <summary>
        /// 
        /// </summary>
        public IDamageProvider provider;

        /// <summary>
        /// 
        /// </summary>
        public int damages;

        /// <summary>
        /// 
        /// </summary>
        public DamageType damageType;

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public enum DamageType
    {
        Normal,
        Burn,
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IDamageable
    {
        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="damageInfo"></param>
        void TakeDamages(DamageInfo damageInfo);

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IDamageProvider
    {
        #region Properties

        /// <summary>
        /// 
        /// </summary>
        string providerName { get; }

        /// <summary>
        /// 
        /// </summary>
        CharacterSpawner providerSpawner { get; }

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public abstract class DamageComponent : MonoBehaviour, IDamageable
    {
        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="damageInfo"></param>
        public abstract void TakeDamages(DamageInfo damageInfo);

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public virtual bool isInvincible => false;

        #endregion
    }
}