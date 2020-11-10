using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class BouncingWall : MonoBehaviour
    {
        #region Messages

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collision"></param>
        private void OnCollisionEnter2D(Collision2D collision)
        {
            var character = collision.rigidbody.GetComponent<Character>();
            if(character != null)
            {
                BouncingWallBuff buff;
                if (character.TryAddBuff(out buff))
                {
                    buff.Initialize(this, collision);
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public float duration => _duration;

        /// <summary>
        /// 
        /// </summary>
        public float multiplier => _multiplier;

        /// <summary>
        /// 
        /// </summary>
        public float maxVelocity => _maxVelocity;

        #endregion

        #region Fields

        [SerializeField]
        private float _duration = 0.2f;

        [SerializeField]
        private float _multiplier = 1.0f;

        [SerializeField]
        private float _maxVelocity = 20.0f;

        #endregion
    }
}