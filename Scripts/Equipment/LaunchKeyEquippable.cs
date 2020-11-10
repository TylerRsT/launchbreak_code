using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class LaunchKeyEquippable : Equippable
    {
        #region Messages

        /// <summary>
        /// 
        /// </summary>
        private void Start()
        {
            spriteRenderer.material.SetColor("_DesiredColor", Pickable.blackOutlineColor);
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        public LaunchKeyPickable Drop()
        {
            Debug.Assert(_launchKeyPickablePrefab != null);

            var positionAdditive = new Vector3(offset.x, offset.y);
            if(shooter.GetComponent<SpriteRenderer>().flipX)
            {
                positionAdditive.x *= -1.0f;
            }

            var launchKeyPickable = Instantiate(_launchKeyPickablePrefab, shooter.transform.position + positionAdditive, Quaternion.Euler(Vector3.zero));
            launchKeyPickable.EmitSound(LaunchKeyPickable.KeyDropSoundKey);

            return launchKeyPickable.GetComponent<LaunchKeyPickable>();
        }

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private GameObject _launchKeyPickablePrefab = default;

        #endregion
    }
}
