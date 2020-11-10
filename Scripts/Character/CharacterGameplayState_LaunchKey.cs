using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class CharacterGameplayState_LaunchKey : CharacterGameplayState_Default
    {
        #region Const

        private const float DropThrowMultiplier = 60.0f;
        private const float DropHitMultiplier = 60.0f;
        private const string KeyTargetSoundKey = "KeyTarget";

        #endregion

        #region Override

        /// <summary>
        /// 
        /// </summary>
        public override void Enter()
        {
            base.Enter();

            _doWeaponSwitchAnim = true;

            _playerKeyIndicator = GameObject.Instantiate(character.playerKeyIndicatorPrefab, character.transform);
            _spawnerPlayerKeyIndicator = GameObject.Instantiate(character.playerKeyIndicatorPrefab, character.spawner.transform);
            _soundInstances.AddRange(_playerKeyIndicator.EmitSound(KeyTargetSoundKey));

            Telemetry.game.Incr("launchKey_pickup");
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Exit()
        {
            base.Exit();

            foreach (var item in _soundInstances)
            {
                item.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            }

            _soundInstances.Clear();

            character.currentWeaponIndex += 1;

            if (_doWeaponSwitchAnim)
            {
                TakeDefaultWeapon(true);
            }
            else
            {
                character.SwitchWeapon();
            }

            GameObject.Destroy(_playerKeyIndicator);
            _playerKeyIndicator = null;

            GameObject.Destroy(_spawnerPlayerKeyIndicator);
            _spawnerPlayerKeyIndicator = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        public override void Action(CharacterAction action)
        {
            switch (action)
            {
                case CharacterAction.Fire:
                    character.PerformTrigger();
                    break;
                case CharacterAction.Pick:
                    if (!character.TryPick((character, pickable) =>
                     {
                         if (!(pickable is WeaponPickable))
                         {
                             pickable.SendPickup(character, false);
                         }
                         else
                         {
                             _doWeaponSwitchAnim = false;
                             Drop(DropEffect.InPlace);
                             character.SetGameplayState(CharacterGameplayState.Default);
                             pickable.SendPickup(character, false);
                         }
                     }))
                    {
                        DropInPlace();
                        character.SetGameplayState(CharacterGameplayState.Default);
                    };
                    break;
                case CharacterAction.Drop:
                case CharacterAction.Dash:
                    if (character.canDash)
                    {
                        Drop(DropEffect.InPlace);
                        character.Dash();
                    }
                    break;
                case CharacterAction.Activate:
                    character.spawner.TriggerAllTraps();
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bullet"></param>
        /// <returns></returns>
        public override bool ReceiveBullet(Bullet bullet)
        {
            if (character.spawner.HasAbility(AbilityId.Greedy) && bullet.damages < character.resources.health)
            {
                character.ScatterSupply();
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="damageInfo"></param>
        public override void OnTakeDamages(DamageInfo damageInfo)
        {
            base.OnTakeDamages(damageInfo);

            if(character.resources.hasArmor)
            {
                return;
            }

            Drop();
            character.SetGameplayState(CharacterGameplayState.Default);
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        public void Drop()
        {
            Drop(DropEffect.Hit);
        }

        /// <summary>
        /// 
        /// </summary>
        public void DropInPlace()
        {
            Drop(DropEffect.InPlace);
        }

        /// <summary>
        /// 
        /// </summary>
        private void Drop(DropEffect dropEffect)
        {
            var launchKeyEquippable = character.currentEquippable as LaunchKeyEquippable;
            Debug.Assert(launchKeyEquippable != null);

            Telemetry.game.Incr($"launchKey_drop_{dropEffect}");

            switch (dropEffect)
            {
                case DropEffect.InPlace:
                    if (character.spawner.GetComponent<Collider2D>().IsTouching(character.GetComponent<Collider2D>()))
                    {
                        GameMode.instance.RedeemKey(character);
                    }
                    else
                    {
                        launchKeyEquippable.Drop();
                    }
                    break;
                case DropEffect.Throw:
                    {
                        var launchKeyPickable = launchKeyEquippable.Drop();
                        var position = launchKeyPickable.transform.position;
                        launchKeyPickable.doFloatingAnimAtStart = false;
                        var throwOrientation = character.targetOrientation * DropThrowMultiplier;
                        launchKeyPickable.DoJumpAnim(new Vector2(throwOrientation.x + position.x, throwOrientation.y + position.y), launchKeyPickable.DoFloatingAnim);
                        break;
                    }
                case DropEffect.Hit:
                    {
                        var launchKeyPickable = launchKeyEquippable.Drop();
                        var position = launchKeyPickable.transform.position;
                        launchKeyPickable.doFloatingAnimAtStart = false;
                        var targetOrientation = character.isMoving ? character.targetOrientation : Vector2.zero;

                        while (targetOrientation == Vector2.zero)
                        {
                            targetOrientation = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
                        }
                        targetOrientation.Normalize();

                        var hitOrientation = targetOrientation * DropHitMultiplier * (-1.0f);
                        launchKeyPickable.DoJumpAnim(new Vector2(hitOrientation.x + position.x, hitOrientation.y + position.y), launchKeyPickable.DoFloatingAnim);
                        break;
                    }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void TakeDefaultWeapon(bool switchWeapon)
        {
            character.StartCoroutine((character.gameplayStateHandler as CharacterGameplayState_Default).TakeWeapon(switchWeapon));
        }

        #endregion

        #region Fields

        private GameObject _playerKeyIndicator;
        private GameObject _spawnerPlayerKeyIndicator;
        private List<FMOD.Studio.EventInstance> _soundInstances = new List<FMOD.Studio.EventInstance>();

        private bool _doWeaponSwitchAnim = true;

        #endregion

        #region Nested

        /// <summary>
        /// 
        /// </summary>
        private enum DropEffect
        {
            InPlace,
            Throw,
            Hit,
        }

        #endregion
    }
}