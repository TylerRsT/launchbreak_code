using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class PushBuff : CharacterBuff
    {
        #region Override

        /// <summary>
        /// 
        /// </summary>
        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            var velocity = orientation;

            if(_useCurve)
            {
                var diff = _curve.Evaluate(elapsed / duration);
                velocity = orientation * diff * power;
            }
            else
            {
                velocity = orientation * power * (elapsed - GameplayStatics.gameFixedDeltaTime);
            }

            character.body.velocity = velocity;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnDestroy()
        {
            base.OnDestroy();

            character.body.velocity = Vector2.zero;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public Vector2 orientation { get; set; } = Vector2.right;

        /// <summary>
        /// 
        /// </summary>
        public AnimationCurve curve
        {
            get { return _curve; }
            set
            {
                _curve = value;
                _useCurve = value != null && value.keys.Length > 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public float power { get; set; } = 1.0f;

        #endregion

        #region Fields

        private AnimationCurve _curve;

        private bool _useCurve = false;

        #endregion
    }
}