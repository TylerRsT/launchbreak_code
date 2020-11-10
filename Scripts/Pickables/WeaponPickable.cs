using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class WeaponPickable : LootPickable
    {
        #region Override

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lootableDescriptor"></param>
        public override void Initialize(LootableDescriptor lootableDescriptor)
        {
            base.Initialize(lootableDescriptor);
            _weaponDescriptor = lootableDescriptor as WeaponDescriptor;

            GetComponent<SpriteRenderer>().material.SetColor("_AltDesiredColor", new Color(0.149f, 0.973f, 1.000f));
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Start()
        {
            base.Start();

            _pickupButton.transform.localPosition = _weaponDescriptor.pickupButtonOffset;
            _pickupButton.gameObject.SetActive(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="response"></param>
        protected override void OnPickup(PickupResponse response)
        {
            base.OnPickup(response);

            if (response.autoPickup)
            {
                if(response.character.gameplayState == CharacterGameplayState.LaunchKey)
                {
                    return;
                }

                for(var i = 0; i < response.character.weapons.Length; ++i)
                {
                    if (response.character.weapons[i] == null)
                    {
                        response.Accept();
                        response.character.SetWeapon(i, _weaponDescriptor);
                        SpawnPickupText(_weaponDescriptor.name);
                        Telemetry.game.Incr($"{_weaponDescriptor.name}_pickup");
                        return;
                    }
                }
            }
            else
            {
                if(response.character.currentEquippable is WeaponEquippable)
                {
                    response.Accept();
                    Telemetry.game.Incr($"{_weaponDescriptor.name}_pickup");
                    for (var i = 0; i < response.character.weapons.Length; ++i)
                    {
                        if (response.character.weapons[i] == null)
                        {
                            response.character.SetWeapon(i, _weaponDescriptor);
                            SpawnPickupText(_weaponDescriptor.name);
                            Telemetry.game.Incr($"{_weaponDescriptor.name}_replace");
                            return;
                        }
                    }
                    response.character.SetWeapon(response.character.currentWeaponIndex, _weaponDescriptor);
                    SpawnPickupText(_weaponDescriptor.name);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void UpdateMaterial()
        {
            base.UpdateMaterial();

            _pickupButton.gameObject.SetActive(charactersStepping.Count > 0);
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public WeaponDescriptor weaponDescriptor => _weaponDescriptor;

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private SpriteRenderer _pickupButton = default;

        private WeaponDescriptor _weaponDescriptor;

        #endregion
    }
}