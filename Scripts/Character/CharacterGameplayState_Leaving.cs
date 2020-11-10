using DG.Tweening;
using Elendow.SpritedowAnimator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class CharacterGameplayState_Leaving : CharacterGameplayStateHandler
    {
        #region Override

        /// <summary>
        /// 
        /// </summary>
        public override void Enter()
        {
            base.Enter();

            character.SetTriggerState(CharacterTriggerState.None);
            var characterBuffs = character.GetComponents<CharacterBuff>();
            foreach(var characterBuff in characterBuffs)
            {
                GameObject.Destroy(characterBuff);
            }

            coroutine = character.StartCoroutine(Leave());
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private IEnumerator Leave()
        {
            var position = character.transform.position;
            var targetPosition = character.spawner.transform.position + character.spawner.spawnOffset
                + Vector3.up * GameConstants.TileSize;

            var orientation = new Vector2();
            if (targetPosition.x > position.x)
            {
                orientation.x = 1.0f;
            }
            else
            {
                orientation.x = -1.0f;
            }

            character.SetOrientations(orientation, orientation);

            foreach (var collider in character.GetComponents<Collider2D>())
            {
                collider.enabled = false;
            }

            yield return new WaitForSeconds(2.0f);

            character.DropAllWeapons(false);

            var spriteAnimator = character.GetComponent<SpriteAnimator>();
            spriteAnimator.Stop();

            var jumpAnimation = character.skinDescriptor.jumpAnimation;
            if (jumpAnimation != null)
            {
                spriteAnimator.Play(jumpAnimation, true);
            }

            Tween tween = character.body.DOJump(targetPosition, 5.0f, 1, 0.4f, true);
            yield return tween.WaitForCompletion();

            GameObject.Destroy(character.GetComponent<TileObjectRenderer>());

            var spriteRenderer = character.GetComponent<SpriteRenderer>();
            spriteRenderer.sortingLayerID = SortingLayerHelper.GetSortingLayerValue(StaticSortingLayer.KeySlotBottom);
            spriteRenderer.sortingOrder = 1;
            spriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;

            targetPosition = character.spawner.transform.position + Vector3.down * GameConstants.TileSize;
            character.floorShadowRenderer.enabled = false;

            yield return new WaitForSeconds(0.4f);

            tween = character.body.DOMove(targetPosition, 0.2f);
            yield return tween.WaitForCompletion();
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool acceptsBuffs => false;

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public Coroutine coroutine { get; private set; }

        #endregion
    }
}