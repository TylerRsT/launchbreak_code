using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    [RequireComponent(typeof(GrenadeBehaviour))]
    public class GrenadePickable : WeaponPickable
    {
        #region Override

        /// <summary>
        /// 
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            _subRenderer = GetComponent<GrenadeBehaviour>().subRenderer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="response"></param>
        protected override void OnPickup(PickupResponse response)
        {
            base.OnPickup(response);

            if(!response.accepted)
            {
                return;
            }

            var weapon = response.character.currentEquippable as GrenadeWeaponEquippable;
            Debug.Assert(weapon != null);

            var grenadeCountdown = GetComponent<GrenadeBehaviour>();
            if(grenadeCountdown == null)
            {
                return;
            }

            var weaponGrenadeCountdown = weapon.gameObject.AddComponent<GrenadeBehaviour>();
            weaponGrenadeCountdown.CopyFrom(grenadeCountdown);
            weaponGrenadeCountdown.spriteRenderer = weapon.GetComponent<SpriteRenderer>();

            weaponGrenadeCountdown.spriteRenderer.sprite = _subRenderer.sprite;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void UpdateMaterial()
        {
            base.UpdateMaterial();

            _subRenderer.material.SetColor("_DesiredColor", blackOutlineColor);
        }

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private SpriteRenderer _subRenderer = default;

        #endregion
    }
}