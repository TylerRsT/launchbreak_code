using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public enum CharacterGameplayState
    {
        Default,
        Dash,
        Dead,
        LaunchKey,
        Leaving,
        Panicking,
    }

    /// <summary>
    /// 
    /// </summary>
    public enum CharacterTriggerState
    {
        None,
        Weapon,
        Construction,
    }

    /// <summary>
    /// 
    /// </summary>
    public abstract class CharacterStateHandler
    {
        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
        public virtual void Initialize(Character character)
        {
            this.character = character;
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void Enter()
        { }

        /// <summary>
        /// 
        /// </summary>
        public virtual void Update()
        { }

        /// <summary>
        /// 
        /// </summary>
        public virtual void FixedUpdate()
        { }

        /// <summary>
        /// 
        /// </summary>
        public virtual void Exit()
        { }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public Character character { get; private set; }

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public abstract class CharacterGameplayStateHandler : CharacterStateHandler
    {
        #region Override

        /// <summary>
        /// 
        /// </summary>
        public override void Enter()
        {
            base.Enter();

            if(!acceptsBuffs)
            {
                foreach(var buff in character.GetComponents<CharacterBuff>())
                {
                    GameObject.Destroy(buff);
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="move"></param>
        /// <param name="target"></param>
        public virtual void Orientate(Vector2 move, Vector2 target)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="constructionModeAxis"></param>
        public virtual void HandleConstructionMode(float constructionModeAxis)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        public virtual void Action(CharacterAction action)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bullet"></param>
        /// <returns></returns>
        public virtual bool ReceiveBullet(Bullet bullet)
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="damageInfo"></param>
        public virtual void OnTakeDamages(DamageInfo damageInfo)
        {

        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public virtual bool acceptsBuffs => true;

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public class CharacterTriggerStateHandler : CharacterStateHandler
    {
        #region Methods

        public virtual void Orientate(Vector2 move, Vector2 target)
        { }

        /// <summary>
        /// 
        /// </summary>
        public virtual void Fire()
        { }

        /// <summary>
        /// 
        /// </summary>
        public virtual void Release()
        { }

        #endregion
    }
}