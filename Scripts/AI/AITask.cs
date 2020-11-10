using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class AITask
    {
        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
        /// <param name="behaviourManager"></param>
        /// <param name="score"></param>
        public void Initialize(Character character, AIBehaviourManager behaviourManager, float score = 0.0f)
        {
            this.score = score;
            this.character = character;
            this.behaviourManager = behaviourManager;

            navigationController = behaviourManager.GetComponent<AINavigationController>();
            aimController = behaviourManager.GetComponent<AIAimController>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerator OnHandleController();

        /// <summary>
        /// 
        /// </summary>
        public virtual void FixedUpdate()
        { }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public float score { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        protected Character character { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        protected AIBehaviourManager behaviourManager { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        protected AINavigationController navigationController { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        protected AIAimController aimController { get; private set; }

        #endregion
    }
}