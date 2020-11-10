using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class DropBeacon : MonoBehaviour
    {
        #region Messages

        /// <summary>
        /// 
        /// </summary>
        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lootableToSpawn"></param>
        public void Spawn(LootableDescriptor lootableToSpawn)
        {
            _lootableToSpawn = lootableToSpawn;
            state = State.TurningOn;
        }

        /// <summary>
        /// 
        /// </summary>
        private void SpawnChest()
        {
            Debug.Assert(_chestPrefab != null);

            var chest = Instantiate(_chestPrefab, transform.position + _chestOffset, transform.rotation, transform).GetComponent<ChestPickable>();
            //chest.doFloatingAnimAtStart = false;
            chest.Initialize(this);
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnStateChanged()
        {
            switch(_state)
            {
                case State.Off:
                    _spriteRenderer.sprite = _offSprite;
                    _lootableToSpawn = null;
                    break;
                case State.TurningOn:
                    StartCoroutine(TurningOn());
                    break;
                case State.On:
                    _spriteRenderer.sprite = _onSprite;
                    SpawnChest();
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private IEnumerator TurningOn()
        {
            var isOn = false;
            var elapsed = 0.0f;
            while(true)
            {
                isOn = !isOn;
                _spriteRenderer.sprite = isOn ? _onSprite : _offSprite;
                yield return new WaitForSeconds(_turningOnInterval);
                elapsed += _turningOnInterval;
                if(elapsed >= _turningOnDuration)
                {
                    break;
                }
            }

            state = State.On;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public State state
        {
            get { return _state; }
            set
            {
                if(_state != value)
                {
                    _state = value;
                    OnStateChanged();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public LootableDescriptor lootableToSpawn => _lootableToSpawn;

        /// <summary>
        /// 
        /// </summary>
        public bool isFreeToDrop => _lootableToSpawn == null;

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Sprite _offSprite = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Sprite _onSprite = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private GameObject _chestPrefab = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Vector3 _chestOffset = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _turningOnDuration = 1.0f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _turningOnInterval = 0.2f;

        private SpriteRenderer _spriteRenderer;

        private State _state = State.Off;

        private LootableDescriptor _lootableToSpawn;

        #endregion

        #region Nested

        /// <summary>
        /// 
        /// </summary>
        public enum State
        {
            Off,
            TurningOn,
            On,
        }

        #endregion
    }
}