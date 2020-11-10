using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class CharacterBuff : MonoBehaviour
    {
        #region Messages

        /// <summary>
        /// 
        /// </summary>
        protected virtual void Awake()
        {
            character = GetComponent<Character>();
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void Start()
        { }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void Update()
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void FixedUpdate()
        {
            elapsed += GameplayStatics.gameFixedDeltaTime;

            if (_duration >= 0.0f && elapsed >= _duration)
            {
                Destroy(this);
            }
        }

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
        /// <param name="bullet"></param>
        /// <returns></returns>
        public virtual bool ReceiveBullet(Bullet bullet)
        {
            return true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public float duration
        {
            get { return _duration; }
            set { _duration = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected float elapsed { get; set; } = 0.0f;

        /// <summary>
        /// 
        /// </summary>
        protected Character character { get; private set; }

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _duration = -1.0f;

        #endregion
    }
}