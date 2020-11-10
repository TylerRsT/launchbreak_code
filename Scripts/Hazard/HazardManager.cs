using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class HazardManager : MonoBehaviour
    {
        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameMode"></param>
        public abstract bool TryInitialize(GameMode gameMode);

        /// <summary>
        /// 
        /// </summary>
        public abstract void OnGameModeStep();

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public abstract class HazardManager<T> : HazardManager
        where T : GameMode
    {
        #region Override

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameMode"></param>
        /// <returns></returns>
        public override bool TryInitialize(GameMode gameMode)
        {
            if(IsCompatibleWith(gameMode))
            {
                Initialize(gameMode);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void OnGameModeStep()
        { }

        #endregion

        #region Messages

        /// <summary>
        /// 
        /// </summary>
        protected virtual void Awake()
        { }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void Start()
        { }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void Update()
        { }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void FixedUpdate()
        { }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void OnDestroy()
        { }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameMode"></param>
        protected virtual void Initialize(GameMode gameMode)
        {
            this.gameMode = gameMode as T;
            enabled = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameMode"></param>
        /// <returns></returns>
        protected virtual bool IsCompatibleWith(GameMode gameMode)
        {
            return gameMode is T;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        protected T gameMode { get; private set; }

        #endregion
    }
}