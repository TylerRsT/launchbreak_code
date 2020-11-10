using Elendow.SpritedowAnimator;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class CharacterGameplayState_Panicking : CharacterGameplayStateHandler
    {
        #region Override

        /// <summary>
        /// 
        /// </summary>
        public override void Enter()
        {
            base.Enter();

            character.GetComponent<SpriteAnimator>().Play(character.skinDescriptor.moveAnimation);
            character.StartCoroutine(Panic());
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool acceptsBuffs => false;

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private IEnumerator Panic()
        {
            var currentVelocity = Vector2.zero;
            while (true)
            {
                var axis = Random.Range(0, 2) == 1;

                Vector2 velocity;
                do
                {
                    velocity = new Vector2(!axis ? MathExtensions.RandOne() : 0.0f, axis ? MathExtensions.RandOne() : 0.0f);
                } while (velocity == currentVelocity);

                currentVelocity = velocity;
                character.SetOrientations(velocity, velocity);

                character.body.velocity = velocity * character.statsDescriptor.speed;

                yield return new WaitForSeconds(Random.Range(0.3f, 0.6f));
            }
        }

        #endregion
    }
}