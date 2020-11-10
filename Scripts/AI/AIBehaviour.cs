using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class AIBehaviour : MonoBehaviour
    {
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
        {
            character = GetComponentInParent<Character>();

            behaviourManager = GetComponent<AIBehaviourManager>();
            navigationController = GetComponent<AINavigationController>();
            aimController = GetComponent<AIAimController>();
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void Update()
        { }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public static bool easyMode { get; set; } = true;

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

    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class AIBehaviourAttribute : System.Attribute
    {

    }
}