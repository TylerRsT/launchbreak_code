using Elendow.SpritedowAnimator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class Construct : MonoBehaviour
    {
        #region Const

        public const string OnConstructedMethodName = "OnConstructed";

        private const string BuildSoundKey = "Build";
        private const string DestroySoundKey = "Destroy";

        #endregion

        #region Messages

        /// <summary>
        /// 
        /// </summary>
        private void Start()
        {
            if(GetComponent<ConstructionValidator>() == null && !_isConstructed && _baseItem != null)
            {
                SendMessage(OnConstructedMethodName, _baseItem, SendMessageOptions.DontRequireReceiver);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        private void OnConstructed(ConstructLoadoutItem item)
        {
            Debug.Assert(!_isConstructed);
            _baseItem = item;
            _isConstructed = true;
            _supplyCost = item.supplyCost;

            if(_doConstructAnimation && _constructAnimation != null)
            {
                GetComponent<SpriteAnimator>().Play(_constructAnimation, true);
            }

            if (_emitSoundAtConstruct)
            {
                this.EmitSound(BuildSoundKey);
            }

            Telemetry.game.Incr("constructs");
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        public void BeginDestroy()
        {
            if(_isDestroyed)
            {
                return;
            }

            _isDestroyed = true;
            this.EmitSound(DestroySoundKey);

            if (_destroyAnimation != null)
            {
                GameplayStatics.SpawnFireAndForgetAnimation(_destroyAnimation, transform.position, transform.rotation)
                    .AddComponent<TileObjectRenderer>();
            }
            GameplayStatics.ScatterSupply(transform, _supplyCost);

            Destroy(gameObject);
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public Character instigator { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int supplyCost
        {
            get { return _supplyCost; }
            set { _supplyCost = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public ConstructLoadoutItem baseItem => _baseItem;

        /// <summary>
        /// 
        /// </summary>
        public bool emitSoundAtConstruct
        {
            get => _emitSoundAtConstruct;
            set => _emitSoundAtConstruct = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool isConstructed => _isConstructed;

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private int _supplyCost = 0;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private ConstructLoadoutItem _baseItem = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private SpriteAnimation _constructAnimation = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private SpriteAnimation _destroyAnimation = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private bool _doConstructAnimation = true;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private bool _emitSoundAtConstruct = true;

        private bool _isConstructed = false;
        private bool _isDestroyed = false;

        #endregion
    }
}