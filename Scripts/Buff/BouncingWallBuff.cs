using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class BouncingWallBuff : CharacterBuff
    {
        #region Messages 

        /// <summary>
        /// 
        /// </summary>
        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            _rigidbody.velocity = _push;
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        public void Initialize(BouncingWall wall, Collision2D collision)
        {
            duration = wall.duration;
            _rigidbody = character.GetComponent<Rigidbody2D>();
            var relativeVelocty = collision.contacts[0].relativeVelocity;
            var maxVelocity = wall.maxVelocity;
            relativeVelocty.x = Mathf.Clamp(relativeVelocty.x, maxVelocity * -1.0f, maxVelocity);
            relativeVelocty.y = Mathf.Clamp(relativeVelocty.y, maxVelocity * -1.0f, maxVelocity);

            _push = relativeVelocty * wall.multiplier * (-1.0f);
        }

        #endregion

        #region Fields

        private Rigidbody2D _rigidbody;
        private Vector2 _push;

        #endregion
    }
}