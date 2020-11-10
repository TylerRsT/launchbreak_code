using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class WeaponEquippable : Equippable, IDamageProvider
    {
        #region IDamageProvider

        /// <summary>
        /// 
        /// </summary>
        string IDamageProvider.providerName => weaponDescriptor.name;

        /// <summary>
        /// 
        /// </summary>
        CharacterSpawner IDamageProvider.providerSpawner => character.spawner;

        #endregion

        #region Override

        /*
        /// <summary>
        /// 
        /// </summary>
        public override void OnEquip()
        {
            base.OnEquip();

            spriteRenderer.transform.localEulerAngles = Vector3.zero;
            spriteRenderer.transform.localPosition = _originalPosition;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void OnUnequip()
        {
            base.OnUnequip();

            spriteRenderer.transform.eulerAngles = Vector3.forward * 315.0f;
            spriteRenderer.transform.localPosition = new Vector2(-2.5f, -3.5f);

            tileObjectRenderer.additive = Mathf.Abs(tileObjectRenderer.additive) * -1;
        }
        */

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orientation"></param>
        public override void SetTargetOrientation(Vector2 orientation)
        {
            base.SetTargetOrientation(orientation);

            if (_rotatesWithTargetOrientation)
            {
                float angle = Vector2.SignedAngle(Vector2.down, orientation);
                float radians = Mathf.Deg2Rad * angle;
                transform.rotation = Quaternion.Euler(0.0f, 0.0f, angle + (radians < 0.0f ? 90.0f : -90.0f));
            }
            else
            {
                float angle = Vector2.SignedAngle(Vector2.right, orientation);
                float radians = Mathf.Deg2Rad * angle;
                transform.localPosition = offset.Rotate(radians);
                transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            _originalPosition = spriteRenderer.transform.localPosition;
            spriteRenderer.material.SetColor("_DesiredColor", Pickable.blackOutlineColor);
        }

        #endregion

        #region Messages

        /// <summary>
        /// 
        /// </summary>
        protected virtual void Start()
        { }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void Update()
        { }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void FixedUpdate()
        { }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        public virtual void Trigger()
        { }

        /// <summary>
        /// 
        /// </summary>
        public virtual void Release()
        { }

        /// <summary>
        /// 
        /// </summary>
        public virtual void Cooldown()
        { }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public WeaponDescriptor weaponDescriptor
        {
            get => _weaponDescriptor;
            set => _weaponDescriptor = value;
        }

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private WeaponDescriptor _weaponDescriptor;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private bool _rotatesWithTargetOrientation = true;

        private Vector2 _originalPosition;

        #endregion
    }
}