using Elendow.SpritedowAnimator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class CharacterGameplayState_Dead : CharacterGameplayStateHandler
    {
        #region Const

        private const string PlayerDeathSoundKey = "PlayerDeath";

        #endregion

        #region Override

        /// <summary>
        /// 
        /// </summary>
        public override void Enter()
        {
            base.Enter();

            _elapsed = 0.0f;
            _hasRespawned = false;

            character.spawner.deathCount++;

            character.SetTriggerState(CharacterTriggerState.None);

            var characterInput = character.GetComponent<CharacterInput>();
            if (characterInput != null)
            {
                characterInput.inputLocked = true;
            }
            character.GetComponent<Rigidbody2D>().simulated = false;

            var spriteRenderer = character.GetComponent<SpriteRenderer>();
            var spriteAnimator = character.GetComponent<SpriteAnimator>();

            if (character.deathDamageType == DamageType.Burn)
            {
                spriteAnimator.Play(character.burnedToDeathAnimation, true);
                spriteAnimator.onFinish.AddListener(() =>
                {
                    spriteRenderer.sprite = null;
                    spriteRenderer.enabled = false;
                    spriteAnimator.enabled = false;
                });
            }
            else
            {
                spriteRenderer.enabled = false;
                spriteAnimator.enabled = false;

                var deathEffectAnimator = new GameObject().AddComponent<SpriteAnimator>();
                deathEffectAnimator.transform.position = character.transform.position;
                deathEffectAnimator.Play(character.deathEffectAnimation, true);
                deathEffectAnimator.onFinish.AddListener(() => GameObject.Destroy(deathEffectAnimator.gameObject));
                deathEffectAnimator.GetComponent<SpriteRenderer>().sortingLayerName = StaticSortingLayer.CharacterHUD.ToString();
            }

            character.floorShadowRenderer.enabled = false;
            character.DropAllWeapons(false);

            character.ScatterSupply();
            character.resources.HideSupply();
            character.EmitSound(PlayerDeathSoundKey);
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Update()
        {
            base.Update();

            if (GameMode.instance.autoRespawn)
            {
                _elapsed += GameplayStatics.gameDeltaTime;

                if (_elapsed >= GameMode.instance.respawnDelay && !_hasRespawned)
                {
                    character.spawner.supply = character.GetComponent<CharacterResources>().supply;
                    character.spawner.BeginSpawn();
                    _hasRespawned = true;
                }
            }

            if (_elapsed >= _durationToDisappear && _hasRespawned)
            {
                foreach(Transform child in character.transform)
                {
                    GameObject.Destroy(child.gameObject);
                }
                GameObject.Destroy(character.gameObject);
                return;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool acceptsBuffs => false;

        #endregion

        #region Fields

        private float _durationToDisappear = 3.0f;
        private float _elapsed = 0.0f;

        private bool _hasRespawned = false;

        #endregion
    }
}