using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class Equippable : MonoBehaviour
    {
        #region Const

        protected const int AdditiveSortingOrder = 7;

        #endregion

        #region Messages

        /// <summary>
        /// 
        /// </summary>
        protected virtual void Awake()
        {
            shooter = GetComponentInParent<ShooterBehaviour>();
            if(_spriteRenderer == null)
            {
                _spriteRenderer = GetComponent<SpriteRenderer>();
            }
            if(_tileObjectRenderer == null)
            {
                _tileObjectRenderer = GetComponent<TileObjectRenderer>();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void OnDestroy()
        {

        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        public virtual void OnEquip()
        {
            _isEquipped = true;
            spriteRenderer.enabled = true;
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void OnUnequip()
        {
            _isEquipped = false;
            spriteRenderer.enabled = false;
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void OnCharacterDied()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orientation"></param>
        public virtual void SetTargetOrientation(Vector2 orientation)
        {
            this.orientation = orientation;

            float angle = Vector2.SignedAngle(Vector2.down, orientation);
            float radians = Mathf.Deg2Rad * angle;

            transform.localScale = new Vector3(radians < 0.0f ? -1.0f : 1.0f, 1.0f, 1.0f);

            tileObjectRenderer.additive = (Mathf.Abs(radians) > Mathf.PI / 2.0f) ? (AdditiveSortingOrder * (-1)) : AdditiveSortingOrder;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public Vector2 offset => _offset;

        /// <summary>
        /// 
        /// </summary>
        public Character character => shooter as Character;

        /// <summary>
        /// 
        /// </summary>
        public SpriteRenderer spriteRenderer => _spriteRenderer;

        /// <summary>
        /// 
        /// </summary>
        public TileObjectRenderer tileObjectRenderer => _tileObjectRenderer;

        /// <summary>
        /// 
        /// </summary>
        public bool isEquipped => _isEquipped;

        /// <summary>
        /// 
        /// </summary>
        public ShooterBehaviour shooter { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        protected Vector2 orientation { get; private set; }

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Vector2 _offset = Vector2.zero;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private SpriteRenderer _spriteRenderer;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private TileObjectRenderer _tileObjectRenderer;

        private bool _isEquipped;

        #endregion
    }
}