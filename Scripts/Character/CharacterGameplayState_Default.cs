using DG.Tweening;
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
    public class CharacterGameplayState_Default : CharacterGameplayStateHandler
    {
        #region Override

        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
        public override void Initialize(Character character)
        {
            base.Initialize(character);

            _rigidbody = character.GetComponent<Rigidbody2D>();
            _spriteAnimator = character.GetComponent<SpriteAnimator>();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Enter()
        {
            base.Enter();

            character.SetTriggerState(CharacterTriggerState.Weapon);
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Exit()
        {
            base.Exit();

            character.isMoving = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="move"></param>
        /// <param name="target"></param>
        public override void Orientate(Vector2 move, Vector2 target)
        {
            base.Orientate(move, target);

            if (move.x != 0.0f)
                _moveOrientation.x = move.x;
            else if (move.y != 0.0f)
                _moveOrientation.x = 0.0f;

            if (move.y != 0.0f)
                _moveOrientation.y = move.y;
            else if (move.x != 0.0f)
                _moveOrientation.y = 0.0f;

            float radians = Mathf.Deg2Rad * Vector2.SignedAngle(Vector2.right, _moveOrientation);
            if (move.x != 0.0f || move.y != 0.0f)
            {
                if (!character.runParticleSystem.isPlaying)
                {
                    character.runParticleSystem.Play();
                }
                _rigidbody.velocity = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)) * character.speed;
            }
            else
            {
                if (character.runParticleSystem.isPlaying)
                {
                    character.runParticleSystem.Stop();
                }
                _rigidbody.velocity = Vector2.zero;
            }

            if (target.x == 0.0f && target.y == 0.0f)
            {
                if (move != Vector2.zero)
                {
                    _targetOrientation = _moveOrientation;
                }
            }
            else
            {
                _targetOrientation.x = target.x;
                _targetOrientation.y = target.y;
            }

            _spriteAnimator.Play(move == Vector2.zero ? character.skinDescriptor.idleAnimation : character.skinDescriptor.moveAnimation);
            character.isMoving = move != Vector2.zero;
            _moveOrientation.Normalize();
            _targetOrientation.Normalize();
            character.SetOrientations(_moveOrientation, _targetOrientation);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="constructionModeAxis"></param>
        public override void HandleConstructionMode(float constructionModeAxis)
        {
            base.HandleConstructionMode(constructionModeAxis);

            character.SetTriggerState(constructionModeAxis == 0.0f ? CharacterTriggerState.Weapon : CharacterTriggerState.Construction);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        public override void Action(CharacterAction action)
        {
            base.Action(action);

            if(character.targetOrientationLocked && action != CharacterAction.Dash && action != CharacterAction.Drop)
            {
                return;
            }

            switch(action)
            {
                case CharacterAction.Fire:
                    character.PerformTrigger();
                    break;
                case CharacterAction.Release:
                    character.PerformRelease();
                    break;
                case CharacterAction.Dash:
                case CharacterAction.Drop:
                    character.Dash();
                    break;
                case CharacterAction.Pick:
                    if(character.triggerState == CharacterTriggerState.Construction)
                    {
                        break;
                    }
                    character.TryPick((character, pickable) =>
                    {
                        pickable.SendPickup(character, false);
                    });
                    break;
                case CharacterAction.Switch:
                    SwitchWeapon();
                    break;
                case CharacterAction.Throw:
                    break;
                case CharacterAction.Activate:
#if !MARKETING_CAMERA
                    if(Debug.isDebugBuild)
                    {
#endif // !MARKETING_CAMERA
                        character.ToggleMarketingZoom();
#if !MARKETING_CAMERA
                    }
#endif // !MARKETING_CAMERA
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="damageInfo"></param>
        public override void OnTakeDamages(DamageInfo damageInfo)
        {
            base.OnTakeDamages(damageInfo);

            character.targetOrientationLocked = false;
        }

#endregion

#region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator TakeWeapon(bool switchWeapon = true)
        {
            character.targetOrientationLocked = true;
            if (switchWeapon)
            {
                character.SwitchWeapon();
            }

            float angle = Vector2.SignedAngle(Vector2.down, character.targetOrientation);
            float radians = Mathf.Deg2Rad * angle;

            character.currentEquippable.transform.eulerAngles = Vector3.forward * (radians < 0.0f ? -90.0f : 90.0f);
            var tweenFinished = false;
            var tween = character.currentEquippable.transform.DOLocalRotate(Vector3.forward * (angle + (radians < 0.0f ? 90.0f : -90.0f)), 0.15f)
                .onComplete += (() => tweenFinished = true);
            yield return new WaitUntil(() => tweenFinished);

            //character.currentEquippable.tileObjectRenderer.additive = originalAdditive;

            character.targetOrientationLocked = false;
        }

        /// <summary>
        /// 
        /// </summary>
        private void SwitchWeapon()
        {
            if (character.triggerState == CharacterTriggerState.Weapon
                && character.weapons.Count(x => x != null) > 1
                && !character.targetOrientationLocked)
            {
                Telemetry.game.Incr("switchWeapon");
                character.StartCoroutine(SwitchWeaponCoroutine());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private IEnumerator SwitchWeaponCoroutine()
        {
            character.targetOrientationLocked = true;

            float angle = Vector2.SignedAngle(Vector2.down, character.targetOrientation);
            float radians = Mathf.Deg2Rad * angle;

            var originalAdditive = character.currentEquippable.tileObjectRenderer.additive;
            character.currentEquippable.tileObjectRenderer.additive = Mathf.Abs(originalAdditive) * -1;

            var tweenFinished = false;
            var tween = character.currentEquippable.transform.DOLocalRotate(Vector3.forward * (radians < 0.0f ? -90.0f : 90.0f), 0.15f)
                .onComplete += (() => tweenFinished = true);
            yield return new WaitUntil(() => tweenFinished);
            yield return TakeWeapon();
        }

#endregion

#region Fields

        private Vector2 _moveOrientation;
        private Vector2 _targetOrientation;

        private Rigidbody2D _rigidbody;
        private SpriteAnimator _spriteAnimator;

#endregion
    }
}